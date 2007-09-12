using System;
using System.Collections.Generic;
using NUnit.Framework;
using SharpMap.Data;
using SharpMap.Features;
using SharpMap.Geometries;
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
			public double[][] RenderedPaths;
		}

		private class TestVectorRenderer : VectorRenderer2D<RenderObject>
		{
			public override RenderObject RenderPath(GraphicsPath2D path, StylePen outline, StylePen highlightOutline,
													StylePen selectOutline)
			{
				RenderObject ro = new RenderObject();

				ro.RenderedPaths = new double[path.Figures.Count][];

				for (int figureIndex = 0; figureIndex < path.Figures.Count; figureIndex++)
				{
					ro.RenderedPaths[figureIndex] = new double[path.Figures[figureIndex].Points.Count];
				}

				return ro;
			}

			public override RenderObject RenderPath(GraphicsPath2D path, StyleBrush fill, StyleBrush highlightFill,
													StyleBrush selectFill, StylePen outline, StylePen highlightOutline,
													StylePen selectOutline)
			{
				RenderObject ro = new RenderObject();

				ro.RenderedPaths = new double[path.Figures.Count][];

				for (int figureIndex = 0; figureIndex < path.Figures.Count; figureIndex++)
				{
					ro.RenderedPaths[figureIndex] = new double[path.Figures[figureIndex].Points.Count];
				}

				return ro;
			}

			public override RenderObject RenderSymbol(Point2D location, Symbol2D symbolData)
			{
				RenderObject ro = new RenderObject();

				ro.RenderedPaths = new double[][] { new double[] { location.X, location.Y } };

				return ro;
			}

			public override RenderObject RenderSymbol(Point2D location, Symbol2D symbolData, ColorMatrix highlight,
													  ColorMatrix select)
			{
				RenderObject ro = new RenderObject();

				ro.RenderedPaths = new double[][] { new double[] { location.X, location.Y } };

				return ro;
			}

			public override RenderObject RenderSymbol(Point2D location, Symbol2D symbolData, Symbol2D highlightSymbolData,
													  Symbol2D selectSymbolData)
			{
				RenderObject ro = new RenderObject();

				ro.RenderedPaths = new double[][] { new double[] { location.X, location.Y } };

				return ro;
			}
		}

		[Test]
		public void RenderFeatureTest()
		{
			IFeatureLayerProvider provider = DataSourceHelper.CreateGeometryDatasource();
			TestVectorRenderer vectorRenderer = new TestVectorRenderer();
			BasicGeometryRenderer2D<RenderObject> renderer = new BasicGeometryRenderer2D<RenderObject>(vectorRenderer);

			FeatureDataTable features = new FeatureDataTable();
			provider.ExecuteIntersectionQuery(provider.GetExtents(), features);

			foreach (FeatureDataRow feature in features)
			{
				Geometry g = feature.Geometry;
				List<PositionedRenderObject2D<RenderObject>> renderedObjects = new List<PositionedRenderObject2D<RenderObject>>(renderer.RenderFeature(feature));

				foreach (PositionedRenderObject2D<RenderObject> ro in renderedObjects)
				{
				}
			}
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