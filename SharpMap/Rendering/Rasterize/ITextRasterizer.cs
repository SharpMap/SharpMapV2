using GeoAPI.Geometries;
using SharpMap.Styles;

namespace SharpMap.Rendering.Rasterize
{
    public interface ITextRasterizer : IRasterizer
    {
        void Rasterize(IGeometry geometry, string text, IStyle style);
    }

    public interface ITextRasterizer<TSurface, TContext>
        : IRasterizer<TSurface, TContext>, ITextRasterizer
    {
    }
}