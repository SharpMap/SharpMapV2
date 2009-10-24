using System.Collections.Generic;
using SharpMap.Data;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Symbolize
{
    public interface ITextSymbolizer : ISymbolizer
    {
        ITextRasterizer Rasterizer { get; }
        ICollection<ITextSymbolizerRule> Rules { get; }
        void Symbolize(IEnumerable<IFeatureDataRecord> records, TextSymbolizingDelegate textSymbolizingDelegate);
    }
}