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
using System.Text;
using GdiPoint = System.Drawing.Point;
using GdiMatrix = System.Drawing.Drawing2D.Matrix;
using GdiFont = System.Drawing.Font;
using GdiBrushes = System.Drawing.Brushes;
using GdiGraphicsPath = System.Drawing.Drawing2D.GraphicsPath;

using SharpMap.Data;
using SharpMap.Styles;
using SharpMap.Presentation;
using SharpMap.Geometries;

namespace SharpMap.Rendering.Gdi
{
    public class GdiLabelRenderer : BaseLabelRenderer2D<GdiRenderObject>
    {
        private Graphics _currentGraphics;

        public GdiLabelRenderer()
            : base(StyleTextRenderingHint.SystemDefault)
        {
        }

        public override ViewSize2D MeasureString(string text, StyleFont font)
        {
            Graphics g = _currentGraphics;

            if (g == null)
            {
                return new ViewSize2D(0, 0);
            }

            using (GdiFont gdiFont = new GdiFont(font.FontFamily.Name, (float)font.Size.Height, ViewConverter.ViewToGdi(font.Style), GraphicsUnit.Pixel))
            {
                return ViewConverter.GdiToView(g.MeasureString(text, gdiFont));
            }
        }

        public override GdiRenderObject RenderLabel(Label label)
        {
            return RenderLabel(label.Text, label.LabelPoint, label.Style.Offset, label.Font, label.Style.ForeColor, label.Style.BackColor, label.Style.Halo, label.Rotation);
        }

        public override GdiRenderObject RenderLabel(string text, ViewPoint2D location, StyleFont font, StyleColor foreColor)
        {
            return RenderLabel(text, location, new ViewPoint2D(0, 0), font, foreColor, null, null, 0);
        }

        public override GdiRenderObject RenderLabel(string text, ViewPoint2D location, ViewPoint2D offset, StyleFont font, StyleColor foreColor, StyleBrush backColor, StylePen halo, float rotation)
        {
            Graphics g = _currentGraphics;

            if (rotation != 0 && rotation != float.NaN)
            {
                GdiMatrix originalTransform = g.Transform;
                g.TranslateTransform((float)location.X, (float)location.Y);
                g.RotateTransform(rotation);
                g.TranslateTransform((float)-font.Size.Width / 2, (float)-font.Size.Height / 2);

                using (Brush backBrush = ViewConverter.ViewToGdi(backColor))
                {
                    if (backColor != null && backBrush != GdiBrushes.Transparent)
                    {
                        g.FillRectangle(backBrush, 0, 0, (float)font.Size.Width * 0.74f + 1f, (float)font.Size.Height * 0.74f);
                    }
                }

                GdiGraphicsPath path = new GdiGraphicsPath();

                path.AddString(text, ViewConverter.ViewToGdi(font.FontFamily), (int)font.Style, (float)font.Size.Height, new GdiPoint(0, 0), null);

                using (Pen haloPen = ViewConverter.ViewToGdi(halo))
                {
                    if (haloPen != null)
                    {
                        g.DrawPath(haloPen, path);
                    }
                }

                using (SolidBrush foreBrush = new SolidBrush(ViewConverter.ViewToGdi(foreColor)))
                {
                    g.FillPath(foreBrush, path);
                }

                //g.Transform = ViewConverter.ViewToGdi(ViewTransformer.MapTransform as ViewMatrix2D);
            }
            else
            {
                using (Brush backBrush = ViewConverter.ViewToGdi(backColor))
                {
                    if (backBrush != null && backBrush != GdiBrushes.Transparent)
                    {
                        g.FillRectangle(backBrush, (float)location.X, (float)location.Y, (float)font.Size.Width * 0.74f + 1, (float)font.Size.Height * 0.74f);
                    }
                }

                GdiGraphicsPath path = new GdiGraphicsPath();

                PointF labelPoint = ViewConverter.ViewToGdi(location);
                path.AddString(text, ViewConverter.ViewToGdi(font.FontFamily), (int)font.Style, (float)font.Size.Height, labelPoint, null);

                using (Pen haloPen = ViewConverter.ViewToGdi(halo))
                {
                    if (haloPen != null)
                    {
                        g.DrawPath(haloPen, path);
                    }
                }

                using (SolidBrush foreBrush = new SolidBrush(ViewConverter.ViewToGdi(foreColor)))
                {
                    g.FillPath(foreBrush, path);
                }
            }

            throw new NotImplementedException();
        }

        protected override IEnumerable<PositionedRenderObject2D<GdiRenderObject>> DoRenderGeometry(IGeometry geometry, LabelStyle style)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
