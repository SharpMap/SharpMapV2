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
using System.Text;

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

using SharpMap.Styles;
using SharpMap.Presentation;
using StyleColorMatrix = SharpMap.Rendering.ColorMatrix;

namespace SharpMap.Rendering.Gdi
{
    /// <summary>
    /// A <see cref="VectorRenderer{GdiRenderObject}"/> which renders to GDI primatives.
    /// </summary>
    public class GdiVectorRenderer : VectorRenderer2D<GdiRenderObject>
    {
        private Dictionary<BrushLookupKey, Brush> _brushCache = new Dictionary<BrushLookupKey,Brush>();
        private Dictionary<PenLookupKey, Pen> _penCache = new Dictionary<PenLookupKey, Pen>();
        private Dictionary<SymbolLookupKey, Bitmap> _symbolCache = new Dictionary<SymbolLookupKey, Bitmap>();

        public GdiVectorRenderer()
        {
        }

        protected override void Dispose(bool disposing)
        {
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
            }

            base.Dispose(disposing);            
        }

        public override GdiRenderObject RenderPath(GraphicsPath2D viewPath, StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill, StylePen outline, StylePen highlightOutline, StylePen selectOutline)
        {
            int pointCount = viewPath.Points.Count;

            GdiGraphicsPath gdiPath = new GdiGraphicsPath();

            foreach (GraphicsFigure2D figure in viewPath.Figures)
            {
                gdiPath.AddLines(ViewConverter.ViewToGdi(figure.Points));

                if (figure.IsClosed)
                {
                    gdiPath.CloseFigure();
                }
            }

            GdiRenderObject holder = new GdiRenderObject(gdiPath, getBrush(fill), getBrush(highlightFill), getBrush(selectFill), getPen(outline), getPen(highlightOutline), getPen(selectOutline));
            return holder;
        }

        public override GdiRenderObject RenderPath(GraphicsPath2D path, StylePen outline, StylePen highlightOutline, StylePen selectOutline)
        {
            SolidStyleBrush transparentBrush = new SolidStyleBrush(StyleColor.Transparent);
            return RenderPath(path, transparentBrush, transparentBrush, transparentBrush, outline, highlightOutline, selectOutline);
        }

        public override GdiRenderObject RenderSymbol(ViewPoint2D location, Symbol2D symbolData)
        {
            return RenderSymbol(location, symbolData, symbolData, symbolData);
        }

        public override GdiRenderObject RenderSymbol(ViewPoint2D location, Symbol2D symbolData, StyleColorMatrix highlight, StyleColorMatrix select)
        {
            Symbol2D highlightSymbol = symbolData.Clone();
            highlightSymbol.ColorTransform = highlight;

            Symbol2D selectSymbol = symbolData.Clone();
            selectSymbol.ColorTransform = select;

            return RenderSymbol(location, symbolData, highlightSymbol, selectSymbol);
        }

        public override GdiRenderObject RenderSymbol(ViewPoint2D location, Symbol2D symbol, Symbol2D highlightSymbol, Symbol2D selectSymbol)
        {
            if (highlightSymbol == null)
            {
                highlightSymbol = symbol;
            }

            if (selectSymbol == null)
            {
                selectSymbol = symbol;
            }

            Bitmap bitmapSymbol = getSymbol(symbol);
            Matrix transform = ViewConverter.ViewToGdi(symbol.AffineTransform);
            GdiColorMatrix colorTransform = ViewConverter.ViewToGdi(symbol.ColorTransform);
            GdiRenderObject holder = new GdiRenderObject(bitmapSymbol, transform, colorTransform);
            return holder;

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
                brush = ViewConverter.ViewToGdi(styleBrush);
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
                pen = ViewConverter.ViewToGdi(stylePen);
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
            Bitmap symbol = null;
            _symbolCache.TryGetValue(key, out symbol);

            if (symbol == null)
            {
                System.IO.MemoryStream data = new System.IO.MemoryStream();
                symbol2D.SymbolData.Position = 0;

                using (System.IO.BinaryReader reader = new System.IO.BinaryReader(symbol2D.SymbolData))
                {
                    data.Write(reader.ReadBytes((int)symbol2D.SymbolData.Length), 0, (int)symbol2D.SymbolData.Length);
                }

                symbol = new Bitmap(data);
                _symbolCache[key] = symbol;
            }

            return symbol;
        }

        private struct BrushLookupKey
        {
            public RuntimeTypeHandle StyleBrushType;
            public string StyleBrushValue;

            public BrushLookupKey(RuntimeTypeHandle type, string styleBrushValue)
            {
                StyleBrushType = type;
                StyleBrushValue = styleBrushValue;
            }
        }

        private struct PenLookupKey
        {
            public RuntimeTypeHandle StylePenType;
            public string StylePenValue;

            public PenLookupKey(RuntimeTypeHandle type, string stylePenValue)
            {
                StylePenType = type;
                StylePenValue = stylePenValue;
            }
        }

        private struct SymbolLookupKey
        {
            public string SymbolValue;

            public SymbolLookupKey(string symbolValue)
            {
                SymbolValue = symbolValue;
            }
        }
    }
}
