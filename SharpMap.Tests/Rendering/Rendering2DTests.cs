using System;
using System.Collections;
using NPack.Interfaces;
using NUnit.Framework;

using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Rendering.Rendering3D;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using ITransformMatrixD = NPack.Interfaces.ITransformMatrix<NPack.DoubleComponent>;
using NPack;

namespace SharpMap.Tests.Rendering
{
    #region ViewPoint2D
    [TestFixture]
    public class ViewPoint2DTests
    {
        private static readonly double _e = 0.0005;

        [Test]
        public void ViewPoint2DEqualityTests()
        {
            ViewPoint2D p1 = new ViewPoint2D();
            ViewPoint2D p2 = ViewPoint2D.Empty;
            ViewPoint2D p3 = ViewPoint2D.Zero;
            ViewPoint2D p4 = new ViewPoint2D(0, 0);
            ViewPoint2D p5 = new ViewPoint2D(9, 10);

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
            IVectorD p1 = ViewPoint2D.Empty;
            IVectorD p2 = ViewPoint2D.Zero;
            IVectorD p3 = new ViewPoint2D(9, 10);

            Assert.AreEqual(0, p1.ComponentCount);
            Assert.AreEqual(2, p2.ComponentCount);
            Assert.AreEqual(9, (double)p3[0], _e);
            Assert.AreEqual(10, (double)p3[1], _e);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IVectorDTests2()
        {
            ViewPoint2D p1 = new ViewPoint2D(9, 10);

            Assert.AreEqual(10, (double)p1[2], _e);
        }

        [Test]
        public void CloneTest()
        {
            ViewPoint2D p1 = new ViewPoint2D(1.1, 2.2);
            ViewPoint2D p2 = p1.Clone();

            Assert.AreEqual(p1, p2);
            Assert.AreNotSame(p1, p2);
        }

        [Test]
        public void IEnumerableTest()
        {
            ViewPoint2D p1 = new ViewPoint2D(1.1, 2.2);

            int index = 0;
            foreach (double component in p1)
            {
                Assert.AreEqual(p1[index++], component);
            }
        }
    }
    #endregion

    #region ViewSize2D
    [TestFixture]
    public class ViewSize2DTests
    {
        private static readonly double _e = 0.0005;

        [Test]
        public void ViewSize2DEqualityTests()
        {
            ViewSize2D s1 = new ViewSize2D();
            ViewSize2D s2 = ViewSize2D.Empty;
            ViewSize2D s3 = ViewSize2D.Zero;
            ViewSize2D s4 = new ViewSize2D(0, 0);
            ViewSize2D s5 = new ViewSize2D(9, 10);

            Assert.AreEqual(s1, s2);
            Assert.AreNotEqual(s1, s3);
            Assert.AreEqual(s3, s4);
            Assert.AreNotEqual(s1, s5);
            Assert.AreNotEqual(s3, s5);

            IVectorD v1 = s1;
            IVectorD v2 = s2;
            IVectorD v3 = s3;
            IVectorD v4 = s4;
            IVectorD v5 = s5;

            Assert.AreEqual(v1, v2);
            Assert.AreNotEqual(v1, v3);
            Assert.AreEqual(v3, v4);
            Assert.AreNotEqual(v1, v5);
            Assert.AreNotEqual(v3, v5);

            Assert.AreEqual(v5, s5);
        }

        [Test]
        public void IVectorDTests1()
        {
            IVectorD s1 = ViewPoint2D.Empty;
            IVectorD s2 = ViewPoint2D.Zero;
            IVectorD s3 = new ViewPoint2D(9, 10);

            Assert.AreEqual(0, s1.ComponentCount);
            Assert.AreEqual(2, s2.ComponentCount);
            Assert.AreEqual(9, (double)s3[0], _e);
            Assert.AreEqual(10, (double)s3[1], _e);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IVectorDTests2()
        {
            IVectorD s1 = new ViewPoint2D(9, 10);

            Assert.AreEqual(10, (double)s1[2], _e);
        }

        [Test]
        public void CloneTest()
        {
            ViewPoint2D s1 = new ViewPoint2D(1.1, 2.2);
            ViewPoint2D s2 = s1.Clone();

            Assert.AreEqual(s1, s2);
            Assert.AreNotSame(s1, s2);
        }

        [Test]
        public void IEnumerableTest()
        {
            ViewPoint2D s1 = new ViewPoint2D(1.1, 2.2);

            int index = 0;
            foreach (double component in s1)
            {
                Assert.AreEqual(s1[index++], component);
            }
        }
    }
    #endregion

    #region ViewRectangle2D
    [TestFixture]
    public class ViewRectangle2DTests
    {
        private static readonly double _e = 0.0005;

        [Test]
        public void ViewRectangle2DEqualityTests()
        {
            ViewRectangle2D r1 = new ViewRectangle2D();
            ViewRectangle2D r2 = ViewRectangle2D.Empty;
            ViewRectangle2D r3 = ViewRectangle2D.Zero;
            ViewRectangle2D r4 = new ViewRectangle2D(0, 0, 0, 0);
            ViewRectangle2D r5 = new ViewRectangle2D(9, 10, -5, -6);
            ViewRectangle2D r6 = new ViewRectangle2D(0, 10, 0, 10);
            ViewRectangle2D r7 = new ViewRectangle2D(new ViewPoint2D(0, 0), new ViewSize2D(10, 10));

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
            ViewRectangle2D r1 = ViewRectangle2D.Empty;
            ViewRectangle2D r2 = ViewRectangle2D.Zero;
            ViewRectangle2D r3 = new ViewRectangle2D(0, 10, 0, 10);
            ViewRectangle2D r4 = new ViewRectangle2D(new ViewPoint2D(5, 5), new ViewSize2D(10, 10));

            Assert.IsFalse(r1.Intersects(ViewRectangle2D.Empty));
            Assert.IsFalse(r1.Intersects(r2));
            Assert.IsFalse(r2.Intersects(r1));
            Assert.IsTrue(r2.Intersects(r3));
            Assert.IsTrue(r3.Intersects(r4));
            Assert.IsTrue(r4.Intersects(r4));
        }

        [Test]
        public void CompareTest()
        {
            ViewRectangle2D r1 = ViewRectangle2D.Empty;
            ViewRectangle2D r2 = ViewRectangle2D.Zero;
            ViewRectangle2D r3 = new ViewRectangle2D(0, 10, 0, 10);
            ViewRectangle2D r4 = new ViewRectangle2D(new ViewPoint2D(11, -11), new ViewSize2D(10, 10));
            ViewRectangle2D r5 = new ViewRectangle2D(new ViewPoint2D(-11, -11), new ViewSize2D(10, 10));
            ViewRectangle2D r6 = new ViewRectangle2D(new ViewPoint2D(11, 11), new ViewSize2D(10, 10));

            Assert.AreEqual(0, r1.CompareTo(ViewRectangle2D.Empty));
            Assert.AreEqual(-1, r1.CompareTo(r2));
            Assert.AreEqual(1, r2.CompareTo(r1));
            Assert.AreEqual(0, r3.CompareTo(r3));
            Assert.AreEqual(0, r3.CompareTo(r2));
            Assert.AreEqual(-1, r4.CompareTo(r3));
            Assert.AreEqual(-1, r5.CompareTo(r3));
            Assert.AreEqual(1, r6.CompareTo(r3));
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void InvertTest()
        {
            ViewRectangle2D r1 = new ViewRectangle2D(0, 1, 0, 1);
            Assert.IsNull((r1 as IMatrixD).Inverse);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void IsInvertableTest()
        {
            ViewRectangle2D r1 = new ViewRectangle2D(0, 1, 0, 1);
            Assert.IsTrue((r1 as IMatrixD).IsInvertible);
        }

        [Test]
        public void ElementsTest1()
        {
            ViewRectangle2D r1 = ViewRectangle2D.Empty;
            ViewRectangle2D r2 = ViewRectangle2D.Zero;
            ViewRectangle2D r3 = new ViewRectangle2D(0, 10, 0, 10);

            Assert.AreEqual(0, (r1 as IMatrixD).ColumnCount);
            Assert.AreEqual(2, (r2 as IMatrixD).ColumnCount);
            Assert.AreEqual(2, (r3 as IMatrixD).ColumnCount);

            DoubleComponent[][] expected = new DoubleComponent[][] { new DoubleComponent[] { 0, 0 }, new DoubleComponent[] { 10, 10 } };
            DoubleComponent[][] actual = (r3 as IMatrixD).Elements;

            Assert.AreEqual(expected[0][0], actual[0][0]);
            Assert.AreEqual(expected[0][1], actual[0][1]);
            Assert.AreEqual(expected[1][0], actual[1][0]);
            Assert.AreEqual(expected[1][1], actual[1][1]);

            IMatrixD r1Matrix = r1;
            r1Matrix.Elements = expected;
            r1 = (ViewRectangle2D) r1Matrix;
            Assert.IsFalse(r1.IsEmpty);
            Assert.AreEqual(r1, r3);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ElementsTest2()
        {
            ViewRectangle2D r1 = ViewRectangle2D.Zero;
            (r1 as IMatrixD).Elements = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ElementsTest3()
        {
            ViewRectangle2D r1 = ViewRectangle2D.Zero;
            (r1 as IMatrixD).Elements = new DoubleComponent[][] 
                { 
                    new DoubleComponent[] { 1, 2, 3 }, new DoubleComponent[] { 2, 3, 4 } 
                };
        }

        [Test]
        [Ignore("Not yet implemented")]
        public void MultiplyTest()
        {
        }

        [Test]
        public void ScaleTest1()
        {
            ViewRectangle2D r1 = ViewRectangle2D.Empty;
            ViewRectangle2D r2 = ViewRectangle2D.Zero;
            ViewRectangle2D r3 = new ViewRectangle2D(0, 1, 0, 1);

            r1.Scale(10);
            Assert.IsTrue(r1.IsEmpty);

            r2.Scale(10);
            Assert.AreEqual(0, r2.Width);
            Assert.AreEqual(0, r2.Height);

            r3.Scale(10);
            Assert.AreEqual(10, r3.Width);
            Assert.AreEqual(10, r3.Height);
            Assert.AreEqual(ViewPoint2D.Zero, r3.Location);

            ViewSize2D scaleSize = new ViewSize2D(-1, 5);

            r1.Scale(scaleSize);
            Assert.IsTrue(r1.IsEmpty);

            r2.Scale(scaleSize);
            Assert.AreEqual(0, r2.Width);
            Assert.AreEqual(0, r2.Height);

            r3.Scale(scaleSize);
            Assert.AreEqual(-10, r3.Width);
            Assert.AreEqual(50, r3.Height);
            Assert.AreEqual(ViewPoint2D.Zero, r3.Location);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ScaleTest2()
        {
            ViewSize3D scaleSize = new ViewSize3D(10, 10, 10);
            ViewRectangle2D r2 = ViewRectangle2D.Zero;
            r2.Scale(scaleSize);
        }

        [Test]
        public void TranslateTest1()
        {
            ViewRectangle2D r1 = ViewRectangle2D.Empty;
            ViewRectangle2D r2 = ViewRectangle2D.Zero;

            r1.Translate(10);
            Assert.IsTrue(r1.IsEmpty);
            Assert.AreEqual(0, r1.X);
            Assert.AreEqual(0, r1.Y);
            Assert.AreEqual(0, r1.Width);
            Assert.AreEqual(0, r1.Height);

            r2.Translate(new ViewPoint2D(3, 5));
            Assert.AreEqual(r2.X, 3);
            Assert.AreEqual(r2.Y, 5);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TranslateTest2()
        {
            ViewRectangle2D r1 = ViewRectangle2D.Zero;
            r1.Translate(new ViewPoint3D(3, 4, 5));
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void TransformTest1()
        {
            ViewRectangle2D rect = new ViewRectangle2D(9, 10, -5, -6);
            IVectorD val = (rect as ITransformMatrixD).TransformVector((IVectorD)ViewPoint2D.Zero);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void TransformTest2()
        {
            ViewRectangle2D rect = new ViewRectangle2D(9, 10, -5, -6);
            (rect as ITransformMatrixD).TransformVector(new DoubleComponent[] { 1, 4 });
        }
    }
    #endregion

    #region ViewMatrix2D
    [TestFixture]
    public class ViewMatrix2DTests
    {
        private static readonly double _e = 0.0005;

        [Test]
        public void ResetTest()
        {
            ViewMatrix2D m1 = new ViewMatrix2D(1, 1, 0, 1, 1, 0);

            m1.Reset();

            Assert.AreEqual(m1, ViewMatrix2D.Identity);
        }

        [Test]
        public void InvertTest()
        {
            ViewMatrix2D m1 = new ViewMatrix2D(1, 2, 0, 3, 4, 0);
            ViewMatrix2D expected = new ViewMatrix2D(-2, 1, 0, 1.5, -0.5, 0);

            IMatrix<DoubleComponent> m1Inverse = m1.Inverse;

            for (int i = 0; i < m1.RowCount; i++)
            {
                for (int j = 0; j < m1.ColumnCount; j++)
                {
                    Assert.AreEqual((double)expected[i, j], (double)m1Inverse[i, j], _e);
                }
            }
        }

        [Test]
        public void IsInvertableTest()
        {
            ViewMatrix2D m1 = new ViewMatrix2D(1, 1, 0, 1, 1, 0);
            ViewMatrix2D m2 = new ViewMatrix2D(1, 2, 0, 3, 4, 0);
            Assert.IsFalse(m1.IsInvertible);
            Assert.IsTrue(m2.IsInvertible);
        }

        [Test]
        public void ElementsTest1()
        {
            ViewMatrix2D m1 = ViewMatrix2D.Identity;
            ViewMatrix2D m2 = ViewMatrix2D.Zero;
            ViewMatrix2D m3 = new ViewMatrix2D(1, 2, 3, 4, 5, 6, 7, 8, 9);

            Assert.AreEqual(3, m1.RowCount);
            Assert.AreEqual(3, m2.ColumnCount);

            DoubleComponent[][] expected = new DoubleComponent[][] 
                { 
                    new DoubleComponent[] { 1, 2, 3 }, 
                    new DoubleComponent[] { 4, 5, 6 }, 
                    new DoubleComponent[] { 7, 8, 9 } 
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void ElementsTest2()
        {
            ViewMatrix2D m1 = ViewMatrix2D.Identity;
            m1.Elements = null;

        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ElementsTest3()
        {
            ViewMatrix2D m1 = ViewMatrix2D.Identity;
            m1.Elements = new DoubleComponent[][] { new DoubleComponent[] { 1, 2, 3 }, new DoubleComponent[] { 2, 3, 4 } };
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void RotateTest()
        {
            ViewMatrix2D m1 = ViewMatrix2D.Identity;
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void RotateAtTest()
        {
            ViewMatrix2D m1 = ViewMatrix2D.Identity;
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void MultiplyTest()
        {
            ViewMatrix2D m1 = ViewMatrix2D.Identity;
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void ScaleTest1()
        {
            ViewMatrix2D m1 = ViewMatrix2D.Zero;
            ViewMatrix2D m2 = ViewMatrix2D.Identity;

            m1.Scale(10);
            Assert.AreEqual(ViewMatrix2D.Zero, m1);

            m2.Scale(10);
            Assert.AreEqual(10, m2.X1);
            Assert.AreEqual(10, m2.Y2);

            ViewSize2D scaleSize = new ViewSize2D(-1, 5);

            m1.Scale(scaleSize);
            Assert.AreEqual(ViewMatrix2D.Zero, m1);

            m2.Scale(scaleSize);
            Assert.AreEqual(-10, m2.X1);
            Assert.AreEqual(50, m2.Y2);
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void ScaleTest2()
        {
            ViewMatrix2D m1 = ViewMatrix2D.Identity;

            // Scale by a vector for which multiplicatio isn't defined...
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void TranslateTest1()
        {
            ViewMatrix2D m1 = ViewMatrix2D.Identity;
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void TranslateTest2()
        {
            ViewMatrix2D m1 = ViewMatrix2D.Identity;
            // Scale by a vector for which multiplicatio isn't defined...
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void TransformTest1()
        {
            ViewMatrix2D m1 = ViewMatrix2D.Identity;
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void Transform2Test2()
        {
            ViewMatrix2D m1 = ViewMatrix2D.Identity;
            // Scale by a vector for which multiplicatio isn't defined...
        }
    }
    #endregion

    #region GraphicsFigure2D
    [TestFixture]
    public class GraphicsFigure2DTests
    {
        [Test]
        public void CreateNewTest()
        {
            ViewPoint2D[] points = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };
            GraphicsFigure2D f1 = new GraphicsFigure2D(points, true);
            Assert.AreEqual(4, f1.Points.Count);

            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(points[i], f1.Points[i]);
            }
        }

        [Test]
        public void EqualityTest()
        {
            ViewPoint2D[] points1 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };
            ViewPoint2D[] points2 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(2, 0), new ViewPoint2D(2, 2), new ViewPoint2D(0, 2) };
            ViewPoint2D[] points3 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(2, 0), new ViewPoint2D(2, 2), new ViewPoint2D(0, 2), new ViewPoint2D(0, 2) };

            GraphicsFigure2D f1 = new GraphicsFigure2D(points1, true);
            GraphicsFigure2D f2 = new GraphicsFigure2D(points1, true);
            GraphicsFigure2D f3 = new GraphicsFigure2D(points1);
            GraphicsFigure2D f4 = new GraphicsFigure2D(points2, true);
            GraphicsFigure2D f5 = new GraphicsFigure2D(points3, true);

            Assert.AreEqual(f1, f2);
            Assert.AreNotEqual(f1, f3);
            Assert.AreNotEqual(f1, f4);
            Assert.AreNotEqual(f4, f5);
            Assert.IsFalse(f1.Equals(new object()));
            f2 = null;
            Assert.IsFalse(f1.Equals(f2));
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void AddPointsTest()
        {
            ViewPoint2D[] points1 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };

            GraphicsFigure2D f1 = new GraphicsFigure2D(points1, false);

            f1.Points.Add(new ViewPoint2D(5, 5));
        }

        [Test]
        public void ToStringTest()
        {
            ViewPoint2D[] points1 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };
            GraphicsFigure2D f1 = new GraphicsFigure2D(points1);

            string expected = String.Format("[{0}] Number of {1} points: 4; Closed: False", typeof(GraphicsFigure2D), typeof(ViewPoint2D).Name);
            Assert.AreEqual(expected, f1.ToString());
        }

        [Test]
        public void CloneTest()
        {
            ViewPoint2D[] points1 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };

            GraphicsFigure2D f1 = new GraphicsFigure2D(points1);
            GraphicsFigure<ViewPoint2D, ViewRectangle2D> f2 = f1.Clone();
            GraphicsFigure2D f3 = (f1 as ICloneable).Clone() as GraphicsFigure2D;

            Assert.AreEqual(f1, f2);
            Assert.AreEqual(f1, f3);
        }

        [Test]
        public void EnumTest()
        {
            ViewPoint2D[] points1 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };

            GraphicsFigure2D f1 = new GraphicsFigure2D(points1);

            IEnumerator e1 = (f1 as IEnumerable).GetEnumerator();

            Assert.IsNotNull(e1);
        }
    }
    #endregion

    #region GraphicsPath2D
    [TestFixture]
    public class GraphicsPath2DTests
    {
        [Test]
        public void CreateNewTest()
        {
            GraphicsPath2D p1 = new GraphicsPath2D();
            Assert.AreEqual(0, p1.Figures.Count);

            ViewPoint2D[] points = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };
            GraphicsPath2D p2 = new GraphicsPath2D(points);
            Assert.AreEqual(1, p2.Figures.Count);
            Assert.AreEqual(4, p2.CurrentFigure.Points.Count);
            Assert.IsFalse(p2.CurrentFigure.IsClosed);

            GraphicsPath2D p3 = new GraphicsPath2D(points, true);
            Assert.IsTrue(p3.CurrentFigure.IsClosed);

            GraphicsFigure2D figure = new GraphicsFigure2D(points, true);
            GraphicsPath2D p4 = new GraphicsPath2D(new GraphicsFigure2D[] { figure });
            Assert.AreEqual(1, p4.Figures.Count);

            GraphicsPath2D p5 = new GraphicsPath2D(new GraphicsFigure2D[] { });
            Assert.IsNull(p5.CurrentFigure);
            Assert.AreEqual(0, p5.Points.Count);
        }

        [Test]
        public void AddFiguresTest()
        {
            ViewPoint2D[] points = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };

            GraphicsPath2D p1 = new GraphicsPath2D();
            p1.NewFigure(points, false);

            Assert.AreEqual(1, p1.Figures.Count);
            Assert.IsNotNull(p1.CurrentFigure);
            Assert.AreEqual(4, p1.CurrentFigure.Points.Count);
            Assert.IsFalse(p1.CurrentFigure.IsClosed);
            Assert.AreEqual(p1.Bounds, p1.CurrentFigure.Bounds);
            Assert.AreEqual(p1.Bounds, new ViewRectangle2D(0, 1, 0, 1));
        }

        [Test]
        public void CurrentFigureTest1()
        {
            ViewPoint2D[] points = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };

            GraphicsFigure2D f1 = new GraphicsFigure2D(points, true);
            GraphicsPath2D p1 = new GraphicsPath2D(f1);

            f1 = p1.CurrentFigure as GraphicsFigure2D;
            Assert.AreEqual(4, p1.Points.Count);

            p1.NewFigure(points, false);
            Assert.AreSame(p1.CurrentFigure, p1.Figures[1]);

            p1.CurrentFigure = f1;
            Assert.AreSame(p1.CurrentFigure, f1);

            GraphicsPath2D p2 = new GraphicsPath2D();
            Assert.IsNull(p2.CurrentFigure);
            Assert.AreEqual(0, p2.Points.Count);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CurrentFigureTest2()
        {
            ViewPoint2D[] points = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };

            GraphicsFigure2D f1 = new GraphicsFigure2D(points, true);
            GraphicsPath2D p1 = new GraphicsPath2D();
            p1.NewFigure(points, false);

            p1.CurrentFigure = f1;
        }

        [Test]
        public void BoundsTest()
        {
            GraphicsPath2D p1 = new GraphicsPath2D();

            Assert.IsTrue(p1.Bounds == ViewRectangle2D.Empty);

            ViewPoint2D[] points1 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };
            ViewPoint2D[] points2 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(10, 0), new ViewPoint2D(10, 1), new ViewPoint2D(0, 1) };
            ViewPoint2D[] points3 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(-1, 0), new ViewPoint2D(-1, -1), new ViewPoint2D(0, -1) };

            p1.NewFigure(points1, true);
            Assert.AreEqual(p1.Bounds, new ViewRectangle2D(0, 1, 0, 1));

            p1.NewFigure(points2, true);
            Assert.AreEqual(p1.Bounds, new ViewRectangle2D(0, 10, 0, 1));

            p1.NewFigure(points3, true);
            Assert.AreEqual(p1.Bounds, new ViewRectangle2D(-1, 10, -1, 1));
        }

        [Test]
        public void CloneTest()
        {
            ViewPoint2D[] points1 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };
            GraphicsPath2D p1 = new GraphicsPath2D(points1, true);

            GraphicsPath<ViewPoint2D, ViewRectangle2D> p2 = p1.Clone();
            Assert.AreEqual(p1, p2);

            GraphicsPath2D p3 = (p1 as ICloneable).Clone() as GraphicsPath2D;
            Assert.AreEqual(p1, p3);

            p2.NewFigure(points1, false);
            Assert.AreNotEqual(p1, p2);
        }

        [Test]
        public void EqualityTest()
        {
            GraphicsPath2D p1 = new GraphicsPath2D();
            GraphicsPath2D p2 = new GraphicsPath2D();
            Assert.IsTrue(p1.Equals(p2));

            ViewPoint2D[] points1 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };
            ViewPoint2D[] points2 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(2, 0), new ViewPoint2D(2, 2), new ViewPoint2D(0, 2) };

            GraphicsPath2D p3 = new GraphicsPath2D(points1);
            GraphicsPath2D p4 = new GraphicsPath2D(points1);
            Assert.IsTrue(p3.Equals(p4));

            GraphicsPath2D p5 = new GraphicsPath2D(p3.CurrentFigure);
            Assert.IsTrue(p3.Equals(p5));

            GraphicsPath2D p6 = new GraphicsPath2D(points1, true);
            Assert.IsFalse(p3.Equals(p6));

            GraphicsFigure2D f1 = new GraphicsFigure2D(points1, false);
            GraphicsPath2D p7 = new GraphicsPath2D(new GraphicsFigure2D[] { f1 });
            Assert.IsTrue(p3.Equals(p7));

            GraphicsFigure2D f2 = new GraphicsFigure2D(points2, true);
            GraphicsPath2D p8 = new GraphicsPath2D(new GraphicsFigure2D[] { f1, f2 });
            Assert.IsFalse(p3.Equals(p8));

            p3.NewFigure(points2, true);
            Assert.IsTrue(p8.Equals(p3));

            p8 = null;
            Assert.IsFalse(p3.Equals(p8));
            Assert.IsFalse(p3.Equals(new object()));
        }

        [Test]
        public void ToStringTest()
        {
            ViewPoint2D[] points1 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };
            GraphicsPath2D p1 = new GraphicsPath2D(points1);

            ViewRectangle2D bounds = new ViewRectangle2D(ViewPoint2D.Zero, ViewSize2D.Unit);
            string expected = String.Format("[{0}] 1 figure of ViewPoint2D points; Bounds: {1}", typeof(GraphicsPath2D), bounds);

            Assert.AreEqual(expected, p1.ToString());
        }

        [Test]
        public void GetHashCodeTest()
        {
            ViewPoint2D[] points1 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };
            GraphicsPath2D p1 = new GraphicsPath2D();
            GraphicsPath2D p2 = new GraphicsPath2D();
            GraphicsPath2D p3 = new GraphicsPath2D(points1);

            GraphicsFigure2D f1 = new GraphicsFigure2D(points1, true);
            GraphicsPath2D p4 = new GraphicsPath2D(points1, true);
            GraphicsPath2D p5 = new GraphicsPath2D(f1);

            Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());
            Assert.AreEqual(p4.GetHashCode(), p5.GetHashCode());
            Assert.AreNotEqual(p1.GetHashCode(), p3.GetHashCode());
            Assert.AreNotEqual(p3.GetHashCode(), p4.GetHashCode());
        }

        [Test]
        public void EnumPointsTest()
        {
            ViewPoint2D[] points1 = new ViewPoint2D[] { new ViewPoint2D(0, 0), new ViewPoint2D(1, 0), new ViewPoint2D(1, 1), new ViewPoint2D(0, 1) };
            GraphicsPath2D p1 = new GraphicsPath2D(points1);

            int i = 0;
            foreach (GraphicsFigure2D figure in p1)
            {
                foreach (ViewPoint2D point in figure)
                {
                    Assert.AreEqual(points1[i++], point);
                }
            }

            IEnumerator e1 = p1.GetEnumerator();
            IEnumerator e2 = (p1 as IEnumerable).GetEnumerator();

            Assert.IsTrue(e1.MoveNext());
            Assert.IsTrue(e2.MoveNext());

            Assert.IsTrue(e2.Current.Equals(e1.Current));

            Assert.IsFalse(e1.MoveNext());
            Assert.IsFalse(e2.MoveNext());
        }
    }
    #endregion

    #region Symbol2D
    [TestFixture]
    public class Symbol2DTests
    {
        [Test]
        public void SizeTest()
        {
            Symbol2D s1 = new Symbol2D(new ViewSize2D(16, 16));
            Assert.AreEqual(new ViewSize2D(16, 16), s1.Size);
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void DataTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void OffsetTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void RotationTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void ScaleTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void CloneTest()
        {
        }
    }
    #endregion

    #region Label2D
    [TestFixture]
    public class Label2DTests
    {
        [Test]
        [Ignore("Test not yet implemented")]
        public void CreateTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void CompareTest()
        {
        }
    }
    #endregion

    #region LabelCollisionDetection2D
    [TestFixture]
    public class LabelCollisionDetection2DTests
    {
        [Test]
        [Ignore("Test not yet implemented")]
        public void SimpleCollisionDetectionTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void ThoroughCollisionDetection()
        {
        }
    }
    #endregion

    #region VectorRenderer2D
    [TestFixture]
    public class VectorRenderer2DTests
    {
    }
    #endregion

    #region RasterRenderer2D
    [TestFixture]
    public class RasterRenderer2DTests
    {
    }
    #endregion

    #region BasicGeometryRenderer2D
    [TestFixture]
    public class BasicGeometryRenderer2DTests
    {
        [Test]
        [Ignore("Test not yet implemented")]
        public void RenderFeatureTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void DrawMultiLineStringTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void DrawLineStringTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void DrawMultiPolygonTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void DrawPolygonTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void DrawPointTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        public void DrawMultiPointTest()
        {
        }

        [Test]
        [Ignore("Test not yet implemented")]
        [ExpectedException(typeof(NotSupportedException))]
        public void UnsupportedGeometryTest()
        {
        }
    }
    #endregion
}
