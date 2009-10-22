using System;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using CairoPath = Cairo.Path;

namespace SharpMap.Rendering.Cairo
{
    internal static class ViewConverter
    {
        public static Path2D Convert(Path2D path)
        {
            return path;
        }

        public static StyleBrush Convert(StyleBrush brush)
        {
            return brush;
        }

        public static StylePen Convert(StylePen outline)
        {
            return outline;
        }
    }
}