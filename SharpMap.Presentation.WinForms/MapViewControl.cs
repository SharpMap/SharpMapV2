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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using GdiPoint = System.Drawing.Point;
using GdiRectangle = System.Drawing.Rectangle;
using GdiMatrix = System.Drawing.Drawing2D.Matrix;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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
using SharpMap.Tools;

namespace SharpMap.Presentation.WinForms
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class MapViewControl : Control, IMapView2D, IToolsView, ISupportInitialize
	{
		private double _dpi = 96;
		private MapViewPort2D _viewPort;
        private MapTool _selectedTool;
        private bool _dragging = false;
        private GdiPoint _mouseDownLocation = GdiPoint.Empty;
        private GdiPoint _mouseRelativeLocation = GdiPoint.Empty;
        private GdiPoint _mousePreviousLocation = GdiPoint.Empty;
        private Queue<KeyValuePair<PointF, GdiRenderObject>> _pathQueue = new Queue<KeyValuePair<PointF, GdiRenderObject>>();
        private Queue<KeyValuePair<PointF, Image>> _tilesQueue = new Queue<KeyValuePair<PointF, Image>>();
        private readonly GdiMapActionEventArgs _globalActionArgs = new GdiMapActionEventArgs();
        private List<MapTool> _tools;

        /// <summary>
        /// Initializes a new map
        /// </summary>
        public MapViewControl()
		{
			using (Graphics g = CreateGraphics())
			{
				_dpi = g.DpiX;
			}

            _selectedTool = MapTool.None;
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

        //public GdiMatrix ViewTransform
        //{
        //    get { return _viewTransform; }
        //    set { _viewTransform = value; }
		//}

		#region IView Members
		public string Title
		{
			get { return Name; }
			set { Name = value; }
		}

		#endregion

		#region IMapView2D Members

		public MapViewPort2D ViewPort
		{
			get { return _viewPort; }
			set { _viewPort = value;  }
		}

		[DesignTimeVisible(false)]
		public double Dpi
		{
			get { return _dpi; }
		}

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

        public event EventHandler<MapActionEventArgs<ViewPoint2D>> Hover;

		public event EventHandler<MapActionEventArgs<ViewPoint2D>> BeginAction;

		public event EventHandler<MapActionEventArgs<ViewPoint2D>> MoveTo;

		public event EventHandler<MapActionEventArgs<ViewPoint2D>> EndAction;

        public event EventHandler ViewSizeChanged;

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
            get { return _selectedTool; }
            set 
            {
                if (_selectedTool != value)
                {
                    OnSelectedToolChanged(value);
                }
            }
        }
        #endregion

        #region Control Overrides
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //g.Transform = ViewTransform;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            MapTool currentTool = SelectedTool;
            SelectedTool = e.Delta > 0 ? MapTool.ZoomIn : MapTool.ZoomOut;

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

            EventHandler<MapActionEventArgs<ViewPoint2D>> @event = Hover;

            if (@event != null)
                @event(this, _globalActionArgs);
        }

        private void OnBeginAction(ViewPoint2D actionLocation)
        {
            _globalActionArgs.SetActionPoint(actionLocation);

			EventHandler<MapActionEventArgs<ViewPoint2D>> @event = BeginAction;

            if (@event != null)
                @event(this, _globalActionArgs);
        }

        private void OnMoveTo(ViewPoint2D actionLocation)
        {
            _globalActionArgs.SetActionPoint(actionLocation);

			EventHandler<MapActionEventArgs<ViewPoint2D>> @event = MoveTo;

            if (@event != null)
                @event(this, _globalActionArgs);
        }

        private void OnEndAction(ViewPoint2D actionLocation)
        {
            _globalActionArgs.SetActionPoint(actionLocation);

			EventHandler<MapActionEventArgs<ViewPoint2D>> @event = EndAction;

            if (@event != null)
                @event(this, _globalActionArgs);
        }

        private void OnSelectedToolChangeRequest(MapTool requestedTool)
        {
            EventHandler<ToolChangeRequestedEventArgs> @event = ToolChangeRequested;

			ToolChangeRequestedEventArgs args = new ToolChangeRequestedEventArgs(requestedTool);

			if (@event != null)
			{
				@event(this, args);
			}
        }
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

    public class GdiMapActionEventArgs : MapActionEventArgs<ViewPoint2D>
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
