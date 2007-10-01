// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using SharpMap.Styles;
using SharpMap.Geometries;
using SharpMap.Data;

namespace SharpMap.Rendering.Rendering2D
{
	/// <summary>
	/// A basic renderer which renders the geometric paths from feature geometry, 
	/// taking into account <see cref="Style">styles</see> and 
	/// <see cref="SharpMap.Rendering.Thematics.ITheme">themes</see>.
	/// </summary>
	/// <typeparam name="TRenderObject">Type of render object to generate.</typeparam>
	public class BasicGeometryRenderer2D<TRenderObject> : FeatureRenderer2D<VectorStyle, TRenderObject>,
		IGeometryRenderer<Symbol2D, TRenderObject>
	{
		#region Type Members
		private static object _defaultSymbol;
		private static readonly object _defaultSymbolSync = new object();

		/// <summary>
		/// The default basic symbol for rendering point data.
		/// </summary>
		public static Symbol2D DefaultSymbol
		{
			get
			{
				if (Thread.VolatileRead(ref _defaultSymbol) == null)
				{
					lock (_defaultSymbolSync)
					{
						if (Thread.VolatileRead(ref _defaultSymbol) == null)
						{
							Stream data = Assembly.GetExecutingAssembly()
								.GetManifestResourceStream("SharpMap.Styles.DefaultSymbol.png");
                            Symbol2D symbol = new Symbol2D(data, new Size2D(16, 16));
							Thread.VolatileWrite(ref _defaultSymbol, symbol);
						}
					}
				}

                return (Symbol2D)(_defaultSymbol as Symbol2D).Clone();
			}
		}

		// DON'T remove - this eliminates the .beforefieldinit IL metadata
		static BasicGeometryRenderer2D() { }
		#endregion

		#region Object construction and disposal
        /// <summary>
        /// Creates a new BasicGeometryRenderer2D with the given VectorRenderer2D instance.
        /// </summary>
        /// <param name="vectorRenderer">
        /// A vector renderer.
        /// </param>
        public BasicGeometryRenderer2D(VectorRenderer2D<TRenderObject> vectorRenderer)
            : this(vectorRenderer, new Matrix2D())
        {
            DefaultStyle = new VectorStyle();
        }

		/// <summary>
		/// Creates a new BasicGeometryRenderer2D with the given VectorRenderer2D instance.
		/// </summary>
		/// <param name="vectorRenderer">
		/// A vector renderer.
		/// </param>
		/// <param name="toViewTransform">
		/// A transform which maps world coordinates to view coordinates.
		/// </param>
		public BasicGeometryRenderer2D(VectorRenderer2D<TRenderObject> vectorRenderer, Matrix2D toViewTransform)
			: base(vectorRenderer)
		{
			DefaultStyle = new VectorStyle();
			ToViewTransform = toViewTransform;
		}

		#region Dispose pattern
		/// <summary>
		/// Finalizer for BasicGeometryRenderer2D.
		/// </summary>
		~BasicGeometryRenderer2D()
		{
			Dispose(false);
		}
		#endregion
		#endregion

		/// <summary>
		/// Renders the geometry of the <paramref name="feature"/>.
		/// </summary>
		/// <param name="feature">The feature to render.</param>
		/// <param name="style">The style to use to render the feature.</param>
		/// <returns>An enumeration of positioned render objects suitable for display.</returns>
		protected override IEnumerable<TRenderObject> DoRenderFeature(IFeatureDataRecord feature, VectorStyle style, RenderState renderState)
		{
			if (feature == null) throw new ArgumentNullException("feature");
			if (style == null) throw new ArgumentNullException("style");

			if (feature.Geometry == null)
			{
				throw new InvalidOperationException("Feature must have a geometry to be rendered.");
			}

			return renderGeometry(feature.Geometry, style, renderState);
		}

		/// <summary>
		/// Renders a <see cref="MultiLineString"/>.
		/// </summary>
		/// <param name="lines">MultiLineString to be rendered.</param>
		/// <param name="fill">Pen used for filling (null or transparent for no filling).</param>
		/// <param name="highlightFill">Pen used for filling when highlighted.</param>
		/// <param name="selectFill">Pen used for filling when selected.</param>
		/// <param name="outline">Outline pen style (null if no outline).</param>
		/// <param name="highlightOutline">Outline pen style used when highlighted.</param>
		/// <param name="selectOutline">Outline pen style used when selected.</param>
		public virtual IEnumerable<TRenderObject> DrawMultiLineString(
			MultiLineString lines, StylePen fill, StylePen highlightFill, StylePen selectFill,
            StylePen outline, StylePen highlightOutline, StylePen selectOutline, RenderState renderState)
		{
			return drawLineStrings(lines.LineStrings, fill, highlightFill, selectFill,
                outline, highlightOutline, selectOutline, renderState);
		}

		/// <summary>
		/// Renders a <see cref="LineString"/>.
		/// </summary>
		/// <param name="line">LineString to render.</param>
		/// <param name="fill">Pen used for filling (null or transparent for no filling).</param>
		/// <param name="highlightFill">Pen used for filling when highlighted.</param>
		/// <param name="selectFill">Pen used for filling when selected.</param>
		/// <param name="outline">Outline pen style (null if no outline).</param>
		/// <param name="highlightOutline">Outline pen style used when highlighted.</param>
		/// <param name="selectOutline">Outline pen style used when selected.</param>
		public virtual IEnumerable<TRenderObject> DrawLineString(
			LineString line, StylePen fill, StylePen highlightFill, StylePen selectFill,
            StylePen outline, StylePen highlightOutline, StylePen selectOutline, RenderState renderState)
		{
			return drawLineStrings(new LineString[] { line }, fill, highlightFill, selectFill,
                outline, highlightOutline, selectOutline, renderState);
		}

		/// <summary>
		/// Renders a <see cref="MultiPolygon"/>.
		/// </summary>
		/// <param name="multipolygon">MultiPolygon to render.</param>
		/// <param name="fill">Brush used for filling (null or transparent for no filling).</param>
		/// <param name="highlightFill">Brush used for filling when highlighted.</param>
		/// <param name="selectFill">Brush used for filling when selected.</param>
		/// <param name="outline">Outline pen style (null if no outline).</param>
		/// <param name="highlightOutline">Outline pen style used when highlighted.</param>
		/// <param name="selectOutline">Outline pen style used when selected.</param>
		public virtual IEnumerable<TRenderObject> DrawMultiPolygon(
			MultiPolygon multipolygon, StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill,
            StylePen outline, StylePen highlightOutline, StylePen selectOutline, RenderState renderState)
		{
			return drawPolygons(multipolygon, fill, highlightFill, selectFill,
                outline, highlightOutline, selectOutline, renderState);
		}

		/// <summary>
		/// Renders a <see cref="Polygon"/>.
		/// </summary>
		/// <param name="polygon">Polygon to render</param>
		/// <param name="fill">Brush used for filling (null or transparent for no filling).</param>
		/// <param name="highlightFill">Brush used for filling when highlighted.</param>
		/// <param name="selectFill">Brush used for filling when selected.</param>
		/// <param name="outline">Outline pen style (null if no outline).</param>
		/// <param name="highlightOutline">Outline pen style used when highlighted.</param>
		/// <param name="selectOutline">Outline pen style used when selected.</param>
		public virtual IEnumerable<TRenderObject> DrawPolygon(
			Polygon polygon, StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill,
			StylePen outline, StylePen highlightOutline, StylePen selectOutline, RenderState renderState)
		{
			return drawPolygons(new Polygon[] { polygon }, fill, highlightFill, selectFill,
                outline, highlightOutline, selectOutline, renderState);
		}

		/// <summary>
		/// Renders a <see cref="Point"/>.
		/// </summary>
		/// <param name="point">Point to render.</param>
		/// <param name="symbol">Symbol to place over point.</param>
		/// <param name="highlightSymbol">Symbol to use for point when point is highlighted.</param>
		/// <param name="selectSymbol">Symbol to use for point when point is selected.</param>
		public virtual IEnumerable<TRenderObject> DrawPoint(Point point, Symbol2D symbol, 
            Symbol2D highlightSymbol, Symbol2D selectSymbol, RenderState renderState)
		{
            return drawPoints(new Point[] { point }, symbol, highlightSymbol, selectSymbol, renderState);
		}

		/// <summary>
		/// Renders a <see cref="MultiPoint"/>.
		/// </summary>
		/// <param name="points">MultiPoint to render.</param>
		/// <param name="symbol">Symbol to place over point.</param>
		/// <param name="highlightSymbol">Symbol to use for point when point is highlighted.</param>
		/// <param name="selectSymbol">Symbol to use for point when point is selected.</param>
		public virtual IEnumerable<TRenderObject> DrawMultiPoint(MultiPoint points,
            Symbol2D symbol, Symbol2D highlightSymbol, Symbol2D selectSymbol, RenderState renderState)
		{
			return drawPoints(points.Points, symbol, highlightSymbol, selectSymbol, renderState);
		}

		#region Private helper methods
		private IEnumerable<TRenderObject> renderGeometry(IGeometry geometry, VectorStyle style, RenderState renderState)
		{
			if (geometry == null) throw new ArgumentNullException("geometry");

			if (geometry is Polygon)
			{
				return DrawPolygon(geometry as Polygon, style.Fill, style.HighlightFill, style.SelectFill, style.Outline,
					style.HighlightOutline, style.SelectOutline, renderState);
			}
			else if (geometry is MultiPolygon)
			{
				return DrawMultiPolygon(geometry as MultiPolygon, style.Fill, style.HighlightFill, style.SelectFill,
                    style.Outline, style.HighlightOutline, style.SelectOutline, renderState);
			}
			else if (geometry is LineString)
			{
				return DrawLineString(geometry as LineString, style.Line, style.HighlightLine, style.SelectLine,
                    style.Outline, style.HighlightOutline, style.SelectOutline, renderState);
			}
			else if (geometry is MultiLineString)
			{
				return DrawMultiLineString(geometry as MultiLineString, style.Line, style.HighlightLine, style.SelectLine,
                    style.Outline, style.HighlightOutline, style.SelectOutline, renderState);
			}
			else if (geometry is Point)
			{
                return DrawPoint(geometry as Point, style.Symbol, style.HighlightSymbol, style.SelectSymbol, renderState);
			}
			else if (geometry is MultiPoint)
			{
                return DrawMultiPoint(geometry as MultiPoint, style.Symbol, style.HighlightSymbol, style.SelectSymbol, renderState);
			}
			else if (geometry is GeometryCollection)
			{
				List<TRenderObject> renderObjects = new List<TRenderObject>();

				foreach (Geometry g in (geometry as GeometryCollection))
				{
                    renderObjects.AddRange(renderGeometry(g, style, renderState));
				}

				return renderObjects;
			}

			throw new NotSupportedException(String.Format("Geometry type is not supported: {0}", geometry.GetType()));
		}

		private IEnumerable<TRenderObject> drawPoints(IEnumerable<Point> points, Symbol2D symbol,
            Symbol2D highlightSymbol, Symbol2D selectSymbol, RenderState renderState)
        {
            // If we have a null symbol, use the default
			if (symbol == null)  symbol = DefaultSymbol;
            if (highlightSymbol == null) highlightSymbol = symbol;
            if (selectSymbol == null) selectSymbol = symbol;

			return VectorRenderer.RenderSymbols(convertPoints(points), symbol, highlightSymbol, selectSymbol, renderState);
		}

		private IEnumerable<TRenderObject> drawLineStrings(IEnumerable<LineString> lines, StylePen fill,
            StylePen highlightFill, StylePen selectFill, StylePen outline, StylePen highlightOutline, StylePen selectOutline, RenderState renderState)
		{
            if (fill == null) throw new ArgumentNullException("fill");

		    IEnumerable<Path2D> paths = convertToPaths(lines);

            if (highlightFill == null) highlightFill = fill;
            if (selectFill == null) selectFill = fill;
		    if (highlightOutline == null) highlightOutline = outline;
            if (selectOutline == null) selectOutline = outline;

		    IEnumerable<TRenderObject> renderedObjects = VectorRenderer.RenderPaths(paths, 
                fill, highlightFill, selectFill, outline, highlightOutline, selectOutline, renderState);

            foreach (TRenderObject ro in renderedObjects)
            {
                yield return ro;
            }
		}

		private IEnumerable<TRenderObject> drawPolygons(IEnumerable<Polygon> polygons, StyleBrush fill, 
            StyleBrush highlightFill, StyleBrush selectFill, StylePen outline, StylePen highlightOutline, 
            StylePen selectOutline, RenderState renderState)
        {
            if (fill == null) throw new ArgumentNullException("fill");
            if (outline == null) throw new ArgumentNullException("outline");

            IEnumerable<Path2D> paths = convertToPaths(polygons);

            if (highlightFill == null) highlightFill = fill;
            if (selectFill == null) selectFill = fill;
            if (highlightOutline == null) highlightOutline = outline;
            if (selectOutline == null) selectOutline = outline;

			IEnumerable<TRenderObject> renderedObjects = VectorRenderer.RenderPaths(
                paths, fill, highlightFill, selectFill, outline, highlightOutline, selectOutline, renderState);

			return renderedObjects;
		}

		private static IEnumerable<Path2D> convertToPaths(IEnumerable<LineString> lines)
        {
            Path2D gp = new Path2D();

			foreach (LineString line in lines)
			{
                if (line.IsEmpty() || line.Vertices.Count <= 1)
				{
					continue;
				}

				gp.NewFigure(convertPoints(line.Vertices), false);
            }

            yield return gp;
		}

		private static IEnumerable<Path2D> convertToPaths(IEnumerable<Polygon> polygons)
        {
            Path2D gp = new Path2D();
			
            foreach (Polygon polygon in polygons)
			{
				if (polygon.IsEmpty())
				{
					continue;
				}

				// Add the exterior polygon
				gp.NewFigure(convertPoints(polygon.ExteriorRing.Vertices), true);

				// Add the interior polygons (holes)
				foreach (LinearRing ring in polygon.InteriorRings)
				{
					gp.NewFigure(convertPoints(ring.Vertices), true);
				}
            }

            yield return gp;
		}

		private static IEnumerable<Point2D> convertPoints(IEnumerable<Point> points)
		{
			foreach (Point geoPoint in points)
			{
				yield return new Point2D(geoPoint.X, geoPoint.Y);
			}
		}
		#endregion
	}
}
