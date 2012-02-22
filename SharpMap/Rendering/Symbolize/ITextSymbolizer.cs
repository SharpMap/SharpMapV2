using System.Collections.Generic;
using SharpMap.Data;
using SharpMap.Presentation;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Rendering.Symbolize
{
    public interface ITextSymbolizer : ISymbolizer
    {
        ICollection<ITextSymbolizerRule> Rules { get; }

        void Symbolize(IEnumerable<IFeatureDataRecord> records, RenderPhase renderPhase, Matrix2D transform, ITextRasterizer rasterizer, TextSymbolizingDelegate textSymbolizingDelegate);

        TextSymbolizingDelegate LabelTextFunction { get; }
    }
}