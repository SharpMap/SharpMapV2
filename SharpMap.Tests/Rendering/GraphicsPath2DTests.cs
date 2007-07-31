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
	#region GraphicsPath2D
	[TestFixture]
	public class GraphicsPath2DTests
	{
		[Test]
		public void CreateNewTest()
		{
			GraphicsPath2D p1 = new GraphicsPath2D();
			Assert.AreEqual(0, p1.Figures.Count);

			Point2D[] points = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };
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
			Point2D[] points = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };

			GraphicsPath2D p1 = new GraphicsPath2D();
			p1.NewFigure(points, false);

			Assert.AreEqual(1, p1.Figures.Count);
			Assert.IsNotNull(p1.CurrentFigure);
			Assert.AreEqual(4, p1.CurrentFigure.Points.Count);
			Assert.IsFalse(p1.CurrentFigure.IsClosed);
			Assert.AreEqual(p1.Bounds, p1.CurrentFigure.Bounds);
			Assert.AreEqual(p1.Bounds, new Rectangle2D(0, 0, 1, 1));
		}

		[Test]
		public void CurrentFigureTest1()
		{
			Point2D[] points = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };

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
			Point2D[] points = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };

			GraphicsFigure2D f1 = new GraphicsFigure2D(points, true);
			GraphicsPath2D p1 = new GraphicsPath2D();
			p1.NewFigure(points, false);

			p1.CurrentFigure = f1;
		}

		[Test]
		public void BoundsTest()
		{
			GraphicsPath2D p1 = new GraphicsPath2D();

			Assert.IsTrue(p1.Bounds == Rectangle2D.Empty);

			Point2D[] points1 = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };
			Point2D[] points2 = new Point2D[] { new Point2D(0, 0), new Point2D(10, 0), new Point2D(10, 1), new Point2D(0, 1) };
			Point2D[] points3 = new Point2D[] { new Point2D(0, 0), new Point2D(-1, 0), new Point2D(-1, -1), new Point2D(0, -1) };

			p1.NewFigure(points1, true);
			Assert.AreEqual(p1.Bounds, new Rectangle2D(0, 0, 1, 1));

			p1.NewFigure(points2, true);
			Assert.AreEqual(p1.Bounds, new Rectangle2D(0, 0, 10, 1));

			p1.NewFigure(points3, true);
			Assert.AreEqual(p1.Bounds, new Rectangle2D(-1, -1, 10, 1));
		}

		[Test]
		public void CloneTest()
		{
			Point2D[] points1 = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };
			GraphicsPath2D p1 = new GraphicsPath2D(points1, true);

			GraphicsPath<Point2D, Rectangle2D> p2 = p1.Clone();
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

			Point2D[] points1 = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };
			Point2D[] points2 = new Point2D[] { new Point2D(0, 0), new Point2D(2, 0), new Point2D(2, 2), new Point2D(0, 2) };

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
			Point2D[] points1 = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };
			GraphicsPath2D p1 = new GraphicsPath2D(points1);

			Rectangle2D bounds = new Rectangle2D(Point2D.Zero, Size2D.Unit);
			string expected = String.Format("[{0}] 1 figure of Point2D points; Bounds: {1}", typeof(GraphicsPath2D), bounds);

			Assert.AreEqual(expected, p1.ToString());
		}

		[Test]
		public void GetHashCodeTest()
		{
			Point2D[] points1 = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };
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
			Point2D[] points1 = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };
			GraphicsPath2D p1 = new GraphicsPath2D(points1);

			int i = 0;
			foreach (GraphicsFigure2D figure in p1)
			{
				foreach (Point2D point in figure)
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
}
