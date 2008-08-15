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
using System.Drawing;
using System.IO;
using GeoAPI.IO;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Utilities;
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

        private readonly Dictionary<SymbolLookupKey, Bitmap> _symbolCache = new Dictionary<SymbolLookupKey, Bitmap>();

        #endregion

        #region Dispose override

        protected override void Dispose(Boolean disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
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
            StyleBrush selectFill, StylePen outline, StylePen highlightOutline, StylePen selectOutline,
            RenderState renderState)
        {
            foreach (Path2D path in paths)
            {
                GdiPath gdiPath = ViewConverter.Convert(path);

                GdiRenderObject holder = new GdiRenderObject(gdiPath, ViewConverter.Convert(fill),
                                                             ViewConverter.Convert(highlightFill),
                                                             ViewConverter.Convert(selectFill),
                                                             null, null, null,
                                                             ViewConverter.Convert(outline),
                                                             ViewConverter.Convert(highlightOutline),
                                                             ViewConverter.Convert(selectOutline));

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
                    new GdiRenderObject(gdiPath, null, null, null, ViewConverter.Convert(line),
                                        ViewConverter.Convert(highlightLine), ViewConverter.Convert(selectLine),
                                        ViewConverter.Convert(outline), ViewConverter.Convert(highlightOutline),
                                        ViewConverter.Convert(selectOutline));

                holder.State = renderState;

                yield return holder;
            }
        }

        public override IEnumerable<GdiRenderObject> RenderPaths(
            IEnumerable<Path2D> paths, StylePen outline, StylePen highlightOutline, StylePen selectOutline,
            RenderState renderState)
        {
            SolidStyleBrush transparentBrush = new SolidStyleBrush(StyleColor.Transparent);

            return RenderPaths(paths, transparentBrush, transparentBrush,
                               transparentBrush, outline, highlightOutline, selectOutline, renderState);
        }

        public override IEnumerable<GdiRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                   RenderState renderState)
        {
            return RenderSymbols(locations, symbolData, symbolData, symbolData, renderState);
        }

        public override IEnumerable<GdiRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                   StyleColorMatrix highlight,
                                                                   StyleColorMatrix select, RenderState renderState)
        {
            Symbol2D highlightSymbol = (Symbol2D)symbolData.Clone();
            highlightSymbol.ColorTransform = highlight;

            Symbol2D selectSymbol = (Symbol2D)symbolData.Clone();
            selectSymbol.ColorTransform = select;

            return RenderSymbols(locations, symbolData, highlightSymbol, selectSymbol, renderState);
        }

        public override IEnumerable<GdiRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbol,
                                                                   Symbol2D highlightSymbol, Symbol2D selectSymbol,
                                                                   RenderState renderState)
        {
            if (renderState == RenderState.Selected) symbol = selectSymbol;
            if (renderState == RenderState.Highlighted) symbol = highlightSymbol;

            foreach (Point2D location in locations)
            {
                Bitmap bitmapSymbol = getSymbol(symbol);
                GdiMatrix transform = ViewConverter.Convert(symbol.AffineTransform);
                GdiColorMatrix colorTransform = ViewConverter.Convert(symbol.ColorTransform);
                RectangleF bounds = new RectangleF(ViewConverter.Convert(location), bitmapSymbol.Size);
                GdiRenderObject holder = new GdiRenderObject(bitmapSymbol, bounds, transform, colorTransform);
                holder.State = renderState;
                yield return holder;
            }
        }
        #endregion

        #region Private helper methods

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

                using (BinaryReader reader = new BinaryReader(new NondisposingStream(symbol2D.SymbolData)))
                {
                    data.Write(reader.ReadBytes((Int32)symbol2D.SymbolData.Length), 0, (Int32)symbol2D.SymbolData.Length);
                }

                symbol = new Bitmap(data);
                _symbolCache[key] = symbol;
            }

            return symbol;
        }

        #endregion

        #region Nested types

        protected struct SymbolLookupKey : IEquatable<SymbolLookupKey>
        {
            public readonly Int32 SymbolId;

            public SymbolLookupKey(Int32 symbolId)
            {
                SymbolId = symbolId;
            }

            #region IEquatable<SymbolLookupKey> Members

            public Boolean Equals(SymbolLookupKey other)
            {
                return other.SymbolId == SymbolId;
            }

            #endregion
        }

        #endregion
    }
}