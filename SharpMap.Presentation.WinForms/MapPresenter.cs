using System;
using System.Collections.Generic;
using System.Drawing;
using GdiMatrix = System.Drawing.Drawing2D.Matrix;
using System.Text;

using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Rendering;
using SharpMap.Rendering.Gdi;
using SharpMap.Presentation;

using IFeatureLayerRenderer = SharpMap.Rendering.IFeatureRenderer<SharpMap.Rendering.ViewPoint2D, SharpMap.Rendering.ViewSize2D, SharpMap.Rendering.ViewRectangle2D, SharpMap.Rendering.Gdi.GdiRenderObject>;

namespace SharpMap.Presentation.WinForms
{
    public class MapPresenter : BaseMapPresenter2D
    {
        private ImageTileCache _imageTileCache;
        private GdiRenderObject[] _renderObjects;
        private GdiMatrix _viewMatrix = new GdiMatrix();

        public MapPresenter(Map map, MapViewControl mapView, IEnumerable<IToolsView> toolsViews)
            : base(map, mapView, toolsViews)
        {
            _imageTileCache = new ImageTileCache(this);
            RegisterRenderer<VectorLayer, PositionedRenderObject2D<GdiRenderObject>>(new GdiVectorRenderer(this));
            RegisterRenderer<LabelLayer, PositionedRenderObject2D<GdiRenderObject>>(new GdiLabelRenderer(this));
        }

        public Image GetImage(RectangleF region)
        {
            BoundingBox worldRegion = ViewToWorld(ViewConverter.GdiToView(region));
            IEnumerable<BoundingBox> missingRegions = _imageTileCache.ComputeMissingRegions(worldRegion);

            foreach (BoundingBox missingRegion in missingRegions)
            {
                IRenderContext renderContext = new RenderContext();
                foreach (ILayer layer in Map.Layers)
	            {
                    if (!(layer is IFeatureLayer))
                    {
                        throw new NotImplementedException();
                    }

                    IEnumerable<FeatureDataRow> features = (layer as IFeatureLayer).GetFeatures(missingRegion);
                    IFeatureLayerRenderer renderer = GetRenderer<IFeatureLayerRenderer>(MapSharedState.ActiveLayer);
                    foreach (FeatureDataRow feature in features)
	                {
                        renderer.RenderFeature(feature);
	                }
                }
            }

            IEnumerable<KeyValuePair<BoundingBox, Image>> images = _imageTileCache.GetCachedImagesForRegion(worldRegion);
            throw new NotImplementedException();
        }

        protected override void OnMapViewHover(ViewPoint2D viewPoint2D)
        {
            base.OnMapViewHover(viewPoint2D);

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

        protected override void OnMapViewBeginAction(ViewPoint2D viewPoint2D)
        {
            base.OnMapViewBeginAction(viewPoint2D);

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

        protected override void OnMapViewMoveTo(ViewPoint2D viewPoint2D)
        {
            base.OnMapViewMoveTo(viewPoint2D);

            switch (MapSharedState.SelectedTool)
            {
                case ToolSet.Pan:
                    break;
                case ToolSet.ZoomIn:
                    MapView.Invalidate(ViewConverter.ViewToGdi(Selection.BoundingRegion));
                    break;
                case ToolSet.ZoomOut:
                    MapView.Invalidate(ViewConverter.ViewToGdi(Selection.BoundingRegion));
                    break;
                case ToolSet.Query:
                    MapView.Invalidate(ViewConverter.ViewToGdi(Selection.BoundingRegion));
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

        protected override void OnMapViewEndAction(ViewPoint2D viewPoint2D)
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

            // Do action here...
            base.OnMapViewEndAction(viewPoint2D);
        }

        protected override void OnMapViewSizeChanged()
        {
            base.OnMapViewSizeChanged();
        }

        public new MapViewControl MapView
        {
            get { return MapView as MapViewControl; }
        }
    }
}
