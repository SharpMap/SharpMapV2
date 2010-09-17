// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Windows.Media;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using WpfPen = System.Windows.Media.Pen;
using WpfBrush = System.Windows.Media.Brush;
using WpfPoint = System.Windows.Point;
using WpfSize = System.Windows.Size;
using WpfRectangle = System.Windows.Rect;
using WpfBrushes = System.Windows.Media.Brushes;
using WpfFont = System.Windows.Media.Typeface;
using WpfFontFamily = System.Windows.Media.FontFamily;
using WpfFontStyle = System.Windows.FontStyle;
using WpfColor = System.Windows.Media.Color;
using WpfPath = System.Windows.Media.StreamGeometry;
using WpfMatrix = System.Windows.Media.Matrix;
//using WpfColorMatrix = System.Windows.Media.Imaging.ColorMatrix;
//using WpfSmoothingMode = System.Drawing SmoothingMode;
//using WpfTextRenderingHint = System.Drawing.Text.TextRenderingHint;

namespace SharpMap.Rendering.Wpf
{
    internal static class ViewConverter
    {

        private static readonly Dictionary<FontLookupKey, WpfFont> FontCache = new Dictionary<FontLookupKey, WpfFont>();
        private static readonly Dictionary<BrushLookupKey, WpfBrush> BrushCache = new Dictionary<BrushLookupKey, WpfBrush>();
        private static readonly Dictionary<PenLookupKey, WpfPen> PenCache = new Dictionary<PenLookupKey, WpfPen>();

        private static WpfFont GetFont(StyleFont styleFont)
        {
            if (styleFont == null)
            {
                return null;
            }

            FontLookupKey key = new FontLookupKey(styleFont.GetHashCode());

            WpfFont font;
            FontCache.TryGetValue(key, out font);
            return font;
        }

        private static void SaveFont(StyleFont styleFont,WpfFont font)
        {
            FontCache[new FontLookupKey(styleFont.GetHashCode())] = font;
        }

        private static WpfBrush GetBrush(StyleBrush styleBrush)
        {
            if (styleBrush == null)
            {
                return null;
            }

            BrushLookupKey key = new BrushLookupKey(styleBrush.GetType().TypeHandle, styleBrush.GetHashCode());
            WpfBrush brush;
            BrushCache.TryGetValue(key, out brush);
            return brush;
        }

        private static void SaveBrush(StyleBrush styleBrush, WpfBrush brush)
        {
            BrushCache[new BrushLookupKey(styleBrush.GetType().TypeHandle, styleBrush.GetHashCode())] = brush;
        }

        private static WpfPen GetPen(StylePen stylePen)
        {
            if (stylePen == null)
            {
                return null;
            }

            PenLookupKey key = new PenLookupKey(stylePen.GetType().TypeHandle, stylePen.GetHashCode());
            WpfPen pen;
            PenCache.TryGetValue(key, out pen);
            return pen;
        }

        private static void SavePen(StylePen stylePen, WpfPen pen)
        {
            PenCache[new PenLookupKey(stylePen.GetType().TypeHandle, stylePen.GetHashCode())] = pen;
        }

        #region Nested types
        private struct BrushLookupKey : IEquatable<BrushLookupKey>
        {
            private readonly RuntimeTypeHandle _styleBrushType;
            private readonly Int32 _styleBrushId;

            public BrushLookupKey(RuntimeTypeHandle type, Int32 styleBrushId)
            {
                _styleBrushType = type;
                _styleBrushId = styleBrushId;
            }

            #region IEquatable<BrushLookupKey> Members

            public Boolean Equals(BrushLookupKey other)
            {
                return other._styleBrushType.Equals(_styleBrushType)
                       && other._styleBrushId == _styleBrushId;
            }

            #endregion
        }

        private struct PenLookupKey : IEquatable<PenLookupKey>
        {
            private readonly RuntimeTypeHandle _stylePenType;
            private readonly Int32 _stylePenId;

            public PenLookupKey(RuntimeTypeHandle type, Int32 stylePenValue)
            {
                _stylePenType = type;
                _stylePenId = stylePenValue;
            }

            #region IEquatable<PenLookupKey> Members

            public Boolean Equals(PenLookupKey other)
            {
                return other._stylePenType.Equals(_stylePenType)
                       && other._stylePenId == _stylePenId;
            }

            #endregion
        }

        private struct FontLookupKey : IEquatable<FontLookupKey>
        {
            private readonly Int32 _fontId;

            public FontLookupKey(Int32 fontId)
            {
                _fontId = fontId;
            }

            #region IEquatable<FontLookupKey> Members

            public Boolean Equals(FontLookupKey other)
            {
                return other._fontId == _fontId;
            }

            #endregion
        }
        #endregion

        public static Rectangle2D Convert(WpfRectangle rectangle)
        {
            return new Rectangle2D(rectangle.X, rectangle.Y,
                                   rectangle.Width + rectangle.X, rectangle.Height + rectangle.Y);
        }

        public static Size2D Convert(WpfSize size)
        {
            return new Size2D(size.Width, size.Height);
        }

        public static Point2D Convert(WpfPoint point)
        {
            return new Point2D(point.X, point.Y);
        }

        public static StyleColor Convert(WpfColor color)
        {
            return StyleColor.FromBgra(color.B, color.G, color.R, color.A);
        }

        public static WpfPoint[] Convert(IEnumerable<Point2D> viewPoints)
        {
            List<WpfPoint> wpfPoints = new List<WpfPoint>();
            foreach (Point2D viewPoint in viewPoints)
                wpfPoints.Add(Convert(viewPoint));

            return wpfPoints.ToArray();
        }

        public static WpfRectangle Convert(Rectangle2D rectangle)
        {
            return new WpfRectangle(rectangle.X, rectangle.Y,
                                    rectangle.Width, rectangle.Height);
        }

        public static WpfFont Convert(StyleFont styleFont)
        {
            WpfFont font = GetFont(styleFont);

            if (font == null)
            {
                font = new WpfFont(styleFont.FontFamily.Name,
                         (Single)styleFont.Size.Width, Convert(styleFont.Style));

                SaveFont(styleFont, font);
            }

            return font;
        }

        public static WpfFontStyle Convert(StyleFontStyle fontStyle)
        {
            switch (fontStyle)
            {
                case StyleFontStyle.Italic:
                    return System.Windows.FontStyles.Italic;
                case StyleFontStyle.Oblique:
                    return System.Windows.FontStyles.Oblique;
                default:
                    return System.Windows.FontStyles.Normal;
            }
        }

        public static WpfMatrix Convert(Matrix2D viewMatrix)
        {
            if (viewMatrix == null)
            {
                return WpfMatrix.Identity;
            }

            WpfMatrix WpfMatrix = new WpfMatrix(
                (double)viewMatrix[0, 0],(double)viewMatrix[0, 1],
                (double)viewMatrix[1, 0], (double)viewMatrix[1, 1],
                (double)viewMatrix[2, 0], (double)viewMatrix[2, 1]);

            return WpfMatrix;
        }

        /*
        public static WpfColorMatrix Convert(ColorMatrix colorMatrix)
        {
            if (colorMatrix == null)
            {
                return null;
            }

            WpfColorMatrix WpfColorMatrix = new WpfColorMatrix();
            WpfColorMatrix.Matrix00 = (Single)colorMatrix.R;
            WpfColorMatrix.Matrix11 = (Single)colorMatrix.G;
            WpfColorMatrix.Matrix22 = (Single)colorMatrix.B;
            WpfColorMatrix.Matrix33 = (Single)colorMatrix.A;
            WpfColorMatrix.Matrix40 = (Single)colorMatrix.RedShift;
            WpfColorMatrix.Matrix41 = (Single)colorMatrix.GreenShift;
            WpfColorMatrix.Matrix42 = (Single)colorMatrix.BlueShift;
            WpfColorMatrix.Matrix44 = 1;
            return WpfColorMatrix;
        }
        */
        public static WpfColor Convert(StyleColor color)
        {
            return WpfColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static WpfFontFamily Convert(StyleFontFamily fontFamily)
        {
            if (fontFamily == null)
            {
                return null;
            }

            return new WpfFontFamily(fontFamily.Name);
        }

        public static DashStyle ConvertLineDashStyle(StylePen pen)
        {
            switch (pen.DashStyle)
            {
                case LineDashStyle.Custom:
                    double[] dashOffsets = new double[pen.DashPattern.Length];
                    for (int i = 0; i < pen.DashPattern.Length; i++)
                        dashOffsets[i] = pen.DashPattern[i];
                    return new DashStyle(dashOffsets, pen.DashOffset);

                case LineDashStyle.Dash:
                    return DashStyles.Dash;
                case LineDashStyle.Dot:
                    return DashStyles.Dot;
                case LineDashStyle.DashDot:
                    return DashStyles.DashDot;
                case LineDashStyle.DashDotDot:
                    return DashStyles.DashDotDot;
                default:
                    return DashStyles.Solid;
            }
        }

        public static WpfPen Convert(StylePen pen)
        {
            if (pen == null)
            {
                return null;
            }

            WpfPen wpfPen = GetPen(pen);

            if (wpfPen == null)
            {
                WpfBrush brush = Convert(pen.BackgroundBrush);
                wpfPen = new WpfPen(brush, (Single)pen.Width);
                
                //wpfPen.Alignment = (PenAlignment)(Int32)pen.Alignment;
                //if (pen.CompoundArray != null) wpfPen.CompoundArray = pen.CompoundArray;
                ////wpfPen.CustomEndCap = new System.Drawing.Drawing2D.CustomLineCap();
                ////wpfPen.CustomStartCap = new System.Drawing.Drawing2D.CustomLineCap();
                wpfPen.DashCap = (PenLineCap)(Int32)pen.DashCap;
                //wpfPen.DashOffset = pen.DashOffset;
                //if (pen.DashPattern != null) wpfPen.DashPattern = pen.DashPattern;
                wpfPen.DashStyle = ConvertLineDashStyle(pen);
                wpfPen.EndLineCap = (PenLineCap)(Int32)pen.EndCap;
                wpfPen.LineJoin = (PenLineJoin)(Int32)pen.LineJoin;
                wpfPen.MiterLimit = pen.MiterLimit;
                //wpfPen.PenType = System.Drawing.Drawing2D.PenType...
                wpfPen.StartLineCap = (PenLineCap)(Int32)pen.StartCap;
                SavePen(pen, wpfPen);
            }

            return wpfPen;
        }

        public static WpfBrush Convert(StyleBrush brush)
        {
            if (brush == null)
            {
                return null;
            }

            WpfBrush wpfBrush = GetBrush(brush);

            if (wpfBrush == null)
            {
                // TODO: need to accomodate other types of view brushes
                if (brush is SolidStyleBrush)
                {
                    SolidStyleBrush ssb = (SolidStyleBrush) brush;
                    wpfBrush = new System.Windows.Media.SolidColorBrush(Convert(ssb.Color));
                }
                    //else if (brush is HatchStyleBrush)
                //
                else if (brush is LinearGradientStyleBrush)
                {
                    LinearGradientStyleBrush lgsb = (LinearGradientStyleBrush) brush;
                    wpfBrush = new System.Windows.Media.LinearGradientBrush(Convert(lgsb.ColorBlend));
                }
                SaveBrush(brush, wpfBrush);
            }

            return wpfBrush;
        }

        public static GradientStopCollection Convert(StyleColorBlend colorBlend)
        {
            if (colorBlend == null)
                return null;

            GradientStopCollection gsc = new GradientStopCollection(colorBlend.Colors.Length);
            for (int i = 0; i < colorBlend.Colors.Length; i++ )
                gsc.Add(new GradientStop(Convert(colorBlend.Colors[i]), colorBlend.Positions[i]));

            return gsc;
        }

        public static WpfPoint Convert(Point2D viewPoint)
        {
            WpfPoint wpfPoint = new WpfPoint(viewPoint.X, viewPoint.Y);
            return wpfPoint;
        }

        //public static WpfPoint ConvertToStreamGeometry(Point2D viewPoint)
        //{
        //    WpfPath wpfPath = new WpfPath();
        //    using (StreamGeometryContext sgc = wpfPath.Open())
        //    {
        //        sgc.
        //        foreach (Figure2D figure in path.Figures)
        //        {
        //            sgc.BeginFigure(Convert(figure.Points[0]), false, figure.IsClosed);
        //            for (int i = 1; i < figure.Points.Count; i++)
        //                sgc.LineTo(Convert(figure.Points[i]), true, true);
        //        }
        //    }
        //    return wpfPath;
        //}

        public static WpfSize Convert(Size2D viewSize)
        {
            WpfSize wpfSize = new WpfSize(viewSize.Width, viewSize.Height);
            return wpfSize;
        }

        public static WpfPath Convert(Path2D path)
        {
            WpfPath wpfPath = new WpfPath();
            using (StreamGeometryContext sgc = wpfPath.Open())
            {
                foreach (Figure2D figure in path.Figures)
                {
                    sgc.BeginFigure(Convert(figure.Points[0]), false, figure.IsClosed);
                    for (int i = 1; i < figure.Points.Count; i++)
                        sgc.LineTo(Convert(figure.Points[i]), true, true);
                }
            }
            return wpfPath;
        }
    }
}