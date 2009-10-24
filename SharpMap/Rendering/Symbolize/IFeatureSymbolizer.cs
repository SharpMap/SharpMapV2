using System.Collections.Generic;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Symbolize
{
    public interface IFeatureSymbolizer : ISymbolizer
    {
        IFeatureRasterizer Rasterizer { get; }
        ICollection<IFeatureSymbolizerRule> Rules { get; }
    }
}