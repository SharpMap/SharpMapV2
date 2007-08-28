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
using System.Windows.Forms;
using SharpMap.Geometries;
using SharpMap.Rendering.Gdi;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Tools;
using GdiPoint = System.Drawing.Point;
using GdiSize = System.Drawing.Size;
using GdiRectangle = System.Drawing.Rectangle;
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
		private Queue<KeyValuePair<PointF, GdiRenderObject>> _pathQueue = new Queue<KeyValuePair<PointF, GdiRenderObject>>();
		private Queue<KeyValuePair<PointF, Image>> _tilesQueue = new Queue<KeyValuePair<PointF, Image>>();
		private readonly GdiMapActionEventArgs _globalActionArgs = new GdiMapActionEventArgs();
		private List<MapTool> _tools;
		private MapPresenter _presenter;
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

			Cursor = Cursors.Cross;

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
                OnRequestBackgroundColorChange(BackgroundColor, value);
                BackgroundBeingSet = false;
            }
        }

        [Browsable(false)]
        public double Dpi
        {
            get { return _dpi; }
        }

        [Browsable(false)]
        public GeoPoint GeoCenter
        {
            get { return _presenter.GeoCenter; }
            set { OnRequestGeoCenterChange(GeoCenter, value); }
        }

        [Browsable(false)]
        public double MaximumWorldWidth
        {
            get { return _presenter.MaximumWorldWidth; }
            set { OnRequestMaximumWorldWidthChange(MaximumWorldWidth, value); }
        }

        [Browsable(false)]
        public double MinimumWorldWidth
        {
            get { return _presenter.MinimumWorldWidth; }
            set { OnRequestMinimumWorldWidthChange(MinimumWorldWidth, value); }
        }

        [Browsable(false)]
        public double PixelWorldWidth
        {
            get { return _presenter.PixelWorldWidth; }
        }

        [Browsable(false)]
        public double PixelWorldHeight
        {
            get { return _presenter.PixelWorldHeight; }
        }

        [Browsable(false)]
        public ViewSelection2D Selection
        {
            get { return _presenter.Selection; }
        }

        [Browsable(false)]
        public Matrix2D ToViewTransform
        {
            get { return _presenter.ToViewTransform; }
        }

        [Browsable(false)]
        public Matrix2D ToWorldTransform
        {
            get { return _presenter.ToWorldTransform; }
        }

        [Browsable(false)]
        public BoundingBox ViewEnvelope
        {
            get { return _presenter.ViewEnvelope; }
            set { OnRequestViewEnvelopeChange(ViewEnvelope, value); ; }
        }

        [Browsable(false)]
        public Size2D ViewSize
        {
            get { return ViewConverter.Convert(Size); }
            set { Size = GdiSize.Round(ViewConverter.Convert(value)); }
        }

        [Browsable(false)]
        public double WorldAspectRatio
        {
            get { return _presenter.WorldAspectRatio; }
            set { OnRequestWorldAspectRatioChange(WorldAspectRatio, value); }
        }

        [Browsable(false)]
        public double WorldHeight
        {
            get { return _presenter.WorldHeight; }
        }

        [Browsable(false)]
        public double WorldWidth
        {
            get { return _presenter.WorldWidth; }
        }

        [Browsable(false)]
        public double WorldUnitsPerPixel
        {
            get { return _presenter.WorldUnitsPerPixel; }
        } 
        #endregion

        #region Methods

        public void ShowRenderedObject(Point2D location, object renderedObject)
        {
            PointF point = ViewConverter.Convert(location);

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

        public void ZoomToExtents()
        {
            OnRequestZoomToExtents();
        }

        public void ZoomToViewBounds(Rectangle2D viewBounds)
        {
            OnRequestZoomToViewBounds(viewBounds);
        }

        public void ZoomToWorldBounds(BoundingBox zoomBox)
        {
            OnRequestZoomToWorldBounds(zoomBox);
        }

        public void ZoomToWorldWidth(double newWorldWidth)
        {
            OnRequestZoomToWorldWidth(newWorldWidth);
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
					OnSelectedToolChangeRequest(value);
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

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			//g.Transform = ViewTransform;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			MapTool currentTool = SelectedTool;
			SelectedTool = e.Delta > 0 ? StandardMapTools2D.ZoomIn : StandardMapTools2D.ZoomOut;

			Rectangle2D selectBox = computeBoxFromWheelDelta(e.Location, e.Delta);
			OnBeginAction(selectBox.Location);
			Point2D endPoint = new Point2D(selectBox.Right, selectBox.Bottom);
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
				OnMoveTo(ViewConverter.Convert(e.Location));
				_mousePreviousLocation = e.Location;
			}

			base.OnMouseMove(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) //dragging
			{
				_mouseDownLocation = e.Location;
				OnBeginAction(ViewConverter.Convert(e.Location));
			}

			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				OnEndAction(ViewConverter.Convert(e.Location));
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

		protected virtual void OnViewSizeChangeRequested(GdiSize sizeRequested)
		{
			EventHandler<MapViewPropertyChangeEventArgs<Size2D>> @event = SizeChangeRequested;

			if (@event != null)
			{
				MapViewPropertyChangeEventArgs<Size2D> args = new MapViewPropertyChangeEventArgs<Size2D>(
					ViewConverter.Convert(Size), ViewConverter.Convert(sizeRequested));

				SizeChangeRequested(this, args);
			}
		}

		protected virtual void OnHover(Point2D actionLocation)
		{
			_globalActionArgs.SetActionPoint(actionLocation);

			EventHandler<MapActionEventArgs<Point2D>> @event = Hover;

			if (@event != null)
			{
				@event(this, _globalActionArgs);
			}
		}

		protected virtual void OnBeginAction(Point2D actionLocation)
		{
			_globalActionArgs.SetActionPoint(actionLocation);

			EventHandler<MapActionEventArgs<Point2D>> @event = BeginAction;

			if (@event != null)
				@event(this, _globalActionArgs);
		}

		protected virtual void OnMoveTo(Point2D actionLocation)
		{
			_globalActionArgs.SetActionPoint(actionLocation);

			EventHandler<MapActionEventArgs<Point2D>> @event = MoveTo;

			if (@event != null)
			{
				@event(this, _globalActionArgs);
			}
		}

		protected virtual void OnEndAction(Point2D actionLocation)
		{
			_globalActionArgs.SetActionPoint(actionLocation);

			EventHandler<MapActionEventArgs<Point2D>> @event = EndAction;

			if (@event != null)
			{
				@event(this, _globalActionArgs);
			}
		}

		protected virtual void OnSelectedToolChangeRequest(MapTool requestedTool)
		{
			EventHandler<ToolChangeRequestedEventArgs> @event = ToolChangeRequested;

			ToolChangeRequestedEventArgs args = new ToolChangeRequestedEventArgs(requestedTool);

			if (@event != null)
			{
				@event(this, args);
			}
		}


		protected virtual void OnRequestBackgroundColorChange(StyleColor current, StyleColor requested)
		{
			EventHandler<MapViewPropertyChangeEventArgs<StyleColor>> e = BackgroundColorChangeRequested;

			if (e != null)
			{
				e(this, new MapViewPropertyChangeEventArgs<StyleColor>(current, requested));
			}
		}

		protected virtual void OnRequestGeoCenterChange(GeoPoint current, GeoPoint requested)
		{
			EventHandler<MapViewPropertyChangeEventArgs<GeoPoint>> e = GeoCenterChangeRequested;

			if (e != null)
			{
				MapViewPropertyChangeEventArgs<GeoPoint> args = 
					new MapViewPropertyChangeEventArgs<GeoPoint>(current, requested);

				e(this, args);
			}
		}

		private void OnRequestMaximumWorldWidthChange(double current, double requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<double>> e = MaximumWorldWidthChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<double> args =
                    new MapViewPropertyChangeEventArgs<double>(current, requested);

                e(this, args);
            }
		}

		private void OnRequestMinimumWorldWidthChange(double current, double requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<double>> e = MinimumWorldWidthChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<double> args =
                    new MapViewPropertyChangeEventArgs<double>(current, requested);

                e(this, args);
            }
		}

		private void OnRequestViewEnvelopeChange(BoundingBox current, BoundingBox requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<BoundingBox>> e = ViewEnvelopeChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<BoundingBox> args =
                    new MapViewPropertyChangeEventArgs<BoundingBox>(current, requested);

                e(this, args);
            }
		}

		private void OnRequestWorldAspectRatioChange(double current, double requested)
        {
            EventHandler<MapViewPropertyChangeEventArgs<double>> e = WorldAspectRatioChangeRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<double> args =
                    new MapViewPropertyChangeEventArgs<double>(current, requested);

                e(this, args);
            }
        }

        private void OnRequestZoomToExtents()
        {
            EventHandler e = ZoomToExtentsRequested;

            if(e != null)
            {
                e(this, EventArgs.Empty);
            }
        }

        private void OnRequestZoomToViewBounds(Rectangle2D viewBounds)
        {
            EventHandler<MapViewPropertyChangeEventArgs<Rectangle2D>> e = ZoomToViewBoundsRequested;

            if(e != null)
            {
                MapViewPropertyChangeEventArgs<Rectangle2D> args =
                    new MapViewPropertyChangeEventArgs<Rectangle2D>(ViewConverter.Convert(ClientRectangle), viewBounds);

                e(this, args);
            }
        }

        private void OnRequestZoomToWorldBounds(BoundingBox zoomBox)
        {
            EventHandler<MapViewPropertyChangeEventArgs<BoundingBox>> e = ZoomToWorldBoundsRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<BoundingBox> args =
                    new MapViewPropertyChangeEventArgs<BoundingBox>(ViewEnvelope, zoomBox);

                e(this, args);
            }
        }

        private void OnRequestZoomToWorldWidth(double newWorldWidth)
        {
            EventHandler<MapViewPropertyChangeEventArgs<double>> e = ZoomToWorldWidthRequested;

            if (e != null)
            {
                MapViewPropertyChangeEventArgs<double> args =
                    new MapViewPropertyChangeEventArgs<double>(WorldWidth, newWorldWidth);

                e(this, args);
            }
        }
		#endregion

		#region Private Helper Methods

		private bool withinDragTolerance(GdiPoint point)
		{
			return Math.Abs(_mouseDownLocation.X - point.X) <= 3 && Math.Abs(_mouseDownLocation.Y - point.Y) <= 3;
		}

		private Rectangle2D computeBoxFromWheelDelta(PointF location, int deltaDegrees)
		{
			float scale = (float) Math.Pow(2, (float) Math.Abs(deltaDegrees)/360f);
			RectangleF zoomBox = ClientRectangle;
			PointF center = new PointF((zoomBox.Width + zoomBox.Left)/2, (zoomBox.Height + zoomBox.Top)/2);
			PointF centerDeltaVector = new PointF(location.X - center.X, location.Y - center.Y);
			zoomBox.Offset(centerDeltaVector);
			return new Rectangle2D(zoomBox.Left, zoomBox.Top, zoomBox.Right, zoomBox.Bottom);
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