using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Rasterize
{
    ///<summary>
    ///</summary>
    public interface IGeometryRasterizer : IRasterizer
    {
        ///<summary>
        ///</summary>
        ///<param name="record"></param>
        ///<param name="style"></param>
        ///<param name="transform"></param>
        void Rasterize(IFeatureDataRecord record, GeometryStyle style, Matrix2D transform);
    }

    ///<summary>
    ///</summary>
    ///<typeparam name="TSurface"></typeparam>
    ///<typeparam name="TContext"></typeparam>
    public interface IGeometryRasterizer<TSurface, TContext>
        : IRasterizer<TSurface, TContext>, IGeometryRasterizer
    {
    }
}