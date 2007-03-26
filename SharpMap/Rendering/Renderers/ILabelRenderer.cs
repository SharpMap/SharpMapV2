using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Styles;

namespace SharpMap.Rendering
{
    public interface ILabelRenderer 
    {
        StyleTextRenderingHint TextRenderingHint { get; set; }
        ViewSize2D MeasureString(string text, StyleFont font);
        void DrawLabel(Label label);
        void DrawLabel(string text, ViewPoint2D location, StyleFont font, StyleColor foreColor);
        void DrawLabel(string text, ViewPoint2D location, ViewPoint2D offset, StyleFont font, StyleColor foreColor, StyleBrush backColor, StylePen halo, float rotation);
    }
}
