using System;
using System.Collections;
using NPack.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;

using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Rendering.Rendering3D;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using ITransformMatrixD = NPack.Interfaces.ITransformMatrix<NPack.DoubleComponent>;
using NPack;
using SharpMap.Styles;

namespace SharpMap.Tests.Rendering
{
	#region VectorRenderer2D
	[TestFixture]
	public class VectorRenderer2DTests
	{

		[Test]
		public void CreateVectorRenderer2DTest()
		{
			TestVector2DRenderer r = new TestVector2DRenderer();
		}

		[Test]
		public void RenderPathWithOutlineOnly()
		{
			Point2D[] points = new Point2D[8];
			points[0] = new Point2D(10, 10);
			points[1] = new Point2D(11, 11);
			points[2] = new Point2D(10, 0);
			points[3] = new Point2D(0, 0);
			points[4] = new Point2D(-1, -1);
			points[5] = new Point2D(0, 0);
			points[6] = new Point2D(0, 5);
			points[7] = new Point2D(0, 10);

			GraphicsPath2D path = new GraphicsPath2D(points, true);
			StylePen outline = new StylePen(new SolidStyleBrush(StyleColor.WhiteSmoke), 1);
			TestVector2DRenderer r = new TestVector2DRenderer();
			string result = r.RenderPath(path, outline, outline, outline);
		}

		public class TestVector2DRenderer : VectorRenderer2D<string>
		{
			public override string RenderPath(GraphicsPath2D path, StylePen outline, StylePen highlightOutline, StylePen selectOutline)
			{
				if (path == null) throw new ArgumentNullException("path");

				return path.ToString() +
					outline == null ? String.Empty : outline.ToString() +
					highlightOutline == null ? String.Empty : highlightOutline.ToString() +
					selectOutline == null ? String.Empty : selectOutline.ToString();
			}

			public override string RenderPath(GraphicsPath2D path, StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill, StylePen outline, StylePen highlightOutline, StylePen selectOutline)
			{
				if (path == null) throw new ArgumentNullException("path");

				return path.ToString() +
					fill == null ? String.Empty : fill.ToString() +
					selectFill == null ? String.Empty : selectFill.ToString() +
					highlightFill == null ? String.Empty : highlightFill.ToString() +
					outline == null ? String.Empty : outline.ToString() +
					highlightOutline == null ? String.Empty : highlightOutline.ToString() +
					selectOutline == null ? String.Empty : selectOutline.ToString();
			}

			public override string RenderSymbol(Point2D location, Symbol2D symbolData)
			{
				if (symbolData == null) throw new ArgumentNullException("symbolData");

				if (location == Point2D.Empty)
				{
					return null;
				}

				return location.ToString() +
					symbolData == null ? String.Empty : symbolData.ToString();
			}

			public override string RenderSymbol(Point2D location, Symbol2D symbolData, ColorMatrix highlight, ColorMatrix select)
			{
				if (symbolData == null) throw new ArgumentNullException("symbolData");

				if (location == Point2D.Empty)
				{
					return null;
				}

				return location.ToString() +
					symbolData == null ? String.Empty : symbolData.ToString() +
					highlight == null ? String.Empty : highlight.ToString() +
					select == null ? String.Empty : select.ToString();
			}

			public override string RenderSymbol(Point2D location, Symbol2D symbolData, Symbol2D highlightSymbolData, Symbol2D selectSymbolData)
			{
				if (symbolData == null) throw new ArgumentNullException("symbolData");

				if (location == Point2D.Empty)
				{
					return null;
				}

				return location.ToString() +
					symbolData == null ? String.Empty : symbolData.ToString() +
					highlightSymbolData == null ? String.Empty : highlightSymbolData.ToString() +
					selectSymbolData == null ? String.Empty : selectSymbolData.ToString();
			}
		}
	}
	#endregion
}
