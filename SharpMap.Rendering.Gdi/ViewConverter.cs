// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using System.Drawing;
using System.Drawing.Drawing2D;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using GdiPoint = System.Drawing.Point;
using GdiSize = System.Drawing.Size;
using GdiRectangle = System.Drawing.Rectangle;
using GdiBrushes = System.Drawing.Brushes;
using GdiFont = System.Drawing.Font;
using GdiFontFamily = System.Drawing.FontFamily;
using GdiFontStyle = System.Drawing.FontStyle;
using GdiColor = System.Drawing.Color;
using GdiPath = System.Drawing.Drawing2D.GraphicsPath;
using GdiMatrix = System.Drawing.Drawing2D.Matrix;
using GdiColorMatrix = System.Drawing.Imaging.ColorMatrix;
using GdiSmoothingMode = System.Drawing.Drawing2D.SmoothingMode;
using GdiTextRenderingHint = System.Drawing.Text.TextRenderingHint;

namespace SharpMap.Rendering.Gdi
{
	public static class ViewConverter
    {

        private static readonly Dictionary<FontLookupKey, Font> _fontCache = new Dictionary<FontLookupKey, Font>();
        private static readonly Dictionary<BrushLookupKey, Brush> _brushCache = new Dictionary<BrushLookupKey, Brush>();
        private static readonly Dictionary<PenLookupKey, Pen> _penCache = new Dictionary<PenLookupKey, Pen>();

        private static Font getFont(StyleFont styleFont)
        {
            if (styleFont == null)
            {
                return null;
            }

            FontLookupKey key = new FontLookupKey(styleFont.GetHashCode());

            Font font;
            _fontCache.TryGetValue(key, out font);
            return font;
        }

        private static void saveFont(StyleFont styleFont, Font font)
        {
            _fontCache[new FontLookupKey(styleFont.GetHashCode())] = font;
        }

        private static Brush getBrush(StyleBrush styleBrush)
        {
            if (styleBrush == null)
            {
                return null;
            }

            BrushLookupKey key = new BrushLookupKey(styleBrush.GetType().TypeHandle, styleBrush.GetHashCode());
            Brush brush;
            _brushCache.TryGetValue(key, out brush);
            return brush;
        }

        private static void saveBrush(StyleBrush styleBrush, Brush brush)
        {
            _brushCache[new BrushLookupKey(styleBrush.GetType().TypeHandle, styleBrush.GetHashCode())] = brush;
        }

        private static Pen getPen(StylePen stylePen)
        {
            if (stylePen == null)
            {
                return null;
            }

            PenLookupKey key = new PenLookupKey(stylePen.GetType().TypeHandle, stylePen.GetHashCode());
            Pen pen;
            _penCache.TryGetValue(key, out pen);
            return pen;
        }

        private static void savePen(StylePen stylePen, Pen pen)
        {
            _penCache[new PenLookupKey(stylePen.GetType().TypeHandle, stylePen.GetHashCode())] = pen;
        }

        #region Nested types
        private struct BrushLookupKey : IEquatable<BrushLookupKey>
        {
            public readonly RuntimeTypeHandle StyleBrushType;
            public readonly int StyleBrushId;

            public BrushLookupKey(RuntimeTypeHandle type, int styleBrushId)
            {
                StyleBrushType = type;
                StyleBrushId = styleBrushId;
            }

            #region IEquatable<BrushLookupKey> Members

            public bool Equals(BrushLookupKey other)
            {
                return other.StyleBrushType.Equals(StyleBrushType)
                       && other.StyleBrushId == StyleBrushId;
            }

            #endregion
        }

        private struct PenLookupKey : IEquatable<PenLookupKey>
        {
            public readonly RuntimeTypeHandle StylePenType;
            public readonly int StylePenId;

            public PenLookupKey(RuntimeTypeHandle type, int stylePenValue)
            {
                StylePenType = type;
                StylePenId = stylePenValue;
            }

            #region IEquatable<PenLookupKey> Members

            public bool Equals(PenLookupKey other)
            {
                return other.StylePenType.Equals(StylePenType)
                       && other.StylePenId == StylePenId;
            }

            #endregion
        }

        private struct FontLookupKey : IEquatable<FontLookupKey>
        {
            public readonly int FontId;

            public FontLookupKey(int fontId)
            {
                FontId = fontId;
            }

            #region IEquatable<FontLookupKey> Members

            public bool Equals(FontLookupKey other)
            {
                return other.FontId == FontId;
            }

            #endregion
        }
        #endregion

		public static Rectangle2D Convert(GdiRectangle rectangle)
		{
			return new Rectangle2D(rectangle.X, rectangle.Y,
								   rectangle.Width + rectangle.X, rectangle.Height + rectangle.Y);
		}

		public static Rectangle2D Convert(RectangleF rectangleF)
		{
			return new Rectangle2D(rectangleF.Left, rectangleF.Top, rectangleF.Right, rectangleF.Bottom);
		}

		public static Size2D Convert(SizeF sizeF)
		{
			return new Size2D(sizeF.Width, sizeF.Height);
		}

		public static Point2D Convert(GdiPoint point)
		{
			return new Point2D(point.X, point.Y);
		}

		public static StyleColor Convert(GdiColor color)
		{
			return StyleColor.FromBgra(color.B, color.G, color.R, color.A);
		}

		public static PointF[] Convert(IEnumerable<Point2D> viewPoints)
		{
			List<PointF> gdiPoints = new List<PointF>();
			foreach (Point2D viewPoint in viewPoints)
			{
				gdiPoints.Add(Convert(viewPoint));
			}

			return gdiPoints.ToArray();
		}

		public static GdiRectangle Convert(Rectangle2D rectangle)
		{
			return new GdiRectangle((int)rectangle.X, (int)rectangle.Y,
									(int)rectangle.Width, (int)rectangle.Height);
		}

        public static GdiFont Convert(StyleFont styleFont)
        {
            GdiFont font = getFont(styleFont);

            if (font == null)
            {
                font = new Font(styleFont.FontFamily.Name,
                         (float) styleFont.Size.Width, Convert(styleFont.Style));

                saveFont(styleFont, font);
            }

            return font;
        }

	    public static GdiFontStyle Convert(StyleFontStyle fontStyle)
		{
			return (GdiFontStyle)(int)(fontStyle);
		}

		public static GdiMatrix Convert(Matrix2D viewMatrix)
		{
			GdiMatrix gdiMatrix = new GdiMatrix(
				(float)viewMatrix[0, 0],
				(float)viewMatrix[0, 1],
				(float)viewMatrix[1, 0],
				(float)viewMatrix[1, 1],
				(float)viewMatrix[2, 0],
				(float)viewMatrix[2, 1]);

			return gdiMatrix;
		}

		public static GdiColorMatrix Convert(ColorMatrix colorMatrix)
		{
			GdiColorMatrix gdiColorMatrix = new GdiColorMatrix();
			gdiColorMatrix.Matrix00 = (float)colorMatrix.R;
			gdiColorMatrix.Matrix11 = (float)colorMatrix.G;
			gdiColorMatrix.Matrix22 = (float)colorMatrix.B;
			gdiColorMatrix.Matrix33 = (float)colorMatrix.A;
			gdiColorMatrix.Matrix40 = (float)colorMatrix.RedShift;
			gdiColorMatrix.Matrix41 = (float)colorMatrix.GreenShift;
			gdiColorMatrix.Matrix42 = (float)colorMatrix.BlueShift;
			gdiColorMatrix.Matrix44 = 1;
			return gdiColorMatrix;
		}

		public static GdiColor Convert(StyleColor color)
		{
			return GdiColor.FromArgb(color.A, color.R, color.G, color.B);
		}

		public static GdiFontFamily Convert(StyleFontFamily fontFamily)
		{
			if (fontFamily == null)
			{
				return null;
			}

			return new GdiFontFamily(fontFamily.Name);
		}

		public static Pen Convert(StylePen pen)
		{
			if (pen == null)
			{
				return null;
			}

		    Pen gdiPen;

		    gdiPen = getPen(pen);

            if (gdiPen == null)
            {
                Brush brush = Convert(pen.BackgroundBrush);
                gdiPen = new Pen(brush, (float) pen.Width);

                gdiPen.Alignment = (PenAlignment) (int) pen.Alignment;
                if (pen.CompoundArray != null) gdiPen.CompoundArray = pen.CompoundArray;
                //gdiPen.CustomEndCap = new System.Drawing.Drawing2D.CustomLineCap();
                //gdiPen.CustomStartCap = new System.Drawing.Drawing2D.CustomLineCap();
                gdiPen.DashCap = (DashCap) (int) pen.DashCap;
                gdiPen.DashOffset = pen.DashOffset;
                if (pen.DashPattern != null) gdiPen.DashPattern = pen.DashPattern;
                gdiPen.DashStyle = (DashStyle) (int) pen.DashStyle;
                gdiPen.EndCap = (LineCap) (int) pen.EndCap;
                gdiPen.LineJoin = (LineJoin) (int) pen.LineJoin;
                gdiPen.MiterLimit = pen.MiterLimit;
                //gdiPen.PenType = System.Drawing.Drawing2D.PenType...
                gdiPen.StartCap = (LineCap) (int) pen.StartCap;
                gdiPen.Transform = Convert(pen.Transform);
                savePen(pen, gdiPen);
            }

		    return gdiPen;
		}

		public static Brush Convert(StyleBrush brush)
		{
			if (brush == null)
			{
				return null;
			}

		    Brush gdiBrush;

		    gdiBrush = getBrush(brush);

            if(gdiBrush == null)
            {
                // TODO: need to accomodate other types of view brushes
                gdiBrush = new SolidBrush(Convert(brush.Color));

                saveBrush(brush, gdiBrush);
            }

			return gdiBrush;
		}

		public static PointF Convert(Point2D viewPoint)
		{
			PointF gdiPoint = new PointF((float)viewPoint.X, (float)viewPoint.Y);
			return gdiPoint;
		}

		public static SizeF Convert(Size2D viewSize)
		{
			SizeF gdiSize = new SizeF((float)viewSize.Width, (float)viewSize.Height);
			return gdiSize;
		}

        public static GraphicsPath Convert(Path2D path)
        {
            GdiPath gdiPath = new GdiPath(FillMode.Winding);

            foreach (Figure2D figure in path.Figures)
            {
                gdiPath.StartFigure();

                gdiPath.AddLines(Convert(figure.Points));

                if (figure.IsClosed)
                {
                    gdiPath.CloseFigure();
                }
            }

            return gdiPath;
        }
	}
}