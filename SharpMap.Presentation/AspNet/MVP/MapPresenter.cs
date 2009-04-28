/*
 *  The attached / following is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Layers;
using SharpMap.Presentation.Presenters;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Presentation.AspNet.MVP
{
    public class MapPresenter
        : MapPresenter2D
    {
        private Boolean _isRenderingAll;
        private Boolean _isRenderingSelection;
        private IRasterRenderer2D _rasterRenderer;
        private ITextRenderer2D _textRenderer;

        private IVectorRenderer2D _vectorRenderer;

        public MapPresenter(Map map, WebMapView view)
            : base(map, view)
        {
        }

        protected new IVectorRenderer2D VectorRenderer
        {
            get
            {
                _vectorRenderer = _vectorRenderer ?? CreateVectorRenderer();
                return _vectorRenderer;
            }
        }

        protected new ITextRenderer2D TextRenderer
        {
            get
            {
                _textRenderer = _textRenderer ?? CreateTextRenderer();
                return _textRenderer;
            }
        }

        protected new IRasterRenderer2D RasterRenderer
        {
            get
            {
                _rasterRenderer = _rasterRenderer ?? CreateRasterRenderer();
                return _rasterRenderer;
            }
        }

        private WebMapView WebMapView
        {
            get { return View as WebMapView; }
        }

        private IWebMapRenderer WebMapRenderer
        {
            get { return WebMapView.WebMapRenderer; }
        }

        protected override Type GeometryRendererType
        {
            get { return WebMapRenderer.GeometryRendererType; }
        }

        protected override Type LabelRendererType
        {
            get { return WebMapRenderer.LabelRendererType; }
        }

        protected override void OnRenderedLayerPhase(ILayer layer, RenderPhase phase)
        {
            if (WebMapView.ClientDisconnected)
                throw new ClientDisconnectedException();
            base.OnRenderedLayerPhase(layer, phase);
        }

        //protected override void CreateGeometryRenderer(Type renderObjectType)
        //{
        //    if (renderObjectType == typeof(GdiRenderObject))
        //        base.CreateGeometryRenderer(renderObjectType);
        //    else if (renderObjectType == typeof(GeoJsonRenderObject))
        //    {
        //        Type basicGeometryRendererType = typeof(GeoJsonGeometryRenderer<>);
        //        Type layerType = typeof(GeometryLayer);

        //        CreateRendererForLayerType(basicGeometryRendererType,
        //                                   renderObjectType,
        //                                   layerType,
        //                                   VectorRenderer);
        //    }
        //}

        protected override IRasterRenderer2D CreateRasterRenderer()
        {
            return WebMapRenderer.CreateRasterRenderer();
        }

        protected override ITextRenderer2D CreateTextRenderer()
        {
            return WebMapRenderer.CreateTextRenderer();
        }

        protected override IVectorRenderer2D CreateVectorRenderer()
        {
            return WebMapRenderer.CreateVectorRenderer();
        }

        protected override Type GetRenderObjectType()
        {
            return WebMapRenderer.GetRenderObjectType();
        }

        protected override void OnRenderingAllLayers()
        {
            _isRenderingAll = true;
            base.OnRenderingAllLayers();
        }

        protected override void OnRenderedAllLayers()
        {
            base.OnRenderedAllLayers();
            _isRenderingAll = false;
        }

        protected override void OnRenderingSelection()
        {
            _isRenderingSelection = true;
        }

        protected override void OnRenderedSelection()
        {
            _isRenderingSelection = false;
        }


        protected override void SetViewLocationInformation(String text)
        {
            //ViewControl.Information = text;
        }


        protected override void SetViewBackgroundColor(StyleColor fromColor, StyleColor toColor)
        {
            BackgroundColorInternal = toColor;
        }

        private IEnumerable<T> EnumerateWhileMonitoringClientConnection<T>(IEnumerable<T> enumerable)
        {
            foreach (T t in enumerable)
            {
                if (WebMapView.ClientDisconnected)
                    throw new ClientDisconnectedException();
                yield return t;
            }
        }

        protected override void RenderFeatureLayer(IFeatureLayer layer, RenderPhase phase)
        {
            IFeatureRenderer renderer = GetRenderer<IFeatureRenderer>(layer);
            renderer.RenderTransform = ToViewTransform;

            Debug.Assert(renderer != null);

            Debug.Assert(layer.Style is FeatureStyle);
            FeatureStyle layerStyle = layer.Style as FeatureStyle;

            switch (phase)
            {
                case RenderPhase.Normal:
                    IEnumerable<FeatureDataRow> features
                        =
                        EnumerateWhileMonitoringClientConnection(layer.Features.Select(ViewEnvelopeInternal.ToGeometry()));

                    foreach (FeatureDataRow feature in features)
                    {
                        FeatureStyle style = getStyleForFeature(layer, feature, layerStyle);

                        IEnumerable renderedFeature = renderer.RenderFeature(feature,
                                                                             style,
                                                                             RenderState.Normal,
                                                                             layer);
                        View.ShowRenderedObjects(renderedFeature);
                    }
                    break;
                case RenderPhase.Selected:
                    IEnumerable<FeatureDataRow> selectedRows =
                        EnumerateWhileMonitoringClientConnection(layer.SelectedFeatures);

                    foreach (FeatureDataRow selectedFeature in selectedRows)
                    {
                        FeatureStyle style = getStyleForFeature(layer, selectedFeature, layerStyle);

                        IEnumerable renderedFeature = renderer.RenderFeature(selectedFeature,
                                                                             style,
                                                                             RenderState.Selected,
                                                                             layer);
                        View.ShowRenderedObjects(renderedFeature);
                    }
                    break;
                case RenderPhase.Highlighted:
                    IEnumerable<FeatureDataRow> highlightedRows =
                        EnumerateWhileMonitoringClientConnection(layer.HighlightedFeatures);

                    foreach (FeatureDataRow highlightedFeature in highlightedRows)
                    {
                        FeatureStyle style = getStyleForFeature(layer, highlightedFeature, layerStyle);

                        IEnumerable renderedFeature = renderer.RenderFeature(highlightedFeature,
                                                                             style,
                                                                             RenderState.Highlighted,
                                                                             layer);
                        View.ShowRenderedObjects(renderedFeature);
                    }
                    break;
                default:
                    break;
            }

            renderer.CleanUp();
        }

        #region MapViewControl accessible members

        internal StyleColor BackgroundColor
        {
            get { return BackgroundColorInternal; }
        }

        internal ICoordinate GeoCenter
        {
            get { return GeoCenterInternal; }
        }

        internal Double MaximumWorldWidth
        {
            get { return MaximumWorldWidthInternal; }
        }

        internal Double MinimumWorldWidth
        {
            get { return MinimumWorldWidthInternal; }
        }

        internal Double PixelWorldWidth
        {
            get { return PixelWorldWidthInternal; }
        }

        internal Double PixelWorldHeight
        {
            get { return PixelWorldHeightInternal; }
        }

        internal ViewSelection2D Selection
        {
            get { return SelectionInternal; }
        }

        internal Matrix2D ToViewTransform
        {
            get { return ToViewTransformInternal; }
        }

        internal Matrix2D ToWorldTransform
        {
            get { return ToWorldTransformInternal; }
        }

        internal IExtents2D ViewEnvelope
        {
            get { return ViewEnvelopeInternal; }
        }

        internal Double WorldAspectRatio
        {
            get { return WorldAspectRatioInternal; }
        }

        internal Double WorldHeight
        {
            get { return WorldHeightInternal; }
        }

        internal Double WorldWidth
        {
            get { return WorldWidthInternal; }
        }

        internal Double WorldUnitsPerPixel
        {
            get { return WorldUnitsPerPixelInternal; }
        }

        public Boolean IsRenderingSelection
        {
            get { return _isRenderingSelection; }
        }

        internal Point2D ToView(ICoordinate point)
        {
            return ToViewInternal(point);
        }

        internal Point2D ToView(Double x, Double y)
        {
            return ToViewInternal(x, y);
        }

        internal ICoordinate ToWorld(Point2D point)
        {
            return ToWorldInternal(point);
        }

        internal ICoordinate ToWorld(Double x, Double y)
        {
            return ToWorldInternal(x, y);
        }

        internal IExtents2D ToWorld(Rectangle2D bounds)
        {
            return ToWorldInternal(bounds);
        }

        internal void ZoomToExtents()
        {
            ZoomToExtentsInternal();
        }

        internal void ZoomToViewBounds(Rectangle2D viewBounds)
        {
            ZoomToViewBoundsInternal(viewBounds);
        }

        internal void ZoomToWorldBounds(IExtents2D zoomBox)
        {
            ZoomToWorldBoundsInternal(zoomBox);
        }

        internal void ZoomToWorldWidth(Double newWorldWidth)
        {
            ZoomToWorldWidthInternal(newWorldWidth);
        }

        #endregion
    }

    public class ClientDisconnectedException : Exception
    {
    }
}