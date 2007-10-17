using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Wpf
{
    public class WpfTextRenderer : TextRenderer2D<DependencyObject>
    {
        public override IEnumerable<DependencyObject> RenderText(string text, StyleFont font,
                                                                 Rectangle2D layoutRectangle, Path2D flowPath,
                                                                 StyleBrush fontBrush, Matrix2D transform)
        {
            throw new NotImplementedException();
        }

        public override Size2D MeasureString(string text, StyleFont font)
        {
            throw new NotImplementedException();
        }
    }
}
