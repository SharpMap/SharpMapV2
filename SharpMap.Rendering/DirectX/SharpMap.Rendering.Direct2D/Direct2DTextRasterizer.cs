using System;
using SharpMap.Data;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SlimDX.Direct2D;

namespace SharpMap.Rendering.Direct2D
{
    public class Direct2DTextRasterizer : Direct2DRasterizer, ITextRasterizer<RenderTarget, RenderTarget>
    {
        public Direct2DTextRasterizer(RenderTarget surface, RenderTarget context) : base(surface, context)
        {
        }

        #region ITextRasterizer<RenderTarget,RenderTarget> Members

        public void Rasterize(IFeatureDataRecord record, string text, LabelStyle style, Matrix2D transform)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}