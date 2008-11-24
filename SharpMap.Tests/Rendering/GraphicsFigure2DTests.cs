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
    #region Figure2D

    
    public class Figure2DTests
    {
        [Fact]
        public void CreateNewTest()
        {
            Point2D[] points =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};
            Figure2D f1 = new Figure2D(points, true);
            Assert.Equal(4, f1.Points.Count);

            for (Int32 i = 0; i < 4; i++)
            {
                Assert.Equal(points[i], f1.Points[i]);
            }
        }

        [Fact]
        public void EqualityTest()
        {
            Point2D[] points1 =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};
            Point2D[] points2 =
                new Point2D[] {new Point2D(0, 0), new Point2D(2, 0), new Point2D(2, 2), new Point2D(0, 2)};
            Point2D[] points3 =
                new Point2D[]
                    {new Point2D(0, 0), new Point2D(2, 0), new Point2D(2, 2), new Point2D(0, 2), new Point2D(0, 2)};

            Figure2D f1 = new Figure2D(points1, true);
            Figure2D f2 = new Figure2D(points1, true);
            Figure2D f3 = new Figure2D(points1);
            Figure2D f4 = new Figure2D(points2, true);
            Figure2D f5 = new Figure2D(points3, true);

            Assert.Equal(f1, f2);
            Assert.NotEqual(f1, f3);
            Assert.NotEqual(f1, f4);
            Assert.NotEqual(f4, f5);
            Assert.False(f1.Equals(new Object()));
            f2 = null;
            Assert.False(f1.Equals(f2));
        }

        [Fact]
        public void AddPointsTest()
        {
            Point2D[] points1 =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};

            Figure2D f1 = new Figure2D(points1, false);

            Assert.Throws<NotSupportedException>(delegate { f1.Points.Add(new Point2D(5, 5)); });
        }

        [Fact]
        public void ToStringTest()
        {
            Point2D[] points1 =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};
            Figure2D f1 = new Figure2D(points1);

            String expected =
                String.Format("[{0}] Number of {1} points: 4; Closed: False", typeof (Figure2D), typeof (Point2D).Name);
            Assert.Equal(expected, f1.ToString());
        }

        [Fact]
        public void CloneTest()
        {
            Point2D[] points1 =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};

            Figure2D f1 = new Figure2D(points1);
            Figure<Point2D, Rectangle2D> f2 = f1.Clone();
            Figure2D f3 = (f1 as ICloneable).Clone() as Figure2D;

            Assert.Equal(f1, f2);
            Assert.Equal(f1, f3);
        }

        [Fact]
        public void EnumTest()
        {
            Point2D[] points1 =
                new Point2D[] {new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)};

            Figure2D f1 = new Figure2D(points1);

            IEnumerator e1 = (f1 as IEnumerable).GetEnumerator();

            Assert.NotNull(e1);
        }
    }

    #endregion
}