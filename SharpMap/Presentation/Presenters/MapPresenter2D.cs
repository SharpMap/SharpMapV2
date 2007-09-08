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
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Tools;
using GeoPoint = SharpMap.Geometries.Point;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using System.ComponentModel;

namespace SharpMap.Presentation
{
    /// <summary>
    /// Provides the input-handling and view-updating logic for a 2D map view.
    /// </summary>
    public abstract class MapPresenter2D : BasePresenter<IMapView2D>
    {
        #region Private fields

        private ViewSelection2D _selection;
        private Point2D? _beginActionLocation;
        private Point2D _lastMoveLocation;
        private double _maximumWorldWidth = Double.PositiveInfinity;
        private double _minimumWorldWidth = 0.0;
        private readonly Matrix2D _originNormalizeTransform = new Matrix2D();
        private readonly Matrix2D _rotationTransform = new Matrix2D();
        private readonly Matrix2D _translationTransform = new Matrix2D();
        private readonly Matrix2D _scaleTransform = new Matrix2D();
        private readonly Matrix2D _toViewCoordinates = new Matrix2D();
        private Matrix2D _toWorldTransform;
        private StyleColor _backgroundColor;
        private readonly IRenderer _vectorRenderer;
        private readonly IRenderer _rasterRenderer;
        #endregion

        #region Object Construction/Destruction

        /// <summary>
        /// Creates a new MapPresenter2D.
        /// </summary>
        /// <param name="map">The map to present.</param>
        /// <param name="mapView">The view to present the map on.</param>
        protected MapPresenter2D(Map map, IMapView2D mapView)
            : base(map, mapView)
        {
            _vectorRenderer = CreateVectorRenderer();
            _rasterRenderer = CreateRasterRenderer();

            Map.LayersChanged += map_LayersChanged;
            Map.PropertyChanged += map_PropertyChanged;

            View.Hover += view_Hover;
            View.BeginAction += view_BeginAction;
            View.MoveTo += view_MoveTo;
            View.EndAction += view_EndAction;
            View.BackgroundColorChangeRequested += view_BackgroundColorChangeRequested;
            View.GeoCenterChangeRequested += view_GeoCenterChangeRequested;
            View.MaximumWorldWidthChangeRequested += view_MaximumWorldWidthChangeRequested;
            View.MinimumWorldWidthChangeRequested += view_MinimumWorldWidthChangeRequested;
            View.OffsetChangeRequested += view_OffsetChangeRequested;
            View.SizeChangeRequested += view_SizeChangeRequested;
            View.ViewEnvelopeChangeRequested += view_ViewEnvelopeChangeRequested;
            View.WorldAspectRatioChangeRequested += view_WorldAspectRatioChangeRequested;
            View.ZoomToExtentsRequested += view_ZoomToExtentsRequested;
            View.ZoomToViewBoundsRequested += view_ZoomToViewBoundsRequested;
            View.ZoomToWorldBoundsRequested += view_ZoomToWorldBoundsRequested;
            View.ZoomToWorldWidthRequested += view_ZoomToWorldWidthRequested;

            BoundingBox extents = map.VisibleRegion;

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

                _scaleTransform.M11 = _scaleTransform.M22 = 1 / initialScale;

                _translationTransform.OffsetX = -geoCenter.X;
                _translationTransform.OffsetY = -geoCenter.Y;

                _toViewCoordinates.Translate(View.ViewSize.Width / 2, -View.ViewSize.Height / 2);
                _toViewCoordinates.Scale(1, -1);
            }

            ToWorldTransformInternal = ToViewTransformInternal.Inverse;
        }

        protected abstract IRenderer CreateVectorRenderer();
        protected abstract IRenderer CreateRasterRenderer();
        protected virtual void SetViewBackgroundColor(StyleColor fromColor, StyleColor toColor) { }
        protected virtual void SetViewGeoCenter(Point fromGeoPoint, Point toGeoPoint) { }
        protected virtual void SetViewMaximumWorldWidth(double fromMaxWidth, double toMaxWidth) { }
        protected virtual void SetViewMinimumWorldWidth(double fromMinWidth, double toMinWidth) { }
        protected virtual void SetViewEnvelope(BoundingBox fromEnvelope, BoundingBox toEnvelope) { }
        protected virtual void SetViewSize(Size2D fromSize, Size2D toSize) { }
        protected virtual void SetViewWorldAspectRatio(double fromRatio, double toRatio) { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets map background color.
        /// </summary>
        /// <remarks>
        /// Defaults to transparent.
        /// </remarks>
        protected StyleColor BackgroundColorInternal
        {
            get { return _backgroundColor; }
            set
            {
                if (_backgroundColor != value)
                {
                    _backgroundColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets center of map in world coordinates.
        /// </summary>
        protected GeoPoint GeoCenterInternal
        {
            get
            {
                Point2D values = ToWorldTransformInternal.TransformVector(
                    View.ViewSize.Width / 2, View.ViewSize.Height / 2);
                return new GeoPoint(values.X, values.Y);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                setViewMetricsInternal(View.ViewSize, value, WorldWidthInternal);
            }
        }

        /// <summary>
        /// Gets or sets the minimum width in world units of the view.
        /// </summary>
        protected double MaximumWorldWidthInternal
        {
            get { return _maximumWorldWidth; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value",
                        value, "Maximum world width must greater than 0.");
                }

                if (_maximumWorldWidth != value)
                {
                    _maximumWorldWidth = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum width in world units of the view.
        /// </summary>
        protected double MinimumWorldWidthInternal
        {
            get { return _minimumWorldWidth; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value",
                        value, "Minimum world width must be 0 or greater.");
                }

                if (_minimumWorldWidth != value)
                {
                    _minimumWorldWidth = value;
                }
            }
        }

        /// <summary>
        /// Gets the width of a pixel in world coordinate units.
        /// </summary>
        /// <remarks>
        /// The value returned is the same as <see cref="WorldUnitsPerPixelInternal"/>.
        /// </remarks>
        protected double PixelWorldWidthInternal
        {
            get { return WorldUnitsPerPixelInternal; }
        }

        /// <summary>
        /// Gets the height of a pixel in world coordinate units.
        /// </summary>
        /// <remarks>
        /// The value returned is the same as <see cref="WorldUnitsPerPixelInternal"/> 
        /// unless <see cref="WorldAspectRatioInternal"/> is different from 1.
        /// </remarks>
        protected double PixelWorldHeightInternal
        {
            get { return WorldUnitsPerPixelInternal * WorldAspectRatioInternal; }
        }

        /// <summary>
        /// A selection on a view.
        /// </summary>
        protected ViewSelection2D SelectionInternal
        {
            get { return _selection; }
            private set { _selection = value; }
        }

        /// <summary>
        /// Gets a <see cref="Matrix2D"/> used to project the world
        /// coordinate system into the view coordinate system. 
        /// The inverse of the <see cref="ToWorldTransformInternal"/> matrix.
        /// </summary>
        protected Matrix2D ToViewTransformInternal
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
        /// The inverse of the <see cref="ToViewTransformInternal"/> matrix.
        /// </summary>
        protected Matrix2D ToWorldTransformInternal
        {
            get { return _toWorldTransform; }
            private set { _toWorldTransform = value; }
        }

        /// <summary>
        /// Gets or sets the extents of the current view in world units.
        /// </summary>
        protected BoundingBox ViewEnvelopeInternal
        {
            get
            {
                Point2D lowerLeft = ToWorldTransformInternal.TransformVector(0, View.ViewSize.Height);
                Point2D upperRight = ToWorldTransformInternal.TransformVector(View.ViewSize.Width, 0);
                return new BoundingBox(lowerLeft.X, lowerLeft.Y, upperRight.X, upperRight.Y);
            }
            set { setViewEnvelopeInternal(value); }
        }

        /// <summary>
        /// Gets or sets the aspect ratio of the <see cref="WorldHeightInternal"/> 
        /// to the <see cref="WorldWidthInternal"/>.
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
        protected double WorldAspectRatioInternal
        {
            get { return 1 / Math.Abs(_scaleTransform.M22 / _scaleTransform.M11); }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value",
                        value, "Invalid pixel aspect ratio.");
                }

                double currentRatio = WorldAspectRatioInternal;

                if (currentRatio != value)
                {
                    double ratioModifier = value / currentRatio;
                    _scaleTransform.M22 /= ratioModifier;
                    ToWorldTransformInternal = ToViewTransformInternal.Inverse;
                }
            }
        }

        /// <summary>
        /// Gets the height of view in world units.
        /// </summary>
        /// <returns>
        /// The height of the view in world units, taking into account <see cref="WorldAspectRatioInternal"/> 
        /// (<see cref="WorldWidthInternal"/> * <see cref="WorldAspectRatioInternal"/> * 
        /// <see cref="BasePresenter{IMapView2D}.View"/> ViewSize height 
        /// / <see cref="BasePresenter{IMapView2D}.View"/> ViewSize width).
        /// </returns>
        protected double WorldHeightInternal
        {
            get { return WorldWidthInternal * WorldAspectRatioInternal * View.ViewSize.Height / View.ViewSize.Width; }
        }

        /// <summary>
        /// Gets the width of a pixel in world coordinate units.
        /// </summary>
        protected double WorldUnitsPerPixelInternal
        {
            get { return ToWorldTransformInternal.M11; }
        }

        /// <summary>
        /// Gets the width of view in world units.
        /// </summary>
        /// <returns>The width of the view in world units (<see cref="BasePresenter{IMapView2D}.View" /> 
        /// height * <see cref="WorldUnitsPerPixelInternal"/>).</returns>
        protected double WorldWidthInternal
        {
            get { return View.ViewSize.Width * WorldUnitsPerPixelInternal; }
        }

        #endregion

        #region Methods

        protected IRenderer GetRendererForLayer(ILayer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }

            IRenderer renderer;

            //_layerRendererCatalog.TryGetValue(layer.GetType().TypeHandle, out renderer);
            renderer = LayerRendererRegistry.Instance.Get<IRenderer>(layer.GetType());
            return renderer;
        }

        protected Point2D ToViewInternal(Point point)
        {
            return worldToView(point);
        }

        protected Point2D ToViewInternal(double x, double y)
        {
            return ToViewTransformInternal.TransformVector(x, y);
        }

        protected Point ToWorldInternal(Point2D point)
        {
            return viewToWorld(point);
        }

        protected Point ToWorldInternal(double x, double y)
        {
            Point2D values = ToWorldTransformInternal.TransformVector(x, y);
            return new GeoPoint(values.X, values.Y);
        }

        /// <summary>
        /// Zooms to the extents of all visible layers in the current <see cref="Map"/>.
        /// </summary>
        protected void ZoomToExtentsInternal()
        {
            ZoomToWorldBoundsInternal(Map.GetExtents());
        }

        /// <summary>
        /// Zooms the map to fit a view bounding box. 
        /// </summary>
        /// <remarks>
        /// Transforms view coordinates into
        /// world coordinates using <see cref="ToWorldTransformInternal"/> to perform zoom. 
        /// This means the heuristic to determine the final value of <see cref="ViewEnvelopeInternal"/>
        /// after the zoom is the same as in <see cref="ZoomToWorldBoundsInternal"/>.
        /// </remarks>
        /// <param name="viewBounds">
        /// The view bounds, translated into world bounds,
        /// to set the zoom to.
        /// </param>
        protected void ZoomToViewBoundsInternal(Rectangle2D viewBounds)
        {
            BoundingBox worldBounds = new BoundingBox(
                ToWorldInternal(viewBounds.LowerLeft), ToWorldInternal(viewBounds.UpperRight));
            setViewEnvelopeInternal(worldBounds);
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
        /// <param name="zoomBox">
        /// <see cref="BoundingBox"/> to set zoom to.
        /// </param>
        protected void ZoomToWorldBoundsInternal(BoundingBox zoomBox)
        {
            setViewEnvelopeInternal(zoomBox);
        }

        /// <summary>
        /// Zooms the view to the given width.
        /// </summary>
        /// <remarks>
        /// View modifiers <see cref="MinimumWorldWidthInternal"/>, 
        /// <see cref="MaximumWorldWidthInternal"/> and <see cref="WorldAspectRatioInternal"/>
        /// are taken into account when zooming to this width.
        /// </remarks>
        protected void ZoomToWorldWidthInternal(double newWorldWidth)
        {
            double newHeight = newWorldWidth * (WorldHeightInternal / WorldWidthInternal);
            double halfWidth = newWorldWidth * 0.5;
            double halfHeight = newHeight * 0.5;

            GeoPoint center = GeoCenterInternal;
            double centerX = center.X, centerY = center.Y;

            BoundingBox widthWorldBounds = new BoundingBox(
                centerX - halfWidth,
                centerY - halfHeight,
                centerX + halfWidth,
                centerY + halfHeight);

            ZoomToWorldBoundsInternal(widthWorldBounds);
        }

        #endregion

        #region Protected Methods

        protected void RegisterRenderer<TLayerType>(IRenderer renderer)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException("renderer");
            }

            LayerRendererRegistry.Instance.Register(typeof(TLayerType), renderer);
        }

        protected TRenderer GetRenderer<TRenderer>(ILayer layer)
            where TRenderer : class
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }

            Type layerType = layer.GetType();
            return LayerRendererRegistry.Instance.Get<TRenderer>(layerType);
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

        #endregion

        #region Event handlers
        #region Map events

        private void map_LayersChanged(object sender, LayersChangedEventArgs e)
        {
            switch (e.ChangeType)
            {
                case LayersChangeType.Added:
                    foreach (ILayer layer in e.LayersAffected)
                    {
                        layer.LayerDataAvailable += layer_LayerDataAvailable;
                    }
                    break;
                case LayersChangeType.Removed:
                    foreach (ILayer layer in e.LayersAffected)
                    {
                        layer.LayerDataAvailable -= layer_LayerDataAvailable;
                    }
                    break;
                case LayersChangeType.Enabled:
                    break;
                case LayersChangeType.Disabled:
                    break;
                default:
                    break;
            }
        }

        void layer_LayerDataAvailable(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void map_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case Map.ActiveToolPropertyName:
                    break;
                case Map.SelectedLayersPropertyName:
                    break;
                case Map.SpatialReferencePropertyName:
                    break;
                case Map.VisibleRegionPropertyName:
                    setViewEnvelopeInternal(Map.VisibleRegion);
                    break;
                default:
                    return;
            }
        }
        #endregion

        #region View events
        // Handles the offset request from the view
        private void view_OffsetChangeRequested(object sender, MapViewPropertyChangeEventArgs<Point2D> e)
        {
            GeoPoint geoOffset = ToWorldInternal(e.RequestedValue);
            GeoPoint newCenter = GeoCenterInternal + geoOffset;
            GeoCenterInternal = newCenter;
        }

        // Handles the size-change request from the view
        private void view_SizeChangeRequested(object sender, MapViewPropertyChangeEventArgs<Size2D> e)
        {
            _toViewCoordinates.OffsetX = e.RequestedValue.Width / 2;
            _toViewCoordinates.OffsetY = e.RequestedValue.Height / 2;
            ToWorldTransformInternal = ToViewTransformInternal.Inverse;
            SetViewSize(e.CurrentValue, e.RequestedValue);
        }

        // Handles the hover request from the view
        private void view_Hover(object sender, MapActionEventArgs<Point2D> e)
        {
            Map.GetActiveTool<IMapView2D, Point2D>().QueryAction(new ActionContext<IMapView2D, Point2D>(Map, View, e));
        }

        // Handles the begin action request from the view
        private void view_BeginAction(object sender, MapActionEventArgs<Point2D> e)
        {
            Map.GetActiveTool<IMapView2D, Point2D>().BeginAction(new ActionContext<IMapView2D, Point2D>(Map, View, e));
        }

        // Handles the move-to request from the view
        private void view_MoveTo(object sender, MapActionEventArgs<Point2D> e)
        {
            Map.GetActiveTool<IMapView2D, Point2D>().ExtendAction(new ActionContext<IMapView2D, Point2D>(Map, View, e));
        }

        // Handles the end action request from the view
        private void view_EndAction(object sender, MapActionEventArgs<Point2D> e)
        {
            Map.GetActiveTool<IMapView2D, Point2D>().EndAction(new ActionContext<IMapView2D, Point2D>(Map, View, e));
        }

        // Handles the background color change request from the view
        private void view_BackgroundColorChangeRequested(object sender, MapViewPropertyChangeEventArgs<StyleColor> e)
        {
            SetViewBackgroundColor(e.CurrentValue, e.RequestedValue);
        }

        // Handles the geographic view center change request from the view
        private void view_GeoCenterChangeRequested(object sender, MapViewPropertyChangeEventArgs<Point> e)
        {
            GeoCenterInternal = e.RequestedValue;

            SetViewGeoCenter(e.CurrentValue, e.RequestedValue);
        }

        // Handles the maximum world width change request from the view
        private void view_MaximumWorldWidthChangeRequested(object sender, MapViewPropertyChangeEventArgs<double> e)
        {
            MaximumWorldWidthInternal = e.RequestedValue;

            SetViewMaximumWorldWidth(e.CurrentValue, e.RequestedValue);
        }

        // Handles the minimum world width change request from the view
        private void view_MinimumWorldWidthChangeRequested(object sender, MapViewPropertyChangeEventArgs<double> e)
        {
            MinimumWorldWidthInternal = e.RequestedValue;

            SetViewMinimumWorldWidth(e.CurrentValue, e.RequestedValue);
        }

        // Handles the view envelope change request from the view
        private void view_ViewEnvelopeChangeRequested(object sender, MapViewPropertyChangeEventArgs<BoundingBox> e)
        {
            ViewEnvelopeInternal = e.RequestedValue;

            SetViewEnvelope(e.CurrentValue, e.RequestedValue);
        }

        // Handles the world aspect ratio change request from the view
        private void view_WorldAspectRatioChangeRequested(object sender, MapViewPropertyChangeEventArgs<double> e)
        {
            WorldAspectRatioInternal = e.RequestedValue;

            SetViewWorldAspectRatio(e.CurrentValue, e.RequestedValue);
        }

        // Handles the view zoom to extents request from the view
        private void view_ZoomToExtentsRequested(object sender, EventArgs e)
        {
            ZoomToExtentsInternal();
        }

        // Handles the view zoom to specified view bounds request from the view
        private void view_ZoomToViewBoundsRequested(object sender, MapViewPropertyChangeEventArgs<Rectangle2D> e)
        {
            ZoomToViewBoundsInternal(e.RequestedValue);
        }

        // Handles the view zoom to specified world bounds request from the view
        private void view_ZoomToWorldBoundsRequested(object sender, MapViewPropertyChangeEventArgs<BoundingBox> e)
        {
            ZoomToWorldBoundsInternal(e.RequestedValue);
        }

        // Handles the view zoom to specified world width request from the view
        private void view_ZoomToWorldWidthRequested(object sender, MapViewPropertyChangeEventArgs<double> e)
        {
            ZoomToWorldWidthInternal(e.RequestedValue);
        }
        #endregion
        #endregion

        #region Private helper methods

        private void modifySelection(double xDelta, double yDelta)
        {
            SelectionInternal.Expand(new Size2D(xDelta, yDelta));
        }

        private void setCenterInternal(GeoPoint newCenter)
        {
            if (GeoCenterInternal == newCenter)
            {
                return;
            }

            setViewMetricsInternal(View.ViewSize, newCenter, WorldWidthInternal);
        }

        // Performs computations to set the view metrics to the given envelope.
        private void setViewEnvelopeInternal(BoundingBox newEnvelope)
        {
            BoundingBox oldEnvelope = ViewEnvelopeInternal;

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

        // Performs computations to set the view metrics given the parameters
        // of the view size, the geographic center of the view, and how much
        // of the world to show in the view by width.
        private void setViewMetricsInternal(Size2D newViewSize, GeoPoint newCenter, double newWorldWidth)
        {
            GeoPoint oldCenter = GeoCenterInternal;

            bool viewMatrixChanged = false;

            // Change view size
            if (newViewSize != View.ViewSize)
            {
                View.ViewSize = newViewSize;
            }

            // Change geographic center of the view by translating pan matrix
            if (!oldCenter.Equals(newCenter))
            {
                _translationTransform.OffsetX += -(newCenter.X - oldCenter.X);
                _translationTransform.OffsetY += -(newCenter.Y - oldCenter.Y);
                viewMatrixChanged = true;
            }

            // Compute new world units per pixel based on view size and desired 
            // world width
            double newWorldUnitsPerPixel = newWorldWidth / View.ViewSize.Width;

            if (newWorldUnitsPerPixel != WorldUnitsPerPixelInternal)
            {
                double newScale = 1 / newWorldUnitsPerPixel;
                _scaleTransform.M22 = newScale / WorldAspectRatioInternal;
                _scaleTransform.M11 = newScale;
                viewMatrixChanged = true;
            }

            // If the view metrics have changed, recompute the inverse view matrix
            // and set the visible region of the map.
            if (viewMatrixChanged)
            {
                ToWorldTransformInternal = ToViewTransformInternal.Inverse;

                if (Map.VisibleRegion != ViewEnvelopeInternal)
                {
                    Map.VisibleRegion = ViewEnvelopeInternal;
                }
            }
        }

        // Computes the view space coordinates of a point in world space
        private Point2D worldToView(GeoPoint geoPoint)
        {
            return ToViewTransformInternal.TransformVector(geoPoint.X, geoPoint.Y);
        }

        // Computes the world space coordinates of a point in view space
        private GeoPoint viewToWorld(Point2D viewPoint)
        {
            Point2D values = ToWorldTransformInternal.TransformVector(viewPoint.X, viewPoint.Y);
            return new GeoPoint(values.X, values.Y);
        }

        #endregion
    }
}