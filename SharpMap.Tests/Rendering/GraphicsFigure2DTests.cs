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
	#region GraphicsFigure2D
	[TestFixture]
	public class GraphicsFigure2DTests
	{
		[Test]
		public void CreateNewTest()
		{
			Point2D[] points = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };
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
			Point2D[] points1 = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };
			Point2D[] points2 = new Point2D[] { new Point2D(0, 0), new Point2D(2, 0), new Point2D(2, 2), new Point2D(0, 2) };
			Point2D[] points3 = new Point2D[] { new Point2D(0, 0), new Point2D(2, 0), new Point2D(2, 2), new Point2D(0, 2), new Point2D(0, 2) };

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
			Point2D[] points1 = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };

			GraphicsFigure2D f1 = new GraphicsFigure2D(points1, false);

			f1.Points.Add(new Point2D(5, 5));
		}

		[Test]
		public void ToStringTest()
		{
			Point2D[] points1 = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };
			GraphicsFigure2D f1 = new GraphicsFigure2D(points1);

			string expected = String.Format("[{0}] Number of {1} points: 4; Closed: False", typeof(GraphicsFigure2D), typeof(Point2D).Name);
			Assert.AreEqual(expected, f1.ToString());
		}

		[Test]
		public void CloneTest()
		{
			Point2D[] points1 = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };

			GraphicsFigure2D f1 = new GraphicsFigure2D(points1);
			GraphicsFigure<Point2D, Rectangle2D> f2 = f1.Clone();
			GraphicsFigure2D f3 = (f1 as ICloneable).Clone() as GraphicsFigure2D;

			Assert.AreEqual(f1, f2);
			Assert.AreEqual(f1, f3);
		}

		[Test]
		public void EnumTest()
		{
			Point2D[] points1 = new Point2D[] { new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1) };

			GraphicsFigure2D f1 = new GraphicsFigure2D(points1);

			IEnumerator e1 = (f1 as IEnumerable).GetEnumerator();

			Assert.IsNotNull(e1);
		}
	}
	#endregion
}
