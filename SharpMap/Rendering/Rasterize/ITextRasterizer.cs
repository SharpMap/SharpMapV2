using GeoAPI.Geometries;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Rasterize
{
    public interface ITextRasterizer : IRasterizer
    {
        void Rasterize(IGeometry geometry, string text, LabelStyle style, Matrix2D transform);
    }

    public interface ITextRasterizer<TSurface, TContext>
        : IRasterizer<TSurface, TContext>, ITextRasterizer
    {
    }
}