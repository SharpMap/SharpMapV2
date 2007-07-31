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
