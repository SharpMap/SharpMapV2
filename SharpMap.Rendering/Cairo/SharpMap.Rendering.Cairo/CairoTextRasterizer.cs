using System;
using Cairo;
using SharpMap.Data;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Cairo
{
    public class CairoTextRasterizer : CairoRasterizer, ITextRasterizer<Surface, Context>
    {
        public CairoTextRasterizer(Surface surface, Context context) : base(surface, context)
        {
        }

        #region ITextRasterizer<Surface,Context> Members

        public void Rasterize(IFeatureDataRecord record, string text, LabelStyle style, Matrix2D transform)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}