// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using GdiPoint = System.Drawing.Point;
using GdiRectangle = System.Drawing.Rectangle;
using GdiMatrix = System.Drawing.Drawing2D.Matrix;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

using SharpMap.Layers;
using SharpMap.Data;
using SharpMap.Geometries;
using GeoPoint = SharpMap.Geometries.Point;
using SharpMap.Presentation;
using SharpMap.Rendering;
using SharpMap.Rendering.Gdi;
using SharpMap.Styles;

namespace SharpMap.Presentation.WinForms
{
    /// <summary>
    /// MapImage Class - MapImage control for Windows forms
    /// </summary>
    /// <remarks>
    /// The MapImage control adds basic functionality to a Windows Form, such as dynamic pan, zoom and data query.
    /// </remarks>
    public class MapViewControl : Control, IMapView2D, IToolsView, ISupportInitialize
    {
        private ToolSet _selectedTool;
        private GdiMatrix _viewTransform = new GdiMatrix();
        private bool _dragging = false;
        private GdiPoint _mouseDownLocation = GdiPoint.Empty;
        private GdiPoint _mouseRelativeLocation = GdiPoint.Empty;
        private GdiPoint _mousePreviousLocation = GdiPoint.Empty;
        private Queue<KeyValuePair<PointF, GdiRenderObject>> _pathQueue = new Queue<KeyValuePair<PointF, GdiRenderObject>>();
        private Queue<KeyValuePair<PointF, Image>> _tilesQueue = new Queue<KeyValuePair<PointF, Image>>();
        private readonly GdiMapActionEventArgs _globalActionArgs = new GdiMapActionEventArgs();
        private List<ToolSet> _tools;

        /// <summary>
        /// Initializes a new map
        /// </summary>
        public MapViewControl()
        {
            _selectedTool = ToolSet.None;
            this.Cursor = Cursors.Cross;

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, false);
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, false);
            SetStyle(ControlStyles.UserMouse, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        public GdiMatrix ViewTransform
        {
            get { return _viewTransform; }
            set { _viewTransform = value; }
        }

        #region IMapView2D Members

        public ViewSize2D ViewSize
        {
            get { return ViewConverter.GdiToView(Size); }
            set { Size = Size.Round(ViewConverter.ViewToGdi(value)); }
        }

        public void ShowRenderedObject(ViewPoint2D location, object renderedObject)
        {
            PointF point = ViewConverter.ViewToGdi(location);

            if (renderedObject is GdiRenderObject)
            {
                GdiRenderObject ro = (GdiRenderObject)renderedObject;
                _pathQueue.Enqueue(new KeyValuePair<PointF, GdiRenderObject>(point, ro));
            }
            else if (renderedObject is Image)
            {
                Image tile = renderedObject as Image;
                _tilesQueue.Enqueue(new KeyValuePair<PointF, Image>(point, tile));
            }
        }

        public event EventHandler<MapActionEventArgs> Hover;

        public event EventHandler<MapActionEventArgs> BeginAction;

        public event EventHandler<MapActionEventArgs> MoveTo;

        public event EventHandler<MapActionEventArgs> EndAction;

        public event EventHandler ViewSizeChanged;

        #endregion

        #region IToolsView Members

        public event EventHandler ToolSelectionChanged;

        public IList<ToolSet> Tools
        {
            get { return _tools; }
            set 
            {
                if (_tools == null)
                    _tools = new List<ToolSet>();

                _tools.Clear();
                _tools.AddRange(value);
            }
        }

        public ToolSet SelectedTool
        {
            get { return _selectedTool; }
            set 
            {
                if (_selectedTool != value)
                {
                    _selectedTool = value;
                    OnSelectedToolChanged();
                }
            }
        }
        #endregion

        #region Control Overrides
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Transform = ViewTransform;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            ToolSet currentTool = SelectedTool;
            SelectedTool = e.Delta > 0 ? ToolSet.ZoomIn : ToolSet.ZoomOut;

            ViewRectangle2D selectBox = computeBoxFromWheelDelta(e.Location, e.Delta);
            OnBeginAction(selectBox.Location);
            ViewPoint2D endPoint = new ViewPoint2D(selectBox.Right, selectBox.Bottom);
            OnMoveTo(endPoint);
            OnEndAction(endPoint);

            SelectedTool = currentTool;

            base.OnMouseWheel(e);
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
                OnMoveTo(ViewConverter.GdiToView(e.Location));
                _mousePreviousLocation = e.Location;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //dragging
            {
                _mouseDownLocation = e.Location;
                OnBeginAction(ViewConverter.GdiToView(e.Location));
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                OnEndAction(ViewConverter.GdiToView(e.Location));
            }

            _mouseDownLocation = GdiPoint.Empty;

            if (_dragging)
            {
                _dragging = false;
                _mouseRelativeLocation = GdiPoint.Empty;
                _mousePreviousLocation = GdiPoint.Empty;
            }

            base.OnMouseUp(e);
        }
        #endregion

        #region Event Invokers
        private void OnViewSizeChanged()
        {
            EventHandler @event = ViewSizeChanged;
            if (@event != null)
                ViewSizeChanged(this, EventArgs.Empty);
        }

        private void OnHover(ViewPoint2D actionLocation)
        {
            _globalActionArgs.SetActionPoint(actionLocation);
            EventHandler<MapActionEventArgs> @event = Hover;
            if (@event != null)
                @event(this, _globalActionArgs);
        }

        private void OnBeginAction(ViewPoint2D actionLocation)
        {
            _globalActionArgs.SetActionPoint(actionLocation);
            EventHandler<MapActionEventArgs> @event = BeginAction;
            if (@event != null)
                @event(this, _globalActionArgs);
        }

        private void OnMoveTo(ViewPoint2D actionLocation)
        {
            _globalActionArgs.SetActionPoint(actionLocation);
            EventHandler<MapActionEventArgs> @event = MoveTo;
            if (@event != null)
                @event(this, _globalActionArgs);
        }

        private void OnEndAction(ViewPoint2D actionLocation)
        {
            _globalActionArgs.SetActionPoint(actionLocation);
            EventHandler<MapActionEventArgs> @event = EndAction;
            if (@event != null)
                @event(this, _globalActionArgs);
        }

        private void OnSelectedToolChanged()
        {
            EventHandler @event = ToolSelectionChanged;
            if (@event != null)
                @event(this, EventArgs.Empty);
        }

        //private void OnActiveQueryToolChanged(Tools tool)
        //{
        //    EventHandler<ActiveQueryToolChangedEventArgs> @event = ActiveToolChanged;
        //    if (@event != null)
        //        @event(this, new ActiveQueryToolChangedEventArgs(tool));
        //}

        //private void OnMapMouseUp(MouseEventArgs e)
        //{
        //    EventHandler<MapMouseEventArgs> @event = MouseUp;
        //    if (@event != null)
        //        @event(this, new MapMouseEventArgs(_mapView.GdiToGeo(e.Location), e.Button, e.Clicks, e.X, e.Y, e.Delta));
        //}

        //private void OnMapMouseDown(MouseEventArgs e)
        //{
        //    EventHandler<MapMouseEventArgs> @event = MouseDown;
        //    if (@event != null)
        //        @event(this, new MapMouseEventArgs(_mapView.GdiToGeo(e.Location), e.Button, e.Clicks, e.X, e.Y, e.Delta));
        //}

        //private void OnMapMouseDrag(MouseEventArgs e)
        //{
        //    EventHandler<MapMouseEventArgs> @event = MouseDrag;
        //    if (@event != null)
        //        @event(this, new MapMouseEventArgs(_mapView.GdiToGeo(e.Location), e.Button, e.Clicks, e.X, e.Y, e.Delta));
        //}

        //private void OnMapMouseMove(MouseEventArgs e)
        //{
        //    EventHandler<MapMouseEventArgs> @event = MouseMove;
        //    if (@event != null)
        //        @event(this, new MapMouseEventArgs(_mapView.GdiToGeo(e.Location), e.Button, e.Clicks, e.X, e.Y, e.Delta));
        //}
        #endregion

        #region Private Helper Methods

        private bool withinDragTolerance(GdiPoint point)
        {
            return Math.Abs(_mouseDownLocation.X - point.X) <= 3 && Math.Abs(_mouseDownLocation.Y - point.Y) <= 3;
        }

        private ViewRectangle2D computeBoxFromWheelDelta(PointF location, int deltaDegrees)
        {
            float scale = (float)Math.Pow(2, (float)Math.Abs(deltaDegrees) / 360f);
            RectangleF zoomBox = ClientRectangle;
            PointF center = new PointF((zoomBox.Width + zoomBox.Left) / 2, (zoomBox.Height + zoomBox.Top) / 2);
            PointF centerDeltaVector = new PointF(location.X - center.X, location.Y - center.Y);
            zoomBox.Offset(centerDeltaVector);
            return new ViewRectangle2D(zoomBox.Left, zoomBox.Right, zoomBox.Top, zoomBox.Bottom);
        }

        //private void createSelectionBox(GdiPoint from, GdiPoint previousTo, GdiPoint to)
        //{
        //    //Debug.Write(String.Format("Old- W:{0}, H:{1}\t", _selectionRectangle.Width, _selectionRectangle.Height));
        //    _selectionRectangle = createRectangleFromPoints(from, to);
        //    //Debug.Write(String.Format("Now- W:{0}, H:{1}\t", _selectionRectangle.Width, _selectionRectangle.Height));

        //    //Debug.WriteLine(String.Format("P- W:{0}, H:{1}\tC- W:{2}, H:{3}", previousTo.X - from.X, previousTo.Y - from.Y, to.X - from.X, to.Y - from.Y));

        //    Rectangle redrawArea = createRectangleFromPoints(from, previousTo);

        //    if (redrawArea.Width < _selectionRectangle.Width || redrawArea.Height < _selectionRectangle.Height)
        //        redrawArea = Rectangle.Union(_selectionRectangle, redrawArea);

        //    redrawArea.Inflate(1, 1);

        //    //Debug.WriteLine(String.Format("\tS: ({0,4}, {1,4}) W:{2,4} H:{3,4}; \tC: ({4,4}, {5,4}) W:{6,4} H:{7,4};",
        //    //    _selectionRectangle.X, _selectionRectangle.Y, _selectionRectangle.Width, _selectionRectangle.Height,
        //    //    redrawArea.X, redrawArea.Y, redrawArea.Width, redrawArea.Height));
        //    Invalidate(redrawArea);
        //}

        //private void drawSelectionBox(Graphics g, Rectangle selectionRectangle)
        //{
        //    g.DrawRectangle(Pens.Black, selectionRectangle);
        //}

        //private void clearSelectionBox(GdiPoint boxCorner1, GdiPoint boxCorner2)
        //{
        //    _selectionRectangle = Rectangle.Empty;
        //    Rectangle box = createRectangleFromPoints(boxCorner1, boxCorner2);
        //    Invalidate(box);
        //}

        //private Rectangle createRectangleFromPoints(GdiPoint boxCorner1, GdiPoint boxCorner2)
        //{
        //    return Rectangle.FromLTRB(
        //        Math.Min(boxCorner1.X, boxCorner2.X),
        //        Math.Min(boxCorner1.Y, boxCorner2.Y),
        //        Math.Max(boxCorner1.X, boxCorner2.X),
        //        Math.Max(boxCorner1.Y, boxCorner2.Y));
        //}
        #endregion

        #region ISupportInitialize Members

        public void BeginInit()
        {
        }

        public void EndInit()
        {
        }

        #endregion
    }

    #region Event Arg Classes

    public class GdiMapActionEventArgs : MapActionEventArgs
    {
        public GdiMapActionEventArgs()
            : base(new ViewPoint2D()) { }

        public void SetActionPoint(ViewPoint2D actionLocation)
        {
            ActionPoint = actionLocation;
        }
    }

    //public class MapMouseEventArgs : MouseEventArgs
    //{
    //    private GeoPoint _worldLocation;

    //    internal MapMouseEventArgs(GeoPoint worldLocation, MouseButtons buttons, int clicks, int x, int y, int delta)
    //        : base(buttons, clicks, x, y, delta)
    //    {
    //        _worldLocation = worldLocation;
    //    }

    //    public GeoPoint WorldLocation
    //    {
    //        get { return _worldLocation; }
    //    }

    //    public GdiPoint ImageLocation
    //    {
    //        get { return this.Location; }
    //    }
    //}

    //public class MapQueryEventArgs : EventArgs
    //{
    //    private IEnumerable<FeatureDataRow> _data;

    //    internal MapQueryEventArgs(IEnumerable<FeatureDataRow> queriedFeatures)
    //    {
    //        _data = queriedFeatures;
    //    }

    //    public IEnumerable<FeatureDataRow> QueriedFeatures
    //    {
    //        get { return _data; }
    //    }
    //}

    //public class ActiveQueryToolChangedEventArgs : EventArgs
    //{
    //    private Tools _tool;

    //    internal ActiveQueryToolChangedEventArgs(Tools tool)
    //    {
    //        _tool = tool;
    //    }

    //    public Tools Tool
    //    {
    //        get { return _tool; }
    //        set { _tool = value; }
    //    }
    //}
    #endregion
}
