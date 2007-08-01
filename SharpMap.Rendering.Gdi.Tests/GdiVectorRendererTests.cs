using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SharpMap.Rendering.Gdi.Tests
{
    [TestFixture]
    public class GdiVectorRendererTests
    {
        private const double _e = 0.0005;

        [Test]
        public void CreateGdiVectorRendererTest()
        {
            GdiVectorRenderer renderer = new GdiVectorRenderer();
            Assert.AreEqual(new Matrix2D(), renderer.RenderTransform);
            Assert.AreEqual(StyleRenderingMode.Default, renderer.StyleRenderingMode);
            renderer.Dispose();
        }

        [Test]
        public void RenderPathOutlineTest()
        {
            GdiVectorRenderer renderer = new GdiVectorRenderer();
            Point2D[] points = new Point2D[] 
                { new Point2D(1, 0), new Point2D(0, 1), new Point2D(-1, 0), new Point2D(0, -1) };
            GraphicsPath2D path = new GraphicsPath2D(points, true);

            StylePen outline = new StylePen(new SolidStyleBrush(StyleColor.Blue), 1);
            StylePen highlight = new StylePen(new SolidStyleBrush(StyleColor.Red), 1);
            StylePen selected = new StylePen(new SolidStyleBrush(StyleColor.Green), 1);

            PositionedRenderObject2D<GdiRenderObject> ro = renderer.RenderPath(path, outline, highlight, selected);

            Assert.AreEqual(0, ro.X);
            Assert.AreEqual(0, ro.Y);
            Assert.AreEqual(GdiRenderObjectState.Normal, ro.RenderObject.State);
            Assert.AreEqual(GdiRenderObjectType.Path, ro.RenderObject.Type);
            Assert.IsInstanceOfType(typeof(SolidBrush), ro.RenderObject.Fill);
            Assert.IsNotNull(ro.RenderObject.GdiPath);
            Assert.AreEqual(4, ro.RenderObject.GdiPath.PointCount);
            Assert.AreEqual(new RectangleF(-1, -1, 2, 2), ro.RenderObject.GdiPath.GetBounds());

            PathData data = ro.RenderObject.GdiPath.PathData;

            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(points[i].X, data.Points[i].X, _e);
                Assert.AreEqual(points[i].Y, data.Points[i].Y, _e);
            }

            Assert.AreEqual(0, data.Types[0]);
            Assert.AreEqual(1, data.Types[1]);
            Assert.AreEqual(1, data.Types[2]);
            Assert.AreEqual(129, data.Types[3]);

            Pen expectedOutline = new Pen(Brushes.Blue, 1.0f);
            Pen expectedHighlight = new Pen(Brushes.Red, 1.0f);
            Pen expectedSelected = new Pen(Brushes.Green, 1.0f);

            Assert.IsTrue(pensAreEqual(expectedOutline, ro.RenderObject.Outline));
            Assert.IsTrue(pensAreEqual(expectedHighlight, ro.RenderObject.HightlightOutline));
            Assert.IsTrue(pensAreEqual(expectedSelected, ro.RenderObject.SelectOutline));

            expectedOutline.Dispose();
            expectedHighlight.Dispose();
            expectedSelected.Dispose();
            renderer.Dispose();
        }

        private static bool pensAreEqual(Pen p1, Pen p2)
        {
            if (ReferenceEquals(p1, p2))
            {
                return true;
            }

            if (p1 == null || p2 == null)
            {
                return false;
            }

            if (p1.Alignment != p2.Alignment) return false;
            if (!brushesAreEqual(p1.Brush, p2.Brush)) return false;
            if (p1.Color != p2.Color) return false;
            if (!elementsAreEqual(p1.CompoundArray, p2.CompoundArray)) return false;
            if (p1.DashStyle == DashStyle.Custom && p1.CustomEndCap != p2.CustomEndCap) return false;
            if (p1.DashStyle == DashStyle.Custom && p1.CustomStartCap != p2.CustomStartCap) return false;
            if (p1.DashCap != p2.DashCap) return false;
            if (p1.DashOffset != p2.DashOffset) return false;
            if (p1.DashStyle == DashStyle.Custom && !elementsAreEqual(p1.DashPattern, p2.DashPattern)) return false;
            if (p1.DashStyle != p2.DashStyle) return false;
            if (p1.EndCap != p2.EndCap) return false;
            if (p1.LineJoin != p2.LineJoin) return false;
            if (p1.MiterLimit != p2.MiterLimit) return false;
            if (p1.PenType != p2.PenType) return false;
            if (p1.StartCap != p2.StartCap) return false;
            if (!elementsAreEqual(p1.Transform.Elements, p2.Transform.Elements)) return false;
            if (p1.Width != p2.Width) return false;

            return true;
        }

        private static bool elementsAreEqual(float[] lhs, float[] rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (lhs == null || rhs == null)
            {
                return false;
            }

            if (lhs.Length != rhs.Length)
            {
                return false;
            }

            for (int i = 0; i < lhs.Length; i++)
            {
                if (lhs[i] != rhs[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool brushesAreEqual(Brush b1, Brush b2)
        {
            if (ReferenceEquals(b1, b2))
            {
                return true;
            }

            if (b1 == null || b2 == null)
            {
                return false;
            }

            if (b1 is SolidBrush && b2 is SolidBrush)
            {
                return solidBrushesAreEqual(b1 as SolidBrush, b2 as SolidBrush);
            }
            else if (b1 is LinearGradientBrush && b2 is LinearGradientBrush)
            {
                throw new NotSupportedException("Test not currently implemented");
            }
            else if (b1 is PathGradientBrush && b2 is PathGradientBrush)
            {
                throw new NotSupportedException("Test not currently implemented");
            }
            else if (b1 is HatchBrush && b2 is HatchBrush)
            {
                throw new NotSupportedException("Test not currently implemented");
            }

            return false;
        }

        private static bool solidBrushesAreEqual(SolidBrush b1, SolidBrush b2)
        {
            return b1.Color == b2.Color;
        }
    }
}
