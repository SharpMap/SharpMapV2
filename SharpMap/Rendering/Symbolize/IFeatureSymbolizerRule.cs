using SharpMap.Data;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Symbolize
{
    public interface IFeatureSymbolizerRule
        : ISymbolizerRule
    {
        IFeatureSymbolizer Symbolizer { get; }
        void Symbolize(IFeatureDataRecord obj, IFeatureRasterizer rasterizer);
    }
}