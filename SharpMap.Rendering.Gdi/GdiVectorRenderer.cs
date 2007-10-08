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
using System.IO;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using GdiPoint = System.Drawing.Point;
using GdiSize = System.Drawing.Size;
using GdiRectangle = System.Drawing.Rectangle;
using GdiPen = System.Drawing.Pen;
using GdiBrush = System.Drawing.Brush;
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
using StyleColorMatrix = SharpMap.Rendering.ColorMatrix;

namespace SharpMap.Rendering.Gdi
{
    /// <summary>
    /// A <see cref="VectorRenderer2D{TRenderObject}"/> 
    /// which renders to GDI+ (System.Drawing) primatives.
    /// </summary>
    public class GdiVectorRenderer : VectorRenderer2D<GdiRenderObject>
    {
        #region Instance fields

        private readonly Dictionary<BrushLookupKey, GdiBrush> _brushCache = new Dictionary<BrushLookupKey, GdiBrush>();
        private readonly Dictionary<PenLookupKey, Pen> _penCache = new Dictionary<PenLookupKey, Pen>();
        private readonly Dictionary<SymbolLookupKey, Bitmap> _symbolCache = new Dictionary<SymbolLookupKey, Bitmap>();
        #endregion

        #region Dispose override

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (Brush brush in _brushCache.Values)
                {
                    brush.Dispose();
                }

                foreach (Pen pen in _penCache.Values)
                {
                    pen.Dispose();
                }

                foreach (Bitmap bitmap in _symbolCache.Values)
                {
                    bitmap.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Render overrides

        public override IEnumerable<GdiRenderObject> RenderPaths(
            IEnumerable<Path2D> paths, StyleBrush fill, StyleBrush highlightFill,
            StyleBrush selectFill, StylePen outline, StylePen highlightOutline, StylePen selectOutline, RenderState renderState)
        {
            foreach (Path2D path in paths)
            {
                GdiPath gdiPath = ViewConverter.Convert(path);

                GdiRenderObject holder = new GdiRenderObject(gdiPath, getBrush(fill), getBrush(highlightFill),
                                                             getBrush(selectFill), null, null, null,
                                                             getPen(outline), getPen(highlightOutline),
                                                             getPen(selectOutline));
															 
				holder.State = renderState;

                yield return holder;
            }
        }

        public override IEnumerable<GdiRenderObject> RenderPaths(IEnumerable<Path2D> paths,
                                                                 StylePen line, StylePen highlightLine,
                                                                 StylePen selectLine,
                                                                 StylePen outline, StylePen highlightOutline,
                                                                 StylePen selectOutline, RenderState renderState)
        {
            foreach (Path2D path in paths)
            {
                GdiPath gdiPath = ViewConverter.Convert(path);

                GdiRenderObject holder =
                    new GdiRenderObject(gdiPath, null, null, null, getPen(line), getPen(highlightLine),
                                        getPen(selectLine), getPen(outline), getPen(highlightOutline),
                                        getPen(selectOutline));
															 
				holder.State = renderState;

                yield return holder;
            }
        }

        public override IEnumerable<GdiRenderObject> RenderPaths(
            IEnumerable<Path2D> paths, StylePen outline, StylePen highlightOutline, StylePen selectOutline, RenderState renderState)
        {
            SolidStyleBrush transparentBrush = new SolidStyleBrush(StyleColor.Transparent);

            return RenderPaths(paths, transparentBrush, transparentBrush,
                               transparentBrush, outline, highlightOutline, selectOutline, renderState);
        }

        public override IEnumerable<GdiRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData, RenderState renderState)
        {
            return RenderSymbols(locations, symbolData, symbolData, symbolData, renderState);
        }

        public override IEnumerable<GdiRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                   StyleColorMatrix highlight,
                                                                   StyleColorMatrix select, RenderState renderState)
        {
            Symbol2D highlightSymbol = (Symbol2D) symbolData.Clone();
            highlightSymbol.ColorTransform = highlight;

            Symbol2D selectSymbol = (Symbol2D) symbolData.Clone();
            selectSymbol.ColorTransform = select;

            return RenderSymbols(locations, symbolData, highlightSymbol, selectSymbol, renderState);
        }

        public override IEnumerable<GdiRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbol,
                                                                   Symbol2D highlightSymbol, Symbol2D selectSymbol, RenderState renderState)
        {
            if (highlightSymbol == null)
            {
                highlightSymbol = symbol;
            }

            if (selectSymbol == null)
            {
                selectSymbol = symbol;
            }

            foreach (Point2D location in locations)
            {
                Bitmap bitmapSymbol = getSymbol(symbol);
                GdiMatrix transform = ViewConverter.Convert(symbol.AffineTransform);
                GdiColorMatrix colorTransform = ViewConverter.Convert(symbol.ColorTransform);
                //transform.Translate((float)location.X, (float)location.Y);
                RectangleF bounds = new RectangleF(ViewConverter.Convert(location), bitmapSymbol.Size);
                GdiRenderObject holder = new GdiRenderObject(bitmapSymbol, bounds, transform, colorTransform);
				holder.State = renderState;
                yield return holder;
            }
        }

        public override GdiRenderObject RenderText(string text, StyleFont font, Rectangle2D layoutRectangle)
        {
            GdiPath path = new GdiPath();
            path.AddString(text, ViewConverter.Convert(font.FontFamily), 
                (int)ViewConverter.Convert(font.Style), (float)font.Size.Width, 
                ViewConverter.Convert(layoutRectangle), StringFormat.GenericDefault);
            GdiRenderObject renderObject = new GdiRenderObject(
                path, null, null, null, null, null, null, null, null, null);
            return renderObject;
        }

        #endregion

        #region Private helper methods
        private Font getFont(StyleFont styleFont)
        {
            throw new NotImplementedException();
        }

        private Brush getBrush(StyleBrush styleBrush)
        {
            if (styleBrush == null)
            {
                return null;
            }

            BrushLookupKey key = new BrushLookupKey(styleBrush.GetType().TypeHandle, styleBrush.GetHashCode());
            Brush brush;
            _brushCache.TryGetValue(key, out brush);

            if (brush == null)
            {
                brush = ViewConverter.Convert(styleBrush);
                _brushCache[key] = brush;
            }

            return brush;
        }

        private Pen getPen(StylePen stylePen)
        {
            if (stylePen == null)
            {
                return null;
            }

            PenLookupKey key = new PenLookupKey(stylePen.GetType().TypeHandle, stylePen.GetHashCode());
            Pen pen;
            _penCache.TryGetValue(key, out pen);

            if (pen == null)
            {
                pen = ViewConverter.Convert(stylePen);
                _penCache[key] = pen;
            }

            return pen;
        }

        private Bitmap getSymbol(Symbol2D symbol2D)
        {
            if (symbol2D == null)
            {
                return null;
            }

            SymbolLookupKey key = new SymbolLookupKey(symbol2D.GetHashCode());
            Bitmap symbol;
            _symbolCache.TryGetValue(key, out symbol);

            if (symbol == null)
            {
                MemoryStream data = new MemoryStream();
                symbol2D.SymbolData.Position = 0;

                using (BinaryReader reader = new BinaryReader(symbol2D.SymbolData))
                {
                    data.Write(reader.ReadBytes((int) symbol2D.SymbolData.Length), 0, (int) symbol2D.SymbolData.Length);
                }

                symbol = new Bitmap(data);
                _symbolCache[key] = symbol;
            }

            return symbol;
        }

        #endregion

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

        private struct SymbolLookupKey : IEquatable<SymbolLookupKey>
        {
            public readonly int SymbolId;

            public SymbolLookupKey(int symbolId)
            {
                SymbolId = symbolId;
            }

            #region IEquatable<SymbolLookupKey> Members

            public bool Equals(SymbolLookupKey other)
            {
                return other.SymbolId == SymbolId;
            }

            #endregion
        }

        #endregion
    }
}