using System;
using NPack;

using SharpMap.Rendering.Rendering2D;
using Xunit;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using ITransformMatrixD = NPack.Interfaces.ITransformMatrix<NPack.DoubleComponent>;

namespace SharpMap.Tests.Rendering
{

    #region Rectangle2D


    public class Rectangle2DTests
    {
        [Fact]
        public void Rectangle2DEqualityTests()
        {
            Rectangle2D r1 = new Rectangle2D();
            Rectangle2D r2 = Rectangle2D.Empty;
            Rectangle2D r3 = Rectangle2D.Zero;
            Rectangle2D r4 = new Rectangle2D(0, 0, 0, 0);
            Rectangle2D r5 = new Rectangle2D(9, 10, -5, -6);
            Rectangle2D r6 = new Rectangle2D(0, 0, 10, 10);
            Rectangle2D r7 = new Rectangle2D(new Point2D(0, 0), new Size2D(10, 10));

            Assert.Equal(r1, r2);
            Assert.NotEqual(r1, r3);
            Assert.Equal(r3, r4);
            Assert.NotEqual(r1, r5);
            Assert.NotEqual(r3, r5);
            Assert.Equal(r6, r7);

            IMatrixD v1 = r1;
            IMatrixD v2 = r2;
            IMatrixD v3 = r3;
            IMatrixD v4 = r4;
            IMatrixD v5 = r5;

            Assert.Equal(v1, v2);
            Assert.NotEqual(v1, v3);
            Assert.Equal(v3, v4);
            Assert.NotEqual(v1, v5);
            Assert.NotEqual(v3, v5);

            Assert.Equal(v5, r5);
        }

        [Fact]
        public void IntersectsTest()
        {
            Rectangle2D r1 = Rectangle2D.Empty;
            Rectangle2D r2 = Rectangle2D.Zero;
            Rectangle2D r3 = new Rectangle2D(0, 0, 10, 10);
            Rectangle2D r4 = new Rectangle2D(new Point2D(5, 5), new Size2D(10, 10));

            Assert.False(r1.Intersects(Rectangle2D.Empty));
            Assert.False(r1.Intersects(r2));
            Assert.False(r2.Intersects(r1));
            Assert.True(r2.Intersects(r3));
            Assert.True(r3.Intersects(r4));
            Assert.True(r4.Intersects(r4));
        }

        [Fact]
        public void CompareTest()
        {
            Rectangle2D r1 = Rectangle2D.Empty;
            Rectangle2D r2 = Rectangle2D.Zero;
            Rectangle2D r3 = new Rectangle2D(0, 0, 10, 10);
            Rectangle2D r4 = new Rectangle2D(new Point2D(11, -11), new Size2D(10, 10));
            Rectangle2D r5 = new Rectangle2D(new Point2D(-11, -11), new Size2D(10, 10));
            Rectangle2D r6 = new Rectangle2D(new Point2D(11, 11), new Size2D(10, 10));

            Assert.Equal(0, r1.CompareTo(Rectangle2D.Empty));
            Assert.Equal(-1, r1.CompareTo(r2));
            Assert.Equal(1, r2.CompareTo(r1));
            Assert.Equal(0, r3.CompareTo(r3));
            Assert.Equal(0, r3.CompareTo(r2));
            Assert.Equal(-1, r4.CompareTo(r3));
            Assert.Equal(-1, r5.CompareTo(r3));
            Assert.Equal(1, r6.CompareTo(r3));
        }

        [Fact]
        public void InvertTest()
        {
            Rectangle2D r1 = new Rectangle2D(0, 0, 1, 1);
            Assert.Throws<NotSupportedException>(delegate
                                                     {
                                                         Assert.Null((r1 as IMatrixD).Inverse);
                                                     });
        }

        [Fact]
        public void IsInvertableTest()
        {
            Rectangle2D r1 = new Rectangle2D(0, 0, 1, 1);
            Assert.Throws<NotSupportedException>(delegate
                                                     {
                                                         Assert.True((r1 as IMatrixD).IsInvertible);
                                                     });
        }

        [Fact(Skip = "Elements array access to matrix has been restricted to get " +
                     "read-only matrixes. Test needs to be rewritten.")]
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
            //Assert.Equal(0, (r1 as IMatrixD).ColumnCount);
            Assert.Equal(2, (r1 as IMatrixD).ColumnCount);
            Assert.Equal(2, (r2 as IMatrixD).ColumnCount);
            Assert.Equal(2, (r3 as IMatrixD).ColumnCount);

            DoubleComponent[][] expected =
                new DoubleComponent[][]
                    {
                        new DoubleComponent[] {0, 0}, 
                        new DoubleComponent[] {10, 10}
                    };
            //DoubleComponent[][] actual = (r3 as IMatrixD).Elements;

            IMatrixD r3Matrix = r1;
            Assert.Equal(expected[0][0], r3Matrix[0, 0]);
            Assert.Equal(expected[0][1], r3Matrix[0, 1]);
            Assert.Equal(expected[1][0], r3Matrix[1, 0]);
            Assert.Equal(expected[1][1], r3Matrix[1, 1]);

            //IMatrixD r1Matrix = r1;
            //r1Matrix.Elements = expected;
            //r1 = (Rectangle2D) r1Matrix;
            //Assert.False(r1.IsEmpty);
            //Assert.Equal(r1, r3);
        }

        //[Fact]
        //[ExpectedException(typeof (ArgumentNullException))]
        //public void ElementsTest2()
        //{
        //    Rectangle2D r1 = Rectangle2D.Zero;
        //    (r1 as IMatrixD).Elements = null;
        //}

        //[Fact]
        //[ExpectedException(typeof (ArgumentException))]
        //public void ElementsTest3()
        //{
        //    Rectangle2D r1 = Rectangle2D.Zero;
        //    (r1 as IMatrixD).Elements = new DoubleComponent[][]
        //        {
        //            new DoubleComponent[] {1, 2, 3}, new DoubleComponent[] {2, 3, 4}
        //        };
        //}

        [Fact(Skip = "Incomplete")]
        public void MultiplyTest()
        {
        }

        [Fact]
        public void ScaleTest1()
        {
            Rectangle2D r1 = Rectangle2D.Empty;
            Rectangle2D r2 = Rectangle2D.Zero;
            Rectangle2D r3 = new Rectangle2D(0, 0, 1, 1);

            r1.Scale(10);
            Assert.True(r1.IsEmpty);

            r2.Scale(10);
            Assert.Equal(0, r2.Width);
            Assert.Equal(0, r2.Height);

            r3.Scale(10);
            Assert.Equal(10, r3.Width);
            Assert.Equal(10, r3.Height);
            Assert.Equal(Point2D.Zero, r3.Location);

            Size2D scaleSize = new Size2D(-1, 5);

            r1.Scale(scaleSize);
            Assert.True(r1.IsEmpty);

            r2.Scale(scaleSize);
            Assert.Equal(0, r2.Width);
            Assert.Equal(0, r2.Height);

            r3.Scale(scaleSize);
            Assert.Equal(-10, r3.Width);
            Assert.Equal(50, r3.Height);
            Assert.Equal(Point2D.Zero, r3.Location);
        }

        [Fact(Skip = "Rendering3D moved...")]
        //[ExpectedException(typeof(ArgumentException))]
        public void ScaleTest2()
        {
            //Size3D scaleSize = new Size3D(10, 10, 10);
            //Rectangle2D r2 = Rectangle2D.Zero;
            //r2.Scale(scaleSize);
        }

        [Fact]
        public void TranslateTest1()
        {
            Rectangle2D r1 = Rectangle2D.Empty;
            Rectangle2D r2 = Rectangle2D.Zero;

            r1.Translate(10);
            Assert.True(r1.IsEmpty);
            Assert.Equal(0, r1.X);
            Assert.Equal(0, r1.Y);
            Assert.Equal(0, r1.Width);
            Assert.Equal(0, r1.Height);

            r2.Translate(new Point2D(3, 5));
            Assert.Equal(r2.X, 3);
            Assert.Equal(r2.Y, 5);
        }

        [Fact(Skip = "Rendering3D moved...")]
        //[ExpectedException(typeof(ArgumentException))]
        public void TranslateTest2()
        {
            //Rectangle2D r1 = Rectangle2D.Zero;
            //r1.Translate(new Point3D(3, 4, 5));
        }

        [Fact]
        public void TransformTest1()
        {
            Rectangle2D rect = new Rectangle2D(9, 10, -5, -6);
            Assert.Throws<NotSupportedException>(delegate
                 {

                     IVectorD val = (rect as ITransformMatrixD).TransformVector((IVectorD) Point2D.Zero);
                 });
        }

        [Fact]
        public void TransformTest2()
        {
            Rectangle2D rect = new Rectangle2D(9, 10, -5, -6);

            Assert.Throws<NotSupportedException>(delegate
                                                     {

                                                         (rect as ITransformMatrixD).TransformVector(
                                                             new DoubleComponent[] {1, 4});
                                                     });
        }
    }

    #endregion
}