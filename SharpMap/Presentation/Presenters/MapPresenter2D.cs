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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Presentation.Views;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Tools;
using GeoPoint = SharpMap.Geometries.Point;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Presentation.Presenters
{
    /// <summary>
    /// Provides the input-handling and view-updating logic for a 2D map view.
    /// </summary>
    public abstract class MapPresenter2D : BasePresenter<IMapView2D>
    {
        #region Private static fields
        private static readonly object _rendererInitSync = new object();
        private static object _vectorRenderer;
        private static object _rasterRenderer;
        private static object _textRenderer;
        #endregion

        #region Private instance fields
        private readonly ViewSelection2D _selection;
        private StyleColor _backgroundColor;
        private readonly List<IFeatureLayer> _wiredLayers = new List<IFeatureLayer>();
        private Size2D _oldViewSize = Size2D.Empty;
        private Double _maximumWorldWidth = Double.PositiveInfinity;
        private Double _minimumWorldWidth;

        // Offsets the origin of the spatial reference system so that the 
        // center of the view coincides with the center of the extents of the map.
        private Boolean _viewIsEmpty = true;

        // The origin projection matrix reflects the coordinate system along
        // the x-axis, and translates the lower left corner of Map.Extents
        // to the view coordinate (0, ViewSize.Height)
        private readonly Matrix2D _originProjectionTransform = new Matrix2D();

        // The rotation matrix rotates world coordinates relative to view coordinates
        private readonly Matrix2D _rotationTransform = new Matrix2D();

        // The translation matrix pans the world coordinates relative to view coordinates
        private readonly Matrix2D _translationTransform = new Matrix2D();

        // The extents center matrix helps translate the map extents to the center of the view.
        private readonly Matrix2D _extentsCenterTransform = new Matrix2D();

        // The scale matrix zooms the world coordinates relative to view coordinates
        private readonly Matrix2D _scaleTransform = new Matrix2D();

        // The view center matrix helps translate the map extents to the center of the view.
        private readonly Matrix2D _viewCenterTransform = new Matrix2D();

        // The to view coordinates matrix shifts world coordinates down by the height 
        // of the view so that the coordinate system normalization started with the 
        // origin normalize transform is completed
        private readonly Matrix2D _toViewCoordinates = new Matrix2D();

        // The computed conatenated matrix which transforms world coordinates to 
        // view coordinates
        private Matrix2D _toViewTransform;

        // The computed conatenated matrix which transforms view coordinates to 
        // world coordinates
        private Matrix2D _toWorldTransform;

        //private readonly Dictionary<RenderedObjectCacheKey, IEnumerable> _renderObjectsCache
        //    = new Dictionary<RenderedObjectCacheKey, IEnumerable>();
        #endregion

        #region Object construction / disposal

        /// <summary>
        /// Creates a new MapPresenter2D.
        /// </summary>
        /// <param name="map">The map to present.</param>
        /// <param name="mapView">The view to present the map on.</param>
        protected MapPresenter2D(Map map, IMapView2D mapView)
            : base(map, mapView)
        {
            createRenderers();

            //_selection = ViewSelection2D.CreateRectangluarSelection(Point2D.Zero, Size2D.Zero);
            _selection = new ViewSelection2D();

            wireupExistingLayers(Map.Layers);
            Map.Layers.ListChanged += handleLayersChanged;
            Map.PropertyChanged += handleMapPropertyChanged;

            View.Hover += handleViewHover;
            View.BeginAction += handleViewBeginAction;
            View.MoveTo += handleViewMoveTo;
            View.EndAction += handleViewEndAction;
            View.BackgroundColorChangeRequested += handleViewBackgroundColorChangeRequested;
            View.GeoCenterChangeRequested += handleViewGeoCenterChangeRequested;
            View.MaximumWorldWidthChangeRequested += handleViewMaximumWorldWidthChangeRequested;
            View.MinimumWorldWidthChangeRequested += handleViewMinimumWorldWidthChangeRequested;
            View.IdentifyLocationRequested += handleIdentifyLocationRequested;
            View.OffsetChangeRequested += handleViewOffsetChangeRequested;
            View.SizeChanged += handleViewSizeChanged;
            View.ViewEnvelopeChangeRequested += handleViewViewEnvelopeChangeRequested;
            View.WorldAspectRatioChangeRequested += handleViewWorldAspectRatioChangeRequested;
            View.ZoomToExtentsRequested += handleViewZoomToExtentsRequested;
            View.ZoomToViewBoundsRequested += handleViewZoomToViewBoundsRequested;
            View.ZoomToWorldBoundsRequested += handleViewZoomToWorldBoundsRequested;
            View.ZoomToWorldWidthRequested += handleViewZoomToWorldWidthRequested;
            _selection.SelectionChanged += handleSelectionChanged;

            _originProjectionTransform.Scale(1, -1);

            BoundingBox extents = Map.Extents;

            if (extents != BoundingBox.Empty)
            {
                projectOrigin(extents);
                setExtentsCenter(extents);
            }

            if (!View.ViewSize.IsEmpty)
            {
                _toViewCoordinates.OffsetY = View.ViewSize.Height;
                setViewCenter(View.ViewSize);
            }
        }
        #endregion

        #region Abstract methods

        protected abstract IRasterRenderer2D CreateRasterRenderer();
        protected abstract ITextRenderer2D CreateTextRenderer();
        protected abstract IVectorRenderer2D CreateVectorRenderer();
        protected abstract Type GetRenderObjectType();

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
                GeoPoint mapCenter = Map.Center;
                
                if(mapCenter.IsEmpty())
                {
                    return GeoPoint.Empty;
                }

                return new GeoPoint(mapCenter.X - _translationTransform.OffsetX, 
                    mapCenter.Y + _translationTransform.OffsetY);
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
        protected Double MaximumWorldWidthInternal
        {
            get
            {
                if (_maximumWorldWidth > RenderMaximumWorldWidth)
                {
                    _maximumWorldWidth = RenderMaximumWorldWidth;
                }

                return _maximumWorldWidth;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "value", value, "Maximum world width must be greater than 0.");
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
        protected Double MinimumWorldWidthInternal
        {
            get
            {
                if (_minimumWorldWidth < RenderMinimumWorldWidth)
                {
                    _minimumWorldWidth = RenderMinimumWorldWidth;
                }

                return _minimumWorldWidth;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "value", value, "Minimum world width must be greater than 0.");
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
        protected Double PixelWorldWidthInternal
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
        protected Double PixelWorldHeightInternal
        {
            get { return WorldUnitsPerPixelInternal * WorldAspectRatioInternal; }
        }

        /// <summary>
        /// Gets the instance of the concrete
        /// <see cref="RasterRenderer2D{TRenderObject}"/> used
        /// for the specific display technology which a base class
        /// is created to support.
        /// </summary>
        protected IRasterRenderer2D RasterRenderer
        {
            get
            {
                if (Thread.VolatileRead(ref _rasterRenderer) == null)
                {
                    lock (_rendererInitSync)
                    {
                        if (Thread.VolatileRead(ref _rasterRenderer) == null)
                        {
                            IRenderer rasterRenderer = CreateRasterRenderer();
                            Thread.VolatileWrite(ref _rasterRenderer, rasterRenderer);
                        }
                    }
                }

                return _rasterRenderer as IRasterRenderer2D;
            }
        }

        protected virtual Double RenderMaximumWorldWidth
        {
            get { return 10000000; }
        }

        protected virtual Double RenderMinimumWorldWidth
        {
            get { return 0.01; }
        }

        /// <summary>
        /// A selection on a view.
        /// </summary>
        protected ViewSelection2D SelectionInternal
        {
            get { return _selection; }
            //private set { _selection = value; }
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
                if (_viewIsEmpty)
                {
                    return null;
                }

                return _toViewTransform;
            }
            private set
            {
                _toViewTransform = value;
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
        /// Gets the instance of the concrete
        /// <see cref="VectorRenderer2D{TRenderObject}"/> used
        /// for the specific display technology which a base class
        /// is created to support.
        /// </summary>
        protected IVectorRenderer2D VectorRenderer
        {
            get
            {
                if (Thread.VolatileRead(ref _vectorRenderer) == null)
                {
                    lock (_rendererInitSync)
                    {
                        if (Thread.VolatileRead(ref _vectorRenderer) == null)
                        {
                            IRenderer vectorRenderer = CreateVectorRenderer();
                            Thread.VolatileWrite(ref _vectorRenderer, vectorRenderer);
                        }
                    }
                }

                return _vectorRenderer as IVectorRenderer2D;
            }
        }

        protected ITextRenderer2D TextRenderer
        {
            get
            {
                if (Thread.VolatileRead(ref _textRenderer) == null)
                {
                    lock (_rendererInitSync)
                    {
                        if (Thread.VolatileRead(ref _textRenderer) == null)
                        {
                            IRenderer textRenderer = CreateTextRenderer();
                            Thread.VolatileWrite(ref _textRenderer, textRenderer);
                        }
                    }
                }

                return _textRenderer as ITextRenderer2D;
            }
        }

        /// <summary>
        /// Gets or sets the extents of the current view in world units.
        /// </summary>
        protected BoundingBox ViewEnvelopeInternal
        {
            get
            {
                if (ToWorldTransformInternal == null)
                {
                    return BoundingBox.Empty;
                }

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
        protected Double WorldAspectRatioInternal
        {
            get { return 1 / Math.Abs(_scaleTransform.M22 / _scaleTransform.M11); }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value",
                                                          value, "Invalid pixel aspect ratio.");
                }

                Double currentRatio = WorldAspectRatioInternal;

                if (currentRatio != value)
                {
                    Double ratioModifier = value / currentRatio;

                    _scaleTransform.M22 /= ratioModifier;

                    if (ToViewTransformInternal != null)
                    {
                        ToViewTransformInternal = concatenateViewMatrix();
                        ToWorldTransformInternal = ToViewTransformInternal.Inverse;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the height of view in world units.
        /// </summary>
        /// <returns>
        /// The height of the view in world units, taking into account <see cref="WorldAspectRatioInternal"/> 
        /// (<see cref="WorldWidthInternal"/> * <see cref="WorldAspectRatioInternal"/> * 
        /// <see cref="Presentation.Presenters.BasePresenter{TView}.View"/> ViewSize height 
        /// / <see cref="Presentation.Presenters.BasePresenter{TView}.View"/> ViewSize width).
        /// </returns>
        protected Double WorldHeightInternal
        {
            get { return WorldWidthInternal * WorldAspectRatioInternal * View.ViewSize.Height / View.ViewSize.Width; }
        }

        /// <summary>
        /// Gets the width of a pixel in world coordinate units.
        /// </summary>
        protected Double WorldUnitsPerPixelInternal
        {
            get
            {
                return ToWorldTransformInternal == null
                           ? 0
                           : ToWorldTransformInternal.M11;
            }
        }

        /// <summary>
        /// Gets the width of view in world units.
        /// </summary>
        /// <returns>The width of the view in world units (<see cref="Presentation.Presenters.BasePresenter{TView}.View" /> 
        /// height * <see cref="WorldUnitsPerPixelInternal"/>).</returns>
        protected Double WorldWidthInternal
        {
            get { return View.ViewSize.Width * WorldUnitsPerPixelInternal; }
        }

        #endregion

        #region Methods
        protected virtual void SetViewBackgroundColor(StyleColor fromColor, StyleColor toColor) { }
        protected virtual void SetViewGeoCenter(Point fromGeoPoint, Point toGeoPoint) { }
        protected virtual void SetViewEnvelope(BoundingBox fromEnvelope, BoundingBox toEnvelope) { }
        protected virtual void SetViewLocationInformation(String text) { }
        protected virtual void SetViewMaximumWorldWidth(Double fromMaxWidth, Double toMaxWidth) { }
        protected virtual void SetViewMinimumWorldWidth(Double fromMinWidth, Double toMinWidth) { }
        //protected virtual void SetViewSize(Size2D fromSize, Size2D toSize) { }
        protected virtual void SetViewWorldAspectRatio(Double fromRatio, Double toRatio) { }

        protected Point2D ToViewInternal(Point point)
        {
            return worldToView(point);
        }

        protected Point2D ToViewInternal(Double x, Double y)
        {
            return ToViewTransformInternal.TransformVector(x, y);
        }

        protected Point ToWorldInternal(Point2D point)
        {
            return viewToWorld(point);
        }

        protected Point ToWorldInternal(Double x, Double y)
        {
            Point2D values = ToWorldTransformInternal.TransformVector(x, y);
            return new GeoPoint(values.X, values.Y);
        }

        protected BoundingBox ToWorldInternal(Rectangle2D bounds)
        {
            if (ToWorldTransformInternal == null)
            {
                return BoundingBox.Empty;
            }

            Point2D lowerRight = ToWorldTransformInternal.TransformVector(bounds.Left, bounds.Bottom);
            Point2D upperLeft = ToWorldTransformInternal.TransformVector(bounds.Right, bounds.Top);
            return new BoundingBox(lowerRight.X, lowerRight.Y, upperLeft.X, upperLeft.Y);
        }

        /// <summary>
        /// Zooms to the extents of all visible layers in the current <see cref="Map"/>.
        /// </summary>
        protected void ZoomToExtentsInternal()
        {
            ZoomToWorldBoundsInternal(Map.Extents);
        }

        /// <summary>
        /// Zooms the map to fit a view bounding box. 
        /// </summary>
        /// <remarks>
        /// Transforms view coordinates into
        /// world coordinates using <see cref="ToWorldTransformInternal"/> 
        /// to perform zoom. 
        /// This means the heuristic to determine the final value of 
        /// <see cref="ViewEnvelopeInternal"/> after the zoom is the same as in 
        /// <see cref="ZoomToWorldBoundsInternal"/>.
        /// </remarks>
        /// <param name="viewBounds">
        /// The view bounds, translated into world bounds,
        /// to set the zoom to.
        /// </param>
        protected void ZoomToViewBoundsInternal(Rectangle2D viewBounds)
        {
            if (_viewIsEmpty)
            {
                throw new InvalidOperationException("No visible region is set for this view.");
            }

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
        /// <see cref="MaximumWorldWidthInternal"/> and 
        /// <see cref="WorldAspectRatioInternal"/>
        /// are taken into account when zooming to this width.
        /// </remarks>
        protected void ZoomToWorldWidthInternal(Double newWorldWidth)
        {
            setViewMetricsInternal(View.ViewSize, GeoCenterInternal, newWorldWidth);
        }

        #endregion

        #region Protected members

        protected virtual void CreateGeometryRenderer(Type renderObjectType)
        {
            Type basicGeometryRendererType = typeof(BasicGeometryRenderer2D<>);
            Type layerType = typeof(GeometryLayer);

            CreateRendererForLayerType(basicGeometryRendererType, renderObjectType, layerType, VectorRenderer);
        }

        private void CreateLabelRenderer(Type renderObjectType)
        {
            Type basicLabelRendererType = typeof(BasicLabelRenderer2D<>);
            Type layerType = typeof(LabelLayer);

            CreateRendererForLayerType(basicLabelRendererType, renderObjectType, layerType, TextRenderer, VectorRenderer);
        }

        protected void CreateRendererForLayerType(Type rendererType, Type renderObjectType, Type layerType, params object[] constructorParams)
        {
            IRenderer renderer;
            Type constructedType = rendererType.MakeGenericType(renderObjectType);
            renderer = Activator.CreateInstance(constructedType, constructorParams) as IRenderer;
            Debug.Assert(renderer != null);
            RegisterRendererForLayerType(layerType, renderer);
        }

        protected static TRenderer GetRenderer<TRenderer, TLayer>()
            where TRenderer : class, IRenderer
        {
            return LayerRendererRegistry.Instance.Get<TRenderer, TLayer>();
        }

        protected static TRenderer GetRenderer<TRenderer>(String name)
            where TRenderer : class, IRenderer
        {
            return LayerRendererRegistry.Instance.Get<TRenderer>(name);
        }

        protected static TRenderer GetRenderer<TRenderer>(ILayer layer)
            where TRenderer : class, IRenderer
        {
            return LayerRendererRegistry.Instance.Get<TRenderer>(layer);
        }

        protected Boolean IsViewMatrixInitialized
        {
            get { return !_viewIsEmpty; }
        }

        protected virtual void OnViewMatrixInitialized() { }

        protected virtual void OnViewMatrixChanged() { }

        protected void RegisterRendererForLayerType(Type layerType, IRenderer renderer)
        {
            LayerRendererRegistry.Instance.Register(layerType, renderer);
        }

        protected void RenderSelection(ViewSelection2D selection)
        {
            OnRenderingSelection();

            IVectorRenderer2D renderer = VectorRenderer;

            Debug.Assert(renderer != null);

            Path2D path = selection.Path.Clone() as Path2D;
            Debug.Assert(path != null);
            path.TransformPoints(ToWorldTransformInternal);
            IEnumerable<Path2D> paths = new Path2D[] { path };

            IEnumerable renderedSelection =
                renderer.RenderPaths(paths,
                    selection.FillBrush, null, null,
                    selection.OutlineStyle, null, null, RenderState.Normal);

            View.ShowRenderedObjects(renderedSelection);

            OnRenderedSelection();
        }

        protected virtual void RenderFeatureLayer(IFeatureLayer layer)
        {
            IFeatureRenderer renderer = GetRenderer<IFeatureRenderer>(layer);

            Debug.Assert(renderer != null);

            IEnumerable<FeatureDataRow> features
                = layer.Features.Select(ViewEnvelopeInternal.ToGeometry());

            Debug.Assert(layer.Style is VectorStyle);
            VectorStyle style = layer.Style as VectorStyle;

            foreach (FeatureDataRow feature in features)
            {
                IEnumerable renderedFeature;

                renderedFeature = renderer.RenderFeature(feature, style, RenderState.Normal);
                View.ShowRenderedObjects(renderedFeature);
            }

            IEnumerable<FeatureDataRow> selectedRows = layer.SelectedFeatures;

            foreach (FeatureDataRow selectedFeature in selectedRows)
            {
                IEnumerable renderedFeature = renderer.RenderFeature(selectedFeature, style, RenderState.Selected);
                View.ShowRenderedObjects(renderedFeature);
            }

            IEnumerable<FeatureDataRow> highlightedRows = layer.HighlightedFeatures;

            foreach (FeatureDataRow highlightedFeature in highlightedRows)
            {
                IEnumerable renderedFeature = renderer.RenderFeature(highlightedFeature, style, RenderState.Highlighted);
                View.ShowRenderedObjects(renderedFeature);
            }
        }

        protected virtual void RenderRasterLayer(IRasterLayer layer)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnRenderingAllLayers() { }

        protected virtual void OnRenderedAllLayers() { }

        protected virtual void OnRenderingLayer(ILayer layer) { }

        protected virtual void OnRenderedLayer(ILayer layer) { }

        protected virtual void OnRenderingSelection() { }

        protected virtual void OnRenderedSelection() { }
        #endregion

        #region Event handlers

        #region Map events
        private void handleLayersChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (Map.Layers[e.NewIndex] is IFeatureLayer)
                    {
                        IFeatureLayer layer = Map.Layers[e.NewIndex] as IFeatureLayer;
                        Debug.Assert(layer != null);
                        wireupLayer(layer);
                    }
                    RenderLayer(Map.Layers[e.NewIndex]);
                    break;
                case ListChangedType.ItemDeleted:
                    // LayerCollection defines an e.NewIndex as -1 when an item is being 
                    // deleted and not yet removed from the collection.
                    if (e.NewIndex >= 0)
                    {
                        RenderAllLayers();
                    }
                    else
                    {
                        IFeatureLayer layer = Map.Layers[e.OldIndex] as IFeatureLayer;

                        if (layer != null)
                        {
                            unwireLayer(layer);
                        }
                    }
                    break;
                case ListChangedType.ItemMoved:
                    RenderAllLayers();
                    break;
                case ListChangedType.Reset:
                    wireupExistingLayers(Map.Layers);
                    RenderAllLayers();
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor.Name == Layer.EnabledProperty.Name)
                    {
                        RenderAllLayers();
                    }
                    break;
                default:
                    break;
            }
        }

        protected void RenderAllLayers()
        {
            OnRenderingAllLayers();

            for (Int32 i = Map.Layers.Count - 1; i >= 0; i--)
            {
                RenderLayer(Map.Layers[i]);
            }

            OnRenderedAllLayers();
        }

        protected void RenderLayer(ILayer layer)
        {
            OnRenderingLayer(layer);

            if (!layer.Enabled ||
                layer.Style.MinVisible > WorldWidthInternal ||
                layer.Style.MaxVisible < WorldWidthInternal)
            {
                return;
            }

            if (layer is IFeatureLayer)
            {
                RenderFeatureLayer(layer as IFeatureLayer);
            }
            else if (layer is IRasterLayer)
            {
                RenderRasterLayer(layer as IRasterLayer);
            }

            OnRenderedLayer(layer);
        }

        private void handleMapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Map.SpatialReferenceProperty.Name)
            {
                //throw new NotImplementedException();
            }

            if (e.PropertyName == Map.SelectedLayersProperty.Name)
            {
                //throw new NotImplementedException();
            }

            if (e.PropertyName == Map.ActiveToolProperty.Name)
            {
                //throw new NotImplementedException();
            }

            if (e.PropertyName == Map.ExtentsProperty.Name)
            {
                BoundingBox extents = Map.Extents;
                projectOrigin(extents);
                setExtentsCenter(extents);
            }
        }

        #endregion

        #region View events
        private void handleIdentifyLocationRequested(object sender, LocationEventArgs e)
        {
            // TODO: if map units are in latitude / longitude, terms easting and northing aren't used... are they?
            SetViewLocationInformation(String.Format("Easting: {0:N4}; Northing: {1:N4}",
                e.Point.X, e.Point.Y));
        }

        // Handles the offset request from the view
        private void handleViewOffsetChangeRequested(object sender, MapViewPropertyChangeEventArgs<Point2D> e)
        {
            Size2D viewSize = View.ViewSize;
            Point2D newViewCenter = new Point2D(viewSize.Width / 2, viewSize.Height / 2) + e.RequestedValue;
            GeoPoint newCenter = ToWorldInternal(newViewCenter);
            GeoCenterInternal = newCenter;
        }

        // Handles the size-change request from the view
        private void handleViewSizeChanged(object sender, EventArgs e)
        {
            setViewMetricsInternal(View.ViewSize, GeoCenterInternal, WorldWidthInternal);
        }

        private Point2D _previousActionPoint = Point2D.Empty;

        // Handles the hover request from the view
        private void handleViewHover(object sender, MapActionEventArgs<Point2D> e)
        {
            ActionContext<IMapView2D, Point2D> context 
                = new ActionContext<IMapView2D, Point2D>(Map, View, _previousActionPoint, e.ActionPoint);
            Map.GetActiveTool<IMapView2D, Point2D>().QueryAction(context);
            _previousActionPoint = e.ActionPoint;
        }

        // Handles the begin action request from the view
        private void handleViewBeginAction(object sender, MapActionEventArgs<Point2D> e)
        {
            ActionContext<IMapView2D, Point2D> context
                = new ActionContext<IMapView2D, Point2D>(Map, View, _previousActionPoint, e.ActionPoint);
            Map.GetActiveTool<IMapView2D, Point2D>().BeginAction(context);
            _previousActionPoint = e.ActionPoint;
        }

        // Handles the move-to request from the view
        private void handleViewMoveTo(object sender, MapActionEventArgs<Point2D> e)
        {
            ActionContext<IMapView2D, Point2D> context
                = new ActionContext<IMapView2D, Point2D>(Map, View, _previousActionPoint, e.ActionPoint);
            Map.GetActiveTool<IMapView2D, Point2D>().ExtendAction(context);
            _previousActionPoint = e.ActionPoint;
        }

        // Handles the end action request from the view
        private void handleViewEndAction(object sender, MapActionEventArgs<Point2D> e)
        {
            ActionContext<IMapView2D, Point2D> context
                = new ActionContext<IMapView2D, Point2D>(Map, View, _previousActionPoint, e.ActionPoint);
            Map.GetActiveTool<IMapView2D, Point2D>().EndAction(context);
            _previousActionPoint = Point2D.Empty;
        }

        // Handles the background color change request from the view
        private void handleViewBackgroundColorChangeRequested(object sender, MapViewPropertyChangeEventArgs<StyleColor> e)
        {
            SetViewBackgroundColor(e.CurrentValue, e.RequestedValue);
        }

        // Handles the geographic view center change request from the view
        private void handleViewGeoCenterChangeRequested(object sender, MapViewPropertyChangeEventArgs<Point> e)
        {
            GeoCenterInternal = e.RequestedValue;

            SetViewGeoCenter(e.CurrentValue, e.RequestedValue);
        }

        // Handles the maximum world width change request from the view
        private void handleViewMaximumWorldWidthChangeRequested(object sender, MapViewPropertyChangeEventArgs<Double> e)
        {
            MaximumWorldWidthInternal = e.RequestedValue;

            SetViewMaximumWorldWidth(e.CurrentValue, e.RequestedValue);
        }

        // Handles the minimum world width change request from the view
        private void handleViewMinimumWorldWidthChangeRequested(object sender, MapViewPropertyChangeEventArgs<Double> e)
        {
            MinimumWorldWidthInternal = e.RequestedValue;

            SetViewMinimumWorldWidth(e.CurrentValue, e.RequestedValue);
        }

        // Handles the view envelope change request from the view
        private void handleViewViewEnvelopeChangeRequested(object sender, MapViewPropertyChangeEventArgs<BoundingBox> e)
        {
            ViewEnvelopeInternal = e.RequestedValue;

            SetViewEnvelope(e.CurrentValue, e.RequestedValue);
        }

        // Handles the world aspect ratio change request from the view
        private void handleViewWorldAspectRatioChangeRequested(object sender, MapViewPropertyChangeEventArgs<Double> e)
        {
            WorldAspectRatioInternal = e.RequestedValue;

            SetViewWorldAspectRatio(e.CurrentValue, e.RequestedValue);
        }

        // Handles the view zoom to extents request from the view
        private void handleViewZoomToExtentsRequested(object sender, EventArgs e)
        {
            ZoomToExtentsInternal();
        }

        // Handles the view zoom to specified view bounds request from the view
        private void handleViewZoomToViewBoundsRequested(object sender, MapViewPropertyChangeEventArgs<Rectangle2D> e)
        {
            ZoomToViewBoundsInternal(e.RequestedValue);
        }

        // Handles the view zoom to specified world bounds request from the view
        private void handleViewZoomToWorldBoundsRequested(object sender, MapViewPropertyChangeEventArgs<BoundingBox> e)
        {
            ZoomToWorldBoundsInternal(e.RequestedValue);
        }

        // Handles the view zoom to specified world width request from the view
        private void handleViewZoomToWorldWidthRequested(object sender, MapViewPropertyChangeEventArgs<Double> e)
        {
            ZoomToWorldWidthInternal(e.RequestedValue);
        }

        // Handles the selection changes on the view
        private void handleSelectionChanged(object sender, EventArgs e)
        {
            if (SelectionInternal.Path.CurrentFigure == null)
            {
                return;
            }

            if (ToWorldTransformInternal != null)
            {
                RenderSelection(SelectionInternal);
            }
        }

        #endregion

        #endregion

        #region Private helper methods
        private void wireupExistingLayers(IEnumerable<ILayer> layerCollection)
        {
            foreach (ILayer layer in layerCollection)
            {
                if (layer is IFeatureLayer)
                {
                    IFeatureLayer featureLayer = layer as IFeatureLayer;
                    Debug.Assert(featureLayer != null);
                    wireupLayer(featureLayer);
                }
            }
        }

        private void wireupLayer(IFeatureLayer layer)
        {
            if (!_wiredLayers.Contains(layer))
            {
                _wiredLayers.Add(layer);
                layer.SelectedFeatures.ListChanged += handleSelectedFeaturesListChanged;
                layer.HighlightedFeatures.ListChanged += handleHighlightedFeaturesListChanged;
            }
        }

        private void unwireLayer(IFeatureLayer layer)
        {
            if (_wiredLayers.Contains(layer))
            {
                _wiredLayers.Remove(layer);
                layer.SelectedFeatures.ListChanged -= handleSelectedFeaturesListChanged;
                layer.HighlightedFeatures.ListChanged -= handleHighlightedFeaturesListChanged;
            }
        }

        private void createRenderers()
        {
            Type renderObjectType = GetRenderObjectType();

            // Create renderer for geometry layers
            CreateGeometryRenderer(renderObjectType);

            // Create renderer for label layers
            CreateLabelRenderer(renderObjectType);

            // TODO: create raster renderer
        }

        // Performs computations to set the view metrics to the given envelope.
        private void setViewEnvelopeInternal(BoundingBox newEnvelope)
        {
            if (newEnvelope.IsEmpty)
            {
                throw new ArgumentException("newEnvelope cannot be empty.");
            }

            BoundingBox oldEnvelope = ViewEnvelopeInternal;

            if (oldEnvelope == newEnvelope)
            {
                return;
            }

            Double oldWidth, oldHeight;

            if (oldEnvelope.IsEmpty)
            {
                oldWidth = 1;
                oldHeight = View.ViewSize.Height / View.ViewSize.Width * WorldAspectRatioInternal;
            }
            else
            {
                oldWidth = oldEnvelope.Width;
                oldHeight = oldEnvelope.Height;
            }

            Double normalizedWidth = newEnvelope.Width == 0 ? _minimumWorldWidth : newEnvelope.Width;

            Double widthZoomRatio = normalizedWidth / oldWidth;
            Double heightZoomRatio = newEnvelope.Height / oldHeight;

            // Rescale the width to allow either the width or the height of the requested
            // world bounds to fit into the current view 
            Double newWorldWidth = widthZoomRatio > heightZoomRatio
                                       ? normalizedWidth
                                       : normalizedWidth * heightZoomRatio / widthZoomRatio;

            setViewMetricsInternal(View.ViewSize, newEnvelope.GetCentroid(), newWorldWidth);
        }

        // Performs computations to set the view metrics given the parameters
        // of the view size, the geographic center of the view, and how much
        // of the world to show in the view by width.
        private void setViewMetricsInternal(Size2D newViewSize, GeoPoint newCenter, Double newWorldWidth)
        {
            GeoPoint oldCenter = GeoCenterInternal;

            // Flag to indicate world matrix needs to be recomputed
            Boolean viewMatrixChanged = false;

            // Change geographic center of the view by translating pan matrix
            if (!oldCenter.Equals(newCenter))
            {
                Point mapCenter = Map.Center;
                _translationTransform.OffsetX = (mapCenter.X - newCenter.X);
                _translationTransform.OffsetY = -(mapCenter.Y - newCenter.Y);
                viewMatrixChanged = true;
            }

            if (newWorldWidth < MinimumWorldWidthInternal)
            {
                newWorldWidth = MinimumWorldWidthInternal;
            }

            if (newWorldWidth > MaximumWorldWidthInternal)
            {
                newWorldWidth = MaximumWorldWidthInternal;
            }

            // Compute new world units per pixel based on view size and desired 
            // world width, and scale the transform accordingly
            Double newWorldUnitsPerPixel = newWorldWidth / newViewSize.Width;

            if (newWorldUnitsPerPixel != WorldUnitsPerPixelInternal)
            {
                Double newScale = 1 / newWorldUnitsPerPixel;
                _scaleTransform.M22 = newScale / WorldAspectRatioInternal;
                _scaleTransform.M11 = newScale;
                viewMatrixChanged = true;
            }

            // Change view size
            if (newViewSize != _oldViewSize)
            {
                _toViewCoordinates.OffsetY = newViewSize.Height;
                //View.ViewSize = newViewSize;
                setViewCenter(newViewSize);
                _oldViewSize = newViewSize;
                viewMatrixChanged = true;
            }

            // If the view metrics have changed, recompute the inverse view matrix
            // and set the visible region of the map
            if (viewMatrixChanged)
            {
                Boolean didInitialize = false;

                if (_viewIsEmpty)
                {
                    _viewIsEmpty = false;
                    didInitialize = true;
                }

                // Test values... ignore
                // [ 0.02, 0.00, 0.00 ]  [ 0.00, -0.02, 0.00 ]  [ -15,344.58, 103,765.90, 1.00 ]
                ToViewTransformInternal = concatenateViewMatrix();

                // Test values... ignore
                // [ 41.49, 0.00, 0.00 ]  [ 0.00, -41.49, 0.00 ]  [ 636,704.56, 4,305,639.00, 1.00 ]
                ToWorldTransformInternal = ToViewTransformInternal.Inverse;

                OnViewMatrixChanged();

                if (didInitialize)
                {
                    OnViewMatrixInitialized();
                }

                RenderAllLayers();
            }
        }

        private void projectOrigin(BoundingBox extents)
        {
            if (!extents.IsEmpty)
            {
                _originProjectionTransform.OffsetX = -extents.Left;
                _originProjectionTransform.OffsetY = extents.Bottom;
            }
        }

        private void setViewCenter(Size2D size)
        {
            _viewCenterTransform.OffsetX = size.Width / 2;
            _viewCenterTransform.OffsetY = -size.Height / 2;
        }

        private void setExtentsCenter(BoundingBox extents)
        {
            _extentsCenterTransform.OffsetX = -extents.Width / 2;
            _extentsCenterTransform.OffsetY = extents.Height / 2;
        }

        private Matrix2D concatenateViewMatrix()
        {
            // Concatenate all view matrixes to generate a projection matrix
            // which transforms world coordinates into view coordinates.
            IMatrixD combined = _originProjectionTransform
                .Multiply(_rotationTransform)
                .Multiply(_translationTransform)
                .Multiply(_extentsCenterTransform)
                .Multiply(_scaleTransform)
                .Multiply(_viewCenterTransform)
                .Multiply(_toViewCoordinates);

            return new Matrix2D(combined);
        }

        // Computes the view space coordinates of a point in world space
        private Point2D worldToView(GeoPoint geoPoint)
        {
            if (ToViewTransformInternal == null)
            {
                return Point2D.Empty;
            }
            else
            {
                return ToViewTransformInternal.TransformVector(geoPoint.X, geoPoint.Y);
            }
        }

        // Computes the world space coordinates of a point in view space
        private GeoPoint viewToWorld(Point2D viewPoint)
        {
            if (ToWorldTransformInternal == null)
            {
                return GeoPoint.Empty;
            }
            else
            {
                Point2D values = ToWorldTransformInternal.TransformVector(viewPoint.X, viewPoint.Y);
                return new GeoPoint(values.X, values.Y);
            }
        }

        private void handleSelectedFeaturesListChanged(object sender, ListChangedEventArgs e)
        {
            IFeatureLayer featureLayer = getLayerFromSelectedView(sender);

            if (featureLayer == null)
            {
                return;
            }

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    RenderFeatureLayer(featureLayer);
                    break;
                case ListChangedType.Reset:
                    if (featureLayer.SelectedFeatures.Count > 0)
                    {
                        RenderFeatureLayer(featureLayer);
                    }
                    break;
                default:
                    break;
            }
        }

        private void handleHighlightedFeaturesListChanged(object sender, ListChangedEventArgs e)
        {
            IFeatureLayer featureLayer = getLayerFromHighlightView(sender);

            if (featureLayer == null)
            {
                return;
            }

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    RenderFeatureLayer(featureLayer);
                    break;
                case ListChangedType.Reset:
                    if (featureLayer.HighlightedFeatures.Count > 0)
                    {
                        RenderFeatureLayer(featureLayer);
                    }
                    break;
                default:
                    break;
            }
        }

        private IFeatureLayer getLayerFromSelectedView(object view)
        {
            return getLayerFromFeatureDataView(view, true);
        }

        private IFeatureLayer getLayerFromHighlightView(object view)
        {
            return getLayerFromFeatureDataView(view, false);
        }

        private IFeatureLayer getLayerFromFeatureDataView(object view, Boolean getSelectedView)
        {
            foreach (ILayer layer in Map.Layers)
            {
                if (layer is IFeatureLayer)
                {
                    FeatureDataView compareView = getSelectedView
                                                      ? (layer as IFeatureLayer).SelectedFeatures
                                                      : (layer as IFeatureLayer).HighlightedFeatures;

                    if (ReferenceEquals(compareView, view))
                    {
                        return layer as IFeatureLayer;
                    }
                }
            }

            return null;
        }
        #endregion
    }
}