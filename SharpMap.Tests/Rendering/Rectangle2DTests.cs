using System;
using NPack;
using NUnit.Framework;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Rendering.Rendering3D;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using ITransformMatrixD = NPack.Interfaces.ITransformMatrix<NPack.DoubleComponent>;

namespace SharpMap.Tests.Rendering
{

    #region Rectangle2D

    [TestFixture]
    public class Rectangle2DTests
    {
        [Test]
        public void Rectangle2DEqualityTests()
        {
            Rectangle2D r1 = new Rectangle2D();
            Rectangle2D r2 = Rectangle2D.Empty;
            Rectangle2D r3 = Rectangle2D.Zero;
            Rectangle2D r4 = new Rectangle2D(0, 0, 0, 0);
            Rectangle2D r5 = new Rectangle2D(9, 10, -5, -6);
            Rectangle2D r6 = new Rectangle2D(0, 0, 10, 10);
            Rectangle2D r7 = new Rectangle2D(new Point2D(0, 0), new Size2D(10, 10));

            Assert.AreEqual(r1, r2);
            Assert.AreNotEqual(r1, r3);
            Assert.AreEqual(r3, r4);
            Assert.AreNotEqual(r1, r5);
            Assert.AreNotEqual(r3, r5);
            Assert.AreEqual(r6, r7);

            IMatrixD v1 = r1;
            IMatrixD v2 = r2;
            IMatrixD v3 = r3;
            IMatrixD v4 = r4;
            IMatrixD v5 = r5;

            Assert.AreEqual(v1, v2);
            Assert.AreNotEqual(v1, v3);
            Assert.AreEqual(v3, v4);
            Assert.AreNotEqual(v1, v5);
            Assert.AreNotEqual(v3, v5);

            Assert.AreEqual(v5, r5);
        }

        [Test]
        public void IntersectsTest()
        {
            Rectangle2D r1 = Rectangle2D.Empty;
            Rectangle2D r2 = Rectangle2D.Zero;
            Rectangle2D r3 = new Rectangle2D(0, 0, 10, 10);
            Rectangle2D r4 = new Rectangle2D(new Point2D(5, 5), new Size2D(10, 10));

            Assert.IsFalse(r1.Intersects(Rectangle2D.Empty));
            Assert.IsFalse(r1.Intersects(r2));
            Assert.IsFalse(r2.Intersects(r1));
            Assert.IsTrue(r2.Intersects(r3));
            Assert.IsTrue(r3.Intersects(r4));
            Assert.IsTrue(r4.Intersects(r4));
        }

        [Test]
        public void CompareTest()
        {
            Rectangle2D r1 = Rectangle2D.Empty;
            Rectangle2D r2 = Rectangle2D.Zero;
            Rectangle2D r3 = new Rectangle2D(0, 0, 10, 10);
            Rectangle2D r4 = new Rectangle2D(new Point2D(11, -11), new Size2D(10, 10));
            Rectangle2D r5 = new Rectangle2D(new Point2D(-11, -11), new Size2D(10, 10));
            Rectangle2D r6 = new Rectangle2D(new Point2D(11, 11), new Size2D(10, 10));

            Assert.AreEqual(0, r1.CompareTo(Rectangle2D.Empty));
            Assert.AreEqual(-1, r1.CompareTo(r2));
            Assert.AreEqual(1, r2.CompareTo(r1));
            Assert.AreEqual(0, r3.CompareTo(r3));
            Assert.AreEqual(0, r3.CompareTo(r2));
            Assert.AreEqual(-1, r4.CompareTo(r3));
            Assert.AreEqual(-1, r5.CompareTo(r3));
            Assert.AreEqual(1, r6.CompareTo(r3));
        }

        [Test]
        [ExpectedException(typeof (NotSupportedException))]
        public void InvertTest()
        {
            Rectangle2D r1 = new Rectangle2D(0, 0, 1, 1);
            Assert.IsNull((r1 as IMatrixD).Inverse);
        }

        [Test]
        [ExpectedException(typeof (NotSupportedException))]
        public void IsInvertableTest()
        {
            Rectangle2D r1 = new Rectangle2D(0, 0, 1, 1);
            Assert.IsTrue((r1 as IMatrixD).IsInvertible);
        }

        [Test]
        public void ElementsTest1()
        {
            Rectangle2D r1 = Rectangle2D.Empty;
            Rectangle2D r2 = Rectangle2D.Zero;
            Rectangle2D r3 = new Rectangle2D(0, 0, 10, 10);

            // 2007-09-27 - codekaizen - 
            // The column count of an empty Rectangle2D used to be
            // 0, indicating that it was empty, but this prevented
            // operations which wanted to manage the Rectangle2D as 
            // an array of arrays of points from property sizing them.
            //Assert.AreEqual(0, (r1 as IMatrixD).ColumnCount);
            Assert.AreEqual(2, (r1 as IMatrixD).ColumnCount);
            Assert.AreEqual(2, (r2 as IMatrixD).ColumnCount);
            Assert.AreEqual(2, (r3 as IMatrixD).ColumnCount);

            DoubleComponent[][] expected =
                new DoubleComponent[][] {new DoubleComponent[] {0, 0}, new DoubleComponent[] {10, 10}};
            //DoubleComponent[][] actual = (r3 as IMatrixD).Elements;

            IMatrixD r3Matrix = r1;
            Assert.AreEqual(expected[0][0], r3Matrix[0, 0]);
            Assert.AreEqual(expected[0][1], r3Matrix[0, 1]);
            Assert.AreEqual(expected[1][0], r3Matrix[1, 0]);
            Assert.AreEqual(expected[1][1], r3Matrix[1, 1]);

            //IMatrixD r1Matrix = r1;
            //r1Matrix.Elements = expected;
            //r1 = (Rectangle2D) r1Matrix;
            //Assert.IsFalse(r1.IsEmpty);
            //Assert.AreEqual(r1, r3);
        }

        //[Test]
        //[ExpectedException(typeof (ArgumentNullException))]
        //public void ElementsTest2()
        //{
        //    Rectangle2D r1 = Rectangle2D.Zero;
        //    (r1 as IMatrixD).Elements = null;
        //}

        //[Test]
        //[ExpectedException(typeof (ArgumentException))]
        //public void ElementsTest3()
        //{
        //    Rectangle2D r1 = Rectangle2D.Zero;
        //    (r1 as IMatrixD).Elements = new DoubleComponent[][]
        //        {
        //            new DoubleComponent[] {1, 2, 3}, new DoubleComponent[] {2, 3, 4}
        //        };
        //}

        [Test]
        [Ignore("Not yet implemented")]
        public void MultiplyTest()
        {
        }

        [Test]
        public void ScaleTest1()
        {
            Rectangle2D r1 = Rectangle2D.Empty;
            Rectangle2D r2 = Rectangle2D.Zero;
            Rectangle2D r3 = new Rectangle2D(0, 0, 1, 1);

            r1.Scale(10);
            Assert.IsTrue(r1.IsEmpty);

            r2.Scale(10);
            Assert.AreEqual(0, r2.Width);
            Assert.AreEqual(0, r2.Height);

            r3.Scale(10);
            Assert.AreEqual(10, r3.Width);
            Assert.AreEqual(10, r3.Height);
            Assert.AreEqual(Point2D.Zero, r3.Location);

            Size2D scaleSize = new Size2D(-1, 5);

            r1.Scale(scaleSize);
            Assert.IsTrue(r1.IsEmpty);

            r2.Scale(scaleSize);
            Assert.AreEqual(0, r2.Width);
            Assert.AreEqual(0, r2.Height);

            r3.Scale(scaleSize);
            Assert.AreEqual(-10, r3.Width);
            Assert.AreEqual(50, r3.Height);
            Assert.AreEqual(Point2D.Zero, r3.Location);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void ScaleTest2()
        {
            Size3D scaleSize = new Size3D(10, 10, 10);
            Rectangle2D r2 = Rectangle2D.Zero;
            r2.Scale(scaleSize);
        }

        [Test]
        public void TranslateTest1()
        {
            Rectangle2D r1 = Rectangle2D.Empty;
            Rectangle2D r2 = Rectangle2D.Zero;

            r1.Translate(10);
            Assert.IsTrue(r1.IsEmpty);
            Assert.AreEqual(0, r1.X);
            Assert.AreEqual(0, r1.Y);
            Assert.AreEqual(0, r1.Width);
            Assert.AreEqual(0, r1.Height);

            r2.Translate(new Point2D(3, 5));
            Assert.AreEqual(r2.X, 3);
            Assert.AreEqual(r2.Y, 5);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void TranslateTest2()
        {
            Rectangle2D r1 = Rectangle2D.Zero;
            r1.Translate(new Point3D(3, 4, 5));
        }

        [Test]
        [ExpectedException(typeof (NotSupportedException))]
        public void TransformTest1()
        {
            Rectangle2D rect = new Rectangle2D(9, 10, -5, -6);
            IVectorD val = (rect as ITransformMatrixD).TransformVector((IVectorD) Point2D.Zero);
        }

        [Test]
        [ExpectedException(typeof (NotSupportedException))]
        public void TransformTest2()
        {
            Rectangle2D rect = new Rectangle2D(9, 10, -5, -6);
            (rect as ITransformMatrixD).TransformVector(new DoubleComponent[] {1, 4});
        }
    }

    #endregion
}