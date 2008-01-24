using System;
using NPack;
using NPack.Interfaces;
using NUnit.Framework;
using SharpMap.Rendering;

namespace SharpMap.Tests.Rendering
{

    #region ColorMatrix

    [TestFixture]
    public class ColorMatrixTests
    {
        private static readonly Double _e = 0.0005;

        [Test]
        public void ResetTest()
        {
            ColorMatrix m1 = new ColorMatrix(1, 1, 1, 1, 0, 0, 0, 0);

            m1.Reset();

            Assert.AreEqual(m1, ColorMatrix.Identity);
        }

        [Test]
        public void InvertTest()
        {
			ColorMatrix m1 = new ColorMatrix(0.5, 0.5, 0.5, 0.5, 10, 20, 30, 0);
            IMatrix<DoubleComponent> expected =
                new Matrix<DoubleComponent>(MatrixFormat.RowMajor, new DoubleComponent[][]
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
                    Assert.AreEqual((Double) expected[i, j], (Double) m1Inverse[i, j], _e);
                }
            }
        }

        [Test]
        public void IsInvertableTest()
        {
			ColorMatrix m1 = new ColorMatrix(1, 1, 1, 1, 0, 0, 0, 0);
            Assert.IsTrue(m1.IsInvertible);
        }

        [Test]
        public void ElementsTest1()
        {
            ColorMatrix m1 = ColorMatrix.Identity;
			ColorMatrix m2 = new ColorMatrix(0.5, 0.5, 0.5, 1, 0, 0, 0, 0);

            Assert.AreEqual(5, m1.RowCount);
            Assert.AreEqual(5, m2.ColumnCount);

            DoubleComponent[][] expected = new DoubleComponent[][]
                {
                    new DoubleComponent[] {0.5, 0, 0, 0, 0},
                    new DoubleComponent[] {0, 0.5, 0, 0, 0},
                    new DoubleComponent[] {0, 0, 0.5, 0, 0},
                    new DoubleComponent[] {0, 0, 0, 1, 0},
                    new DoubleComponent[] {0, 0, 0, 0, 1}
                };

            //DoubleComponent[][] actual = m2.Elements;

            Assert.AreEqual(expected[0][0], m2[0, 0]);
            Assert.AreEqual(expected[0][1], m2[0, 1]);
            Assert.AreEqual(expected[0][2], m2[0, 2]);
            Assert.AreEqual(expected[1][0], m2[1, 0]);
            Assert.AreEqual(expected[1][1], m2[1, 1]);
            Assert.AreEqual(expected[1][2], m2[1, 2]);
            Assert.AreEqual(expected[2][0], m2[2, 0]);
            Assert.AreEqual(expected[2][1], m2[2, 1]);
            Assert.AreEqual(expected[2][2], m2[2, 2]);

            //m1.Elements = expected;
            m1 = new ColorMatrix(new Matrix<DoubleComponent>(MatrixFormat.RowMajor, expected));
            Assert.AreEqual(m1, m2);
            Assert.IsTrue(m1.Equals(m2 as IMatrix<DoubleComponent>));
        }

        //[Test]
        //[ExpectedException(typeof (ArgumentNullException))]
        //public void ElementsTest2()
        //{
        //    ColorMatrix m1 = ColorMatrix.Identity;
        //    m1.Elements = null;
        //}

        //[Test]
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

        [Test]
        [Ignore("Test not yet implemented")]
        public void MultiplyTest()
        {
            ColorMatrix m1 = ColorMatrix.Identity;
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void ScaleTest1()
        {
			ColorMatrix m1 = new ColorMatrix(0, 0, 0, 0, 0, 0, 0, 0);
            ColorMatrix m2 = ColorMatrix.Identity;
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void ScaleTest2()
        {
            ColorMatrix m1 = ColorMatrix.Identity;

            // Scale by a vector for which multiplication isn't defined...
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void TranslateTest1()
        {
            ColorMatrix m1 = ColorMatrix.Identity;
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void TranslateTest2()
        {
            ColorMatrix m1 = ColorMatrix.Identity;
            // Scale by a vector for which multiplicatio isn't defined...
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void TransformTest1()
        {
            ColorMatrix m1 = ColorMatrix.Identity;
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void Transform2Test2()
        {
            ColorMatrix m1 = ColorMatrix.Identity;
            // Scale by a vector for which multiplicatio isn't defined...
        }

		[Test]
		public void ConstructorInitializedPropertiesTest()
		{
			ColorMatrix m1 = new ColorMatrix(2, 3, 4, 5, 6, 7, 8, 9);

			Assert.AreEqual(2, m1.R);
			Assert.AreEqual(3, m1.G);
			Assert.AreEqual(4, m1.B);
			Assert.AreEqual(5, m1.A);
			Assert.AreEqual(6, m1.RedShift);
			Assert.AreEqual(7, m1.GreenShift);
			Assert.AreEqual(8, m1.BlueShift);
			Assert.AreEqual(9, m1.AlphaShift);
		}

	}

	#endregion
}