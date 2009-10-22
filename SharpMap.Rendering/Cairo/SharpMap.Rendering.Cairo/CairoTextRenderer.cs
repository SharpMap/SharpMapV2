using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Rendering.Cairo
{
    public class CairoTextRenderer
    : TextRenderer2D<CairoRenderObject>
    {
        public override IEnumerable<CairoRenderObject> RenderText(string text, SharpMap.Styles.StyleFont font, Rectangle2D layoutRectangle, Path2D flowPath, SharpMap.Styles.StyleBrush fontBrush, Matrix2D transform)
        {
            throw new NotImplementedException();
        }

        public override Size2D MeasureString(string text, SharpMap.Styles.StyleFont font)
        {
            throw new NotImplementedException();
        }
    }
}
