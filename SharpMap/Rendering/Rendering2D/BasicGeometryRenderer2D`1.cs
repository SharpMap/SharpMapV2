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
using System.IO;
using System.Reflection;
using System.Threading;
using GeoAPI.Coordinates;
using SharpMap.Styles;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Layers;

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// A basic renderer which renders the geometric paths from feature geometry, 
    /// taking into account <see cref="Style">styles</see> and 
    /// <see cref="SharpMap.Rendering.Thematics.ITheme">themes</see>.
    /// </summary>
    /// <typeparam name="TRenderObject">Type of render object to generate.</typeparam>
    public class BasicGeometryRenderer2D<TRenderObject>
        : FeatureRenderer2D<GeometryStyle, TRenderObject>, IGeometryRenderer<Symbol2D, TRenderObject>
    {
        #region Type Members
        private static Object _defaultSymbol;
        private static readonly Object _defaultSymbolSync = new Object();

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
            : this(vectorRenderer, new GeometryStyle()) { }

        /// <summary>
        /// Creates a new BasicGeometryRenderer2D with the given VectorRenderer2D instance.
        /// </summary>
        /// <param name="vectorRenderer">
        /// A vector renderer.
        /// </param>
        /// <param name="defaultStyle"> 
        /// The default style to apply to a feature's geometry.
        /// </param>
        public BasicGeometryRenderer2D(VectorRenderer2D<TRenderObject> vectorRenderer, GeometryStyle defaultStyle)
            : base(vectorRenderer)
        {
            DefaultStyle = defaultStyle;
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
        protected override IEnumerable<TRenderObject> DoRenderFeature(IFeatureDataRecord feature, GeometryStyle style, RenderState renderState, ILayer layer)
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
        /// Renders a <see cref="IMultiLineString"/>.
        /// </summary>
        /// <param name="lines">IMultiLineString to be rendered.</param>
        /// <param name="fill">Pen used for filling (null or transparent for no filling).</param>
        /// <param name="highlightFill">Pen used for filling when highlighted.</param>
        /// <param name="selectFill">Pen used for filling when selected.</param>
        /// <param name="outline">Outline pen style (null if no outline).</param>
        /// <param name="highlightOutline">Outline pen style used when highlighted.</param>
        /// <param name="selectOutline">Outline pen style used when selected.</param>
        public virtual IEnumerable<TRenderObject> DrawMultiLineString(
            IMultiLineString lines, StylePen fill, StylePen highlightFill, StylePen selectFill,
            StylePen outline, StylePen highlightOutline, StylePen selectOutline, RenderState renderState)
        {
            if (lines == null) throw new ArgumentNullException("lines");

            return drawLineStrings(lines, fill, highlightFill, selectFill,
                outline, highlightOutline, selectOutline, renderState);
        }

        /// <summary>
        /// Renders a <see cref="ILineString"/>.
        /// </summary>
        /// <param name="line">ILineString to render.</param>
        /// <param name="fill">
        /// Pen used for filling (null or transparent for no filling).
        /// </param>
        /// <param name="highlightFill">
        /// Pen used for filling when highlighted.
        /// </param>
        /// <param name="selectFill">
        /// Pen used for filling when selected.
        /// </param>
        /// <param name="outline">
        /// Outline pen style (null if no outline).
        /// </param>
        /// <param name="highlightOutline">
        /// Outline pen style used when highlighted.
        /// </param>
        /// <param name="selectOutline">
        /// Outline pen style used when selected.
        /// </param>
        public virtual IEnumerable<TRenderObject> DrawLineString(
            ILineString line, StylePen fill, StylePen highlightFill, StylePen selectFill,
            StylePen outline, StylePen highlightOutline, StylePen selectOutline, RenderState renderState)
        {
            if (line == null) throw new ArgumentNullException("line");

            return drawLineStrings(new ILineString[] { line },
                fill, highlightFill, selectFill,
                outline, highlightOutline, selectOutline,
                renderState);
        }

        /// <summary>
        /// Renders a <see cref="IMultiPolygon"/>.
        /// </summary>
        /// <param name="multipolygon">IMultiPolygon to render.</param>
        /// <param name="fill">
        /// Brush used for filling (null or transparent for no filling).
        /// </param>
        /// <param name="highlightFill">
        /// Brush used for filling when highlighted.
        /// </param>
        /// <param name="selectFill">
        /// Brush used for filling when selected.
        /// </param>
        /// <param name="outline">
        /// Outline pen style (null if no outline).
        /// </param>
        /// <param name="highlightOutline">
        /// Outline pen style used when highlighted.
        /// </param>
        /// <param name="selectOutline">
        /// Outline pen style used when selected.
        /// </param>
        public virtual IEnumerable<TRenderObject> DrawMultiPolygon(IMultiPolygon multipolygon,
            StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill,
            StylePen outline, StylePen highlightOutline, StylePen selectOutline,
            RenderState renderState)
        {
            if (multipolygon == null) throw new ArgumentNullException("multipolygon");

            return drawPolygons(multipolygon, fill, highlightFill, selectFill,
                outline, highlightOutline, selectOutline, renderState);
        }

        /// <summary>
        /// Renders a <see cref="IPolygon"/>.
        /// </summary>
        /// <param name="polygon">IPolygon to render</param>
        /// <param name="fill">Brush used for filling (null or transparent for no filling).</param>
        /// <param name="highlightFill">Brush used for filling when highlighted.</param>
        /// <param name="selectFill">Brush used for filling when selected.</param>
        /// <param name="outline">Outline pen style (null if no outline).</param>
        /// <param name="highlightOutline">Outline pen style used when highlighted.</param>
        /// <param name="selectOutline">Outline pen style used when selected.</param>
        public virtual IEnumerable<TRenderObject> DrawPolygon(IPolygon polygon,
            StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill,
            StylePen outline, StylePen highlightOutline, StylePen selectOutline,
            RenderState renderState)
        {
            if (polygon == null) throw new ArgumentNullException("polygon");

            return drawPolygons(new IPolygon[] { polygon },
                fill, highlightFill, selectFill,
                outline, highlightOutline, selectOutline,
                renderState);
        }

        /// <summary>
        /// Renders a <see cref="IPoint"/>.
        /// </summary>
        /// <param name="point">IPoint to render.</param>
        /// <param name="symbol">Symbol to place over point.</param>
        /// <param name="highlightSymbol">
        /// Symbol to use for point when point is highlighted.
        /// </param>
        /// <param name="selectSymbol">
        /// Symbol to use for point when point is selected.
        /// </param>
        public virtual IEnumerable<TRenderObject> DrawPoint(IPoint point,
            Symbol2D symbol, Symbol2D highlightSymbol, Symbol2D selectSymbol,
            RenderState renderState)
        {
            if (point == null) throw new ArgumentNullException("point");

            return drawPoints(new IPoint[] { point },
                symbol, highlightSymbol, selectSymbol, renderState);
        }

        /// <summary>
        /// Renders a <see cref="IMultiPoint"/>.
        /// </summary>
        /// <param name="points">IMultiPoint to render.</param>
        /// <param name="symbol">Symbol to place over point.</param>
        /// <param name="highlightSymbol">
        /// Symbol to use for point when point is highlighted.
        /// </param>
        /// <param name="selectSymbol">
        /// Symbol to use for point when point is selected.
        /// </param>
        public virtual IEnumerable<TRenderObject> DrawMultiPoint(IMultiPoint points,
            Symbol2D symbol, Symbol2D highlightSymbol, Symbol2D selectSymbol,
            RenderState renderState)
        {
            if (points == null) throw new ArgumentNullException("points");

            return drawPoints(points,
                symbol, highlightSymbol, selectSymbol, renderState);
        }

        #region Private helper methods
        private IEnumerable<TRenderObject> renderGeometry(
            IGeometry geometry, GeometryStyle style, RenderState renderState)
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
            if (symbol == null) symbol = DefaultSymbol;
            if (highlightSymbol == null) highlightSymbol = symbol;
            if (selectSymbol == null) selectSymbol = symbol;

            return VectorRenderer.RenderSymbols(
                convertPoints(points),
                symbol,
                highlightSymbol,
                selectSymbol,
                renderState);
        }

        private IEnumerable<TRenderObject> drawLineStrings(IEnumerable<ILineString> lines,
            StylePen fill, StylePen highlightFill, StylePen selectFill,
            StylePen outline, StylePen highlightOutline, StylePen selectOutline,
            RenderState renderState)
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
        private IEnumerable<Path2D> convertToPaths(IEnumerable<ILineString> lines)
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

        private IEnumerable<Path2D> convertToPaths(IEnumerable<IPolygon> polygons)
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

        /// <summary>
        /// kbd4hire 20090318
        /// Move transformation to view coordindates here.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        private IEnumerable<Point2D> convertCoordinates(IEnumerable coordinates)
        {
            foreach (ICoordinate coordinate in coordinates)
            {
                yield return RenderTransform.TransformVector(
                    coordinate[Ordinates.X],
                    coordinate[Ordinates.Y]);
            }
        }

        /// <summary>
        /// kbd4hire 20090318
        /// Move transformation to view coordindates here.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private IEnumerable<Point2D> convertPoints(IEnumerable<IPoint> points)
        {
            foreach (IPoint2D point in points)
            {
                yield return RenderTransform.TransformVector(point.X, point.Y);
            }
        }
        #endregion
    }
}
