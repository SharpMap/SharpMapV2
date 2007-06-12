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

using SharpMap.Map;
using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Layers;
using GeoPoint = SharpMap.Geometries.Point;
using SharpMap.Rendering;
using SharpMap.Styles;
using SharpMap.Utilities;

namespace SharpMap.Presentation
{
    /// <summary>
    /// Provides the input-handling and view-updating logic for a 2D map view.
    /// </summary>
    public class MapPresenter2D //: IViewTransformer<ViewPoint2D, ViewRectangle2D>
    {
        private SharpMap.Map.Map _map;
        private IMapView2D _concreteView;
        private ViewSelection2D _selection;

        private ViewPoint2D? _beginActionLocation;
        private ViewPoint2D _lastMoveLocation;

        public MapPresenter2D(SharpMap.Map.Map map, IMapView2D mapView)
        {
            if (mapView.ViewPort == null)
            {
                throw new InvalidOperationException("Parameter mapView must have an initialized ViewPort property.");
            }

            Map = map;
            Map.LayersAdded += new EventHandler<LayersChangedEventArgs>(Map_LayersChanged);
            Map.LayersRemoved += new EventHandler<LayersChangedEventArgs>(Map_LayersChanged);
            Map.SelectedFeaturesChanged += new EventHandler(Map_SelectedFeaturesChanged);

            MapView = mapView;
            MapView.Hover += new EventHandler<MapActionEventArgs<ViewPoint2D>>(MapView_Hover);
            MapView.BeginAction += new EventHandler<MapActionEventArgs<ViewPoint2D>>(MapView_BeginAction);
            MapView.MoveTo += new EventHandler<MapActionEventArgs<ViewPoint2D>>(MapView_MoveTo);
            MapView.EndAction += new EventHandler<MapActionEventArgs<ViewPoint2D>>(MapView_EndAction);

            MapViewPort2D viewPort = MapView.ViewPort;
            viewPort.CenterChanged += new EventHandler<MapPresentationPropertyChangedEventArgs<ViewPoint2D, Point>>(ViewPort_CenterChanged);
            viewPort.MapTransformChanged += new EventHandler<MapPresentationPropertyChangedEventArgs<IViewMatrix>>(ViewPort_MapTransformChanged);
            viewPort.MaximumWorldWidthChanged += new EventHandler<MapPresentationPropertyChangedEventArgs<double>>(ViewPort_MaximumZoomChanged);
            viewPort.MinimumWorldWidthChanged += new EventHandler<MapPresentationPropertyChangedEventArgs<double>>(ViewPort_MinimumZoomChanged);
            viewPort.PixelAspectRatioChanged += new EventHandler<MapPresentationPropertyChangedEventArgs<double>>(ViewPort_PixelAspectRatioChanged);
            viewPort.SizeChanged += new EventHandler<MapPresentationPropertyChangedEventArgs<ViewSize2D>>(ViewPort_SizeChanged);
            viewPort.StyleRenderingModeChanged += new EventHandler<MapPresentationPropertyChangedEventArgs<StyleRenderingMode>>(ViewPort_StyleRenderingModeChanged);
            viewPort.ViewRectangleChanged += new EventHandler<MapPresentationPropertyChangedEventArgs<ViewRectangle2D, BoundingBox>>(ViewPort_ViewRectangleChanged);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion

        /// <summary>
        /// The map.
        /// </summary>
        public SharpMap.Map.Map Map
        {
            get { return _map; }
            protected set { _map = value; }
        }

        public IMapView2D MapView
        {
            get { return _concreteView; }
            protected set { _concreteView = value; }
        }

        /// <summary>
        /// A selection on a view.
        /// </summary>
        public ViewSelection2D Selection
        {
            get { return _selection; }
            private set { _selection = value; }
        }

        ///// <summary>
        ///// Converts a <see cref="SharpMap.Geometries.Point" /> from world coordinates to image 
        ///// coordinates based on the current <see cref="IMapPresenter.Zoom"/>, <see cref="IMapPresenter.Center"/> 
        ///// and <see cref="IMapPresenter.Size"/>.
        ///// </summary>
        ///// <param name="p"><see cref="SharpMap.Geometries.Point" /> in world coordinates</param>
        ///// <returns><see cref="ViewPoint">Point</see> in image coordinates</returns>
        //public ViewPoint2D WorldToView(GeoPoint p, IMapView2D view)
        //{
        //    return Transform2D.WorldToMap(p, view.ViewPort);
        //}

        ///// <summary>
        ///// Converts a <see cref="BoundingBox"/> from world coordinates to image coordinates based on the current
        ///// <see cref="IMapPresenter.Zoom"/>, <see cref="IMapPresenter.Center"/> and <see cref="IMapPresenter.Size"/>.
        ///// </summary>
        ///// <param name="bbox"><see cref="BoundingBox">Rectangle</see> in world coordinates</param>
        ///// <returns><see cref="ViewRectangle"/> in image coordinates</returns>
        //public ViewRectangle2D WorldToView(BoundingBox bbox, IMapView2D view)
        //{
        //    return Transform2D.WorldToMap(bbox, view.ViewPort);
        //}

        ///// <summary>
        ///// Converts a <see cref="System.Drawing.PointF"/> from image coordinates to world 
        ///// coordinates based on the current <see cref="IMapPresenter.Zoom"/>, 
        ///// <see cref="IMapPresenter.Center"/> and <see cref="IMapPresenter.Size"/>.
        ///// </summary>
        ///// <param name="p"><see cref="ViewPoint">Point</see> in image coordinates.</param>
        ///// <returns><see cref="SharpMap.Geometries.Point" /> in world coordinates.</returns>
        //public GeoPoint ViewToWorld(ViewPoint2D p, IMapView2D view)
        //{
        //    return Transform2D.MapToWorld(p, view.ViewPort);
        //}

        ///// <summary>
        ///// Converts a <see cref="ViewRectangle"/> from image coordinates to world coordinates 
        ///// based on the current <see cref="IMapPresenter.Zoom"/>, 
        ///// <see cref="IMapPresenter.Center"/> and <see cref="IMapPresenter.Size"/>.
        ///// </summary>
        ///// <param name="rect"><see cref="ViewRectangle"/> in image coordinates</param>
        ///// <returns><see cref="BoundingBox">BoundingBox</see> in world coordinates</returns>
        //public BoundingBox ViewToWorld(ViewRectangle2D rect, IMapView2D view)
        //{
        //    return Transform2D.MapToWorld(rect, view.ViewPort);
        //}

        //public IEnumerable<ViewPoint2D> TransformToView(IEnumerable<GeoPoint> points)
        //{
        //    foreach (GeoPoint point in points)
        //        yield return WorldToView(point);
        //}

        //public ViewPoint2D TransformToView(GeoPoint point)
        //{
        //    return WorldToView(point);
        //}

        protected void RegisterRenderer<TLayerType, TRenderObject>(IRenderer<ViewPoint2D, ViewSize2D, ViewRectangle2D, TRenderObject> renderer)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException("renderer");
            }

            LayerRendererCatalog.Instance.Register<ViewPoint2D, ViewSize2D, ViewRectangle2D, TRenderObject>(typeof(TLayerType), renderer);
        }

        public IRenderer<ViewPoint2D, ViewSize2D, ViewRectangle2D, TRenderObject> GetRendererForLayer<TRenderObject>(ILayer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }

            IRenderer<ViewPoint2D, ViewSize2D, ViewRectangle2D, TRenderObject> renderer = null;

            //_layerRendererCatalog.TryGetValue(layer.GetType().TypeHandle, out renderer);
            renderer = LayerRendererCatalog.Instance.Get<IRenderer<ViewPoint2D, ViewSize2D, ViewRectangle2D, TRenderObject>>(layer.GetType());
            return renderer;
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

        protected void CreateSelection(ViewPoint2D location)
        {
            _selection = ViewSelection2D.CreateRectangluarSelection(location, ViewSize2D.Zero);
        }

        protected void ModifySelection(ViewPoint2D location, ViewSize2D size)
        {
            _selection.MoveBy(location);
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
        }

        protected virtual void OnMapViewBeginAction(ViewPoint2D location)
        {
            _beginActionLocation = location;
        }

        protected virtual void OnMapViewMoveTo(ViewPoint2D location)
        {
            double xDelta = location.X - _lastMoveLocation.X;
            double yDelta = location.Y - _lastMoveLocation.Y;

            if (Selection == null)
            {
                CreateSelection(location);
            }

            _lastMoveLocation = location;
        }

        protected virtual void OnMapViewEndAction(ViewPoint2D location)
        {
            _beginActionLocation = null;
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

        #region Event handlers
        private void Map_SelectedFeaturesChanged(object sender, EventArgs e)
        {
            OnSelectedFeaturesChanged();
        }

        private void Map_LayersChanged(object sender, LayersChangedEventArgs e)
        {
            OnLayersChanged();
        }

        private void MapView_EndAction(object sender, MapActionEventArgs<ViewPoint2D> e)
        {
            OnMapViewEndAction(e.ActionPoint);
        }

        private void MapView_MoveTo(object sender, MapActionEventArgs<ViewPoint2D> e)
        {
            OnMapViewMoveTo(e.ActionPoint);
        }

        private void MapView_BeginAction(object sender, MapActionEventArgs<ViewPoint2D> e)
        {
            OnMapViewBeginAction(e.ActionPoint);
        }

        private void MapView_Hover(object sender, MapActionEventArgs<ViewPoint2D> e)
        {
            OnMapViewHover(e.ActionPoint);
        }

        void ViewPort_CenterChanged(object sender, MapPresentationPropertyChangedEventArgs<ViewPoint2D, Point> e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void ViewPort_MapTransformChanged(object sender, MapPresentationPropertyChangedEventArgs<IViewMatrix> e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void ViewPort_MaximumZoomChanged(object sender, MapPresentationPropertyChangedEventArgs<double> e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void ViewPort_MinimumZoomChanged(object sender, MapPresentationPropertyChangedEventArgs<double> e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void ViewPort_PixelAspectRatioChanged(object sender, MapPresentationPropertyChangedEventArgs<double> e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void ViewPort_SizeChanged(object sender, MapPresentationPropertyChangedEventArgs<ViewSize2D> e)
        {
            OnMapViewSizeChanged();
        }

        void ViewPort_StyleRenderingModeChanged(object sender, MapPresentationPropertyChangedEventArgs<StyleRenderingMode> e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void ViewPort_ViewRectangleChanged(object sender, MapPresentationPropertyChangedEventArgs<ViewRectangle2D, BoundingBox> e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void ViewPort_ZoomChanged(object sender, MapZoomChangedEventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region Private helper methods
        private void modifySelection(double xDelta, double yDelta)
        {
            Selection.Expand(new ViewSize2D(xDelta, yDelta));
            OnSelectionModified();
        }
        #endregion

        //#region IViewTransformer Members

        //IViewVector IViewTransformer.TransformToView(Point point)
        //{
        //    return TransformToView(point);
        //}

        //IEnumerable<IViewVector> IViewTransformer.TransformToView(IEnumerable<Point> points)
        //{
        //    foreach (GeoPoint geoPoint in points)
        //    {
        //        yield return TransformToView(geoPoint);
        //    }
        //}

        //public Point ViewToWorld(IViewVector viewPoint)
        //{
        //    if (!(viewPoint is ViewPoint2D))
        //    {
        //        throw new ArgumentException("Parameter must be an instance of ViewPoint2D", "viewPoint");
        //    }

        //    return ViewToWorld((ViewPoint2D)viewPoint);
        //}

        //public BoundingBox ViewToWorld(IViewMatrix viewRectangle)
        //{
        //    if (!(viewRectangle is ViewRectangle2D))
        //    {
        //        throw new ArgumentException("Parameter must be an instance of ViewRectangle2D", "viewRectangle");
        //    }

        //    return ViewToWorld((ViewRectangle2D)viewRectangle);
        //}

        //IViewVector IViewTransformer.WorldToView(Point geoPoint)
        //{
        //    return WorldToView(geoPoint);
        //}

        //IViewMatrix IViewTransformer.WorldToView(BoundingBox bounds)
        //{
        //    return WorldToView(bounds);
        //}

        //#endregion
    }
}
