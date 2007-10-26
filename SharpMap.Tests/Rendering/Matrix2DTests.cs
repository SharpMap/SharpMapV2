using System;
using NPack;
using NUnit.Framework;
using SharpMap.Rendering.Rendering2D;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using ITransformMatrixD = NPack.Interfaces.ITransformMatrix<NPack.DoubleComponent>;

namespace SharpMap.Tests.Rendering
{

    #region Matrix2D

    [TestFixture]
    public class Matrix2DTests
    {
        [Test]
        public void IdentityTest()
        {
            Matrix2D m1 = Matrix2D.Identity;
            m1[0, 0] = 3;
            Assert.AreNotEqual(Matrix2D.Identity, m1);
        }

        [Test]
        public void ResetTest()
        {
            Matrix2D m1 = new Matrix2D(1, 1, 0, 1, 1, 0);

            m1.Reset();

            Assert.AreEqual(Matrix2D.Identity, m1);
        }

        [Test]
        public void InvertTest()
        {
            Matrix2D m1 = new Matrix2D(1, 2, 3, 4, 0, 0);
            Matrix2D expected = new Matrix2D(-2, 1, 1.5, -0.5, 0, 0);

            IMatrixD m1Inverse = m1.Inverse;

            for (Int32 i = 0; i < m1.RowCount; i++)
            {
                for (Int32 j = 0; j < m1.ColumnCount; j++)
                {
                    Assert.AreEqual((Double)expected[i, j], (Double)m1Inverse[i, j], TestConstants.Epsilon);
                }
            }
        }

        [Test]
        public void IsInvertableTest()
        {
            Matrix2D m1 = new Matrix2D(1, 1, 1, 1, 0, 0);
            Matrix2D m2 = new Matrix2D(1, 2, 3, 4, 0, 0);
            Assert.IsFalse(m1.IsInvertible);
            Assert.IsTrue(m2.IsInvertible);
        }

        [Test]
        public void ElementsTest1()
        {
            Matrix2D m1 = Matrix2D.Identity;
            Matrix2D m2 = new Matrix2D(0, 0, 0, 0, 0, 0);
            Matrix2D m3 = new Matrix2D(1, 2, 4, 5, 3, 6);

            Assert.AreEqual(3, m1.RowCount);
            Assert.AreEqual(3, m2.ColumnCount);

            DoubleComponent[][] expected = new DoubleComponent[][]
                {
                    new DoubleComponent[] {1, 4, 0},
                    new DoubleComponent[] {2, 5, 0},
                    new DoubleComponent[] {3, 6, 1}
                };
            DoubleComponent[][] actual = m3.Elements;

            Assert.AreEqual(expected[0][0], actual[0][0]);
            Assert.AreEqual(expected[0][1], actual[0][1]);
            Assert.AreEqual(expected[0][2], actual[0][2]);
            Assert.AreEqual(expected[1][0], actual[1][0]);
            Assert.AreEqual(expected[1][1], actual[1][1]);
            Assert.AreEqual(expected[1][2], actual[1][2]);
            Assert.AreEqual(expected[2][0], actual[2][0]);
            Assert.AreEqual(expected[2][1], actual[2][1]);
            Assert.AreEqual(expected[2][2], actual[2][2]);

            m1.Elements = expected;
            Assert.AreEqual(m1, m3);
            Assert.IsTrue(m1.Equals(m3 as IMatrixD));
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ElementsTest2()
        {
            Matrix2D m1 = Matrix2D.Identity;
            m1.Elements = null;
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ElementsTest3()
        {
            Matrix2D m1 = Matrix2D.Identity;
            m1.Elements = new DoubleComponent[][] {new DoubleComponent[] {1, 2, 3}, new DoubleComponent[] {2, 3, 4}};
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void RotateTest()
        {
            Matrix2D m1 = Matrix2D.Identity;
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void RotateAtTest()
        {
            Matrix2D m1 = Matrix2D.Identity;
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void MultiplyTest()
        {
            Matrix2D m1 = Matrix2D.Identity;
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void ScaleTest1()
        {
            Matrix2D m1 = new Matrix2D(0, 0, 0, 0, 0, 0);
            Matrix2D m2 = Matrix2D.Identity;

            m1.Scale(10);
            Assert.AreEqual(new Matrix2D(0, 0, 0, 0, 0, 0), m1);

            m2.Scale(10);
            Assert.AreEqual(10, m2.M11);
            Assert.AreEqual(10, m2.M22);

            Size2D scaleSize = new Size2D(-1, 5);

            m1.Scale(scaleSize);
            Assert.AreEqual(new Matrix2D(0, 0, 0, 0, 0, 0), m1);

            m2.Scale(scaleSize);
            Assert.AreEqual(-10, m2.M11);
            Assert.AreEqual(50, m2.M22);
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void ScaleTest2()
        {
            Matrix2D m1 = Matrix2D.Identity;

            // Scale by a vector for which multiplicatio isn't defined...
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void TranslateTest1()
        {
            Matrix2D m1 = Matrix2D.Identity;
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void TranslateTest2()
        {
            Matrix2D m1 = Matrix2D.Identity;
            // Scale by a vector for which multiplicatio isn't defined...
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void TransformTest1()
        {
            Matrix2D m1 = Matrix2D.Identity;
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void Transform2Test2()
        {
            Matrix2D m1 = Matrix2D.Identity;
            // Scale by a vector for which multiplicatio isn't defined...
        }
    }

    #endregion
}