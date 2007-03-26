using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.Geometries;

namespace SharpMap.Data.Providers
{
    public interface IUpdateableProvider : IProvider
    {
        void Save(FeatureDataTable features);
        void Save(FeatureDataRow feature);
        void Delete(FeatureDataRow feature);
    }
}
