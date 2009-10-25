using SharpMap.Data;
using SharpMap.Presentation;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Symbolize
{
    public interface ITextSymbolizerRule : ISymbolizerRule
    {
        void Symbolize(IFeatureDataRecord obj, string text, RenderPhase renderPhase, Matrix2D transform, ITextRasterizer rasterizer);
        bool EvaluateStyle(IFeatureDataRecord record, RenderPhase phase, out LabelStyle style);
    }
}