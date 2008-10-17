// Portions copyright 2005 - 2006: Morten Nielsen (www.iter.dk)
// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using SharpMap.Presentation.Views;
using SharpMap.Rendering;
using SharpMap.Rendering.Gdi;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Tools;
using GdiPoint = System.Drawing.Point;
using GdiSize = System.Drawing.Size;
using GdiRectangle = System.Drawing.Rectangle;
using GdiColorMatrix = System.Drawing.Imaging.ColorMatrix;
using GdiPath = System.Drawing.Drawing2D.GraphicsPath;
using GdiMatrix = System.Drawing.Drawing2D.Matrix;

namespace SharpMap.Presentation.WinForms
{
    /// <summary>
    /// Provides a WinForms control for viewing maps.
    /// </summary>
    public class MapViewControl : Control, IMapView2D, IToolsView
    {
        private IMapTool _selectedTool;
        private readonly Double _dpi;
        private Boolean _dragging;
        private GdiPoint _mouseDownLocation = GdiPoint.Empty;
        private GdiPoint _mouseRelativeLocation = GdiPoint.Empty;
        private GdiPoint _mousePreviousLocation = GdiPoint.Empty;
        private readonly Queue<GdiRenderObject> _renderObjectQueue = new Queue<GdiRenderObject>();
        private readonly GdiMapActionEventArgs _globalActionArgs = new GdiMapActionEventArgs();
        private MapToolSet _tools;
        private MapPresenter _presenter;
        private GdiMatrix _gdiViewMatrix;
        private readonly StringFormat _format;
        private readonly PointF[] _symbolTargetPointsTransfer = new PointF[3];
        private Boolean _backgroundBeingSet;
        private readonly Label _infoLabel = new Label();
        private Bitmap _bufferedMapImage;

        /// <summary>
        /// Initializes a new WinForms map view control.
        /// </summary>
        public MapViewControl()
        {
            using (Graphics g = CreateGraphics())
            {
                _dpi = g.DpiX;
            }

            _format = new StringFormat(StringFormat.GenericTypographic);
            _format.Alignment = StringAlignment.Near;
            _format.LineAlignment = StringAlignment.Center;

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, false);
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, false);
            SetStyle(ControlStyles.UserMouse, true);
            SetStyle(ControlStyles.UserPaint, true);

            _infoLabel.Dock = DockStyle.Bottom;
            _infoLabel.Height = 12;
            _infoLabel.BackColor = Color.FromArgb(128,
                                                  base.BackColor.R,
                                                  base.BackColor.G,
                                                  base.BackColor.B);

            Controls.Add(_infoLabel);
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (Map != null)
                {
                    Map.Dispose();
                }

                if (_bufferedMapImage != null)
                {
                    _bufferedMapImage.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Sets the map to display.
        /// </summary>
        public Map Map
        {
            private get { return _presenter == null ? null : _presenter.Map; }
            set { _presenter = new MapPresenter(value, this); }
        }

        internal Boolean BackgroundBeingSet
        {
            get { return _backgroundBeingSet; }
            set { _backgroundBeingSet = value; }
        }

        internal String Information
        {
            get { return _infoLabel.Text; }
            set { _infoLabel.Text = value; }
        }

        #region IView Members

        public String Title
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

        public event EventHandler<MapViewPropertyChangeEventArgs<StyleColor>>
            BackgroundColorChangeRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<ICoordinate>>
            GeoCenterChangeRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<Double>>
            MaximumWorldWidthChangeRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<Double>>
            MinimumWorldWidthChangeRequested;

        public event EventHandler<LocationEventArgs> IdentifyLocationRequested;
        public event EventHandler<MapViewPropertyChangeEventArgs<Point2D>> OffsetChangeRequested;
        public new event EventHandler SizeChanged;

        public event EventHandler<MapViewPropertyChangeEventArgs<IExtents2D>>
            ViewEnvelopeChangeRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<Double>>
            WorldAspectRatioChangeRequested;

        public event EventHandler ZoomToExtentsRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<Rectangle2D>>
            ZoomToViewBoundsRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<IExtents2D>>
            ZoomToWorldBoundsRequested;

        public event EventHandler<MapViewPropertyChangeEventArgs<Double>> ZoomToWorldWidthRequested;

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
        public Double Dpi
        {
            get { return _dpi; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICoordinate GeoCenter
        {
            get { return _presenter.GeoCenter; }
            set { onRequestGeoCenterChange(GeoCenter, value); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Double MaximumWorldWidth
        {
            get { return _presenter.MaximumWorldWidth; }
            set { onRequestMaximumWorldWidthChange(MaximumWorldWidth, value); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Double MinimumWorldWidth
        {
            get { return _presenter.MinimumWorldWidth; }
            set { onRequestMinimumWorldWidthChange(MinimumWorldWidth, value); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Double PixelWorldWidth
        {
            get { return _presenter.PixelWorldWidth; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Double PixelWorldHeight
        {
            get { return _presenter.PixelWorldHeight; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ViewSelection2D Selection
        {
            get { return _presenter.Selection; }
        }

        public Point2D ToView(ICoordinate point)
        {
            return _presenter.ToView(point);
        }

        public Point2D ToView(Double x, Double y)
        {
            return _presenter.ToView(x, y);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Matrix2D ToViewTransform
        {
            get { return _presenter.ToViewTransform; }
        }

        public ICoordinate ToWorld(Point2D point)
        {
            return _presenter.ToWorld(point);
        }

        public ICoordinate ToWorld(Double x, Double y)
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
        public IExtents2D ViewEnvelope
        {
            get { return _presenter.ViewEnvelope; }
            set
            {
                onRequestViewEnvelopeChange(ViewEnvelope, value);
                ;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size2D ViewSize
        {
            get { return ViewConverter.Convert(Size); }
            set { Size = Size.Round(ViewConverter.Convert(value)); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Double WorldAspectRatio
        {
            get { return _presenter.WorldAspectRatio; }
            set { onRequestWorldAspectRatioChange(WorldAspectRatio, value); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Double WorldHeight
        {
            get { return _presenter.WorldHeight; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Double WorldWidth
        {
            get { return _presenter.WorldWidth; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Double WorldUnitsPerPixel
        {
            get { return _presenter.WorldUnitsPerPixel; }
        }

        #endregion

        #region Methods

        public void IdentifyLocation(ICoordinate location)
        {
            onRequestIdentifyLocation(location);
        }

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
            if (renderedObjects == null)
            {
                throw new ArgumentNullException("renderedObjects");
            }

            foreach (GdiRenderObject ro in renderedObjects)
            {
                GdiRenderObject go = ro;
                _renderObjectQueue.Enqueue(go);
            }
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

        public void ZoomToWorldBounds(IExtents2D zoomBox)
        {
            onRequestZoomToWorldBounds(zoomBox);
        }

        public void ZoomToWorldWidth(Double newWorldWidth)
        {
            onRequestZoomToWorldWidth(newWorldWidth);
        }

        #endregion

        #endregion

        #region IToolsView Members

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IMapToolSet Tools
        {
            get { return _tools; }
            set
            {
                if (_tools == null)
                {
                    _tools = new MapToolSet("Map View Control Toolset", value);
                }
                else
                {
                    _tools.Clear();

                    foreach (IMapTool mapTool in value)
                    {
                        _tools.Add(mapTool);
                    }
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IMapTool SelectedTool
        {
            get { return _selectedTool; }
            set { _selectedTool = value; }
        }

        #endregion

        #region Control Overrides

        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                if (BackgroundBeingSet)
                {
                    return;
                }

                BackgroundBeingSet = true;
                base.BackColor = value;
                _infoLabel.BackColor = Color.FromArgb(128, value.R, value.G, value.B);
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

            if (e.Button == MouseButtons.Left) // dragging
            {
                onBeginAction(ViewConverter.Convert(e.Location));
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!_dragging && _mouseDownLocation != Point.Empty && e.Button == MouseButtons.Left)
            {
                _mouseRelativeLocation = new Point(e.X - _mouseDownLocation.X,
                                                   e.Y - _mouseDownLocation.Y);

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
            else
            {
                onHover(ViewConverter.Convert(e.Location));
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

            _mouseDownLocation = Point.Empty;

            if (_dragging)
            {
                _dragging = false;
                _mouseRelativeLocation = Point.Empty;
                _mousePreviousLocation = Point.Empty;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            throw new NotImplementedException();

            //MapTool currentTool = SelectedTool;

            //SelectedTool = e.Delta > 0
            //                   ? StandardMapTools2D.ZoomOut
            //                   : StandardMapTools2D.ZoomIn;

            //onEndAction(ViewConverter.Convert(e.Location));

            //if (currentTool != StandardMapTools2D.None)
            //{
            //    SelectedTool = currentTool;
            //}

            //base.OnMouseWheel(e);

            // **************** Using the tool path...

            //SelectedTool = e.Delta > 0 ? StandardMapTools2D.ZoomIn : StandardMapTools2D.ZoomOut;

            //Rectangle2D selectBox = computeBoxFromWheelDelta(e.Location, e.Delta);
            //onBeginAction(selectBox.Location);
            //Point2D endPoint = new Point2D(selectBox.Right, selectBox.Bottom);
            //onMoveTo(endPoint);
            //onEndAction(endPoint);
            //SelectedTool = currentTool;

            //base.OnMouseWheel(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Graphics screenGraphics = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (DesignMode || _presenter == null)
            {
                g.Clear(BackColor);
                return;
            }

            // dump to screen and return
            if (_bufferedMapImage != null)
            {
                g.Clear(BackColor);

                g.DrawImageUnscaled(_bufferedMapImage,
                                    (Int32)((Width - _bufferedMapImage.Width) / 2.0f),
                                    (Int32)((Height - _bufferedMapImage.Height) / 2.0f));

                if (_renderObjectQueue.Count == 0)
                {
                    return;
                }
            }

            if (_bufferedMapImage != null &&
                (_bufferedMapImage.Width != Width || _bufferedMapImage.Height != Height))
            {
                _bufferedMapImage.Dispose();
                _bufferedMapImage = null;
            }

            if (_bufferedMapImage == null)
            {
                _bufferedMapImage = new Bitmap(Width, Height);
            }

            g = Graphics.FromImage(_bufferedMapImage);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.Transform = getGdiViewTransform();

            if (!_presenter.IsRenderingSelection)
            {
                g.Clear(BackColor);
            }

            while (_renderObjectQueue.Count > 0)
            {
                GdiRenderObject ro = _renderObjectQueue.Dequeue();

                if (ro.State == RenderState.Unknown)
                {
                    continue;
                }
               //gdi crashes when trying to draw objects at 0 width and height
                    switch (ro.State)
                    {
                        case RenderState.Normal:
                            if (ro.GdiPath != null)
                            {
                                if (ro.Line != null)
                                {
                                    if (ro.Outline != null)
                                    {
                                        g.DrawPath(ro.Outline, ro.GdiPath);
                                    }

                                    g.DrawPath(ro.Line, ro.GdiPath);
                                }
                                else if (ro.Fill != null)
                                {
                                    g.FillPath(ro.Fill, ro.GdiPath);

                                    if (ro.Outline != null)
                                    {
                                        g.DrawPath(ro.Outline, ro.GdiPath);
                                    }
                                }
                            }

                            if (ro.Text != null)
                            {
                                RectangleF newBounds = AdjustForLabel(g, ro);
                                g.DrawString(ro.Text, ro.Font, ro.Fill, newBounds.Location);
                                g.Transform = getGdiViewTransform();
                            }

                            break;
                        case RenderState.Highlighted:
                            if (ro.GdiPath != null)
                            {
                                if (ro.HighlightLine != null)
                                {
                                    if (ro.HighlightOutline != null)
                                    {
                                        g.DrawPath(ro.HighlightOutline, ro.GdiPath);
                                    }

                                    g.DrawPath(ro.HighlightLine, ro.GdiPath);
                                }
                                else if (ro.HighlightFill != null)
                                {
                                    g.FillPath(ro.HighlightFill, ro.GdiPath);

                                    if (ro.HighlightOutline != null)
                                    {
                                        g.DrawPath(ro.HighlightOutline, ro.GdiPath);
                                    }
                                }
                            }

                            if (ro.Text != null)
                            {
                                RectangleF newBounds = AdjustForLabel(g, ro);
                                g.DrawString(ro.Text, ro.Font, ro.HighlightFill, newBounds);
                                g.Transform = getGdiViewTransform();
                            }

                            break;
                        case RenderState.Selected:
                            if (ro.GdiPath != null)
                            {
                                if (ro.SelectLine != null)
                                {
                                    if (ro.SelectOutline != null)
                                    {
                                        g.DrawPath(ro.SelectOutline, ro.GdiPath);
                                    }

                                    g.DrawPath(ro.SelectLine, ro.GdiPath);
                                }
                                else if (ro.SelectFill != null)
                                {
                                    g.FillPath(ro.SelectFill, ro.GdiPath);

                                    if (ro.SelectOutline != null)
                                    {
                                        g.DrawPath(ro.SelectOutline, ro.GdiPath);
                                    }
                                }
                            }

                            if (ro.Text != null)
                            {
                                RectangleF newBounds = AdjustForLabel(g, ro);
                                g.DrawString(ro.Text, ro.Font, ro.SelectFill, newBounds);
                                g.Transform = getGdiViewTransform();
                            }
                            break;
                        case RenderState.Unknown:
                        default:
                            break;
                    }

                if (ro.Image != null)
                {
                    ImageAttributes imageAttributes = null;

                    if (ro.ColorTransform != null)
                    {
                        imageAttributes = new ImageAttributes();
                        imageAttributes.SetColorMatrix(ro.ColorTransform);
                    }

                    if (imageAttributes != null)
                    {
                        g.DrawImage(ro.Image,
                                    getPoints(ro.Bounds),
                                    getSourceRegion(ro.Image),
                                    GraphicsUnit.Pixel,
                                    imageAttributes);
                    }
                    else
                    {
                        g.DrawImage(ro.Image,
                                    getPoints(ro.Bounds),
                                    getSourceRegion(ro.Image),
                                    GraphicsUnit.Pixel);
                    }
                }
            }

            g.ResetTransform();

            if (g != screenGraphics)
            {
                screenGraphics.DrawImageUnscaled(_bufferedMapImage, 0, 0);
            }
        }

        /// <summary>
        /// Without this change, labels render upside down and don't all scale readably.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="ro"></param>
        /// <returns></returns>
        protected RectangleF AdjustForLabel(Graphics g, GdiRenderObject ro)
        {
            // this transform goes from the underlying coordinates to 
            // screen coordinates, but for some reason renders text upside down
            // we cannot just scale by 1, -1 because offsets are affected also
            GdiMatrix m = g.Transform;
            // used to scale text size for the current zoom level
            float scale = Math.Abs(m.Elements[0]);

            // get the bounds of the label in the underlying coordinate space
            Point ll = new Point((Int32)ro.Bounds.X, (Int32)ro.Bounds.Y);
            Point ur = new Point((Int32)(ro.Bounds.X + ro.Bounds.Width),
                                 (Int32)(ro.Bounds.Y + ro.Bounds.Height));

            Point[] transformedPoints1 =
                {
                    new Point((Int32) ro.Bounds.X, (Int32) ro.Bounds.Y),
                    new Point((Int32) (ro.Bounds.X + ro.Bounds.Width),
                              (Int32) (ro.Bounds.Y + ro.Bounds.Height))
                };

            // get the label bounds transformed into screen coordinates
            // note that if we just render this as-is the label is upside down
            m.TransformPoints(transformedPoints1);

            // for labels, we're going to use an identity matrix and screen coordinates
            GdiMatrix newM = new GdiMatrix();

            Boolean scaleText = true;

            /*
                        if (ro.Layer != null)
                        {
                            Double min = ro.Layer.Style.MinVisible;
                            Double max = ro.Layer.Style.MaxVisible;
                            float scaleMult = Double.IsInfinity(max) ? 2.0f : 1.0f;

                            //max = Math.Min(max, _presenter.MaximumWorldWidth);
                            max = Math.Min(max, Map.Extents.Width);
                            //Double pct = (max - _presenter.WorldWidth) / (max - min);
                            Double pct = 1 - (Math.Min(_presenter.WorldWidth, Map.Extents.Width) - min) / (max - min);

                            if (scaleMult > 1)
                            {
                                pct = Math.Max(.5, pct * 2);
                            }

                            scale = (float)pct*scaleMult;
                            labelScale = scale;
                        }
            */

            // ok, I lied, if we're scaling labels we need to scale our new matrix, but still no offsets
            if (scaleText)
            {
                newM.Scale(scale, scale);
            }
            else
            {
                scale = 1.0f;
            }

            g.Transform = newM;

            Int32 pixelWidth = ur.X - ll.X;
            Int32 pixelHeight = ur.Y - ll.Y;

            // if we're scaling text, then x,y position will get multiplied by our 
            // scale, so adjust for it here so that we can use actual pixel x,y
            // Also center our label on the coordinate instead of putting the label origin on the coordinate
            RectangleF newBounds = new RectangleF(transformedPoints1[0].X / scale,
                                                  (transformedPoints1[0].Y / scale) - pixelHeight,
                                                  pixelWidth,
                                                  pixelHeight);
            //RectangleF newBounds = new RectangleF(transformedPoints1[0].X / scale - (pixelWidth / 2), transformedPoints1[0].Y / scale - (pixelHeight / 2), pixelWidth, pixelHeight);

            return newBounds;
        }

        protected override void OnSizeChanged(EventArgs args)
        {
            EventHandler e = SizeChanged;

            _infoLabel.Visible = false;

            if (e != null)
            {
                e(this, args);
            }

            _infoLabel.Visible = true;
        }

        #endregion

        #region Event Invokers

        private void onBeginAction(Point2D actionLocation)
        {
            EventHandler<MapActionEventArgs<Point2D>> e = BeginAction;

            if (e != null)
            {
                _globalActionArgs.SetActionPoint(actionLocation);
                e(this, _globalActionArgs);
            }
        }

        private void onHover(Point2D actionLocation)
        {
            EventHandler<MapActionEventArgs<Point2D>> e = Hover;

            if (e != null)
            {
                _globalActionArgs.SetActionPoint(actionLocation);
                e(this, _globalActionArgs);
            }
        }

        private void onEndAction(Point2D actionLocation)
        {
            EventHandler<MapActionEventArgs<Point2D>> e = EndAction;

            if (e != null)
            {
                _globalActionArgs.SetActionPoint(actionLocation);
                e(this, _globalActionArgs);
            }
        }

        private void onMoveTo(Point2D actionLocation)
        {
            EventHandler<MapActionEventArgs<Point2D>> e = MoveTo;

            if (e != null)
            {
                _globalActionArgs.SetActionPoint(actionLocation);
                e(this, _globalActionArgs);
            }
        }

        private void onRequestBackgroundColorChange(StyleColor current, StyleColor requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<StyleColor>> e =
                BackgroundColorChangeRequested;

            if (e != null)
            {
                e(this, new MapViewPropertyChangeEventArgs<StyleColor>(current, requested));
            }
        }

        private void onRequestGeoCenterChange(ICoordinate current, ICoordinate requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<ICoordinate>> e = GeoCenterChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<ICoordinate> args =
                    new MapViewPropertyChangeEventArgs<ICoordinate>(current, requested);

                e(this, args);
            }
        }

        private void onRequestMaximumWorldWidthChange(Double current, Double requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Double>> e =
                MaximumWorldWidthChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<Double> args =
                    new MapViewPropertyChangeEventArgs<Double>(current, requested);

                e(this, args);
            }
        }

        private void onRequestMinimumWorldWidthChange(Double current, Double requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Double>> e =
                MinimumWorldWidthChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<Double> args =
                    new MapViewPropertyChangeEventArgs<Double>(current, requested);

                e(this, args);
            }
        }

        private void onRequestIdentifyLocation(ICoordinate location)
        {
            EventHandler<LocationEventArgs> e = IdentifyLocationRequested;

            if (e != null)
            {
                LocationEventArgs args = new LocationEventArgs(location);
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

        private void onRequestViewEnvelopeChange(IExtents2D current, IExtents2D requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<IExtents2D>> e = ViewEnvelopeChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<IExtents2D> args =
                    new MapViewPropertyChangeEventArgs<IExtents2D>(current, requested);

                e(this, args);
            }
        }

        private void onRequestWorldAspectRatioChange(Double current, Double requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Double>> e = WorldAspectRatioChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<Double> args =
                    new MapViewPropertyChangeEventArgs<Double>(current, requested);

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
                    new MapViewPropertyChangeEventArgs<Rectangle2D>(
                        ViewConverter.Convert(ClientRectangle), viewBounds);

                e(this, args);
            }
        }

        private void onRequestZoomToWorldBounds(IExtents2D zoomBox)
        {
            EventHandler<MapViewPropertyChangeEventArgs<IExtents2D>> e = ZoomToWorldBoundsRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<IExtents2D> args =
                    new MapViewPropertyChangeEventArgs<IExtents2D>(ViewEnvelope, zoomBox);

                e(this, args);
            }
        }

        private void onRequestZoomToWorldWidth(Double newWorldWidth)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Double>> e = ZoomToWorldWidthRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<Double> args =
                    new MapViewPropertyChangeEventArgs<Double>(WorldWidth, newWorldWidth);

                e(this, args);
            }
        }

        #endregion

        #region Private Helper Methods

        private PointF[] getPoints(RectangleF bounds)
        {
            // NOTE: This flips the image along the x-axis at the image's center
            // in order to compensate for the Transform which is in effect 
            // on the Graphics object during OnPaint
            PointF location = bounds.Location;
            _symbolTargetPointsTransfer[0] = new PointF(location.X, location.Y + bounds.Height);
            _symbolTargetPointsTransfer[1] = new PointF(location.X + bounds.Width,
                                                        location.Y + bounds.Height);
            _symbolTargetPointsTransfer[2] = location;
            return _symbolTargetPointsTransfer;
        }

        private static Rectangle getSourceRegion(Image bitmap)
        {
            return new Rectangle(new Point(0, 0), bitmap.Size);
        }

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
            Single[] gdiElements = _gdiViewMatrix.Elements;

            if (gdiElements[0] != (Single)viewMatrix.M11
                || gdiElements[1] != (Single)viewMatrix.M12
                || gdiElements[2] != (Single)viewMatrix.M21
                || gdiElements[3] != (Single)viewMatrix.M22
                || gdiElements[4] != (Single)viewMatrix.OffsetX
                || gdiElements[5] != (Single)viewMatrix.OffsetY)
            {
                Debug.WriteLine(
                    String.Format(
                        "Disposing GDI matrix on values: {0} : {1}; {2} : {3}; {4} : {5}; {6} : {7}; {8} : {9}; {10} : {11}",
                        gdiElements[0],
                        (Single)viewMatrix.M11,
                        gdiElements[1],
                        (Single)viewMatrix.M12,
                        gdiElements[2],
                        (Single)viewMatrix.M21,
                        gdiElements[3],
                        (Single)viewMatrix.M22,
                        gdiElements[4],
                        (Single)viewMatrix.OffsetX,
                        gdiElements[5],
                        (Single)viewMatrix.OffsetY));

                _gdiViewMatrix.Dispose();
                _gdiViewMatrix = ViewConverter.Convert(ToViewTransform);
            }

            return _gdiViewMatrix;
        }

        private Boolean withinDragTolerance(Point point)
        {
            return Math.Abs(_mouseDownLocation.X - point.X) <= 3
                   && Math.Abs(_mouseDownLocation.Y - point.Y) <= 3;
        }

        private Rectangle2D computeBoxFromWheelDelta(PointF location, Int32 deltaDegrees)
        {
            Single scale = (Single)Math.Pow(2, Math.Abs(deltaDegrees) / 360f);
            RectangleF zoomBox = ClientRectangle;
            PointF center = new PointF((zoomBox.Width + zoomBox.Left) / 2,
                                       (zoomBox.Height + zoomBox.Top) / 2);
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
            : base(new Point2D()) { }

        public void SetActionPoint(Point2D actionLocation)
        {
            ActionPoint = actionLocation;
        }
    }

    #endregion
}