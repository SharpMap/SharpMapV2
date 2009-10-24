using GeoAPI.Geometries;
using SharpMap.Styles;

namespace SharpMap.Rendering.Rasterize
{
    public interface IFeatureRasterizer : IRasterizer
    {
        void Rasterize(IGeometry geometry, IStyle style);
    }

    public interface IFeatureRasterizer<TSurface, TContext>
        : IRasterizer<TSurface, TContext>, IFeatureRasterizer
    {
    }
}