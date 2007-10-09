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
    public class GdiLabelRenderer : LabelRenderer2D<GdiRenderObject>
    {
        private readonly Dictionary<FontLookupKey, Font> _fontCache = new Dictionary<FontLookupKey, Font>();

        public GdiLabelRenderer(GdiVectorRenderer vectorRenderer)
            : base(vectorRenderer, StyleTextRenderingHint.SystemDefault)
        {
        }

        public override Size2D MeasureString(string text, StyleFont font)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<GdiRenderObject> RenderLabel(Label2D label)
        {
            return
                RenderLabel(label.Text, label.Font, label.Location, label.CollisionBuffer, label.FlowPath,
                label.Style.Foreground, label.Style.Background, label.Style.Halo, createTransformFromLabel(label));
        }

        public override IEnumerable<GdiRenderObject> RenderLabel(string text, StyleFont font, Point2D location, StyleBrush foreground)
        {
            return RenderLabel(text, font, location, Size2D.Empty, null, foreground, null, null, null);
        }

        public override IEnumerable<GdiRenderObject> RenderLabel(string text, StyleFont font, Point2D location, Size2D collisionBuffer, Path2D flowPath,
                                                    StyleBrush foreground, StyleBrush background, StylePen halo, Matrix2D transform)
        {
            Rectangle2D layoutRectangle = computeLayoutRectangle(location, collisionBuffer, text, font);

            if (halo != null)
            {
                IEnumerable<GdiRenderObject> haloPath = VectorRenderer.RenderPaths(
                    generateHaloPath(layoutRectangle), background, background, background,
                    halo, halo, halo, RenderState.Normal);

                foreach (GdiRenderObject ro in haloPath)
                {
                    yield return ro;
                }
            }

            GdiRenderObject fontPath = VectorRenderer.RenderText(text, font, layoutRectangle);
            yield return new GdiRenderObject(fontPath.GdiPath, ViewConverter.Convert(foreground), 
                null, null, null, null, null, ViewConverter.Convert(halo), null, null);
        }

        private static Matrix2D createTransformFromLabel(Label2D label)
        {
            Matrix2D transform = new Matrix2D();
            transform.Rotate(label.Rotation);
            transform.Translate(label.Offset);
            return transform;
        }

        private static Rectangle2D computeLayoutRectangle(Point2D location, Size2D collisionBuffer, string text, StyleFont font)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<Path2D> generateHaloPath(Rectangle2D layoutRectangle)
        {
            Path2D path = new Path2D(layoutRectangle.GetVertices(), true);
            yield return path;
        }


        private Font getFont(StyleFont styleFont)
        {
            if (styleFont == null)
            {
                return null;
            }

            FontLookupKey key = new FontLookupKey(styleFont.GetHashCode());
            
            Font font;
            _fontCache.TryGetValue(key, out font);

            if (font == null)
            {
                font = ViewConverter.Convert(styleFont);
                _fontCache[key] = font;
            }

            return font;
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
    }
}