using System.Collections.Generic;
using SharpMap.Data;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Symbolize
{
    public interface ISymbolizer
    {
        IRasterizer Rasterizer { get; }
        ICollection<ISymbolizerRule> Rules { get; }
        void Symbolize(IEnumerable<IFeatureDataRecord> features);
    }
}