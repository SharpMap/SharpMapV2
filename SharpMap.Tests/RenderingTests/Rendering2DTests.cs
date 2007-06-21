using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Tests.RenderingTests
{
	#region ViewPoint2D
	[TestFixture]
	public class ViewPoint2DTests
	{
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

			IViewVector v1 = (IViewVector)p1;
			IViewVector v2 = (IViewVector)p2;
			IViewVector v3 = (IViewVector)p3;
			IViewVector v4 = (IViewVector)p4;
			IViewVector v5 = (IViewVector)p5;

			Assert.AreEqual(v1, v2);
			Assert.AreNotEqual(v1, v3);
			Assert.AreEqual(v3, v4);
			Assert.AreNotEqual(v1, v5);
			Assert.AreNotEqual(v3, v5);

			Assert.AreEqual(v5, p5);
		}

		[Test]
		public void IViewVectorTests1()
		{
			ViewPoint2D p1 = ViewPoint2D.Empty;
			ViewPoint2D p2 = ViewPoint2D.Zero;
			ViewPoint2D p3 = new ViewPoint2D(9, 10);

			Assert.AreEqual(0, p1.Elements.Length);
			Assert.AreEqual(2, p2.Elements.Length);
			Assert.AreEqual(9, p3[0]);
			Assert.AreEqual(10, p3[1]);
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IViewVectorTests2()
		{
			ViewPoint2D p1 = new ViewPoint2D(9, 10);

			Assert.AreEqual(10, p1[2]);
		}
		
		[Test]
		public void CloneTest()
		{
			ViewPoint2D p1 = new ViewPoint2D(1.1, 2.2);
			ViewPoint2D p2 = (ViewPoint2D)p1.Clone();

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

			IViewVector v1 = (IViewVector)s1;
			IViewVector v2 = (IViewVector)s2;
			IViewVector v3 = (IViewVector)s3;
			IViewVector v4 = (IViewVector)s4;
			IViewVector v5 = (IViewVector)s5;

			Assert.AreEqual(v1, v2);
			Assert.AreNotEqual(v1, v3);
			Assert.AreEqual(v3, v4);
			Assert.AreNotEqual(v1, v5);
			Assert.AreNotEqual(v3, v5);

			Assert.AreEqual(v5, s5);
		}

		[Test]
		public void IViewVectorTests1()
		{
			ViewPoint2D s1 = ViewPoint2D.Empty;
			ViewPoint2D s2 = ViewPoint2D.Zero;
			ViewPoint2D s3 = new ViewPoint2D(9, 10);

			Assert.AreEqual(0, s1.Elements.Length);
			Assert.AreEqual(2, s2.Elements.Length);
			Assert.AreEqual(9, s3[0]);
			Assert.AreEqual(10, s3[1]);
		}

		[Test]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void IViewVectorTests2()
		{
			ViewPoint2D s1 = new ViewPoint2D(9, 10);

			Assert.AreEqual(10, s1[2]);
		}

		[Test]
		public void CloneTest()
		{
			ViewPoint2D s1 = new ViewPoint2D(1.1, 2.2);
			ViewPoint2D s2 = (ViewPoint2D)s1.Clone();

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
		[Test]
		public void ViewSize2DEqualityTests()
		{
			ViewRectangle2D r1 = new ViewRectangle2D();
			ViewRectangle2D r2 = ViewRectangle2D.Empty;
			ViewRectangle2D r3 = ViewRectangle2D.Zero;
			ViewRectangle2D r4 = new ViewRectangle2D(0, 0, 0, 0);
			ViewRectangle2D r5 = new ViewRectangle2D(9, 10, -5, -6);

			Assert.AreEqual(r1, r2);
			Assert.AreNotEqual(r1, r3);
			Assert.AreEqual(r3, r4);
			Assert.AreNotEqual(r1, r5);
			Assert.AreNotEqual(r3, r5);

			IViewMatrix v1 = (IViewMatrix)r1;
			IViewMatrix v2 = (IViewMatrix)r2;
			IViewMatrix v3 = (IViewMatrix)r3;
			IViewMatrix v4 = (IViewMatrix)r4;
			IViewMatrix v5 = (IViewMatrix)r5;

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
		}

		[Test]
		public void CompareTest()
		{
		}

		[Test]
		public void ResetTest()
		{
		}

		[Test]
		public void InvertTest()
		{
		}

		[Test]
		public void IsInvertableTest()
		{
		}

		[Test]
		public void ElementsTest1()
		{
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ElementsTest2()
		{
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ElementsTest3()
		{
		}		

		[Test]
		public void RotateTest()
		{
		}
		
		[Test]
		public void RotateAtTest()
		{
		}
		
		[Test]
		public void GetOffsetTest()
		{
		}
		
		[Test]
		public void OffsetTest()
		{
		}
		
		[Test]
		public void MultiplyTest()
		{
		}
		
		[Test]
		public void ScaleTest1()
		{
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ScaleTest2()
		{
		}
		
		[Test]
		public void TranslateTest1()
		{
		}
		
		[Test]
		public void TranslateTest2()
		{
		}
		
		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void TransformTest1()
		{
			ViewRectangle2D rect = new ViewRectangle2D(9, 10, -5, -6);
			IViewVector val = rect.Transform((IViewVector)ViewPoint2D.Zero);
		}
		
		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void Transform2Test2()
		{
			ViewRectangle2D rect = new ViewRectangle2D(9, 10, -5, -6);
		}
	}
	#endregion

	#region ViewMatrix2D
	[TestFixture]
	public class ViewMatrix2DTests
	{
	}
	#endregion

	#region VectorRenderer2D
	[TestFixture]
	public class VectorRenderer2DTests
	{
	}
	#endregion

	#region Symbol2D
	[TestFixture]
	public class Symbol2DTests
	{
	}
	#endregion

	#region RasterRenderer2D
	[TestFixture]
	public class RasterRenderer2DTests
	{
	}
	#endregion

	#region LabelCollisionDetection2D
	[TestFixture]
	public class LabelCollisionDetection2DTests
	{
	}
	#endregion
	
	#region Label2D
	[TestFixture]
	public class Label2DTests
	{
	}
	#endregion
	
	#region GraphicsPath2D
	[TestFixture]
	public class GraphicsPath2DTests
	{
	}
	#endregion
	
	#region BasicGeometryRenderer2D
	[TestFixture]
	public class BasicGeometryRenderer2DTests
	{
	}
	#endregion
}
