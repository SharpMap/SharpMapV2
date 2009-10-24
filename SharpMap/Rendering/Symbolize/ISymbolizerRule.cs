using SharpMap.Data;
using SharpMap.Styles;

namespace SharpMap.Rendering.Symbolize
{
    public interface ISymbolizerRule
    {
        ISymbolizer Symbolizer { get; }
        bool Enabled { get; }
        bool EvaluateStyle(IFeatureDataRecord record, out IStyle style);
    }
}