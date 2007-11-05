// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using GdiPoint = System.Drawing.Point;
using GdiMatrix = System.Drawing.Drawing2D.Matrix;
using GdiFont = System.Drawing.Font;
using GdiBrushes = System.Drawing.Brushes;
using GdiPath = System.Drawing.Drawing2D.GraphicsPath;

namespace SharpMap.Rendering.Gdi
{
    public class GdiTextRenderer : TextRenderer2D<GdiRenderObject>
    {
        private readonly Graphics _internalGraphics;

        public GdiTextRenderer()
            : this(StyleTextRenderingHint.SystemDefault) { }

        public GdiTextRenderer(StyleTextRenderingHint textRenderingHint)
            : base(textRenderingHint)
        {
            Bitmap bitmap = new Bitmap(1, 1);
            _internalGraphics = Graphics.FromImage(bitmap);   
        }

        protected override void Dispose(bool disposing)
        {
            if(IsDisposed)
            {
                return;
            }

            if(_internalGraphics != null)
            {
                _internalGraphics.Dispose();
            }

            base.Dispose(disposing);
        }

        public override Size2D MeasureString(string text, StyleFont font)
        {
            GdiFont gdiFont = ViewConverter.Convert(font);
            SizeF gdiSize = _internalGraphics.MeasureString(
                text, gdiFont, Int32.MaxValue, StringFormat.GenericTypographic);
            return ViewConverter.Convert(gdiSize);
        }

        public override IEnumerable<GdiRenderObject> RenderText(string text, StyleFont font, Rectangle2D layoutRectangle, 
            Path2D flowPath, StyleBrush fontBrush, Matrix2D transform)
        {
            GdiRenderObject renderedText = new GdiRenderObject(
                text, ViewConverter.Convert(font), ViewConverter.Convert(layoutRectangle),
                ViewConverter.Convert(fontBrush), null, null, null, null, null);

            yield return renderedText;
        }
    }
}