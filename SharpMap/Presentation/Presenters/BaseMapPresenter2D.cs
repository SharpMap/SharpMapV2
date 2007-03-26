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
using System.Collections.Generic;
using System.IO;
using System.Text;

using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Layers;
using GeoPoint = SharpMap.Geometries.Point;
using SharpMap.Rendering;
using SharpMap.Styles;
using SharpMap.Utilities;

namespace SharpMap.Presentation
{
    public abstract class BaseMapPresenter2D : IMapPresenter<ViewPoint2D, ViewSize2D, ViewRectangle2D>
    {
        //private Dictionary<RuntimeTypeHandle, IRenderer<ViewPoint2D, ViewSize2D, ViewRectangle2D, TRenderObject>> _layerRendererCatalog = new Dictionary<RuntimeTypeHandle, IRenderer<ViewPoint2D, ViewSize2D, ViewRectangle2D, TRenderObject>>();
        private ViewRectangle2D _viewRectangle;
        private double _widthInWorldUnits;
        private GeoPoint _center;
        private double _maximumZoom;
        private double _minimumZoom;
        private IViewMatrix _mapTransform = new ViewMatrix2D();
        private IViewMatrix _mapTransformInverted = new ViewMatrix2D();
        private StyleColor _backgroundColor;
        private double _pixelAspectRatio = 1.0;
        private List<IToolsView> _toolsViews = new List<IToolsView>();
        private ToolSet _activeTool;
        private IMapView2D _concreteView;
        private Map _map;
        private ViewSelection2D _selection;
        private ViewPoint2D? _beginActionLocation;
        private ViewPoint2D _lastMoveLocation;

        public BaseMapPresenter2D(Map map, IMapView2D mapView, IEnumerable<IToolsView> toolsViews)
        {
            Map = map;
            
            MapView = mapView;
            MapView.Hover += new EventHandler<MapActionEventArgs>(MapView_Hover);
            MapView.BeginAction += new EventHandler<MapActionEventArgs>(MapView_BeginAction);
            MapView.MoveTo += new EventHandler<MapActionEventArgs>(MapView_MoveTo);
            MapView.EndAction += new EventHandler<MapActionEventArgs>(MapView_EndAction);
            MapView.ViewSizeChanged += new EventHandler(MapView_ViewSizeChanged);

            MapSharedState.LayersChanged += new EventHandler(MapSharedState_LayersChanged);
            MapSharedState.SelectedFeaturesChanged += new EventHandler(MapSharedState_SelectedFeaturesChanged);

            ToolsViews = new List<IToolsView>(toolsViews);
            foreach (IToolsView toolView in ToolsViews)
                toolView.ToolSelectionChanged += new EventHandler(ToolsView_ToolSelectionChanged);
        }

        protected void RegisterRenderer<TLayerType, TRenderObject>(IRenderer<ViewPoint2D, ViewSize2D, ViewRectangle2D, TRenderObject> renderer)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");

            LayerRendererCatalog.Instance.Register<ViewPoint2D, ViewSize2D, ViewRectangle2D, TRenderObject>(typeof(TLayerType), renderer);
        }

        protected TRenderer GetRenderer<TRenderer>(ILayer layer)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            Type layerType = layer.GetType();
            return LayerRendererCatalog.Instance.Get<TRenderer>(layerType);
        }

        protected void CreateSelection(ViewPoint2D location)
        {
            _selection = ViewSelection2D.CreateRectangluarSelection(location, ViewSize2D.Zero);
        }

        protected void ModifySelection(ViewPoint2D location, ViewSize2D size)
        {
            _selection.MoveTo(location);
            _selection.Expand(size);
        }

        protected void DestroySelection()
        {
            _selection = null;
        }

        protected virtual void OnMapViewSizeChanged()
        {
        }

        protected virtual void OnMapViewHover(ViewPoint2D viewPoint2D)
        {
            switch (MapSharedState.SelectedTool)
            {
                case ToolSet.Pan:
                    break;
                case ToolSet.ZoomIn:
                    break;
                case ToolSet.ZoomOut:
                    break;
                case ToolSet.Query:
                    break;
                case ToolSet.QueryAdd:
                    break;
                case ToolSet.QueryRemove:
                    break;
                case ToolSet.FeatureAdd:
                    break;
                case ToolSet.FeatureRemove:
                    break;
                case ToolSet.None:
                default:
                    break;
            }
        }

        protected virtual void OnMapViewBeginAction(ViewPoint2D location)
        {
            _beginActionLocation = location;

            switch (MapSharedState.SelectedTool)
            {
                case ToolSet.Pan:
                    break;
                case ToolSet.ZoomIn:
                    break;
                case ToolSet.ZoomOut:
                    break;
                case ToolSet.Query:
                    break;
                case ToolSet.QueryAdd:
                    break;
                case ToolSet.QueryRemove:
                    break;
                case ToolSet.FeatureAdd:
                    break;
                case ToolSet.FeatureRemove:
                    break;
                case ToolSet.None:
                default:
                    break;
            }
        }

        protected virtual void OnMapViewMoveTo(ViewPoint2D location)
        {
            double xDelta = location.X - _lastMoveLocation.X;
            double yDelta = location.Y - _lastMoveLocation.Y;

            if (Selection == null)
                CreateSelection(location);

            switch (MapSharedState.SelectedTool)
            {
                case ToolSet.Pan:
                    break;
                case ToolSet.ZoomIn:
                    modifySelection(xDelta, yDelta);
                    break;
                case ToolSet.ZoomOut:
                    modifySelection(xDelta, yDelta);
                    break;
                case ToolSet.Query:
                    modifySelection(xDelta, yDelta);
                    break;
                case ToolSet.QueryAdd:
                    break;
                case ToolSet.QueryRemove:
                    break;
                case ToolSet.FeatureAdd:
                    break;
                case ToolSet.FeatureRemove:
                    break;
                case ToolSet.None:
                default:
                    break;
            }

            _lastMoveLocation = location;
        }

        protected virtual void OnMapViewEndAction(ViewPoint2D location)
        {
            _beginActionLocation = null;

            switch (MapSharedState.SelectedTool)
            {
                case ToolSet.Pan:
                    break;
                case ToolSet.ZoomIn:
                    break;
                case ToolSet.ZoomOut:
                    break;
                case ToolSet.Query:
                    BoundingBox queryRegion = ViewToWorld(Selection.BoundingRegion);
                    IFeatureLayer queryLayer = MapSharedState.ActiveLayer as IFeatureLayer;

                    if (queryLayer != null)
                        MapSharedState.SelectedFeatures = queryLayer.GetFeatures(queryRegion);

                    DestroySelection();
                    break;
                case ToolSet.QueryAdd:
                    break;
                case ToolSet.QueryRemove:
                    break;
                case ToolSet.FeatureAdd:
                    break;
                case ToolSet.FeatureRemove:
                    break;
                case ToolSet.None:
                default:
                    break;
            }
        }

        protected virtual void OnSelectionModified()
        {
        }

        protected virtual void OnSelectedFeaturesChanged()
        {
        }

        protected virtual void OnLayersChanged()
        {
        }

        #region IMapPresenter<ViewPoint2D,ViewSize2D,ViewRectangle2D> Members
        /// <summary>
        /// Event fired when Map is resized.
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<ViewSize2D>> SizeChanged;

        /// <summary>
        /// Event fired when the zoom value has changed
        /// </summary>
        public event EventHandler<MapZoomChangedEventArgs> ZoomChanged;

        /// <summary>
        /// Event fired when the center of the map has changed
        /// </summary>
        public event EventHandler<MapPresentationPropertyChangedEventArgs<GeoPoint>> CenterChanged;

        /// <summary>
        /// Event fired when any view property is changed.
        /// </summary>
        public event EventHandler MapViewChanged;

        public event EventHandler<MapPresentationPropertyChangedEventArgs<double>> MinimumZoomChanged;
        public event EventHandler<MapPresentationPropertyChangedEventArgs<double>> MaximumZoomChanged;
        public event EventHandler<MapPresentationPropertyChangedEventArgs<double>> PixelAspectRatioChanged;
        public event EventHandler<MapPresentationPropertyChangedEventArgs<ViewRectangle2D>> ViewRectangleChanged;
        public event EventHandler<MapPresentationPropertyChangedEventArgs<IViewMatrix>> MapTransformChanged;
        public event EventHandler<MapPresentationPropertyChangedEventArgs<StyleColor>> BackgroundColorChanged;
        public event EventHandler<MapPresentationPropertyChangedEventArgs<StyleRenderingMode>> StyleRenderingModeChanged;

        public Map Map
        {
            get { return _map; }
            protected set { _map = value; }
        }

        public IMapView2D MapView
        {
            get { return _concreteView; }
            set
            {
                if (_concreteView != value)
                {
                    _concreteView = value;
                    OnMapViewChanged();
                }
            }
        }

        public IList<IToolsView> ToolsViews
        {
            get { return _toolsViews; }
            set 
            {
                _toolsViews.Clear();
                _toolsViews.AddRange(value);
            }
        }

        //public ToolSet ActiveTool
        //{
        //    get { return _activeTool; }
        //}

        public ViewSelection2D Selection
        {
            get { return _selection; }
            private set { _selection = value; }
        }

        //public StyleSmoothingMode SmoothingMode
        //{
        //    get { return _smoothingMode; }
        //    set
        //    {
        //        if (_smoothingMode != value)
        //        {
        //            StyleSmoothingMode oldValue = _smoothingMode;
        //            _smoothingMode = value;
        //            OnSmoothingModeChanged(oldValue, _smoothingMode);
        //        }
        //    }
        //}

        /// <summary>
        /// Height of view in world units
        /// </summary>
        /// <returns></returns>
        public double ViewHeight
        {
            get { return (Zoom * ViewSize.Height) / ViewSize.Width * PixelAspectRatio; }
        }

        public ViewRectangle2D ViewRectangle
        {
            get { return _viewRectangle; }
        }

        public ViewSize2D ViewSize
        {
            get { return ViewRectangle.Size; }
            set
            {
                if (value != ViewRectangle.Size)
                {
                    setViewMetricsInternal(new ViewRectangle2D(ViewRectangle.Location, ViewRectangle.Size), _center, _widthInWorldUnits);
                }
            }
        }

        /// <summary>
        /// Gets or sets the extents of the current map based on the current <see cref="Zoom"/>, 
        /// <see cref="ViewCenter"/> and <see cref="ViewRectangle"/>.
        /// </summary>
        public BoundingBox ViewEnvelope
        {
            get { return ViewToWorld(ViewRectangle); }
            set 
            {
                setViewEnvelopeInternal(value);
            }
        }

        /// <summary>
        /// Gets or sets center of map in world coordinates.
        /// </summary>
        public GeoPoint ViewCenter
        {
            get { return _center.Clone(); }
            set { setViewMetricsInternal(ViewRectangle, value, _widthInWorldUnits); }
        }

        /// <summary>
        /// Gets or sets the zoom level of map.
        /// </summary>
        /// <remarks>
        /// <para>The zoom level corresponds to the <see cref="Map.Width"/> of the map in world coordinate units.</para>
        /// <para>A zoom level &lt;= 0 will result in an empty map being rendered, but will not throw an exception.</para>
        /// </remarks>
        public double Zoom
        {
            get { return _widthInWorldUnits; }
            set { setViewMetricsInternal(ViewRectangle, ViewCenter, value); }
        }

        /// <summary>
        /// Minimum zoom amount allowed.
        /// </summary>
        public double MinimumZoom
        {
            get { return _minimumZoom; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Minimum zoom must be 0 or more");

                if (_minimumZoom != value)
                {
                    double oldValue = _minimumZoom;
                    _minimumZoom = value;
                    OnMinimumZoomChanged(oldValue, _minimumZoom);
                }
            }
        }

        /// <summary>
        /// Maximum zoom amount allowed.
        /// </summary>
        public double MaximumZoom
        {
            get { return _maximumZoom; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Maximum zoom must larger than 0");

                if (_maximumZoom != value)
                {
                    double oldValue = _maximumZoom;
                    _maximumZoom = value;
                    OnMaximumZoomChanged(oldValue, _maximumZoom);
                }
            }
        }

        /// <summary>
        /// Gets the size of a pixel in world coordinate units.
        /// </summary>
        public double PixelSize
        {
            get { return this.Zoom / this.ViewSize.Width; }
        }

        /// <summary>
        /// Gets the width of a pixel in world coordinate units.
        /// </summary>
        /// <remarks>The value returned is the same as <see cref="PixelSize"/>.</remarks>
        public double PixelWidth
        {
            get { return PixelSize; }
        }

        /// <summary>
        /// Gets the height of a pixel in world coordinate units.
        /// </summary>
        /// <remarks>The value returned is the same as <see cref="PixelSize"/> 
        /// unless <see cref="PixelAspectRatio"/> is different from 1.</remarks>
        public double PixelHeight
        {
            get { return PixelSize * _pixelAspectRatio; }
        }

        /// <summary>
        /// Gets or sets the aspect-ratio of the pixel scales. A value less than 
        /// 1 will make the map streach upwards, and larger than 1 will make it smaller.
        /// </summary>
        /// <exception cref="ArgumentException">Throws an argument exception when value 
        /// is 0 or less.</exception>
        public double PixelAspectRatio
        {
            get { return _pixelAspectRatio; }
            set
            {
                if (_pixelAspectRatio <= 0)
                    throw new ArgumentOutOfRangeException("Invalid pixel aspect ratio");

                if (_pixelAspectRatio != value)
                {
                    double oldValue = _pixelAspectRatio;
                    _pixelAspectRatio = value;
                    OnPixelAspectRatioChanged(oldValue, _pixelAspectRatio);
                }
            }
        }

        /// <summary>
        /// Gets or sets map background color.
        /// </summary>
        /// <remarks>
        /// Defaults to transparent.
        /// </remarks>
        public StyleColor BackColor
        {
            get { return _backgroundColor; }
            set
            {
                if (_backgroundColor != value)
                {
                    StyleColor oldValue = _backgroundColor;
                    _backgroundColor = value;
                    OnBackgroundColorChanged(oldValue, _backgroundColor);
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Matrix"/> used to transform the map image.
        /// </summary>
        /// <remarks>
        /// Using the <see cref="MapTransform"/> you can alter the coordinate system of the map rendering.
        /// This makes it possible to rotate or rescale the image, for instance to have another direction 
        /// than north upwards.
        /// </remarks>
        /// <example>
        /// Rotate the map output 45 degrees around its center:
        /// <code lang="C#">
        /// IViewMatrix maptransform = new SharpMap.Rendering.ViewMatrix2D(); //Create transformation matrix
        ///	maptransform.RotateAt(45, myMap.ViewCenter);    //Apply 45 degrees rotation around the center of the map
        ///	myMap.MapTransform = maptransform;              //Apply transformation to map
        /// </code>
        /// </example>
        public IViewMatrix MapTransform
        {
            get { return _mapTransform; }
            set
            {
                if (_mapTransform != value)
                {
                    IViewMatrix oldValue = _mapTransform;
                    _mapTransform = value;

                    if (_mapTransform.IsInvertible)
                    {
                        MapTransformInverted = _mapTransform.Clone() as IViewMatrix;
                        MapTransformInverted.Invert();
                    }
                    else
                    {
                        MapTransformInverted.Reset();
                    }

                    OnMapTransformChanged(oldValue, _mapTransform);
                }
            }
        }

        public IViewMatrix MapTransformInverted
        {
            get { return _mapTransformInverted; }
            private set { _mapTransformInverted = value; }
        }

        //public IEnumerable<FeatureDataRow> Query(BoundingBox region)
        //{
        //    IFeatureLayer layer = ActiveLayer as IFeatureLayer;
        //    if (layer == null)
        //        return new FeatureDataRow[0];

        //    return layer.GetFeatures(region);
        //}

        //public IEnumerable<FeatureDataRow> Query(GeoPoint location)
        //{
        //    return Query(location.GetBoundingBox());
        //}

        public IRenderer<ViewPoint2D, ViewSize2D, ViewRectangle2D, TRenderObject> GetRendererForLayer<TRenderObject>(ILayer layer)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            IRenderer<ViewPoint2D, ViewSize2D, ViewRectangle2D, TRenderObject> renderer = null;
            
            //_layerRendererCatalog.TryGetValue(layer.GetType().TypeHandle, out renderer);
            renderer = LayerRendererCatalog.Instance.Get<IRenderer<ViewPoint2D, ViewSize2D, ViewRectangle2D, TRenderObject>>(layer.GetType());
            return renderer;
        }

        //public void SetActiveLayer(int index)
        //{
        //    if (index < 0 || _map.Layers.Count >= index)
        //        throw new ArgumentOutOfRangeException("index");

        //    _activeLayer = index;
        //}

        //public void SetActiveLayer(string layerName)
        //{
        //    ILayer layer = _map.GetLayerByName(layerName);
            
        //    if (layer == null)
        //        throw new ArgumentException(String.Format("No layer exists with name {0}", layerName));

        //    _activeLayer = _map.Layers.IndexOf(layer);
        //}

        ///// <summary>
        ///// Renders the map to a view.
        ///// </summary>
        //public void RenderMap()
        //{
        //    RenderMap(ViewEnvelope);
        //}

        ///// <summary>
        ///// Renders the map to a view.
        ///// </summary>
        ///// <param name="region">Region to restrict rendering to.</param>
        //public void RenderMap(BoundingBox region)
        //{
        //    if (_map.Layers == null || _map.Layers.Count == 0)
        //        throw new InvalidOperationException("No layers to render");

        //    foreach (ILayer layer in _map.Layers)
        //    {
        //        ILayerRenderer presenter = GetRendererForLayer(layer.LayerName);
        //        presenter.Render(_concreteView, region);
        //    }

        //    OnMapRendered(_concreteView);
        //}
        
        /// <summary>
        /// Converts a <see cref="SharpMap.Geometries.Point" /> from world coordinates to image 
        /// coordinates based on the current <see cref="IMapPresenter.Zoom"/>, <see cref="IMapPresenter.Center"/> 
        /// and <see cref="IMapPresenter.Size"/>.
        /// </summary>
        /// <param name="p"><see cref="SharpMap.Geometries.Point" /> in world coordinates</param>
        /// <returns><see cref="ViewPoint">Point</see> in image coordinates</returns>
        public ViewPoint2D WorldToView(GeoPoint p)
        {
            return Transform2D.WorldToMap(p, this);
        }

        /// <summary>
        /// Converts a <see cref="BoundingBox"/> from world coordinates to image coordinates based on the current
        /// <see cref="IMapPresenter.Zoom"/>, <see cref="IMapPresenter.Center"/> and <see cref="IMapPresenter.Size"/>.
        /// </summary>
        /// <param name="bbox"><see cref="BoundingBox">Rectangle</see> in world coordinates</param>
        /// <returns><see cref="ViewRectangle"/> in image coordinates</returns>
        public ViewRectangle2D WorldToView(BoundingBox bbox)
        {
            return Transform2D.WorldToMap(bbox, this);
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.PointF"/> from image coordinates to world 
        /// coordinates based on the current <see cref="IMapPresenter.Zoom"/>, 
        /// <see cref="IMapPresenter.Center"/> and <see cref="IMapPresenter.Size"/>.
        /// </summary>
        /// <param name="p"><see cref="ViewPoint">Point</see> in image coordinates.</param>
        /// <returns><see cref="SharpMap.Geometries.Point" /> in world coordinates.</returns>
        public GeoPoint ViewToWorld(ViewPoint2D p)
        {
            return Transform2D.MapToWorld(p, this);
        }

        /// <summary>
        /// Converts a <see cref="ViewRectangle"/> from image coordinates to world coordinates 
        /// based on the current <see cref="IMapPresenter.Zoom"/>, 
        /// <see cref="IMapPresenter.Center"/> and <see cref="IMapPresenter.Size"/>.
        /// </summary>
        /// <param name="rect"><see cref="ViewRectangle"/> in image coordinates</param>
        /// <returns><see cref="BoundingBox">BoundingBox</see> in world coordinates</returns>
        public BoundingBox ViewToWorld(ViewRectangle2D rect)
        {
            return Transform2D.MapToWorld(rect, this);
        }

        /// <summary>
        /// Zooms to the extents of all layers
        /// </summary>
        public void ZoomToExtents()
        {
            this.ZoomToBox(_map.GetExtents());
        }

        /// <summary>
        /// Zooms the map to fit a bounding box.
        /// </summary>
        /// <remarks>
        /// If the ratio of either the width of the current map <see cref="Envelope">envelope</see> 
        /// to the width of <paramref name="zoomBox"/> or of the height of the current map <see cref="Envelope">envelope</see> 
        /// to the height of <paramref name="zoomBox"/> is greater, the map envelope will be enlarged to contain the 
        /// <paramref name="zoomBox"/> parameter.
        /// </remarks>
        /// <param name="zoomBox"><see cref="BoundingBox"/> to set zoom to.</param>
        public void ZoomToBox(BoundingBox zoomBox)
        {
            setViewEnvelopeInternal(zoomBox);
        }

        public IEnumerable<ViewPoint2D> TransformToView(IEnumerable<GeoPoint> points)
        {
            foreach (GeoPoint point in points)
                yield return WorldToView(point);
        }

        public ViewPoint2D TransformToView(GeoPoint point)
        {
            return WorldToView(point);
        }


        IViewSelection<ViewPoint2D, ViewSize2D, ViewRectangle2D> IMapPresenter<ViewPoint2D, ViewSize2D, ViewRectangle2D>.Selection
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) { }

        #endregion	

        #region Private helper methods

        private void MapSharedState_SelectedFeaturesChanged(object sender, EventArgs e)
        {
            OnSelectedFeaturesChanged();
        }

        private void MapSharedState_LayersChanged(object sender, EventArgs e)
        {
            OnLayersChanged();
        }

        private void MapView_ViewSizeChanged(object sender, EventArgs e)
        {
            OnMapViewSizeChanged();
        }

        private void MapView_EndAction(object sender, MapActionEventArgs e)
        {
            OnMapViewEndAction(e.ActionPoint);
        }

        private void MapView_MoveTo(object sender, MapActionEventArgs e)
        {
            OnMapViewMoveTo(e.ActionPoint);
        }

        private void MapView_BeginAction(object sender, MapActionEventArgs e)
        {
            OnMapViewBeginAction(e.ActionPoint);
        }

        private void MapView_Hover(object sender, MapActionEventArgs e)
        {
            OnMapViewHover(e.ActionPoint);
        }

        private void ToolsView_ToolSelectionChanged(object sender, EventArgs e)
        {
            IToolsView senderView = sender as IToolsView;

            _activeTool = senderView.SelectedTool;

            foreach (IToolsView toolView in ToolsViews)
            {
                if (!Object.ReferenceEquals(senderView, toolView))
                    toolView.SelectedTool = _activeTool;
            }
        }

        private void modifySelection(double xDelta, double yDelta)
        {
            Selection.Expand(new ViewSize2D(xDelta, yDelta));
            OnSelectionModified();
        }

        private void setCenterInternal(GeoPoint newCenter)
        {
            if (_center == newCenter)
                return;

            setViewMetricsInternal(ViewRectangle, newCenter, _widthInWorldUnits);
        }

        private void setViewEnvelopeInternal(BoundingBox newEnvelope)
        {
            BoundingBox currentEnvelope = ViewEnvelope;

            if (currentEnvelope == newEnvelope)
                return;

            double widthZoomRatio = newEnvelope.Width / currentEnvelope.Width;
            double heightZoomRatio = newEnvelope.Height / currentEnvelope.Height;
            double newWidth = widthZoomRatio > heightZoomRatio ? newEnvelope.Width : newEnvelope.Width * heightZoomRatio;

            if (newWidth < _minimumZoom)
                newWidth = _minimumZoom;

            if (newWidth > _maximumZoom)
                newWidth = _maximumZoom;

            setViewMetricsInternal(ViewRectangle, newEnvelope.GetCentroid(), newWidth);
        }

        private void setViewMetricsInternal(ViewRectangle2D viewRectangle, GeoPoint center, double widthInWorldUnits)
        {
            if (viewRectangle == _viewRectangle && center == _center && widthInWorldUnits == _widthInWorldUnits)
                return;

            ViewRectangle2D oldViewRectangle = _viewRectangle;
            double oldWidthInWorldUnits = _widthInWorldUnits;
            GeoPoint oldCenter = _center;

            _viewRectangle = viewRectangle;
            _widthInWorldUnits = widthInWorldUnits;
            _center = center;

            if (_center != oldCenter)
                OnMapCenterChanged(oldCenter, _center);

            if (_widthInWorldUnits != oldWidthInWorldUnits)
                OnZoomChanged(oldWidthInWorldUnits, _widthInWorldUnits, oldCenter, _center);

            if (_viewRectangle != oldViewRectangle)
            {
                OnViewRectangleChanged(oldViewRectangle, _viewRectangle);

                if (_viewRectangle.Size != oldViewRectangle.Size)
                {
                    OnSizeChanged(oldViewRectangle.Size, _viewRectangle.Size);
                }
            }
        }
        #endregion

        #region Protected Property Change Notification Methods
        //protected virtual void OnMapRendered(IMapView2D view)
        //{
        //    EventHandler @event = MapRendered;
        //    if (@event != null)
        //        @event(this, new MapRenderedEventArgs(view)); //Fire render event
        //}

        protected virtual void OnMapViewChanged()
        {
            EventHandler @event = MapViewChanged;
            if (@event != null)
                @event(this, EventArgs.Empty);
        }

        protected virtual void OnMinimumZoomChanged(double oldMinimumZoom, double newMinimumZoom)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<double>> @event = MinimumZoomChanged;
            if (@event != null)
                @event(this, new MapPresentationPropertyChangedEventArgs<double>(oldMinimumZoom, newMinimumZoom));

            //OnMapViewChanged();
        }

        protected virtual void OnMaximumZoomChanged(double oldMaximumZoom, double newMaximumZoom)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<double>> @event = MaximumZoomChanged;
            if (@event != null)
                @event(this, new MapPresentationPropertyChangedEventArgs<double>(oldMaximumZoom, newMaximumZoom));

            //OnMapViewChanged();
        }

        protected virtual void OnPixelAspectRatioChanged(double oldPixelAspectRatio, double newPixelAspectRatio)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<double>> @event = PixelAspectRatioChanged;
            if (@event != null)
                @event(this, new MapPresentationPropertyChangedEventArgs<double>(oldPixelAspectRatio, newPixelAspectRatio));

            //OnMapViewChanged();
        }

        protected virtual void OnViewRectangleChanged(ViewRectangle2D oldViewRectangle, ViewRectangle2D currentViewRectangle)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<ViewRectangle2D>> @event = ViewRectangleChanged;
            if (@event != null)
                @event(this, new MapPresentationPropertyChangedEventArgs<ViewRectangle2D>(oldViewRectangle, currentViewRectangle));

            //OnMapViewChanged();
        }

        protected virtual void OnMapTransformChanged(IViewMatrix oldTransform, IViewMatrix newTransform)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<IViewMatrix>> @event = MapTransformChanged;
            if (@event != null)
                @event(this, new MapPresentationPropertyChangedEventArgs<IViewMatrix>(oldTransform, newTransform));

            //OnMapViewChanged();
        }

        protected virtual void OnBackgroundColorChanged(StyleColor oldColor, StyleColor newColor)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<StyleColor>> @event = BackgroundColorChanged;
            if (@event != null)
                @event(this, new MapPresentationPropertyChangedEventArgs<StyleColor>(oldColor, newColor));

            //OnMapViewChanged();
        }

        protected virtual void OnStyleRenderingModeChanged(StyleRenderingMode oldValue, StyleRenderingMode newValue)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<StyleRenderingMode>> @event = StyleRenderingModeChanged;
            if (@event != null)
                @event(this, new MapPresentationPropertyChangedEventArgs<StyleRenderingMode>(oldValue, newValue));

            //OnMapViewChanged();
        }

        protected virtual void OnZoomChanged(double previousZoom, double currentZoom, GeoPoint previousCenter, GeoPoint currentCenter)
        {
            EventHandler<MapZoomChangedEventArgs> @event = ZoomChanged;
            if (@event != null)
                @event(this, new MapZoomChangedEventArgs(previousZoom, currentZoom, previousCenter, currentCenter));

            //OnMapViewChanged();
        }

        protected virtual void OnMapCenterChanged(GeoPoint previousCenter, GeoPoint currentCenter)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<GeoPoint>> @event = CenterChanged;
            if (@event != null)
                @event(this, new MapPresentationPropertyChangedEventArgs<GeoPoint>(previousCenter, currentCenter));

            //OnMapViewChanged();
        }

        protected virtual void OnSizeChanged(ViewSize2D oldSize, ViewSize2D newSize)
        {
            EventHandler<MapPresentationPropertyChangedEventArgs<ViewSize2D>> @event = SizeChanged;
            if (@event != null)
                @event(this, new MapPresentationPropertyChangedEventArgs<ViewSize2D>(oldSize, newSize));

            //OnMapViewChanged();
        }

        #endregion
    }
}
