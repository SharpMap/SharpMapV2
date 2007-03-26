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

namespace SharpMap.Rendering.Gdi
{
    public class GdiLabelRenderer : LabelRenderer<GdiRenderObject>
    {
        private Graphics _currentGraphics;

        public GdiLabelRenderer(IViewTransformer<ViewPoint2D, ViewRectangle2D> transformer)
        {
            ViewTransformer = transformer;
        }

        protected override IEnumerable<PositionedRenderObject2D<GdiRenderObject>> DoRenderFeature(FeatureDataRow feature, IRenderContext renderContext)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override ViewSize2D MeasureString(string text, StyleFont font)
        {
            Graphics g = _currentGraphics;

            if (g == null)
                return new ViewSize2D(0, 0);

            using (GdiFont gdiFont = new GdiFont(font.FontFamily.Name, (float)font.Size.Height, ViewConverter.ViewToGdi(font.Style), GraphicsUnit.Pixel))
                return ViewConverter.GdiToView(g.MeasureString(text, gdiFont));
        }

        public override void DrawLabel(Label label)
        {
            DrawLabel(label.Text, label.LabelPoint, label.Style.Offset, label.Font, label.Style.ForeColor, label.Style.BackColor, label.Style.Halo, label.Rotation);
        }

        public override void DrawLabel(string text, ViewPoint2D location, StyleFont font, StyleColor foreColor)
        {
            DrawLabel(text, location, new ViewPoint2D(0, 0), font, foreColor, null, null, 0);
        }

        public override void DrawLabel(string text, ViewPoint2D location, ViewPoint2D offset, StyleFont font, StyleColor foreColor, StyleBrush backColor, StylePen halo, float rotation)
        {
            Graphics g = _currentGraphics;

            if (g == null)
                return;

            if (rotation != 0 && rotation != float.NaN)
            {
                GdiMatrix originalTransform = g.Transform;
                g.TranslateTransform((float)location.X, (float)location.Y);
                g.RotateTransform(rotation);
                g.TranslateTransform((float)-font.Size.Width / 2, (float)-font.Size.Height / 2);

                using (Brush backBrush = ViewConverter.ViewToGdi(backColor))
                {
                    if (backColor != null && backBrush != GdiBrushes.Transparent)
                        g.FillRectangle(backBrush, 0, 0, (float)font.Size.Width * 0.74f + 1f, (float)font.Size.Height * 0.74f);
                }

                GdiGraphicsPath path = new GdiGraphicsPath();

                path.AddString(text, ViewConverter.ViewToGdi(font.FontFamily), (int)font.Style, (float)font.Size.Height, new GdiPoint(0, 0), null);

                using (Pen haloPen = ViewConverter.ViewToGdi(halo))
                {
                    if (haloPen != null)
                        g.DrawPath(haloPen, path);
                }

                using (SolidBrush foreBrush = new SolidBrush(ViewConverter.ViewToGdi(foreColor)))
                    g.FillPath(foreBrush, path);

                //g.Transform = ViewConverter.ViewToGdi(ViewTransformer.MapTransform as ViewMatrix2D);
            }
            else
            {
                using (Brush backBrush = ViewConverter.ViewToGdi(backColor))
                {
                    if (backBrush != null && backBrush != GdiBrushes.Transparent)
                        g.FillRectangle(backBrush, (float)location.X, (float)location.Y, (float)font.Size.Width * 0.74f + 1, (float)font.Size.Height * 0.74f);
                }

                GdiGraphicsPath path = new GdiGraphicsPath();

                PointF labelPoint = ViewConverter.ViewToGdi(location);
                path.AddString(text, ViewConverter.ViewToGdi(font.FontFamily), (int)font.Style, (float)font.Size.Height, labelPoint, null);

                using (Pen haloPen = ViewConverter.ViewToGdi(halo))
                {
                    if (haloPen != null)
                        g.DrawPath(haloPen, path);
                }

                using (SolidBrush foreBrush = new SolidBrush(ViewConverter.ViewToGdi(foreColor)))
                    g.FillPath(foreBrush, path);
            }
        }
    }
}
