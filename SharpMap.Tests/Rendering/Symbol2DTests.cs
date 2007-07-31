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
	#region Symbol2D
	[TestFixture]
	public class Symbol2DTests
	{
		[Test]
		public void SizeTest()
		{
			Symbol2D s1 = new Symbol2D(new Size2D(16, 16));
			Assert.AreEqual(new Size2D(16, 16), s1.Size);
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
}
