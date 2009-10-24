using SharpMap.Data;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Symbolize
{
    public interface ITextSymbolizerRule : ISymbolizerRule
    {
        ITextSymbolizer Symbolizer { get; }
        void Symbolize(IFeatureDataRecord obj, string text, ITextRasterizer rasterizer);
    }
}