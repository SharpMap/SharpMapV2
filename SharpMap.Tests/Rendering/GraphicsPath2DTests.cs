using System;
using System.Collections;

using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using Xunit;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using ITransformMatrixD = NPack.Interfaces.ITransformMatrix<NPack.DoubleComponent>;

namespace SharpMap.Tests.Rendering
{
    #region Path2D

    
    public class Path2DTests
    {
        [Fact]
        public void CreateNewTest()
        {
            Path2D p1 = new Path2D();
            Assert.Equal(0, p1.Figures.Count);

            Point2D[] points =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};
            Path2D p2 = new Path2D(points);
            Assert.Equal(1, p2.Figures.Count);
            Assert.Equal(4, p2.CurrentFigure.Points.Count);
            Assert.False(p2.CurrentFigure.IsClosed);

            Path2D p3 = new Path2D(points, true);
            Assert.True(p3.CurrentFigure.IsClosed);

            Figure2D figure = new Figure2D(points, true);
            Path2D p4 = new Path2D(new Figure2D[] {figure});
            Assert.Equal(1, p4.Figures.Count);

            Path2D p5 = new Path2D(new Figure2D[] {});
            Assert.Null(p5.CurrentFigure);
            Assert.Equal(0, p5.Points.Count);
        }

        [Fact]
        public void AddFiguresTest()
        {
            Point2D[] points =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};

            Path2D p1 = new Path2D();
            p1.NewFigure(points, false);

            Assert.Equal(1, p1.Figures.Count);
            Assert.NotNull(p1.CurrentFigure);
            Assert.Equal(4, p1.CurrentFigure.Points.Count);
            Assert.False(p1.CurrentFigure.IsClosed);
            Assert.Equal(p1.Bounds, p1.CurrentFigure.Bounds);
            Assert.Equal(p1.Bounds, new Rectangle2D(0, 0, 1, 1));
        }

        [Fact]
        public void CurrentFigureTest1()
        {
            Point2D[] points =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};

            Figure2D f1 = new Figure2D(points, true);
            Path2D p1 = new Path2D(f1);

            f1 = p1.CurrentFigure as Figure2D;
            Assert.Equal(4, p1.Points.Count);

            p1.NewFigure(points, false);
            Assert.Same(p1.CurrentFigure, p1.Figures[1]);

            p1.CurrentFigure = f1;
            Assert.Same(p1.CurrentFigure, f1);

            Path2D p2 = new Path2D();
            Assert.Null(p2.CurrentFigure);
            Assert.Equal(0, p2.Points.Count);
        }

        [Fact]
        public void CurrentFigureTest2()
        {
            Point2D[] points =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};

            Figure2D f1 = new Figure2D(points, true);
            Path2D p1 = new Path2D();
            p1.NewFigure(points, false);

            Assert.Throws<InvalidOperationException>(delegate { p1.CurrentFigure = f1; });
        }

        [Fact]
        public void BoundsTest()
        {
            Path2D p1 = new Path2D();

            Assert.True(p1.Bounds == Rectangle2D.Empty);

            Point2D[] points1 =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};
            Point2D[] points2 =
                new Point2D[] {new Point2D(0, 0), new Point2D(10, 0), new Point2D(10, 1), new Point2D(0, 1)};
            Point2D[] points3 =
                new Point2D[] {new Point2D(0, 0), new Point2D(-1, 0), new Point2D(-1, -1), new Point2D(0, -1)};

            p1.NewFigure(points1, true);
            Assert.Equal(p1.Bounds, new Rectangle2D(0, 0, 1, 1));

            p1.NewFigure(points2, true);
            Assert.Equal(p1.Bounds, new Rectangle2D(0, 0, 10, 1));

            p1.NewFigure(points3, true);
            Assert.Equal(p1.Bounds, new Rectangle2D(-1, -1, 10, 1));
        }

        [Fact]
        public void CloneTest()
        {
            Point2D[] points1 =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};
            Path2D p1 = new Path2D(points1, true);

            Path<Point2D, Rectangle2D> p2 = p1.Clone();
            Assert.Equal(p1, p2);

            Path2D p3 = (p1 as ICloneable).Clone() as Path2D;
            Assert.Equal(p1, p3);

            p2.NewFigure(points1, false);
            Assert.NotEqual(p1, p2);
        }

        [Fact]
        public void EqualityTest()
        {
            Path2D p1 = new Path2D();
            Path2D p2 = new Path2D();
            Assert.True(p1.Equals(p2));

            Point2D[] points1 =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};
            Point2D[] points2 =
                new Point2D[] {new Point2D(0, 0), new Point2D(2, 0), new Point2D(2, 2), new Point2D(0, 2)};

            Path2D p3 = new Path2D(points1);
            Path2D p4 = new Path2D(points1);
            Assert.True(p3.Equals(p4));

            Path2D p5 = new Path2D(p3.CurrentFigure);
            Assert.True(p3.Equals(p5));

            Path2D p6 = new Path2D(points1, true);
            Assert.False(p3.Equals(p6));

            Figure2D f1 = new Figure2D(points1, false);
            Path2D p7 = new Path2D(new Figure2D[] {f1});
            Assert.True(p3.Equals(p7));

            Figure2D f2 = new Figure2D(points2, true);
            Path2D p8 = new Path2D(new Figure2D[] {f1, f2});
            Assert.False(p3.Equals(p8));

            p3.NewFigure(points2, true);
            Assert.True(p8.Equals(p3));

            p8 = null;
            Assert.False(p3.Equals(p8));
            Assert.False(p3.Equals(new Object()));
        }

        [Fact]
        public void ToStringTest()
        {
            Point2D[] points1 =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};
            Path2D p1 = new Path2D(points1);

            Rectangle2D bounds = new Rectangle2D(Point2D.Zero, Size2D.Unit);
            String expected = String.Format("[{0}] 1 figure of Point2D points; Bounds: {1}", typeof (Path2D), bounds);

            Assert.Equal(expected, p1.ToString());
        }

        [Fact]
        public void GetHashCodeTest()
        {
            Point2D[] points1 =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};
            Path2D p1 = new Path2D();
            Path2D p2 = new Path2D();
            Path2D p3 = new Path2D(points1);

            Figure2D f1 = new Figure2D(points1, true);
            Path2D p4 = new Path2D(points1, true);
            Path2D p5 = new Path2D(f1);

            Assert.Equal(p1.GetHashCode(), p2.GetHashCode());
            Assert.Equal(p4.GetHashCode(), p5.GetHashCode());
            Assert.NotEqual(p1.GetHashCode(), p3.GetHashCode());
            Assert.NotEqual(p3.GetHashCode(), p4.GetHashCode());
        }

        [Fact]
        public void EnumPointsTest()
        {
            Point2D[] points1 =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};
            Path2D p1 = new Path2D(points1);

            Int32 i = 0;
            foreach (Figure2D figure in p1)
            {
                foreach (Point2D point in figure)
                {
                    Assert.Equal(points1[i++], point);
                }
            }

            IEnumerator e1 = p1.GetEnumerator();
            IEnumerator e2 = (p1 as IEnumerable).GetEnumerator();

            Assert.True(e1.MoveNext());
            Assert.True(e2.MoveNext());

            Assert.True(e2.Current.Equals(e1.Current));

            Assert.False(e1.MoveNext());
            Assert.False(e2.MoveNext());
        }
    }

    #endregion
}