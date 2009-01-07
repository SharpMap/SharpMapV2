// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using System.Collections;
using System.Collections.Generic;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using NPack.Interfaces;
using SharpMap.Data;
using SharpMap.Layers;
using SharpMap.Symbology;

namespace SharpMap.Rendering
{
	/// <summary>
	/// A basic renderer which renders the geometric paths from feature geometry.
	/// </summary>
    public class BasicGeometryRenderer<TCoordinate> : FeatureRenderer<TCoordinate>//, IGeometryRenderer<TCoordinate>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
	{
		#region Type Members

		// DON'T remove - this eliminates the .beforefieldinit IL metadata
		static BasicGeometryRenderer() { }
		#endregion

        private ISymbol<TCoordinate> _defaultSymbol;

		#region Object construction and disposal
        /// <summary>
        /// Creates a new BasicGeometryRenderer2D with the given VectorRenderer2D instance.
        /// </summary>
        /// <param name="vectorRenderer">
        /// A vector renderer.
        /// </param>
        public BasicGeometryRenderer(VectorRenderer<TCoordinate> vectorRenderer, TextRenderer<TCoordinate> textRenderer)
            : this(vectorRenderer, textRenderer, new FeatureStyle()) { } 

		/// <summary>
		/// Creates a new BasicGeometryRenderer2D with the given VectorRenderer2D instance.
		/// </summary>
		/// <param name="vectorRenderer">
		/// A vector renderer.
		/// </param>
		/// <param name="defaultStyle"> 
		/// The default style to apply to a feature's geometry.
		/// </param>
		public BasicGeometryRenderer(VectorRenderer<TCoordinate> vectorRenderer, TextRenderer<TCoordinate> textRenderer, FeatureStyle defaultStyle)
			: base(vectorRenderer, textRenderer)
		{
            DefaultStyle = defaultStyle;
		}

		#region Dispose pattern
		/// <summary>
		/// Finalizer for BasicGeometryRenderer2D.
		/// </summary>
		~BasicGeometryRenderer()
		{
			Dispose(false);
		}
		#endregion
        #endregion

        /// <summary>
        /// The default basic symbol for rendering point data.
        /// </summary>
        public ISymbol<TCoordinate> DefaultSymbol
        {
            get
            {
                return _defaultSymbol;
            }
        }

		/// <summary>
		/// Renders the geometry of the <paramref name="feature"/>.
        /// </summary>
        /// <param name="scene">The <see cref="IScene"/> to render to.</param>
		/// <param name="feature">The feature to render.</param>
		/// <param name="style">The style to use to render the feature.</param>
		/// <returns>An enumeration of positioned render objects suitable for display.</returns>
		protected override void DoRenderFeature(IScene scene, ILayer layer, IFeatureDataRecord feature, FeatureStyle style, RenderState state)
		{
			if (feature == null) throw new ArgumentNullException("feature");
			if (style == null) throw new ArgumentNullException("style");

			if (feature.Geometry == null)
			{
				throw new InvalidOperationException("Feature must have a geometry to be rendered.");
			}

			return renderGeometry(feature.Geometry, style, state);
		}

		/// <summary>
		/// Renders a <see cref="IMultiLineString"/>.
		/// </summary>
        public virtual void DrawMultiLineString(IScene scene, IMultiLineString<TCoordinate> multiLineString, IPen stroke, Double perpendicularOffset, RenderState renderState)
		{
		    if (multiLineString == null) throw new ArgumentNullException("multiLineString");

            drawLineStrings(scene, multiLineString, symbolizer, renderState);
		}

	    /// <summary>
		/// Renders a <see cref="ILineString"/>.
		/// </summary>
        public virtual void DrawLineString(IScene scene, ILineString<TCoordinate> line, IPen stroke, Double perpendicularOffset, RenderState renderState)
		{
		    if (line == null) throw new ArgumentNullException("line");

		    drawLineStrings(scene, line, symbolizer, renderState);
		}

	    /// <summary>
		/// Renders a <see cref="IMultiPolygon"/>.
		/// </summary>
        public virtual void DrawMultiPolygon(IScene scene, IMultiPolygon<TCoordinate> multipolygon, IPen stroke, IBrush fill, Double perpendicularOffset, TCoordinate displacement, RenderState renderState)
	    {
	        if (multipolygon == null) throw new ArgumentNullException("multipolygon");

	        drawPolygons(scene, multipolygon, symbolizer, renderState);
	    }

	    /// <summary>
		/// Renders a <see cref="IPolygon"/>.
		/// </summary>
        public virtual void DrawPolygon(IScene scene, IPolygon<TCoordinate> polygon, IPen stroke, IBrush fill, Double perpendicularOffset, TCoordinate displacement, RenderState renderState)
	    {
	        if (polygon == null) throw new ArgumentNullException("polygon");

	        drawPolygons(scene, polygon , symbolizer, renderState);
	    }

	    /// <summary>
		/// Renders a <see cref="IPoint"/>.
		/// </summary>
        public virtual void DrawPoint(IScene scene, IPoint<TCoordinate> point, ISymbol<TCoordinate> graphic, RenderState renderState)
	    {
	        if (point == null) throw new ArgumentNullException("point");

	        drawPoints(scene, point, symbolizer, renderState);
	    }

	    /// <summary>
		/// Renders a <see cref="IMultiPoint"/>.
		/// </summary>
        public virtual void DrawMultiPoint(IScene scene, IMultiPoint<TCoordinate> multiPoint, ISymbol<TCoordinate> graphic, RenderState renderState)
	    {
	        if (multiPoint == null) throw new ArgumentNullException("multiPoint");

	        drawPoints(scene, multiPoint, symbolizer, renderState);
	    }

	    #region Private helper methods
		private void renderGeometry(IScene scene, IGeometry geometry, FeatureStyle style, RenderState renderState)
		{
			if (geometry is IPolygon)
			{
				return DrawPolygon(geometry as IPolygon, 
                    style.Fill, style.HighlightFill, style.SelectFill, 
                    style.Outline, style.HighlightOutline, style.SelectOutline, 
                    renderState);
			}
            
            if (geometry is IMultiPolygon)
			{
				return DrawMultiPolygon(geometry as IMultiPolygon, 
                    style.Fill, style.HighlightFill, style.SelectFill,
                    style.Outline, style.HighlightOutline, style.SelectOutline, 
                    renderState);
			}
            
            if (geometry is ILineString)
			{
				return DrawLineString(geometry as ILineString, 
                    style.Line, style.HighlightLine, style.SelectLine,
                    style.Outline, style.HighlightOutline, style.SelectOutline, 
                    renderState);
			}
            
            if (geometry is IMultiLineString)
			{
				return DrawMultiLineString(geometry as IMultiLineString, 
                    style.Line, style.HighlightLine, style.SelectLine,
                    style.Outline, style.HighlightOutline, style.SelectOutline, 
                    renderState);
			}
            
            if (geometry is IPoint)
			{
                return DrawPoint(geometry as IPoint, 
                    style.Symbol, style.HighlightSymbol, style.SelectSymbol, 
                    renderState);
			}
            
            if (geometry is IMultiPoint)
			{
                return DrawMultiPoint(geometry as IMultiPoint, 
                    style.Symbol, style.HighlightSymbol, style.SelectSymbol, 
                    renderState);
			}
            
            if (geometry is IGeometryCollection)
			{
				//List<TRenderObject> renderObjects = new List<TRenderObject>();

                //foreach (IGeometry g in (geometry as IGeometryCollection))
                //{
                //    renderObjects.AddRange(renderGeometry(g, style, renderState));
                //}

				//return renderObjects;

			    return enumerateGeoCollection(geometry as IGeometryCollection, style, renderState);
			}

			throw new NotSupportedException(
                String.Format("IGeometry type is not supported: {0}", geometry.GetType()));
		}

        private IEnumerable<TRenderObject> enumerateGeoCollection(
            IGeometryCollection collection, GeometryStyle style, RenderState renderState)
        {
            foreach (IGeometry geometry in collection)
            {
                IEnumerable<TRenderObject> rendering 
                    = renderGeometry(geometry, style, renderState);

                foreach (TRenderObject renderObject in rendering)
                {
                    yield return renderObject;
                }
            }
        }

		private IEnumerable<TRenderObject> drawPoints(IEnumerable<IPoint> points, 
            Symbol2D symbol, Symbol2D highlightSymbol, Symbol2D selectSymbol, 
            RenderState renderState)
        {
            // If we have a null symbol, use the default
			if (symbol == null)  symbol = DefaultSymbol;
            if (highlightSymbol == null) highlightSymbol = symbol;
            if (selectSymbol == null) selectSymbol = symbol;

            return VectorRenderer.RenderSymbols(
                convertPoints(points), 
                symbol, 
                highlightSymbol, 
                selectSymbol, 
                renderState);
		}

		private void drawLineStrings(IScene scene, Object lines, LineSymbolizer symbolizer, RenderState renderState)
		{
            if (fill == null) throw new ArgumentNullException("fill");

		    IEnumerable<Path2D> paths = convertToPaths(lines);

            if (highlightFill == null) highlightFill = fill;
            if (selectFill == null) selectFill = fill;
		    if (highlightOutline == null) highlightOutline = outline;
            if (selectOutline == null) selectOutline = outline;

            IEnumerable<TRenderObject> renderedObjects = VectorRenderer.RenderPaths(paths, 
                fill, highlightFill, selectFill, outline, highlightOutline, selectOutline, 
                renderState);

            foreach (TRenderObject ro in renderedObjects)
            {
                yield return ro;
            }
		}

		private IEnumerable<TRenderObject> drawPolygons(IEnumerable<IPolygon> polygons,
            StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill, 
            StylePen outline, StylePen highlightOutline, StylePen selectOutline, 
            RenderState renderState)
        {
            if (fill == null) throw new ArgumentNullException("fill");
            if (outline == null) throw new ArgumentNullException("outline");

            IEnumerable<Path2D> paths = convertToPaths(polygons);

            if (highlightFill == null) highlightFill = fill;
            if (selectFill == null) selectFill = fill;
            if (highlightOutline == null) highlightOutline = outline;
            if (selectOutline == null) selectOutline = outline;

            IEnumerable<TRenderObject> renderedObjects = VectorRenderer.RenderPaths(
                paths, fill, highlightFill, selectFill, outline, 
                highlightOutline, selectOutline, renderState);

			return renderedObjects;
		}

        // TODO: these next two methods would benefit from handling TCoordinates
		private static IEnumerable<Path2D> convertToPaths(IEnumerable<ILineString> lines)
        {
            Path2D gp = new Path2D();

			foreach (ILineString line in lines)
			{
                if (line.IsEmpty || line.PointCount <= 1)
				{
					continue;
				}

				gp.NewFigure(convertCoordinates(line.Coordinates), false);
            }

            yield return gp;
		}

		private static IEnumerable<Path2D> convertToPaths(IEnumerable<IPolygon> polygons)
        {
            Path2D gp = new Path2D();
			
            foreach (IPolygon polygon in polygons)
			{
				if (polygon.IsEmpty)
				{
					continue;
				}

				// Add the exterior polygon
				gp.NewFigure(convertCoordinates(polygon.ExteriorRing.Coordinates), true);

				// Add the interior polygons (holes)
				foreach (ILinearRing ring in polygon.InteriorRings)
				{
					gp.NewFigure(convertCoordinates(ring.Coordinates), true);
				}
            }

            yield return gp;
		}

// TODO: put transform shortcut bug fix here?
        private static IEnumerable<Point2D> convertCoordinates(IEnumerable coordinates)
		{
            foreach (ICoordinate coordinate in coordinates)
			{
                yield return new Point2D(
                    coordinate[Ordinates.X], 
                    coordinate[Ordinates.Y]);
			}
        }

        private IEnumerable<Point2D> convertPoints(IEnumerable<IPoint> points)
        {
            foreach (IPoint2D point in points)
            {
                yield return new Point2D(point.X, point.Y);
            }
        }
		#endregion
	}
}
