// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using SharpMap.Geometries;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Gdi;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Tools;
using GdiPoint = System.Drawing.Point;
using GdiSize = System.Drawing.Size;
using GdiRectangle = System.Drawing.Rectangle;
using GdiGraphicsPath = System.Drawing.Drawing2D.GraphicsPath;
using GdiMatrix = System.Drawing.Drawing2D.Matrix;
using GeoPoint = SharpMap.Geometries.Point;

namespace SharpMap.Presentation.WinForms
{
    /// <summary>
    /// Provides a WinForms control for viewing maps.
    /// </summary>
    public class MapViewControl : Control, IMapView2D, IToolsView
    {
        private readonly double _dpi = 96;
        private bool _dragging = false;
        private GdiPoint _mouseDownLocation = GdiPoint.Empty;
        private GdiPoint _mouseRelativeLocation = GdiPoint.Empty;
        private GdiPoint _mousePreviousLocation = GdiPoint.Empty;
        private readonly Queue<GdiRenderObject> _renderObjectQueue = new Queue<GdiRenderObject>();
        private readonly GdiMapActionEventArgs _globalActionArgs = new GdiMapActionEventArgs();
        private List<MapTool> _tools;
        private MapPresenter _presenter;
        private GdiMatrix _gdiViewMatrix;
        private readonly PointF[] _symbolTargetPointsTransfer = new PointF[3];
        private bool _backgroundBeingSet;

        /// <summary>
        /// Initializes a new WinForms map view control.
        /// </summary>
        public MapViewControl()
        {
            using (Graphics g = CreateGraphics())
            {
                _dpi = g.DpiX;
            }

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, false);
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, false);
            SetStyle(ControlStyles.UserMouse, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        /// <summary>
        /// Sets the map to display.
        /// </summary>
        public Map Map
        {
            private get
            {
                return _presenter.Map;
            }
            set
            {
                _presenter = new MapPresenter(value, this);
            }
        }

        internal bool BackgroundBeingSet
        {
            get { return _backgroundBeingSet; }
            set { _backgroundBeingSet = value; }
        }

        #region IView Members

        public string Title
        {
            get { return Name; }
            set { Name = value; }
        }

        #endregion

        #region IMapView2D Members

        #region Events
        public event EventHandler<MapActionEventArgs<Point2D>> Hover;
        public event EventHandler<MapActionEventArgs<Point2D>> BeginAction;
        public event EventHandler<MapActionEventArgs<Point2D>> MoveTo;
        public event EventHandler<MapActionEventArgs<Point2D>> EndAction;
        public event EventHandler<MapViewPropertyChangeEventArgs<StyleColor>> BackgroundColorChangeRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<GeoPoint>> GeoCenterChangeRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<double>> MaximumWorldWidthChangeRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<double>> MinimumWorldWidthChangeRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<Point2D>> OffsetChangeRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<Size2D>> SizeChangeRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<BoundingBox>> ViewEnvelopeChangeRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<double>> WorldAspectRatioChangeRequested;
        public event EventHandler ZoomToExtentsRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<Rectangle2D>> ZoomToViewBoundsRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<BoundingBox>> ZoomToWorldBoundsRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<double>> ZoomToWorldWidthRequested;
        #endregion

        #region Properties

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StyleColor BackgroundColor
        {
            get { return _presenter.BackgroundColor; }
            set
            {
                if (BackgroundBeingSet)
                {
                    return;
                }

                BackgroundBeingSet = true;
                onRequestBackgroundColorChange(BackgroundColor, value);
                BackgroundBeingSet = false;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double Dpi
        {
            get { return _dpi; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GeoPoint GeoCenter
        {
            get { return _presenter.GeoCenter; }
            set { onRequestGeoCenterChange(GeoCenter, value); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double MaximumWorldWidth
        {
            get { return _presenter.MaximumWorldWidth; }
            set { onRequestMaximumWorldWidthChange(MaximumWorldWidth, value); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double MinimumWorldWidth
        {
            get { return _presenter.MinimumWorldWidth; }
            set { onRequestMinimumWorldWidthChange(MinimumWorldWidth, value); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double PixelWorldWidth
        {
            get { return _presenter.PixelWorldWidth; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double PixelWorldHeight
        {
            get { return _presenter.PixelWorldHeight; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ViewSelection2D Selection
        {
            get { return _presenter.Selection; }
        }

        public Point2D ToView(GeoPoint point)
        {
            return _presenter.ToView(point);
        }

        public Point2D ToView(double x, double y)
        {
            return _presenter.ToView(x, y);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Matrix2D ToViewTransform
        {
            get { return _presenter.ToViewTransform; }
        }

        public GeoPoint ToWorld(Point2D point)
        {
            return _presenter.ToWorld(point);
        }

        public GeoPoint ToWorld(double x, double y)
        {
            return _presenter.ToWorld(x, y);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Matrix2D ToWorldTransform
        {
            get { return _presenter.ToWorldTransform; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BoundingBox ViewEnvelope
        {
            get { return _presenter.ViewEnvelope; }
            set { onRequestViewEnvelopeChange(ViewEnvelope, value); ; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size2D ViewSize
        {
            get { return ViewConverter.Convert(Size); }
            set { Size = GdiSize.Round(ViewConverter.Convert(value)); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double WorldAspectRatio
        {
            get { return _presenter.WorldAspectRatio; }
            set { onRequestWorldAspectRatioChange(WorldAspectRatio, value); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double WorldHeight
        {
            get { return _presenter.WorldHeight; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double WorldWidth
        {
            get { return _presenter.WorldWidth; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double WorldUnitsPerPixel
        {
            get { return _presenter.WorldUnitsPerPixel; }
        }
        #endregion

        #region Methods
        public void Offset(Point2D offsetVector)
        {
            onRequestOffset(offsetVector);
        }

        /// <summary>
        /// Draws the rendered object to the view.
        /// </summary>
        /// <param name="renderedObjects">The rendered objects to draw.</param>
        public void ShowRenderedObjects(IEnumerable<GdiRenderObject> renderedObjects)
        {
            if (renderedObjects == null) throw new ArgumentNullException("renderedObjects");

            RectangleF invalidBounds = GdiRectangle.Empty;
            GdiMatrix transform = getGdiViewTransform();

            foreach (GdiRenderObject ro in renderedObjects)
            {
                invalidBounds = invalidBounds.IsEmpty
                    ? ro.Type == GdiRenderObjectType.Path 
                        ? ro.GdiPath.GetBounds(transform)
                        : ro.ImageBounds
                    : RectangleF.Union(
                        ro.Type == GdiRenderObjectType.Path 
                            ? ro.GdiPath.GetBounds(transform)
                            : ro.ImageBounds, 
                        invalidBounds);

                _renderObjectQueue.Enqueue(ro);
            }

            Invalidate(GdiRectangle.Ceiling(invalidBounds));
        }

        void IMapView2D.ShowRenderedObjects(IEnumerable renderedObjects)
        {
            if (renderedObjects is IEnumerable<GdiRenderObject>)
            {
                ShowRenderedObjects(renderedObjects as IEnumerable<GdiRenderObject>);
                return;
            }

            throw new ArgumentException("Rendered objects must be an IEnumerable<GdiRenderObject>.");
        }

        public void ZoomToExtents()
        {
            onRequestZoomToExtents();
        }

        public void ZoomToViewBounds(Rectangle2D viewBounds)
        {
            onRequestZoomToViewBounds(viewBounds);
        }

        public void ZoomToWorldBounds(BoundingBox zoomBox)
        {
            onRequestZoomToWorldBounds(zoomBox);
        }

        public void ZoomToWorldWidth(double newWorldWidth)
        {
            onRequestZoomToWorldWidth(newWorldWidth);
        }
        #endregion

        #endregion

        #region IToolsView Members

        public event EventHandler<ToolChangeRequestedEventArgs> ToolChangeRequested;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<MapTool> Tools
        {
            get { return _tools; }
            set
            {
                if (_tools == null)
                    _tools = new List<MapTool>();

                _tools.Clear();
                _tools.AddRange(value);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MapTool SelectedTool
        {
            get { return Map.ActiveTool; }
            set
            {
                if (Map.ActiveTool != value)
                {
                    onSelectedToolChangeRequest(value);
                }
            }
        }

        #endregion

        #region Control Overrides

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                if (BackgroundBeingSet)
                {
                    return;
                }

                BackgroundBeingSet = true;
                base.BackColor = value;
                BackgroundBeingSet = false;
            }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            BackgroundColor = ViewConverter.Convert(BackColor);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _mouseDownLocation = e.Location;

            if (e.Button == MouseButtons.Left) //dragging
            {
                onBeginAction(ViewConverter.Convert(e.Location));
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!_dragging && _mouseDownLocation != GdiPoint.Empty && e.Button == MouseButtons.Left)
            {
                _mouseRelativeLocation = new GdiPoint(e.X - _mouseDownLocation.X, e.Y - _mouseDownLocation.Y);

                if (!withinDragTolerance(e.Location))
                {
                    _dragging = true;
                    _mousePreviousLocation = _mouseDownLocation;
                }
            }

            if (_dragging)
            {
                onMoveTo(ViewConverter.Convert(e.Location));
                _mousePreviousLocation = e.Location;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                onEndAction(ViewConverter.Convert(e.Location));
            }

            _mouseDownLocation = GdiPoint.Empty;

            if (_dragging)
            {
                _dragging = false;
                _mouseRelativeLocation = GdiPoint.Empty;
                _mousePreviousLocation = GdiPoint.Empty;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            MapTool currentTool = SelectedTool;
            SelectedTool = e.Delta > 0 ? StandardMapTools2D.ZoomIn : StandardMapTools2D.ZoomOut;

            Rectangle2D selectBox = computeBoxFromWheelDelta(e.Location, e.Delta);
            onBeginAction(selectBox.Location);
            Point2D endPoint = new Point2D(selectBox.Right, selectBox.Bottom);
            onMoveTo(endPoint);
            onEndAction(endPoint);

            SelectedTool = currentTool;

            base.OnMouseWheel(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (DesignMode || _presenter == null)
            {
                g.Clear(BackColor);
                return;
            }

            g.Transform = getGdiViewTransform();
            g.Clear(BackColor);

            while(_renderObjectQueue.Count > 0)
            {
                GdiRenderObject ro = _renderObjectQueue.Dequeue();
                if (ro.Fill != null) g.FillPath(ro.Fill, ro.GdiPath);
                if (ro.Outline != null) g.DrawPath(ro.Outline, ro.GdiPath);
                if (ro.Image != null)
                {
                    ImageAttributes imageAttributes = null;

                    if(ro.ColorTransform != null)
                    {
                        imageAttributes = new ImageAttributes();
                        imageAttributes.SetColorMatrix(ro.ColorTransform);
                    }

                    if (imageAttributes != null)
                    {
                        g.DrawImage(ro.Image, getPoints(ro.ImageBounds), getSourceRegion(ro.Image), GraphicsUnit.Pixel, imageAttributes);
                    }
                }
            }

            g.ResetTransform();
        }

        private PointF[] getPoints(RectangleF bounds)
        {
            PointF location = bounds.Location;
            _symbolTargetPointsTransfer[0] = location;
            _symbolTargetPointsTransfer[1] = new PointF(location.X + bounds.Width, location.Y);
            _symbolTargetPointsTransfer[2] = new PointF(location.X, location.Y + bounds.Height);
            return _symbolTargetPointsTransfer;
        }

        private static Rectangle getSourceRegion(Image bitmap)
        {
            return new GdiRectangle(new GdiPoint(0, 0), bitmap.Size);
        }

        #endregion

        #region Event Invokers

        private void onBeginAction(Point2D actionLocation)
        {
            EventHandler<MapActionEventArgs<Point2D>> @event = BeginAction;

            if (@event != null)
            {
                _globalActionArgs.SetActionPoint(actionLocation);
                @event(this, _globalActionArgs);
            }
        }

        private void onHover(Point2D actionLocation)
        {
            EventHandler<MapActionEventArgs<Point2D>> @event = Hover;

            if (@event != null)
            {
                _globalActionArgs.SetActionPoint(actionLocation);
                @event(this, _globalActionArgs);
            }
        }

        private void onEndAction(Point2D actionLocation)
        {
            EventHandler<MapActionEventArgs<Point2D>> @event = EndAction;

            if (@event != null)
            {
                _globalActionArgs.SetActionPoint(actionLocation);
                @event(this, _globalActionArgs);
            }
        }

        private void onMoveTo(Point2D actionLocation)
        {
            EventHandler<MapActionEventArgs<Point2D>> @event = MoveTo;

            if (@event != null)
            {
                _globalActionArgs.SetActionPoint(actionLocation);
                @event(this, _globalActionArgs);
            }
        }

        private void onRequestBackgroundColorChange(StyleColor current, StyleColor requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<StyleColor>> e = BackgroundColorChangeRequested;

            if (e != null)
            {
                e(this, new MapViewPropertyChangeEventArgs<StyleColor>(current, requested));
            }
        }

        private void onRequestGeoCenterChange(GeoPoint current, GeoPoint requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<GeoPoint>> e = GeoCenterChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<GeoPoint> args =
                    new MapViewPropertyChangeEventArgs<GeoPoint>(current, requested);

                e(this, args);
            }
        }

        private void onRequestMaximumWorldWidthChange(double current, double requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<double>> e = MaximumWorldWidthChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<double> args =
                    new MapViewPropertyChangeEventArgs<double>(current, requested);

                e(this, args);
            }
        }

        private void onRequestMinimumWorldWidthChange(double current, double requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<double>> e = MinimumWorldWidthChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<double> args =
                    new MapViewPropertyChangeEventArgs<double>(current, requested);

                e(this, args);
            }
        }

        private void onRequestOffset(Point2D offset)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Point2D>> e = OffsetChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<Point2D> args =
                    new MapViewPropertyChangeEventArgs<Point2D>(Point2D.Zero, offset);

                e(this, args);
            }
        }

        private void onRequestViewEnvelopeChange(BoundingBox current, BoundingBox requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<BoundingBox>> e = ViewEnvelopeChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<BoundingBox> args =
                    new MapViewPropertyChangeEventArgs<BoundingBox>(current, requested);

                e(this, args);
            }
        }

        private void onRequestWorldAspectRatioChange(double current, double requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<double>> e = WorldAspectRatioChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<double> args =
                    new MapViewPropertyChangeEventArgs<double>(current, requested);

                e(this, args);
            }
        }

        private void onRequestZoomToExtents()
        {
            EventHandler e = ZoomToExtentsRequested;

            if (e != null)
            {
                e(this, EventArgs.Empty);
            }
        }

        private void onRequestZoomToViewBounds(Rectangle2D viewBounds)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Rectangle2D>> e = ZoomToViewBoundsRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<Rectangle2D> args =
                    new MapViewPropertyChangeEventArgs<Rectangle2D>(ViewConverter.Convert(ClientRectangle), viewBounds);

                e(this, args);
            }
        }

        private void onRequestZoomToWorldBounds(BoundingBox zoomBox)
        {
            EventHandler<MapViewPropertyChangeEventArgs<BoundingBox>> e = ZoomToWorldBoundsRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<BoundingBox> args =
                    new MapViewPropertyChangeEventArgs<BoundingBox>(ViewEnvelope, zoomBox);

                e(this, args);
            }
        }

        private void onRequestZoomToWorldWidth(double newWorldWidth)
        {
            EventHandler<MapViewPropertyChangeEventArgs<double>> e = ZoomToWorldWidthRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<double> args =
                    new MapViewPropertyChangeEventArgs<double>(WorldWidth, newWorldWidth);

                e(this, args);
            }
        }

        private void onSelectedToolChangeRequest(MapTool requestedTool)
        {
            EventHandler<ToolChangeRequestedEventArgs> @event = ToolChangeRequested;

            ToolChangeRequestedEventArgs args = new ToolChangeRequestedEventArgs(requestedTool);

            if (@event != null)
            {
                @event(this, args);
            }
        }

        private void onViewSizeChangeRequested(GdiSize sizeRequested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Size2D>> @event = SizeChangeRequested;

            if (@event != null)
            {
                MapViewPropertyChangeEventArgs<Size2D> args = new MapViewPropertyChangeEventArgs<Size2D>(
                    ViewConverter.Convert(Size), ViewConverter.Convert(sizeRequested));

                SizeChangeRequested(this, args);
            }
        }
        #endregion

        #region Private Helper Methods

        private GdiMatrix getGdiViewTransform()
        {
            if (_gdiViewMatrix == null)
            {
                _gdiViewMatrix = ToViewTransform == null
                    ? new GdiMatrix()
                    : ViewConverter.Convert(ToViewTransform);

                return _gdiViewMatrix;
            }

            Matrix2D viewMatrix = ToViewTransform ?? new Matrix2D();
            float[] gdiElements = _gdiViewMatrix.Elements;

            if (gdiElements[0] != (float)viewMatrix.M11
                || gdiElements[1] != (float)viewMatrix.M12
                || gdiElements[2] != (float)viewMatrix.M21
                || gdiElements[3] != (float)viewMatrix.M22
                || gdiElements[4] != (float)viewMatrix.OffsetX
                || gdiElements[5] != (float)viewMatrix.OffsetY)
            {
                Debug.WriteLine(String.Format("Disposing GDI matrix on values: {0} != {1}; {2} != {3}; {4} != {5}; {6} != {7}; {8} != {9}; {10} != {11}",
                    gdiElements[0], (float)viewMatrix.M11, gdiElements[1], (float)viewMatrix.M12, gdiElements[2], (float)viewMatrix.M21,
                    gdiElements[3], (float)viewMatrix.M22, gdiElements[4], (float)viewMatrix.OffsetX, gdiElements[5], (float)viewMatrix.OffsetY));
                _gdiViewMatrix.Dispose();
                _gdiViewMatrix = ViewConverter.Convert(ToViewTransform);
            }

            return _gdiViewMatrix;
        }

        private bool withinDragTolerance(GdiPoint point)
        {
            return Math.Abs(_mouseDownLocation.X - point.X) <= 3 && Math.Abs(_mouseDownLocation.Y - point.Y) <= 3;
        }
        private Rectangle2D computeBoxFromWheelDelta(PointF location, int deltaDegrees)
        {
            float scale = (float)Math.Pow(2, (float)Math.Abs(deltaDegrees) / 360f);
            RectangleF zoomBox = ClientRectangle;
            PointF center = new PointF((zoomBox.Width + zoomBox.Left) / 2, (zoomBox.Height + zoomBox.Top) / 2);
            PointF centerDeltaVector = new PointF(location.X - center.X, location.Y - center.Y);
            zoomBox.Offset(centerDeltaVector);
            return new Rectangle2D(zoomBox.Left, zoomBox.Top, zoomBox.Right, zoomBox.Bottom);
        }
        #endregion
    }

    #region Event Arg Classes

    public class GdiMapActionEventArgs : MapActionEventArgs<Point2D>
    {
        public GdiMapActionEventArgs()
            : base(new Point2D())
        {
        }

        public void SetActionPoint(Point2D actionLocation)
        {
            ActionPoint = actionLocation;
        }
    }

    #endregion
}
