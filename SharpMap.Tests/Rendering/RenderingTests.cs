using System;

using NUnit.Framework;

using SharpMap.Rendering;

namespace SharpMap.Tests.Rendering
{
	#region ColorMatrix
	[TestFixture]
	public class ColorMatrixTests
	{
		[Test]
		public void ResetTest()
		{
			ColorMatrix m1 = new ColorMatrix(1, 1, 1, 1, 0, 0, 0);

			m1.Reset();

			Assert.AreEqual(m1, ColorMatrix.Identity);
		}

		[Test]
		[Ignore("Matrix invert not implemented on ColorMatrix")]
		public void InvertTest()
		{
			ColorMatrix m1 = new ColorMatrix(1, 1, 1, 1, 0, 0, 0);

			m1.Invert();
		}

		[Test]
		public void IsInvertableTest()
		{
			ColorMatrix m1 = new ColorMatrix(1, 1, 1, 1, 0, 0, 0);
			Assert.IsTrue(m1.IsInvertible);
		}

		[Test]
		public void ElementsTest1()
		{
			ColorMatrix m1 = ColorMatrix.Identity;
			ColorMatrix m2 = new ColorMatrix(0.5, 0.5, 0.5, 1, 0, 0, 0);

			Assert.AreEqual(25, m1.Elements.Length);
			Assert.AreEqual(25, m2.Elements.Length);

			double[,] expected = new double[,] { { 0.5, 0, 0, 0, 0 }, { 0, 0.5, 0, 0, 0 }, { 0, 0, 0.5, 0, 0 }, { 0, 0, 0, 1, 0 }, { 0, 0, 0, 0, 1 } };
			double[,] actual = m2.Elements;

			Assert.AreEqual(expected[0, 0], actual[0, 0]);
			Assert.AreEqual(expected[0, 1], actual[0, 1]);
			Assert.AreEqual(expected[0, 2], actual[0, 2]);
			Assert.AreEqual(expected[1, 0], actual[1, 0]);
			Assert.AreEqual(expected[1, 1], actual[1, 1]);
			Assert.AreEqual(expected[1, 2], actual[1, 2]);
			Assert.AreEqual(expected[2, 0], actual[2, 0]);
			Assert.AreEqual(expected[2, 1], actual[2, 1]);
			Assert.AreEqual(expected[2, 2], actual[2, 2]);

			m1.Elements = expected;
			Assert.AreEqual(m1, m2);
			Assert.IsTrue(m1.Equals(m2 as IViewMatrix));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ElementsTest2()
		{
			ColorMatrix m1 = ColorMatrix.Identity;
			m1.Elements = null;
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ElementsTest3()
		{
			ColorMatrix m1 = ColorMatrix.Identity;
			m1.Elements = new double[,] { { 1, 2, 3 }, { 2, 3, 4 } };
		}

		[Test]
		[Ignore("Not yet implemented")]
		public void RotateTest()
		{
			ColorMatrix m1 = ColorMatrix.Identity;
		}

		[Test]
		[Ignore("Not yet implemented")]
		public void RotateAtTest()
		{
			ColorMatrix m1 = ColorMatrix.Identity;
		}

		[Test]
		[Ignore("Not yet implemented")]
		public void GetOffsetTest()
		{
			ColorMatrix m1 = ColorMatrix.Identity;
		}

		[Test]
		[Ignore("Not yet implemented")]
		public void OffsetTest()
		{
			ColorMatrix m1 = ColorMatrix.Identity;
		}

		[Test]
		[Ignore("Not yet implemented")]
		public void MultiplyTest()
		{
			ColorMatrix m1 = ColorMatrix.Identity;
		}

		[Test]
		[Ignore("Not yet implemented")]
		public void ScaleTest1()
		{
			ColorMatrix m1 = ColorMatrix.Zero;
			ColorMatrix m2 = ColorMatrix.Identity;
		}

		[Test]
		[Ignore("Not yet implemented")]
		public void ScaleTest2()
		{
			ColorMatrix m1 = ColorMatrix.Identity;

			// Scale by a vector for which multiplicatio isn't defined...
		}

		[Test]
		[Ignore("Not yet implemented")]
		public void TranslateTest1()
		{
			ColorMatrix m1 = ColorMatrix.Identity;
		}

		[Test]
		[Ignore("Not yet implemented")]
		public void TranslateTest2()
		{
			ColorMatrix m1 = ColorMatrix.Identity;
			// Scale by a vector for which multiplicatio isn't defined...
		}

		[Test]
		[Ignore("Not yet implemented")]
		public void TransformTest1()
		{
			ColorMatrix m1 = ColorMatrix.Identity;
		}

		[Test]
		[Ignore("Not yet implemented")]
		public void Transform2Test2()
		{
			ColorMatrix m1 = ColorMatrix.Identity;
			// Scale by a vector for which multiplicatio isn't defined...
		}
	}
	#endregion
}
