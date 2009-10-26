using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Rasterize
{
    public interface IGeometryRasterizer : IRasterizer
    {
        void Rasterize(IFeatureDataRecord record, GeometryStyle style, Matrix2D transform);
    }

    public interface IGeometryRasterizer<TSurface, TContext>
        : IRasterizer<TSurface, TContext>, IGeometryRasterizer
    {
    }
}