using System;
using System.IO;
using GeoAPI.Geometries;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Data
{
    ///<summary>
    /// Interface for returning necessary information to render
    /// Raster data
    ///</summary>
    public interface IRasterRecord
    {
        Stream GetImage(IExtents viewPort, Matrix2D toViewTransform);
        Rectangle2D ViewBounds { get; }
        Rectangle2D RasterBounds { get; }
    }

    //public interface IRasterRecord
    //{
    //    Int32 DataType { get; }
    //    IntPtr DataStore { get;}
    //}
}