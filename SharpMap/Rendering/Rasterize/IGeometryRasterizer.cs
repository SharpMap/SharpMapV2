using GeoAPI.Geometries;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Rasterize
{
    public interface IGeometryRasterizer : IRasterizer
    {
        void Rasterize(IGeometry geometry, GeometryStyle style, Matrix2D transform);
    }

    public interface IGeometryRasterizer<TSurface, TContext>
        : IRasterizer<TSurface, TContext>, IGeometryRasterizer
    {
    }
}