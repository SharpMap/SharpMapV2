using System;
using NPack;
using NPack.Interfaces;

using SharpMap.Rendering;
using Xunit;

namespace SharpMap.Tests.Rendering
{
    #region ColorMatrix
    public class ColorMatrixTests
    {
        [Fact]
        public void ResetTest()
        {
            ColorMatrix m1 = new ColorMatrix(1, 1, 1, 1, 0, 0, 0, 0);

            m1.Reset();

            Assert.Equal(m1, ColorMatrix.Identity);
        }

        [Fact]
        public void InvertTest()
        {
            ColorMatrix m1 = new ColorMatrix(0.5, 0.5, 0.5, 0.5, 10, 20, 30, 0);
            IMatrix<DoubleComponent> expected =
                new Matrix(MatrixFormat.RowMajor, new DoubleComponent[][]
                                               {
                                                   new DoubleComponent[] {2, 0, 0, 0, 0},
                                                   new DoubleComponent[] {0, 2, 0, 0, 0},
                                                   new DoubleComponent[] {0, 0, 2, 0, 0},
                                                   new DoubleComponent[] {0, 0, 0, 2, 0},
                                                   new DoubleComponent[] {-20, -40, -60, 0, 1}
                                               });

            IMatrix<DoubleComponent> m1Inverse = m1.Inverse;

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
            ColorMatrix m1 = new ColorMatrix(1, 1, 1, 1, 0, 0, 0, 0);
            Assert.True(m1.IsInvertible);
        }

        [Fact]
        public void ElementsTest1()
        {
            ColorMatrix m1 = ColorMatrix.Identity;
            ColorMatrix m2 = new ColorMatrix(0.5, 0.5, 0.5, 1, 0, 0, 0, 0);

            Assert.Equal(5, m1.RowCount);
            Assert.Equal(5, m2.ColumnCount);

            DoubleComponent[][] expected = new DoubleComponent[][]
                {
                    new DoubleComponent[] {0.5, 0, 0, 0, 0},
                    new DoubleComponent[] {0, 0.5, 0, 0, 0},
                    new DoubleComponent[] {0, 0, 0.5, 0, 0},
                    new DoubleComponent[] {0, 0, 0, 1, 0},
                    new DoubleComponent[] {0, 0, 0, 0, 1}
                };

            //DoubleComponent[][] actual = m2.Elements;

            Assert.Equal(expected[0][0], m2[0, 0]);
            Assert.Equal(expected[0][1], m2[0, 1]);
            Assert.Equal(expected[0][2], m2[0, 2]);
            Assert.Equal(expected[1][0], m2[1, 0]);
            Assert.Equal(expected[1][1], m2[1, 1]);
            Assert.Equal(expected[1][2], m2[1, 2]);
            Assert.Equal(expected[2][0], m2[2, 0]);
            Assert.Equal(expected[2][1], m2[2, 1]);
            Assert.Equal(expected[2][2], m2[2, 2]);

            //m1.Elements = expected;
            m1 = new ColorMatrix(new Matrix(MatrixFormat.RowMajor, expected));
            Assert.Equal(m1, m2);
            Assert.True(m1.Equals(m2 as IMatrix<DoubleComponent>));
        }

        //[Fact]
        //[ExpectedException(typeof (ArgumentNullException))]
        //public void ElementsTest2()
        //{
        //    ColorMatrix m1 = ColorMatrix.Identity;
        //    m1.Elements = null;
        //}

        //[Fact]
        //[ExpectedException(typeof (ArgumentException))]
        //public void ElementsTest3()
        //{
        //    ColorMatrix m1 = ColorMatrix.Identity;
        //    m1.Elements = new DoubleComponent[][]
        //        {
        //            new DoubleComponent[] {1, 2, 3},
        //            new DoubleComponent[] {2, 3, 4}
        //        };
        //}

        [Fact(Skip = "Incomplete")]
        public void MultiplyTest()
        {
            ColorMatrix m1 = ColorMatrix.Identity;
        }

        [Fact(Skip = "Incomplete")]
        public void ScaleTest1()
        {
            ColorMatrix m1 = new ColorMatrix(0, 0, 0, 0, 0, 0, 0, 0);
            ColorMatrix m2 = ColorMatrix.Identity;
        }

        [Fact(Skip = "Incomplete")]
        public void ScaleTest2()
        {
            ColorMatrix m1 = ColorMatrix.Identity;

            // Scale by a vector for which multiplication isn't defined...
        }

        [Fact(Skip = "Incomplete")]
        public void TranslateTest1()
        {
            ColorMatrix m1 = ColorMatrix.Identity;
        }

        [Fact(Skip = "Incomplete")]
        public void TranslateTest2()
        {
            ColorMatrix m1 = ColorMatrix.Identity;
            // Scale by a vector for which multiplicatio isn't defined...
        }

        [Fact(Skip = "Incomplete")]
        public void TransformTest1()
        {
            ColorMatrix m1 = ColorMatrix.Identity;
        }

        [Fact(Skip = "Incomplete")]
        public void Transform2Test2()
        {
            ColorMatrix m1 = ColorMatrix.Identity;
            // Scale by a vector for which multiplicatio isn't defined...
        }

        [Fact]
        public void ConstructorInitializedPropertiesTest()
        {
            ColorMatrix m1 = new ColorMatrix(2, 3, 4, 5, 6, 7, 8, 9);

            Assert.Equal(2, m1.R);
            Assert.Equal(3, m1.G);
            Assert.Equal(4, m1.B);
            Assert.Equal(5, m1.A);
            Assert.Equal(6, m1.RedShift);
            Assert.Equal(7, m1.GreenShift);
            Assert.Equal(8, m1.BlueShift);
            Assert.Equal(9, m1.AlphaShift);
        }

    }

    #endregion
}