using System;

using SharpMap.Rendering.Rendering2D;
using Xunit;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using ITransformMatrixD = NPack.Interfaces.ITransformMatrix<NPack.DoubleComponent>;

namespace SharpMap.Tests.Rendering
{

    #region Size2D


    public class Size2DTests
    {
        [Fact]
        public void Size2DEqualityTests()
        {
            Size2D s1 = new Size2D();
            Size2D s2 = Size2D.Empty;
            Size2D s3 = Size2D.Zero;
            Size2D s4 = new Size2D(0, 0);
            Size2D s5 = new Size2D(9, 10);

            Assert.Equal(s1, s2);
            Assert.NotEqual(s1, s3);
            Assert.Equal(s3, s4);
            Assert.NotEqual(s1, s5);
            Assert.NotEqual(s3, s5);

            IVectorD v1 = s1;
            IVectorD v2 = s2;
            IVectorD v3 = s3;
            IVectorD v4 = s4;
            IVectorD v5 = s5;

            Assert.Equal(v1, v2);
            Assert.NotEqual(v1, v3);
            Assert.Equal(v3, v4);
            Assert.NotEqual(v1, v5);
            Assert.NotEqual(v3, v5);

            Assert.Equal(v5, s5);
        }

        [Fact]
        public void IVectorDTests1()
        {
            IVectorD s1 = Point2D.Empty;
            IVectorD s2 = Point2D.Zero;
            IVectorD s3 = new Point2D(9, 10);

            Assert.Equal(0, s1.ComponentCount);
            Assert.Equal(2, s2.ComponentCount);
            Assert.Equal<Double>(9, (Double)s3[0], EpsilonComparer.Default);
            Assert.Equal<Double>(10, (Double)s3[1], EpsilonComparer.Default);
        }

        [Fact]
        public void IVectorDTests2()
        {
            IVectorD s1 = new Point2D(9, 10);
            Assert.Throws<ArgumentOutOfRangeException>(delegate
                                   {
                                       Assert.Equal<Double>(10, (Double)s1[2], EpsilonComparer.Default);
                                   });
        }

        [Fact]
        public void CloneTest()
        {
            Point2D s1 = new Point2D(1.1, 2.2);
            Point2D s2 = s1.Clone();

            Assert.Equal(s1, s2);
            Assert.NotSame(s1, s2);
        }

        [Fact]
        public void IEnumerableTest()
        {
            Point2D s1 = new Point2D(1.1, 2.2);

            Int32 index = 0;
            foreach (Double component in s1)
            {
                Assert.Equal(s1[index++], component);
            }
        }
    }

    #endregion
}