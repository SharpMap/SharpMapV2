// Portions copyright 2007 - Rory Plaire (codekaizen@gmail.com)
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
using System.IO;
using System.Net.Mime;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Wpf
{
    public static class ViewConverter
    {
        private static readonly Dictionary<FontLookupKey, Typeface> _fontCache = new Dictionary<FontLookupKey, Typeface>();
        private static readonly Dictionary<BrushLookupKey, Brush> _brushCache = new Dictionary<BrushLookupKey, Brush>();
        private static readonly Dictionary<PenLookupKey, Pen> _penCache = new Dictionary<PenLookupKey, Pen>();
        private static readonly Dictionary<SymbolLookupKey, ImageSource> _symbolCache = new Dictionary<SymbolLookupKey, ImageSource>();

        //TODO: thread safety

        private static readonly ReaderWriterLockSlim _fontCacheLock = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim _brushCacheLock = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim _penCacheLock = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim _symbolCacheLock = new ReaderWriterLockSlim();

        private static readonly TextDecorationCollection _decorationsNone = new System.Windows.TextDecorationCollection();

        private static readonly
        TextDecorationCollection _decorationsUderlineStrikeThrough = new TextDecorationCollection
                                            { 
                                                System.Windows.TextDecorations.Underline, 
                                                System.Windows.TextDecorations.Strikethrough 
                                            };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline"
            , Justification = "static constructor used to freeze constant freezables")]
        static ViewConverter()
        {
            _decorationsNone.Freeze();
            _decorationsUderlineStrikeThrough.Freeze();
        }

        public static Rectangle2D Convert(Rect rectangle)
        {
            return new Rectangle2D(rectangle.X, rectangle.Y,
                                   rectangle.Width + rectangle.X, rectangle.Height + rectangle.Y);
        }

        public static Size2D Convert(Size sizeF)
        {
            return new Size2D(sizeF.Width, sizeF.Height);
        }

        public static Point2D Convert(Point point)
        {
            return new Point2D(point.X, point.Y);
        }

        public static System.Windows.Vector Convert2Vector(Point2D point)
        {
            return new System.Windows.Vector(point.X, point.Y);
        }

        public static StyleColor Convert(Color color)
        {
            return StyleColor.FromBgra(color.B, color.G, color.R, color.A);
        }

        public static Point[] Convert(IEnumerable<Point2D> sourcePoints)
        {
            List<Point> wpfPoints = new List<Point>();
            foreach (Point2D viewPoint in sourcePoints)
            {
                wpfPoints.Add(Convert(viewPoint));
            }

            return wpfPoints.ToArray();
        }

        public static Rect Convert(Rectangle2D rectangle)
        {
            return new Rect((int)rectangle.X, (int)rectangle.Y,
                                    (int)rectangle.Width, (int)rectangle.Height);
        }

        public static FontStyle ConvertFontStyle(StyleFontStyle fontStyle)
        {
            if ((fontStyle & StyleFontStyle.Italic) != 0)
            {
                return System.Windows.FontStyles.Italic;
            }
            return System.Windows.FontStyles.Normal;
        }

        public static FontWeight ConvertFontWeight(StyleFontStyle fontStyle)
        {
            if ((fontStyle & StyleFontStyle.Bold) != 0)
            {
                return System.Windows.FontWeights.Bold;
            }
            return System.Windows.FontWeights.Normal;
        }

        public static TextDecorationCollection ConvertFontDecoration(StyleFontStyle fontStyle)
        {
            if ((fontStyle & StyleFontStyle.Strikeout) == 0
                && (fontStyle & StyleFontStyle.Underline) == 0)
            {
                return _decorationsNone;
            }
            if ((fontStyle & StyleFontStyle.Strikeout) != 0
                && (fontStyle & StyleFontStyle.Underline) == 0)
            {
                return System.Windows.TextDecorations.Strikethrough;
            }
            if ((fontStyle & StyleFontStyle.Strikeout) == 0
                && (fontStyle & StyleFontStyle.Underline) != 0)
            {
                return System.Windows.TextDecorations.Underline;
            }
            if ((fontStyle & StyleFontStyle.Strikeout) != 0
                    && (fontStyle & StyleFontStyle.Underline) != 0)
            {
                return _decorationsUderlineStrikeThrough;
            }
            throw new ArgumentException("StyleFontStyle should be empty or a combination of Underline and Strikethrough",
                "fontStyle");

        }

        public static System.Windows.Media.Matrix Convert(Matrix2D viewMatrix)
        {
            if (viewMatrix == null)
            {
                return System.Windows.Media.Matrix.Identity;
            }
            System.Windows.Media.Matrix wpfMatrix = new System.Windows.Media.Matrix(
            viewMatrix.M11,
            viewMatrix.M12,
            viewMatrix.M21,
            viewMatrix.M22,
            viewMatrix.OffsetX,
            viewMatrix.OffsetY);

            return wpfMatrix;
        }

        public static Matrix2D Convert(System.Windows.Media.Matrix viewMatrix)
        {
            if (viewMatrix == null)
            {
                return Matrix2D.Identity;
            }
            Matrix2D wpfMatrix = new Matrix2D(
            viewMatrix.M11,
            viewMatrix.M12,
            viewMatrix.M21,
            viewMatrix.M22,
            viewMatrix.OffsetX,
            viewMatrix.OffsetY);

            return wpfMatrix;
        }


        public static Color Convert(StyleColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static FontFamily Convert(StyleFontFamily fontFamily)
        {
            if (fontFamily == null)
            {
                return null;
            }

            return new FontFamily(fontFamily.Name);
        }

        public static Typeface Convert(StyleFont styleFont)
        {
            Typeface font = getFont(styleFont);
            if (font == null)
            {
                font = new Typeface(Convert(styleFont.FontFamily), ConvertFontStyle(styleFont.Style),
                                    ConvertFontWeight(styleFont.Style), System.Windows.FontStretches.Normal);
                saveFont(styleFont, font);
            }
            return font;
        }

        public static Double ConvertPointsToFontSize(Double emPoint)
        {
            return (4.0 / 3.0) * emPoint;
        }

        public static Pen Convert(StylePen pen)
        {
            if (pen == null)
            {
                return null;
            }

            Brush brush = Convert(pen.BackgroundBrush);
            Pen wpfPen = getPen(pen);
            if (wpfPen == null)
            {
                wpfPen = new Pen(brush, (float)pen.Width);

                wpfPen.EndLineCap = Convert(pen.EndCap);
                wpfPen.StartLineCap = Convert(pen.StartCap);
                wpfPen.DashCap = Convert(pen.DashCap);

                wpfPen.DashStyle = convertDashStyle(pen.DashStyle, pen.DashPattern, pen.DashOffset);


                wpfPen.LineJoin = Convert(pen.LineJoin);
                wpfPen.MiterLimit = pen.MiterLimit;
                savePen(pen, wpfPen);
            }
            return wpfPen;
        }

        private static PenLineJoin Convert(StyleLineJoin sourcePoints)
        {
            switch (sourcePoints)
            {
                case StyleLineJoin.Miter:
                case StyleLineJoin.MiterClipped:
                    return PenLineJoin.Miter;
                case StyleLineJoin.Bevel:
                    return PenLineJoin.Bevel;
                case StyleLineJoin.Round:
                    return PenLineJoin.Round;
                default:
                    throw new ArgumentOutOfRangeException("sourcePoints");
            }
        }

        private static PenLineCap Convert(LineDashCap sourcePoints)
        {
            switch (sourcePoints)
            {
                case LineDashCap.Flat:
                    return PenLineCap.Flat;
                case LineDashCap.Round:
                    return PenLineCap.Round;
                case LineDashCap.Triangle:
                    return PenLineCap.Triangle;
                default:
                    throw new ArgumentOutOfRangeException("sourcePoints");
            }
        }

        private static PenLineCap Convert(StyleLineCap sourcePoints)
        {
            switch (sourcePoints)
            {
                case StyleLineCap.Custom:
                case StyleLineCap.Flat:
                case StyleLineCap.NoAnchor:
                    return PenLineCap.Flat;
                case StyleLineCap.Round:
                case StyleLineCap.RoundAnchor:
                    return PenLineCap.Round;;
                case StyleLineCap.Square:
                case StyleLineCap.SquareAnchor:
                    return PenLineCap.Square;
                case StyleLineCap.AnchorMask:
                case StyleLineCap.ArrowAnchor:
                case StyleLineCap.DiamondAnchor:
                case StyleLineCap.Triangle:
                    return PenLineCap.Triangle;
                default:
                    throw new ArgumentOutOfRangeException("sourcePoints");
            }
        }

        public static StylePen Convert(Pen pen)
        {
            if (pen == null)
            {
                return null;
            }

            StyleBrush brush = Convert(pen.Brush);
            StylePen mapPen = new StylePen(brush, pen.Thickness);

            if (brush == null)
            {
                mapPen.EndCap = (StyleLineCap)(int)(pen.EndLineCap);
                mapPen.StartCap = (StyleLineCap)(int)(pen.StartLineCap);
                mapPen.DashCap = (LineDashCap)(int)(pen.DashCap);


                //TODO: complete this conversion;
                //wpfPen.DashStyle = convertDashStyle(pen.DashStyle, pen.DashPattern, pen.DashOffset);


                mapPen.LineJoin = (StyleLineJoin)(int)pen.LineJoin;
                mapPen.MiterLimit = (float)pen.MiterLimit;

            }
            return mapPen;
        }


        public static Brush Convert(StyleBrush brush)
        {
            if (brush == null)
            {
                return null;
            }

            Brush wpfBrush;

            wpfBrush = getBrush(brush);

            if (wpfBrush == null)
            {
                // TODO: need to accomodate other types of view brushes
                wpfBrush = new SolidColorBrush(Convert(brush.Color));

                saveBrush(brush, wpfBrush);
            }

            return wpfBrush;
        }

        public static StyleBrush Convert(Brush wpfBrush)
        {
            SolidColorBrush solidBrush = wpfBrush as SolidColorBrush;
            if (solidBrush == null)
            {
                return null;
            }

            SharpMap.Styles.SolidStyleBrush brush = new SharpMap.Styles.SolidStyleBrush(Convert(solidBrush.Color)); ;
            return brush;
        }

        public static Point Convert(Point2D sourcePoint)
        {
            Point wpfPoint = new Point(sourcePoint.X, sourcePoint.Y);
            return wpfPoint;
        }

        public static Size Convert(Size2D viewSize)
        {
            Size wpfSize = new Size((float)viewSize.Width, (float)viewSize.Height);
            return wpfSize;
        }

        public static ImageSource Convert(Symbol2D symbol2D)
        {
            if (symbol2D == null)
            {
                return null;
            }

            var key = new SymbolLookupKey(symbol2D.GetHashCode());
            ImageSource symbol;

            _symbolCacheLock.EnterReadLock();
            try
            {
                _symbolCache.TryGetValue(key, out symbol);
            }
            finally { _symbolCacheLock.ExitReadLock(); }

            //TODO: handle other formats
            if (symbol == null)
            {
                var clone = symbol2D.Clone();

                var bmpImg = new BitmapImage();
                bmpImg.BeginInit();
                bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                bmpImg.StreamSource = clone.SymbolData;
                bmpImg.EndInit();
                symbol = bmpImg;
                symbol.Freeze();
                _symbolCacheLock.EnterWriteLock();
                try
                {
                    _symbolCache[key] = symbol;
                }
                finally { _symbolCacheLock.ExitWriteLock(); }
            }
            return symbol;
        }

        public static void AttachRenderMode(System.Windows.DependencyObject target, StyleRenderingMode renderingMode)
        {
            switch (renderingMode)
            {
                case StyleRenderingMode.Default:
                    break;
                case StyleRenderingMode.None:
                    RenderOptions.SetEdgeMode(target, EdgeMode.Aliased);
                    break;
                case StyleRenderingMode.AntiAlias:
                    RenderOptions.SetEdgeMode(target, EdgeMode.Unspecified);
                    break;
                case StyleRenderingMode.HighQuality:
                    RenderOptions.SetEdgeMode(target, EdgeMode.Unspecified);
                    break;
                case StyleRenderingMode.HighSpeed:
                    RenderOptions.SetEdgeMode(target, EdgeMode.Aliased);
                    break;
                default:
                    break;
            }
        }

        #region private



        private static DashStyle convertDashStyle(LineDashStyle lineDashStyle, float[] dashPattern, Double offset)
        {
            DashStyle retVal = null;
            if (dashPattern != null)
            {
                DoubleCollection dashes = new DoubleCollection(dashPattern.Length);
                int i = 0;
                foreach (float item in dashPattern)
                {
                    dashes.Add(item);
                    ++i;
                }
                retVal = new DashStyle(dashes, offset);
                return retVal;
            }

            switch (lineDashStyle)
            {
                case LineDashStyle.Solid:
                    retVal = DashStyles.Solid;
                    break;
                case LineDashStyle.Dash:
                    retVal = DashStyles.Dash;
                    break;
                case LineDashStyle.Dot:
                    retVal = DashStyles.Dot;
                    break;
                case LineDashStyle.DashDot:
                    retVal = DashStyles.DashDot;
                    break;
                case LineDashStyle.DashDotDot:
                    retVal = DashStyles.DashDotDot;
                    break;
                case LineDashStyle.Custom:
                    retVal = null;
                    break;
                default:
                    retVal = null;
                    break;
            }
            return retVal;
        }

        private static Typeface getFont(StyleFont styleFont)
        {
            if (styleFont == null)
            {
                return null;
            }

            FontLookupKey key = new FontLookupKey(styleFont.GetHashCode());

            Typeface font;
            _fontCacheLock.EnterReadLock();
            try
            {
                _fontCache.TryGetValue(key, out font);
            }
            finally { _fontCacheLock.ExitReadLock(); }
            return font;
        }

        private static Brush getBrush(StyleBrush styleBrush)
        {
            if (styleBrush == null)
            {
                return null;
            }

            BrushLookupKey key = new BrushLookupKey(styleBrush.GetType().TypeHandle, styleBrush.GetHashCode());
            Brush brush;
            _brushCacheLock.EnterReadLock();
            try
            {
                _brushCache.TryGetValue(key, out brush);
            }
            finally { _brushCacheLock.ExitReadLock(); }
            return brush;
        }

        private static void saveBrush(StyleBrush styleBrush, Brush brush)
        {
            brush.Freeze();
            _brushCacheLock.EnterWriteLock();
            try
            {
                _brushCache[new BrushLookupKey(styleBrush.GetType().TypeHandle, styleBrush.GetHashCode())] = brush;
            }
            finally { _brushCacheLock.ExitWriteLock(); }
        }

        private static Pen getPen(StylePen stylePen)
        {
            if (stylePen == null)
            {
                return null;
            }

            PenLookupKey key = new PenLookupKey(stylePen.GetType().TypeHandle, stylePen.GetHashCode());
            Pen pen;
            _penCacheLock.EnterReadLock();
            try { _penCache.TryGetValue(key, out pen); }
            finally { _penCacheLock.ExitReadLock(); }
            return pen;
        }

        private static void savePen(StylePen stylePen, Pen pen)
        {
            _penCacheLock.EnterWriteLock();
            try
            {
                pen.Freeze();
                _penCache[new PenLookupKey(stylePen.GetType().TypeHandle, stylePen.GetHashCode())] = pen;
            }
            finally
            {
                _penCacheLock.ExitWriteLock();
            }

        }

        private static void saveFont(StyleFont styleFont, Typeface font)
        {
            _fontCacheLock.EnterWriteLock();
            try { _fontCache[new FontLookupKey(styleFont.GetHashCode())] = font; }
            finally { _fontCacheLock.ExitWriteLock(); }
        }
        #endregion //private
    }
}