using System;
using NUnit.Framework;
using SharpMap.Data;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using ITransformMatrixD = NPack.Interfaces.ITransformMatrix<NPack.DoubleComponent>;

namespace SharpMap.Tests.Rendering
{

	#region BasicGeometryRenderer2D

	[TestFixture]
	public class BasicGeometryRenderer2DTests
	{
		private struct RenderObject
		{
		}

		private class TestVectorRenderer : VectorRenderer2D<RenderObject>
		{
			public override RenderObject RenderPath(GraphicsPath2D path, StylePen outline, StylePen highlightOutline,
			                                        StylePen selectOutline)
			{
				throw new NotImplementedException();
			}

			public override RenderObject RenderPath(GraphicsPath2D path, StyleBrush fill, StyleBrush highlightFill,
			                                        StyleBrush selectFill, StylePen outline, StylePen highlightOutline,
			                                        StylePen selectOutline)
			{
				throw new NotImplementedException();
			}

			public override RenderObject RenderSymbol(Point2D location, Symbol2D symbolData)
			{
				throw new NotImplementedException();
			}

			public override RenderObject RenderSymbol(Point2D location, Symbol2D symbolData, ColorMatrix highlight,
			                                          ColorMatrix select)
			{
				throw new NotImplementedException();
			}

			public override RenderObject RenderSymbol(Point2D location, Symbol2D symbolData, Symbol2D highlightSymbolData,
			                                          Symbol2D selectSymbolData)
			{
				throw new NotImplementedException();
			}
		}

		[Test]
		[Ignore("Test not yet implemented")]
		public void RenderFeatureTest()
		{
			IVectorLayerProvider provider = DataSourceHelper.CreateGeometryDatasource();
			TestVectorRenderer vectorRenderer = new TestVectorRenderer();
			BasicGeometryRenderer2D<RenderObject> renderer = new BasicGeometryRenderer2D<RenderObject>(vectorRenderer);
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
		[ExpectedException(typeof (NotSupportedException))]
		public void UnsupportedGeometryTest()
		{
		}
	}

	#endregion
}