using System;
using NUnit.Framework;
using SharpMap.Rendering.Rendering2D;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using ITransformMatrixD = NPack.Interfaces.ITransformMatrix<NPack.DoubleComponent>;

namespace SharpMap.Tests.Rendering
{

    #region Point2D

    [TestFixture]
    public class Point2DTests
    {
        [Test]
        public void Point2DEqualityTests()
        {
            Point2D p1 = new Point2D();
            Point2D p2 = Point2D.Empty;
            Point2D p3 = Point2D.Zero;
            Point2D p4 = new Point2D(0, 0);
            Point2D p5 = new Point2D(9, 10);

            Assert.AreEqual(p1, p2);
            Assert.AreNotEqual(p1, p3);
            Assert.AreEqual(p3, p4);
            Assert.AreNotEqual(p1, p5);
            Assert.AreNotEqual(p3, p5);

            IVectorD v1 = p1;
            IVectorD v2 = p2;
            IVectorD v3 = p3;
            IVectorD v4 = p4;
            IVectorD v5 = p5;

            Assert.AreEqual(v1, v2);
            Assert.AreNotEqual(v1, v3);
            Assert.AreEqual(v3, v4);
            Assert.AreNotEqual(v1, v5);
            Assert.AreNotEqual(v3, v5);

            Assert.AreEqual(v5, p5);
        }

        [Test]
        public void IVectorDTests1()
        {
            IVectorD p1 = Point2D.Empty;
            IVectorD p2 = Point2D.Zero;
            IVectorD p3 = new Point2D(9, 10);

            Assert.AreEqual(0, p1.ComponentCount);
            Assert.AreEqual(2, p2.ComponentCount);
            Assert.AreEqual(9, (double) p3[0], TestConstants.Epsilon);
            Assert.AreEqual(10, (double) p3[1], TestConstants.Epsilon);
        }

        [Test]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void IVectorDTests2()
        {
            Point2D p1 = new Point2D(9, 10);

            Assert.AreEqual(10, (double) p1[2], TestConstants.Epsilon);
        }

        [Test]
        public void CloneTest()
        {
            Point2D p1 = new Point2D(1.1, 2.2);
            Point2D p2 = p1.Clone();

            Assert.AreEqual(p1, p2);
            Assert.AreNotSame(p1, p2);
        }

        [Test]
        public void IEnumerableTest()
        {
            Point2D p1 = new Point2D(1.1, 2.2);

            int index = 0;
            foreach (double component in p1)
            {
                Assert.AreEqual(p1[index++], component);
            }
        }
    }

    #endregion
}