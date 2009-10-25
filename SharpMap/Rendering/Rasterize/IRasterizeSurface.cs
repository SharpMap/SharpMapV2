using System;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Rendering.Rasterize
{
    public interface IRasterizeSurface
    {
        IMapView2D MapView { get; }
        event EventHandler RenderComplete;
        void RenderCompleted();
        object BackSurface { get; }
        object FrontSurface { get; }
        IRasterizers RetrieveSurface();
        IRasterizers CreateSurface();
    }


    public interface IRasterizeSurface<TSurface, TContext> : IRasterizeSurface
    {
        TSurface BackSurface { get; }
        TSurface FrontSurface { get; }
        IRasterizers<TSurface, TContext> RetrieveSurface();
        IRasterizers<TSurface, TContext> CreateSurface();
    }


    public interface IRasterizers
    {
        IGeometryRasterizer GeometryRasterizer { get; }
        ITextRasterizer TextRasterizer { get; }
        IRasterRasterizer RasterRasterizer { get; }
    }

    public interface IRasterizers<TSurface, TContext> : IRasterizers
    {
        IGeometryRasterizer<TSurface, TContext> GeometryRasterizer { get; }
        ITextRasterizer<TSurface, TContext> TextRasterizer { get; }
        IRasterRasterizer<TSurface, TContext> RasterRasterizer { get; }

    }
}