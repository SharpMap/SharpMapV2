using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Data;
using SharpMap.Geometries;

namespace SharpMap.Layers
{
    public interface IFeatureLayer : ILayer
    {
        IEnumerable<FeatureDataRow> GetFeatures(BoundingBox region);
    }
}
