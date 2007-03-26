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

namespace SharpMap.Presentation
{
    public interface IMapPresenter<TViewPoint, TViewSize, TViewRectangle> : IViewTransformer<TViewPoint, TViewRectangle>, IDisposable
        where TViewPoint : IViewVector
        where TViewSize : IViewVector
        where TViewRectangle : IViewMatrix
    {
        Map Map { get; }
        IMapView2D MapView { get; set; }
        //ToolSet ActiveTool { get; }
        IList<IToolsView> ToolsViews { get; set; }
        IViewSelection<TViewPoint, TViewSize, TViewRectangle> Selection { get; }
        double ViewHeight { get; }
        TViewSize ViewSize { get; set; }
        TViewRectangle ViewRectangle { get; }
        BoundingBox ViewEnvelope { get; set; }
        GeoPoint ViewCenter { get; set; }
        double Zoom { get; set; }
        double MinimumZoom { get; set; }
        double MaximumZoom { get; set; }
        double PixelSize { get; }
        double PixelWidth { get; }
        double PixelHeight { get; }
        double PixelAspectRatio { get; set; }
        StyleColor BackColor { get; set; }
        IViewMatrix MapTransform { get; set; }
        IViewMatrix MapTransformInverted { get; }
        //event EventHandler SizeChanged;
        //event EventHandler<MapZoomChangedEventArgs> ZoomChanged;
        //event EventHandler<MapCenterChangedEventArgs> CenterChanged;

        //IEnumerable<FeatureDataRow> Query(BoundingBox region);
        //IEnumerable<FeatureDataRow> Query(GeoPoint location);
        IRenderer<TViewPoint, TViewSize, TViewRectangle, TRenderObject> GetRendererForLayer<TRenderObject>(ILayer layer);
        void ZoomToBox(BoundingBox zoomBox);
        void ZoomToExtents();
    }
}
