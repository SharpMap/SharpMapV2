using System;
using NPack;

using SharpMap.Rendering.Rendering2D;
using Xunit;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using ITransformMatrixD = NPack.Interfaces.ITransformMatrix<NPack.DoubleComponent>;

namespace SharpMap.Tests.Rendering
{

    #region Matrix2D


    public class Matrix2DTests
    {
        [Fact]
        public void IdentityTest()
        {
            Matrix2D m1 = Matrix2D.Identity;
            m1[0, 0] = 3;
            Assert.NotEqual(Matrix2D.Identity, m1);
        }

        [Fact]
        public void ResetTest()
        {
            Matrix2D m1 = new Matrix2D(1, 1, 0, 1, 1, 0);

            m1.Reset();

            Assert.Equal(Matrix2D.Identity, m1);
        }

        [Fact]
        public void InvertTest()
        {
            Matrix2D m1 = new Matrix2D(1, 2, 3, 4, 0, 0);
            Matrix2D expected = new Matrix2D(-2, 1, 1.5, -0.5, 0, 0);

            IMatrixD m1Inverse = m1.Inverse;

            for (Int32 i = 0; i < m1.RowCount; i++)
            {
                for (Int32 j = 0; j < m1.ColumnCount; j++)
                {
                    Assert.Equal<Double>((Double)expected[i, j], (Double)m1Inverse[i, j], EpsilonComparer.Default);
                }
            }
        }

        [Fact]
        public void IsInvertableTest()
        {
            Matrix2D m1 = new Matrix2D(1, 1, 1, 1, 0, 0);
            Matrix2D m2 = new Matrix2D(1, 2, 3, 4, 0, 0);
            Assert.False(m1.IsInvertible);
            Assert.True(m2.IsInvertible);
        }

        [Fact]
        public void ElementsTest1()
        {
            Matrix2D m1 = Matrix2D.Identity;
            Matrix2D m2 = new Matrix2D(0, 0, 0, 0, 0, 0);
            Matrix2D m3 = new Matrix2D(1, 2, 4, 5, 3, 6);

            Assert.Equal(3, m1.RowCount);
            Assert.Equal(3, m2.ColumnCount);

            DoubleComponent[][] expected = new DoubleComponent[][]
                {
                    new DoubleComponent[] {1, 4, 0},
                    new DoubleComponent[] {2, 5, 0},
                    new DoubleComponent[] {3, 6, 1}
                };

            Assert.Equal(expected[0][0], m3[0, 0]);
            Assert.Equal(expected[0][1], m3[0, 1]);
            Assert.Equal(expected[0][2], m3[0, 2]);
            Assert.Equal(expected[1][0], m3[1, 0]);
            Assert.Equal(expected[1][1], m3[1, 1]);
            Assert.Equal(expected[1][2], m3[1, 2]);
            Assert.Equal(expected[2][0], m3[2, 0]);
            Assert.Equal(expected[2][1], m3[2, 1]);
            Assert.Equal(expected[2][2], m3[2, 2]);

            //m1.Elements = expected;
            //Assert.Equal(m1, m3);
            //Assert.True(m1.Equals(m3 as IMatrixD));
        }

        //[Fact]
        //[ExpectedException(typeof (ArgumentNullException))]
        //public void ElementsTest2()
        //{
        //    Matrix2D m1 = Matrix2D.Identity;
        //    m1.Elements = null;
        //}

        //[Fact]
        //[ExpectedException(typeof (ArgumentException))]
        //public void ElementsTest3()
        //{
        //    Matrix2D m1 = Matrix2D.Identity;
        //    m1.Elements = new DoubleComponent[][] {new DoubleComponent[] {1, 2, 3}, new DoubleComponent[] {2, 3, 4}};
        //}

        [Fact(Skip = "Incomplete")]
        public void RotateTest()
        {
            Matrix2D m1 = Matrix2D.Identity;
        }

        [Fact(Skip = "Incomplete")]
        public void RotateAtTest()
        {
            Matrix2D m1 = Matrix2D.Identity;
        }

        [Fact(Skip = "Incomplete")]
        public void MultiplyTest()
        {
            Matrix2D m1 = Matrix2D.Identity;
        }

        [Fact(Skip = "Incomplete")]
        public void ScaleTest1()
        {
            Matrix2D m1 = new Matrix2D(0, 0, 0, 0, 0, 0);
            Matrix2D m2 = Matrix2D.Identity;

            m1.Scale(10);
            Assert.Equal(new Matrix2D(0, 0, 0, 0, 0, 0), m1);

            m2.Scale(10);
            Assert.Equal(10, m2.M11);
            Assert.Equal(10, m2.M22);

            Size2D scaleSize = new Size2D(-1, 5);

            m1.Scale(scaleSize);
            Assert.Equal(new Matrix2D(0, 0, 0, 0, 0, 0), m1);

            m2.Scale(scaleSize);
            Assert.Equal(-10, m2.M11);
            Assert.Equal(50, m2.M22);
        }

        [Fact(Skip = "Incomplete")]
        public void ScaleTest2()
        {
            Matrix2D m1 = Matrix2D.Identity;

            // Scale by a vector for which multiplicatio isn't defined...
        }

        [Fact(Skip = "Incomplete")]
        public void TranslateTest1()
        {
            Matrix2D m1 = Matrix2D.Identity;
        }

        [Fact(Skip = "Incomplete")]
        public void TranslateTest2()
        {
            Matrix2D m1 = Matrix2D.Identity;
            // Scale by a vector for which multiplicatio isn't defined...
        }

        [Fact(Skip = "Incomplete")]
        public void TransformTest1()
        {
            Matrix2D m1 = Matrix2D.Identity;
        }

        [Fact(Skip = "Incomplete")]
        public void Transform2Test2()
        {
            Matrix2D m1 = Matrix2D.Identity;
            // Scale by a vector for which multiplicatio isn't defined...
        }
    }

    #endregion
}