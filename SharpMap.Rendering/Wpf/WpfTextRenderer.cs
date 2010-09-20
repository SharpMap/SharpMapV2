using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

using WpfFont = System.Windows.Media.Typeface;
using WpfPoint = System.Windows.Point;

namespace SharpMap.Rendering.Wpf
{
    public class WpfTextRenderer : TextRenderer2D<WpfRenderObject>
    {
        public override IEnumerable<WpfRenderObject> RenderText(string text, StyleFont font,
                                                                 Rectangle2D layoutRectangle, Path2D flowPath,
                                                                 StyleBrush fontBrush, Matrix2D transform)
        {
            if (!layoutRectangle.IsEmpty)
            {
                FormattedText ft = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, ViewConverter.Convert(font), font.Size.Height, ViewConverter.Convert(fontBrush));
                WpfTextRenderObject<WpfPoint> renderedText =
                    new WpfTextRenderObject<WpfPoint>(RenderState.Normal, ft, new Point(layoutRectangle.UpperLeft.X, layoutRectangle.UpperLeft.Y));
                yield return renderedText;
            }
            else if (flowPath != null)
            {
                yield return null;
            }
            else
                yield return null;
        }

        public override Size2D MeasureString(string text, StyleFont font)
        {
            WpfFont wpfFont = ViewConverter.Convert(font);
            FormattedText ft = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, wpfFont,
                                                 font.Size.Height, Brushes.Black);
            return new Size2D(Math.Ceiling(ft.Width), Math.Ceiling(ft.Height));
        }
    }
}
