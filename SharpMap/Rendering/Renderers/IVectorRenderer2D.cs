using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Styles;

namespace SharpMap.Rendering
{
    public interface IVectorRenderer2D<TRenderObject>
    {
        TRenderObject DrawPath(GraphicsPath2D path, StylePen outline, StylePen highlightOutline, StylePen selectOutline);
        TRenderObject DrawPath(GraphicsPath2D path, StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill, StylePen outline, StylePen highlightOutline, StylePen selectOutline);
        TRenderObject DrawSymbol(ViewPoint2D location, Symbol2D symbolData);
        TRenderObject DrawSymbol(ViewPoint2D location, Symbol2D symbolData, ColorMatrix highlight, ColorMatrix select);
        TRenderObject DrawSymbol(ViewPoint2D location, Symbol2D symbolData, Symbol2D highlightSymbolData, Symbol2D selectSymbolData);
    }
}
