using System;
using System.Threading;
using SharpMap.Styles;
using SlimDX;
using SlimDX.Direct2D;

namespace SharpMap.Rendering.Direct2D
{
    internal class ViewConverter
    {
        public static Color4 Convert(StyleColor color)
        {
            return new Color4(
                (float)255 / color.A,
                (float)255 / color.R,
                (float)255 / color.G,
                (float)255 / color.B);
        }

        public static Brush Convert(RenderTarget trgt, StyleBrush brush)
        {
            //todo: more complex brushes
            return new SolidColorBrush(trgt, Convert(brush.Color));
        }

        public static StrokeStyle Convert(RenderTarget surface, StylePen pen)
        {

            return new StrokeStyle(new Factory(), new StrokeStyleProperties()
                                                     {
                                                         EndCap = Convert(pen.EndCap),
                                                         StartCap = Convert(pen.StartCap),
                                                         DashCap = Convert(pen.DashCap),
                                                         DashOffset = pen.DashOffset,
                                                         DashStyle = Convert(pen.DashStyle),
                                                         LineJoin = Convert(pen.LineJoin),
                                                         MiterLimit = pen.MiterLimit
                                                     });
        }

        private static LineJoin Convert(StyleLineJoin lineJoin)
        {
            switch(lineJoin)
            {
                case StyleLineJoin.Bevel:
                    return LineJoin.Bevel;
                case StyleLineJoin.Miter:
                    return LineJoin.Miter;
                case StyleLineJoin.MiterClipped:
                    return LineJoin.MiterOrBevel;
                case StyleLineJoin.Round:
                    return LineJoin.Round;
            }
            throw new NotImplementedException();
        }

        private static DashStyle Convert(LineDashStyle dashStyle)
        {
            switch (dashStyle)
            {
                case LineDashStyle.Dash:
                    return DashStyle.Dash;
                case LineDashStyle.DashDot:
                    return DashStyle.DashDot;
                case LineDashStyle.DashDotDot:
                    return DashStyle.DashDotDot;
                case LineDashStyle.Dot:
                    return DashStyle.Dot;
                case LineDashStyle.Solid:
                    return DashStyle.Solid;
                case LineDashStyle.Custom:
                    return DashStyle.Custom;
            }
            throw new NotImplementedException();
        }

        private static CapStyle Convert(LineDashCap dashCap)
        {
            switch (dashCap)
            {
                case LineDashCap.Flat:
                    return CapStyle.Flat;
                case LineDashCap.Round:
                    return CapStyle.Round;
                case LineDashCap.Triangle:
                    return CapStyle.Triangle;
            }
            throw new NotImplementedException();
        }


        private static CapStyle Convert(StyleLineCap cap)
        {
            switch (cap)
            {
                case StyleLineCap.Triangle:
                    return CapStyle.Triangle;
                case StyleLineCap.Flat:
                    return CapStyle.Flat;
                case StyleLineCap.Round:
                    return CapStyle.Round;
                case StyleLineCap.Square:
                    return CapStyle.Square;
                default:
                    return CapStyle.Flat;
            }
        }
    }
}