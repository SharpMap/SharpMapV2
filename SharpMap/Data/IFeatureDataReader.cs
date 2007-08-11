using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using SharpMap.Geometries;

namespace SharpMap.Data
{
    /// <summary>
    /// Provides a fast-forward, read-only data stream to geometry data.
    /// </summary>
    public interface IFeatureDataReader : IDataReader
    {
        Geometry GetGeometry();
    }
}
