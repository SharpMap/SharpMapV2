using System;
using SharpMap.Data;
using SharpMap.Presentation;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Symbolize
{
    public abstract class TextSymbolizerRule : SymbolizerRule, ITextSymbolizerRule
    {
        #region ITextSymbolizerRule Members

        public void Symbolize(IFeatureDataRecord obj, string text, RenderPhase renderPhase, Matrix2D transform,
                              ITextRasterizer rasterizer)
        {
            LabelStyle style;
            if (EvaluateStyle(obj, renderPhase, out style))
                rasterizer.Rasterize(obj, text, style, transform);
        }

        public abstract bool EvaluateStyle(IFeatureDataRecord record, RenderPhase phase, out LabelStyle style);

        #endregion
    }
}