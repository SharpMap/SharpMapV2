// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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

using SharpMap.Geometries;
using SharpMap.Layers;
using GeoPoint = SharpMap.Geometries.Point;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Presentation
{
    /// <summary>
    /// Provides the input-handling and view-updating logic for a 2D map view.
    /// </summary>
    public class MapPresenter2D : BasePresenter<IMapView2D>
    {
        #region Private fields
        private ViewSelection2D _selection;

        private Point2D? _beginActionLocation;
        private Point2D _lastMoveLocation;

        //private readonly double _viewDpi;
        //private double _worldUnitsPerInch;
        //private ViewSize2D _viewSize;
        //private GeoPoint _center;
        private double _maximumWorldWidth = Double.PositiveInfinity;
        private double _minimumWorldWidth = 0.0;
        private Matrix2D _originNormalizeTransform = new Matrix2D();
        private Matrix2D _rotationTransform = new Matrix2D();
        private Matrix2D _translationTransform = new Matrix2D();
        private Matrix2D _scaleTransform = new Matrix2D();
        private Matrix2D _toViewCoordinates = new Matrix2D();
        private Matrix2D _toWorldTransform;
        private StyleColor _backgroundColor;
        #endregion

        #region Object Construction/Destruction
        /// <summary>
        /// Creates a new MapPresenter2D.
        /// </summary>
        /// <param name="map">The map to present.</param>
        /// <param name="mapView">The view to present the map on.</param>
        public MapPresenter2D(Map map, IMapView2D mapView)
            : base(map, mapView)
        {
            Map.LayersCollectionChanged += Map_LayersChanged;

            View.SizeChangeRequested += View_SizeChangeRequested;
            View.Hover += View_Hover;
            View.BeginAction += View_BeginAction;
            View.MoveTo += View_MoveTo;
            View.EndAction += View_EndAction;
            //_viewDpi = View.Dpi;

            BoundingBox extents = map.VisibleEnvelope;

            GeoPoint geoCenter = extents.GetCentroid();

            double initialScale = extents.Width / View.ViewSize.Width;

            if (extents.Height / View.ViewSize.Height < initialScale)
            {
                initialScale = extents.Height / View.ViewSize.Height;
            }

            if (geoCenter != GeoPoint.Empty)
            {
                _originNormalizeTransform.OffsetX = -extents.Left;
                _originNormalizeTransform.OffsetY = -extents.Bottom;

                _scaleTransform.X1 =  _scaleTransform.Y2 = 1 / initialScale;

                _translationTransform.OffsetX = -geoCenter.X;
                _translationTransform.OffsetY = -geoCenter.Y;

                _toViewCoordinates.Translate(View.ViewSize.Width / 2, -View.ViewSize.Height / 2);
                _toViewCoordinates.Scale(1, -1);
            }

            ToWorldTransform = ToViewTransform.Inverse as Matrix2D;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets map background color.
        /// </summary>
        /// <remarks>
        /// Defaults to transparent.
        /// </remarks>
        public StyleColor BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                if (_backgroundColor != value)
                {
                    StyleColor oldValue = _backgroundColor;
                    _backgroundColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets center of map in world coordinates.
        /// </summary>
        public GeoPoint GeoCenter
        {
            get 
            {
                Point2D values = ToWorldTransform.TransformVector(View.ViewSize.Width / 2, View.ViewSize.Height / 2);
                return new GeoPoint(values.X, values.Y);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                setViewMetricsInternal(View.ViewSize, value, WorldWidth);
            }
        }

        /// <summary>
        /// Gets or sets the minimum width in world units of the view.
        /// </summary>
        public double MaximumWorldWidth
        {
            get { return _maximumWorldWidth; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Maximum world width must greater than 0.");
                }

                if (_maximumWorldWidth != value)
                {
                    double oldValue = _maximumWorldWidth;
                    _maximumWorldWidth = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum width in world units of the view.
        /// </summary>
        public double MinimumWorldWidth
        {
            get { return _minimumWorldWidth; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Minimum world width must be 0 or greater.");
                }

                if (_minimumWorldWidth != value)
                {
                    double oldValue = _minimumWorldWidth;
                    _minimumWorldWidth = value;
                }
            }
        }

        /// <summary>
        /// Gets the width of a pixel in world coordinate units.
        /// </summary>
        /// <remarks>The value returned is the same as <see cref="WorldUnitsPerPixel"/>.</remarks>
        public double PixelWorldWidth
        {
            get { return WorldUnitsPerPixel; }
        }

        /// <summary>
        /// Gets the height of a pixel in world coordinate units.
        /// </summary>
        /// <remarks>The value returned is the same as <see cref="WorldUnitsPerPixel"/> 
        /// unless <see cref="PixelAspectRatio"/> is different from 1.</remarks>
        public double PixelWorldHeight
        {
            get { return WorldUnitsPerPixel * WorldAspectRatio; }
        }

        /// <summary>
        /// A selection on a view.
        /// </summary>
        public ViewSelection2D Selection
        {
            get { return _selection; }
            private set { _selection = value; }
        }

        /// <summary>
        /// Gets or sets a <see cref="Matrix2D"/> used to project the world
        /// coordinate system into the view coordinate system. 
        /// The inverse of the <see cref="ToWorldTransform"/> matrix.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        /// </code>
        /// </example>
        public Matrix2D ToViewTransform
        {
            get 
            {
                IMatrixD combined = _originNormalizeTransform
                    .Multiply(_rotationTransform)
                    .Multiply(_translationTransform)
                    .Multiply(_scaleTransform)
                    .Multiply(_toViewCoordinates);

                return new Matrix2D(combined); 
            }
        }

        /// <summary>
        /// Gets a <see cref="Matrix2D"/> used to reverse the view projection
        /// transform to get world coordinates.
        /// The inverse of the <see cref="ToViewTransform"/> matrix.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public Matrix2D ToWorldTransform
        {
            get { return _toWorldTransform; }
            private set { _toWorldTransform = value; }
        }

        /// <summary>
        /// Gets or sets the extents of the current view in world units.
        /// </summary>
        public BoundingBox ViewEnvelope
        {
            get 
            {
                Point2D lowerLeft = ToWorldTransform.TransformVector(0, View.ViewSize.Height);
                Point2D upperRight = ToWorldTransform.TransformVector(View.ViewSize.Width, 0);
                return new BoundingBox(lowerLeft.X, lowerLeft.Y, upperRight.X, upperRight.Y);
            }
            set
            {
                setViewEnvelopeInternal(value);
            }
        }

        /// <summary>
        /// Gets or sets the aspect ratio of the <see cref="WorldHeight"/> 
        /// to the <see cref="WorldWidth"/>.
        /// </summary>
        /// <remarks> 
        /// A value less than 1 will make the map stretch upwards 
        /// (the view will cover less world distance vertically), 
        /// and greater than 1 will make it more squat (the view will 
        /// cover more world distance vertically).
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws an exception when value is 0 or less.
        /// </exception>
        public double WorldAspectRatio
        {
            get { return 1 / Math.Abs(_scaleTransform.Y2 / _scaleTransform.X1); }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Invalid pixel aspect ratio.");
                }

                double currentRatio = WorldAspectRatio;

                if (currentRatio != value)
                {
                    double ratioModifier = value / currentRatio;
                    _scaleTransform.Y2 /= ratioModifier;
                    ToWorldTransform = ToViewTransform.Inverse as Matrix2D;
                }
            }
        }

        /// <summary>
        /// Gets the height of view in world units.
        /// </summary>
        /// <returns>
        /// The height of the view in world units, taking into account <see cref="PixelAspectRatio"/> 
        /// (<see cref="WorldWidth"/> * <see cref="PixelAspectRatio"/> * 
        /// <see cref="ViewSize"/> height / <see cref="ViewSize"/> width).
        /// </returns>
        public double WorldHeight
        {
            get { return WorldWidth * WorldAspectRatio * View.ViewSize.Height / View.ViewSize.Width; }
        }

        /// <summary>
        /// Gets the width of view in world units.
        /// </summary>
        /// <returns>The width of the view in world units (<see cref="ViewSize" /> 
        /// height * <see cref="WorldUnitsPerPixel"/>).</returns>
        public double WorldWidth
        {
            get { return View.ViewSize.Width * WorldUnitsPerPixel; }
        }

        /// <summary>
        /// Gets the width of a pixel in world coordinate units.
        /// </summary>
        public double WorldUnitsPerPixel
        {
            get { return ToWorldTransform.X1; }
        }
        #endregion

        #region Methods
        public IRenderer GetRendererForLayer<TRenderObject>(ILayer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }

            IRenderer renderer = null;

            //_layerRendererCatalog.TryGetValue(layer.GetType().TypeHandle, out renderer);
            renderer = LayerRendererCatalog.Instance.Get<IRenderer>(layer.GetType());
            return renderer;
        }

        public Point2D ToView(Point point)
		{
			return worldToView(point);
		}

		public Point2D ToView(double x, double y)
		{
			return ToViewTransform.TransformVector(x, y);
		}

		public Point ToWorld(Point2D point)
		{
			return viewToWorld(point);
		}

		public Point ToWorld(double x, double y)
		{
			Point2D values = ToWorldTransform.TransformVector(x, y);
			return new GeoPoint(values.X, values.Y);
		}

        /// <summary>
        /// Zooms the view to the given width.
        /// </summary>
        /// <remarks>
        /// View modifiers <see cref="MinimumWorldWidth"/>, 
        /// <see cref="MaximumWorldWidth"/> and <see cref="PixelAspectRatio"/>
        /// are taken into account when zooming to this width.
        /// </remarks>
        public void ZoomToWorldWidth(double newWorldWidth)
        {
            double newHeight = newWorldWidth * (WorldHeight / WorldWidth);
            double halfWidth = newWorldWidth * 0.5;
            double halfHeight = newHeight * 0.5;

            GeoPoint center = GeoCenter;
            double centerX = center.X, centerY = center.Y;

            BoundingBox widthWorldBounds = new BoundingBox(
                centerX - halfWidth, 
                centerY - halfHeight,
                centerX + halfWidth,
                centerY + halfHeight);

            ZoomToWorldBounds(widthWorldBounds);
        }

        /// <summary>
        /// Zooms to the extents of all visible layers in the current <see cref="Map"/>.
        /// </summary>
        public void ZoomToExtents()
        {
            ZoomToWorldBounds(Map.GetExtents());
        }

        /// <summary>
        /// Zooms the map to fit a world bounding box.
        /// </summary>
        /// <remarks>
        /// The <paramref name="zoomBox"/> value is stretched
        /// to fit the current view. For example, if a view has a size of
        /// 100 x 100, which is a 1:1 ratio, and the bounds of zoomBox are 
        /// 200 x 100, which is a 2:1 ratio, the bounds are stretched to be 
        /// 200 x 200 to match the view size ratio. This ensures that at least
        /// all of the area selected in the zoomBox is displayed, and possibly more
        /// area.
        /// </remarks>
        /// <param name="zoomBox"><see cref="BoundingBox"/> to set zoom to.</param>
        public void ZoomToWorldBounds(BoundingBox zoomBox)
        {
            setViewEnvelopeInternal(zoomBox);
        }

        /// <summary>
        /// Zooms the map to fit a view bounding box. 
        /// </summary>
        /// <remarks>
        /// Transforms view coordinates into
        /// world coordinates using <see cref="ToWorldTransform"/> to perform zoom. 
        /// This means the heuristic to determine the final value of <see cref="ViewEnvelope"/>
        /// after the zoom is the same as in <see cref="ZoomToWorldBounds"/>.
        /// </remarks>
        /// <param name="viewBounds">
        /// The view bounds, translated into world bounds,
        /// to set the zoom to.
        /// </param>
        public void ZoomToViewBounds(Rectangle2D viewBounds)
        {
            BoundingBox worldBounds = new BoundingBox(ToWorld(viewBounds.LowerBounds), ToWorld(viewBounds.UpperBounds));
            setViewEnvelopeInternal(worldBounds);
        }
        #endregion

        #region Protected Methods
        protected void RegisterRenderer<TLayerType, TRenderObject>(IRenderer renderer)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException("renderer");
            }

            LayerRendererCatalog.Instance.Register<Point2D, Size2D, Rectangle2D, TRenderObject>(
                typeof(TLayerType), renderer);
        }

        protected TRenderer GetRenderer<TRenderer>(ILayer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }

            Type layerType = layer.GetType();
            return LayerRendererCatalog.Instance.Get<TRenderer>(layerType);
        }

        protected void CreateSelection(Point2D location)
        {
            _selection = ViewSelection2D.CreateRectangluarSelection(location, Size2D.Zero);
        }

        protected void ModifySelection(Point2D location, Size2D size)
        {
            _selection.MoveBy(location);
            _selection.Expand(size);
        }

        protected void DestroySelection()
        {
            _selection = null;
        }

        #region Event Generators
        protected virtual void OnMapViewHover(Point2D viewPoint2D)
        {
        }

        protected virtual void OnMapViewBeginAction(Point2D location)
        {
            _beginActionLocation = location;
        }

        protected virtual void OnMapViewMoveTo(Point2D location)
        {
            double xDelta = location.X - _lastMoveLocation.X;
            double yDelta = location.Y - _lastMoveLocation.Y;

            if (Selection == null)
            {
                CreateSelection(location);
            }

            _lastMoveLocation = location;
        }

        protected virtual void OnMapViewEndAction(Point2D location)
        {
            _beginActionLocation = null;
        }

        protected virtual void OnSelectedFeaturesChanged()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnLayersChanged()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region Event handlers
        private void Map_SelectedFeaturesChanged(object sender, EventArgs e)
        {
            OnSelectedFeaturesChanged();
        }

        private void Map_LayersChanged(object sender, ModelCollectionChangedEventArgs<ILayer> e)
        {
            OnLayersChanged();
        }

        private void View_SizeChangeRequested(object sender, SizeChangeEventArgs<Size2D> e)
        {
            _toViewCoordinates.OffsetX = e.Size.Width / 2;
            _toViewCoordinates.OffsetY = e.Size.Height / 2;
            ToWorldTransform = ToViewTransform.Inverse;
            View.ViewSize = e.Size;
        }

        private void View_Hover(object sender, MapActionEventArgs<Point2D> e)
        {
            OnMapViewHover(e.ActionPoint);
        }

        private void View_BeginAction(object sender, MapActionEventArgs<Point2D> e)
        {
            OnMapViewBeginAction(e.ActionPoint);
        }

        private void View_MoveTo(object sender, MapActionEventArgs<Point2D> e)
        {
            OnMapViewMoveTo(e.ActionPoint);
        }

        private void View_EndAction(object sender, MapActionEventArgs<Point2D> e)
        {
            OnMapViewEndAction(e.ActionPoint);
        }
        #endregion

        #region Private helper methods
        private void modifySelection(double xDelta, double yDelta)
        {
            Selection.Expand(new Size2D(xDelta, yDelta));
        }

        private void setCenterInternal(GeoPoint newCenter)
        {
            if (GeoCenter == newCenter)
            {
                return;
            }

            setViewMetricsInternal(View.ViewSize, newCenter, WorldWidth);
        }

        private void setViewEnvelopeInternal(BoundingBox newEnvelope)
        {
            BoundingBox oldEnvelope = ViewEnvelope;

            if (oldEnvelope == newEnvelope)
            {
                return;
            }

            double widthZoomRatio = newEnvelope.Width / oldEnvelope.Width;
            double heightZoomRatio = newEnvelope.Height / oldEnvelope.Height;

			double newWorldWidth = widthZoomRatio > heightZoomRatio 
                ? newEnvelope.Width 
                : newEnvelope.Width * heightZoomRatio / widthZoomRatio;

            if (newWorldWidth < _minimumWorldWidth)
            {
                newWorldWidth = _minimumWorldWidth;
            }

            if (newWorldWidth > _maximumWorldWidth)
            {
                newWorldWidth = _maximumWorldWidth;
            }

            setViewMetricsInternal(View.ViewSize, newEnvelope.GetCentroid(), newWorldWidth);
        }

        private void setViewMetricsInternal(Size2D newViewSize, GeoPoint newCenter, double newWorldWidth)
        {
            Size2D oldViewSize = View.ViewSize;
            GeoPoint oldCenter = GeoCenter;
            double oldWorldWidth = WorldWidth;
            double oldWorldHeight = WorldHeight;
            double newWorldHeight = newWorldWidth * (oldWorldHeight / oldWorldWidth);

			bool viewMatrixChanged = false;

            if (newViewSize != View.ViewSize)
			{
				View.ViewSize = newViewSize;
            }

            if (!oldCenter.Equals(newCenter))
			{
                //// Compute how much the scaling shifted the edges from the center, 
                //// and how much the center shift offsets the origin.
                //double x = -(newWorldWidth - oldWorldWidth) / 2 + (newCenter.X - oldCenter.X);
                //double y = -(newWorldHeight - oldWorldHeight) / 2 + (newCenter.Y - oldCenter.Y);
                _translationTransform.OffsetX = -newCenter.X;
                _translationTransform.OffsetY = -newCenter.Y;
				viewMatrixChanged = true;
			}

            double newWorldUnitsPerPixel = newWorldWidth / View.ViewSize.Width;

			if (newWorldUnitsPerPixel != WorldUnitsPerPixel)
			{
                double newScale = 1 / newWorldUnitsPerPixel;
                _scaleTransform.Y2 = newScale / WorldAspectRatio;
                _scaleTransform.X1 = newScale;
				viewMatrixChanged = true;
			}

			if (viewMatrixChanged)
			{
				ToWorldTransform = ToViewTransform.Inverse;
			}
        }

        private Point2D worldToView(GeoPoint geoPoint)
        {
            return ToViewTransform.TransformVector(geoPoint.X, geoPoint.Y);
        }

        private GeoPoint viewToWorld(Point2D viewPoint)
        {
            Point2D values = ToWorldTransform.TransformVector(viewPoint.X, viewPoint.Y);
            return new GeoPoint(values.X, values.Y);
        }
        #endregion
	}
}
