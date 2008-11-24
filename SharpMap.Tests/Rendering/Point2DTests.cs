using System;

using SharpMap.Rendering.Rendering2D;
using Xunit;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using ITransformMatrixD = NPack.Interfaces.ITransformMatrix<NPack.DoubleComponent>;

namespace SharpMap.Tests.Rendering
{

    #region Point2D


    public class Point2DTests
    {
        [Fact]
        public void Point2DEqualityTests()
        {
            Point2D p1 = new Point2D();
            Point2D p2 = Point2D.Empty;
            Point2D p3 = Point2D.Zero;
            Point2D p4 = new Point2D(0, 0);
            Point2D p5 = new Point2D(9, 10);

            Assert.Equal(p1, p2);
            Assert.NotEqual(p1, p3);
            Assert.Equal(p3, p4);
            Assert.NotEqual(p1, p5);
            Assert.NotEqual(p3, p5);

            IVectorD v1 = p1;
            IVectorD v2 = p2;
            IVectorD v3 = p3;
            IVectorD v4 = p4;
            IVectorD v5 = p5;

            Assert.Equal(v1, v2);
            Assert.NotEqual(v1, v3);
            Assert.Equal(v3, v4);
            Assert.NotEqual(v1, v5);
            Assert.NotEqual(v3, v5);

            Assert.Equal(v5, p5);
        }

        [Fact]
        public void IVectorDTests1()
        {
            IVectorD p1 = Point2D.Empty;
            IVectorD p2 = Point2D.Zero;
            IVectorD p3 = new Point2D(9, 10);

            Assert.Equal(0, p1.ComponentCount);
            Assert.Equal(2, p2.ComponentCount);
            Assert.Equal<Double>(9, (Double)p3[0], EpsilonComparer.Default);
            Assert.Equal<Double>(10, (Double)p3[1], EpsilonComparer.Default);
        }

        [Fact]
        public void IVectorDTests2()
        {
            Point2D p1 = new Point2D(9, 10);
            Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                Assert.Equal<Double>(10, (Double)p1[2], EpsilonComparer.Default);
            });
        }

        [Fact]
        public void CloneTest()
        {
            Point2D p1 = new Point2D(1.1, 2.2);
            Point2D p2 = p1.Clone();

            Assert.Equal(p1, p2);
            Assert.NotSame(p1, p2);
        }

        [Fact]
        public void IEnumerableTest()
        {
            Point2D p1 = new Point2D(1.1, 2.2);

            Int32 index = 0;
            foreach (Double component in p1)
            {
                Assert.Equal(p1[index++], component);
            }
        }
    }

    #endregion
}