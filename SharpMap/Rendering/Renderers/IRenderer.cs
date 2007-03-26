using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

namespace SharpMap.Rendering
{
    public interface IRenderer<TViewPoint, TViewSize, TViewRectangle, TRenderObject> : IDisposable
        where TViewPoint : IViewVector
        where TViewSize : IViewVector
        where TViewRectangle : IViewMatrix
    {
        IStyle Style { get; set; }
        ITheme Theme { get; set; }
        IViewTransformer<ViewPoint2D, ViewRectangle2D> ViewTransformer { get; set; }
        StyleRenderingMode StyleRenderingMode { get; set; }

        //StyleSmoothingMode SmoothingMode { get; set; }

        //void BeginObject(IRenderContext renderContext);
        //void EndObject(IRenderContext renderContext);
        //IList<TRenderObject> RenderedObjects { get; }


        ///// <summary>
        ///// Renders the layer to the <paramref name="mapView">presenter</paramref>.
        ///// </summary>
        ///// <param name="mapView">The <see cref="IMapView"/> used to render the map.</param>
        //void Render(IMapView mapView);

        ///// <summary>
        ///// Renders the layer to the <paramref name="mapView">presenter</paramref>.
        ///// </summary>
        ///// <param name="mapView">The <see cref="IMapView"/> used to render the map.</param>
        ///// <param name="region">Region to restrict rendering to.</param>
        //void Render(IMapView mapView, BoundingBox region);

        ///// <summary>
        ///// Minimum visible zoom level
        ///// </summary>
        //double MinVisible { get; set; }

        ///// <summary>
        ///// Minimum visible zoom level
        ///// </summary>
        //double MaxVisible { get; set; }
    }
}
