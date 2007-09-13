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
using GdiGraphicsPath = System.Drawing.Drawing2D.GraphicsPath;
using GdiMatrix = System.Drawing.Drawing2D.Matrix;
using GdiColorMatrix = System.Drawing.Imaging.ColorMatrix;
using GdiSmoothingMode = System.Drawing.Drawing2D.SmoothingMode;
using GdiTextRenderingHint = System.Drawing.Text.TextRenderingHint;
using StyleColorMatrix = SharpMap.Rendering.ColorMatrix;

namespace SharpMap.Rendering.Gdi
{
    /// <summary>
    /// A <see cref="VectorRenderer2D{TRenderObject}"/> 
    /// which renders to GDI primatives.
    /// </summary>
    public class GdiVectorRenderer : VectorRenderer2D<GdiRenderObject>
    {
        #region Instance fields
        private readonly Dictionary<BrushLookupKey, GdiBrush> _brushCache = new Dictionary<BrushLookupKey, Brush>();
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
            IEnumerable<GraphicsPath2D> paths, StyleBrush fill, StyleBrush highlightFill,
            StyleBrush selectFill, StylePen outline, StylePen highlightOutline, StylePen selectOutline)
        {
            foreach (GraphicsPath2D path in paths)
            {
                GdiGraphicsPath gdiPath = new GdiGraphicsPath(FillMode.Winding);

                foreach (GraphicsFigure2D figure in path.Figures)
                {
                    gdiPath.StartFigure();

                    gdiPath.AddLines(ViewConverter.Convert(figure.Points));

                    if (figure.IsClosed)
                    {
                        gdiPath.CloseFigure();
                    }
                }

                GdiRenderObject holder = new GdiRenderObject(gdiPath, getBrush(fill), getBrush(highlightFill),
                                                             getBrush(selectFill), getPen(outline), getPen(highlightOutline),
                                                             getPen(selectOutline));

                yield return holder;
            }
        }

        public override IEnumerable<GdiRenderObject> RenderPaths(
            IEnumerable<GraphicsPath2D> path, StylePen outline, StylePen highlightOutline, StylePen selectOutline)
        {
            SolidStyleBrush transparentBrush = new SolidStyleBrush(StyleColor.Transparent);

            return RenderPaths(path, transparentBrush, transparentBrush,
                transparentBrush, outline, highlightOutline, selectOutline);
        }

        public override IEnumerable<GdiRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData)
        {
            return RenderSymbols(locations, symbolData, symbolData, symbolData);
        }

        public override IEnumerable<GdiRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
																			   StyleColorMatrix highlight,
																			   StyleColorMatrix select)
        {
            Symbol2D highlightSymbol = symbolData.Clone();
            highlightSymbol.ColorTransform = highlight;

            Symbol2D selectSymbol = symbolData.Clone();
            selectSymbol.ColorTransform = select;

            return RenderSymbols(locations, symbolData, highlightSymbol, selectSymbol);
        }

        public override IEnumerable<GdiRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbol,
																			   Symbol2D highlightSymbol, Symbol2D selectSymbol)
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
                transform.Translate((float)location.X, (float)location.Y);
                GdiColorMatrix colorTransform = ViewConverter.Convert(symbol.ColorTransform);
                GdiRenderObject holder = new GdiRenderObject(bitmapSymbol, transform, colorTransform);
                yield return holder;
            }

            //if (symbolRotation != 0 && symbolRotation != float.NaN)
            //{
            //    g.TranslateTransform(pointLocation.X, pointLocation.Y);
            //    g.RotateTransform(symbolRotation);
            //    g.TranslateTransform(-symbol.Width / 2, -symbol.Height / 2);
            //    if (symbolScale == 1f)
            //        g.DrawImageUnscaled(symbol, (int)(pointLocation.X - symbol.Width / 2 + symbolOffset.X), (int)(pointLocation.Y - symbol.Height / 2 + symbolOffset.Y));
            //    else
            //    {
            //        float width = symbol.Width * symbolScale;
            //        float height = symbol.Height * symbolScale;
            //        g.DrawImage(symbol, (int)pointLocation.X - width / 2 + symbolOffset.X * symbolScale, (int)pointLocation.Y - height / 2 + symbolOffset.Y * symbolScale, width, height);
            //    }
            //    g.Transform = map.MapTransform;
            //}
            //else
            //{
            //    if (symbolScale == 1f)
            //        g.DrawImageUnscaled(symbol, (int)(pointLocation.X - symbol.Width / 2 + symbolOffset.X), (int)(pointLocation.Y - symbol.Height / 2 + symbolOffset.Y));
            //    else
            //    {
            //        float width = symbol.Width * symbolScale;
            //        float height = symbol.Height * symbolScale;

            //    }
            //}
        }

        #endregion

        #region Private helper methods

        private Brush getBrush(StyleBrush styleBrush)
        {
            if (styleBrush == null)
            {
                return null;
            }

            BrushLookupKey key = new BrushLookupKey(styleBrush.GetType().TypeHandle, styleBrush.ToString());
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

            PenLookupKey key = new PenLookupKey(stylePen.GetType().TypeHandle, stylePen.ToString());
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

            SymbolLookupKey key = new SymbolLookupKey(symbol2D.ToString());
            Bitmap symbol;
            _symbolCache.TryGetValue(key, out symbol);

            if (symbol == null)
            {
                MemoryStream data = new MemoryStream();
                symbol2D.SymbolData.Position = 0;

                using (BinaryReader reader = new BinaryReader(symbol2D.SymbolData))
                {
                    data.Write(reader.ReadBytes((int)symbol2D.SymbolData.Length), 0, (int)symbol2D.SymbolData.Length);
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
            public readonly string StyleBrushValue;

            public BrushLookupKey(RuntimeTypeHandle type, string styleBrushValue)
            {
                StyleBrushType = type;
                StyleBrushValue = styleBrushValue;
            }

            #region IEquatable<BrushLookupKey> Members

            public bool Equals(BrushLookupKey other)
            {
                return other.StyleBrushType.Equals(StyleBrushType)
                       && other.StyleBrushValue == StyleBrushValue;
            }

            #endregion
        }

        private struct PenLookupKey : IEquatable<PenLookupKey>
        {
            public readonly RuntimeTypeHandle StylePenType;
            public readonly string StylePenValue;

            public PenLookupKey(RuntimeTypeHandle type, string stylePenValue)
            {
                StylePenType = type;
                StylePenValue = stylePenValue;
            }

            #region IEquatable<PenLookupKey> Members

            public bool Equals(PenLookupKey other)
            {
                return other.StylePenType.Equals(StylePenType)
                       && other.StylePenValue == StylePenValue;
            }

            #endregion
        }

        private struct SymbolLookupKey : IEquatable<SymbolLookupKey>
        {
            public readonly string SymbolValue;

            public SymbolLookupKey(string symbolValue)
            {
                SymbolValue = symbolValue;
            }

            #region IEquatable<SymbolLookupKey> Members

            public bool Equals(SymbolLookupKey other)
            {
                return other.SymbolValue == SymbolValue;
            }

            #endregion
        }
        #endregion
    }
}