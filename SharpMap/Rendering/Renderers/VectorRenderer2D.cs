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
using System.Text;

using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Styles;
using SharpMap.Rendering.Thematics;
using SharpMap.CoordinateSystems.Transformations;

namespace SharpMap.Rendering
{
    public abstract class VectorRenderer2D<TRenderObject> : BaseFeatureRenderer2D<VectorStyle, TRenderObject>, IVectorRenderer2D<TRenderObject>
    {
        private static Symbol2D _defaultSymbol;

        static VectorRenderer2D()
        {
        }

        public static Symbol2D DefaultSymbol
        {
            get 
            {
                if (_defaultSymbol == null)
                {
                    lock (_defaultSymbol)
                    {
                        if (_defaultSymbol == null)
                        {
                            Stream data = Assembly.GetExecutingAssembly().GetManifestResourceStream("SharpMap.Styles.DefaultSymbol.png");
                            _defaultSymbol = new Symbol2D(data, new ViewSize2D(16, 16));
                        }
                    }
                }

                return _defaultSymbol; 
            }
        }

        public VectorRenderer2D()
        {
            this.StyleRenderingMode = StyleRenderingMode.AntiAlias;
        }

        #region IVectorLayerRenderer Members
        public abstract TRenderObject RenderPath(GraphicsPath2D path, StylePen outline, StylePen highlightOutline, StylePen selectOutline);
        public abstract TRenderObject RenderPath(GraphicsPath2D path, StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill, StylePen outline, StylePen highlightOutline, StylePen selectOutline);
        public abstract TRenderObject RenderSymbol(ViewPoint2D location, Symbol2D symbolData);
        public abstract TRenderObject RenderSymbol(ViewPoint2D location, Symbol2D symbolData, ColorMatrix highlight, ColorMatrix select);
        public abstract TRenderObject RenderSymbol(ViewPoint2D location, Symbol2D symbolData, Symbol2D highlightSymbolData, Symbol2D selectSymbolData);
        #endregion

        protected override IEnumerable<PositionedRenderObject2D<TRenderObject>> DoRenderGeometry(IGeometry geometry, VectorStyle style)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException("geometry");
            }

            if (style == null)
            {
                throw new ArgumentNullException("style");
            }

            StylePen outline = null, highlightOutline = null, selectOutline = null;

            if (style.EnableOutline)
            {
                outline = style.Outline;
                highlightOutline = style.HighlightOutline;
                selectOutline = style.SelectOutline;
            }

            if (geometry is Polygon)
            {
                return DrawPolygon(geometry as Polygon, style.Fill, style.HighlightFill, style.SelectFill, style.Outline, style.HighlightOutline, style.SelectOutline);
            }
            else if (geometry is MultiPolygon)
            {
                return DrawMultiPolygon(geometry as MultiPolygon, style.Fill, style.HighlightFill, style.SelectFill, style.Outline, style.HighlightOutline, style.SelectOutline);
            }
            else if (geometry is LineString)
            {
                return DrawLineString(geometry as LineString, style.Line, style.HighlightLine, style.SelectLine, style.Outline, style.HighlightOutline, style.SelectOutline);
            }
            else if (geometry is MultiLineString)
            {
                return DrawMultiLineString(geometry as MultiLineString, style.Line, style.HighlightLine, style.SelectLine, style.Outline, style.HighlightOutline, style.SelectOutline);
            }
            else if (geometry is Point)
            {
                return DrawPoint(geometry as Point, style.Symbol, style.HighlightSymbol, style.SelectSymbol);
            }
            else if (geometry is MultiPoint)
            {
                return DrawMultiPoint(geometry as MultiPoint, style.Symbol, style.HighlightSymbol, style.SelectSymbol);
            }
            else if (geometry is GeometryCollection)
            {
                foreach (Geometry g in (geometry as GeometryCollection))
                {
                    return RenderGeometry(g, style);
                }
            }

            throw new NotSupportedException(String.Format("Geometry type is not supported: {0}", geometry.GetType()));
        }

        /// <summary>
        /// Renders a <see cref="MultiLineString"/> to the <paramref name="view"/>.
        /// </summary>
        /// <param name="view">View to draw on.</param>
        /// <param name="lines">MultiLineString to be rendered.</param>
        /// <param name="pen">Pen style used for rendering.</param>
        protected IEnumerable<PositionedRenderObject2D<TRenderObject>> DrawMultiLineString(MultiLineString lines, StylePen fill, StylePen highlightFill, StylePen selectFill, 
            StylePen outline, StylePen highlightOutline, StylePen selectOutline)
        {
            return drawLineStrings(lines.LineStrings, fill, highlightFill, selectFill, outline, highlightOutline, selectOutline);
        }

        /// <summary>
        /// Renders a <see cref="LineString"/> to the <paramref name="view"/>.
        /// </summary>
        /// <param name="view">View to draw on.</param>
        /// <param name="line">LineString to render.</param>
        /// <param name="fill">Pen used for filling (null or transparent for no filling).</param>
        /// <param name="highlightFill">Pen used for filling when highlighted.</param>
        /// <param name="selectFill">Pen used for filling when selected.</param>
        /// <param name="outline">Outline pen style (null if no outline).</param>
        /// <param name="highlightOutline">Outline pen style used when highlighted.</param>
        /// <param name="selectOutline">Outline pen style used when selected.</param>
        protected IEnumerable<PositionedRenderObject2D<TRenderObject>> DrawLineString(LineString line, StylePen fill, StylePen highlightFill, StylePen selectFill,
            StylePen outline, StylePen highlightOutline, StylePen selectOutline)
        {
            return drawLineStrings(new LineString[] { line }, fill, highlightFill, selectFill, outline, highlightOutline, selectOutline);
        }

        /// <summary>
        /// Renders a <see cref="MultiPolygon"/> to the <paramref name="view"/>.
        /// </summary>
        /// <param name="view">View to draw on.</param>
        /// <param name="multipolygon">MultiPolygon to render.</param>
        /// <param name="fill">Brush used for filling (null or transparent for no filling).</param>
        /// <param name="highlightFill">Brush used for filling when highlighted.</param>
        /// <param name="selectFill">Brush used for filling when selected.</param>
        /// <param name="outline">Outline pen style (null if no outline).</param>
        /// <param name="highlightOutline">Outline pen style used when highlighted.</param>
        /// <param name="selectOutline">Outline pen style used when selected.</param>
        protected IEnumerable<PositionedRenderObject2D<TRenderObject>> DrawMultiPolygon(MultiPolygon multipolygon, StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill, StylePen outline, StylePen highlightOutline, StylePen selectOutline)
        {
            return drawPolygons(multipolygon.Polygons, fill, highlightFill, selectFill, outline, highlightOutline, selectOutline);
        }

        /// <summary>
        /// Renders a <see cref="Polygon"/> to the <paramref name="view"/>.
        /// </summary>
        /// <param name="view">View to draw on.</param>
        /// <param name="polygon">Polygon to render</param>
        /// <param name="fill">Brush used for filling (null or transparent for no filling).</param>
        /// <param name="highlightFill">Brush used for filling when highlighted.</param>
        /// <param name="selectFill">Brush used for filling when selected.</param>
        /// <param name="outline">Outline pen style (null if no outline).</param>
        /// <param name="highlightOutline">Outline pen style used when highlighted.</param>
        /// <param name="selectOutline">Outline pen style used when selected.</param>
        protected IEnumerable<PositionedRenderObject2D<TRenderObject>> DrawPolygon(Polygon polygon, StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill, StylePen outline, StylePen highlightOutline, StylePen selectOutline)
        {
            return drawPolygons(new Polygon[] { polygon }, fill, highlightFill, selectFill, outline, highlightOutline, selectOutline);
        }

        /// <summary>
        /// Renders a point to the <paramref name="view"/>.
        /// </summary>
        /// <param name="view">View to draw on.</param>
        /// <param name="point">Point to render.</param>
        /// <param name="symbol">Symbol to place over point.</param>
        /// <param name="highlightSymbol">Symbol to use for point when point is highlighted.</param>
        /// <param name="selectSymbol">Symbol to use for point when point is selected.</param>
        protected IEnumerable<PositionedRenderObject2D<TRenderObject>> DrawPoint(Point point, Symbol2D symbol, Symbol2D highlightSymbol, Symbol2D selectSymbol)
        {
            return drawPoints(new Point[] { point }, symbol, highlightSymbol, selectSymbol);
        }

        /// <summary>
        /// Renders a <see cref="SharpMap.Geometries.MultiPoint"/> to the <paramref name="view"/>.
        /// </summary>
        /// <param name="view">View to draw on.</param>
        /// <param name="points">MultiPoint to render.</param>
        /// <param name="symbol">Symbol to place over point.</param>
        /// <param name="highlightSymbol">Symbol to use for point when point is highlighted.</param>
        /// <param name="selectSymbol">Symbol to use for point when point is selected.</param>
        protected IEnumerable<PositionedRenderObject2D<TRenderObject>> DrawMultiPoint(MultiPoint points, Symbol2D symbol, Symbol2D highlightSymbol, Symbol2D selectSymbol)
        {
            return drawPoints(points.Points, symbol, highlightSymbol, selectSymbol);
        }

        private IEnumerable<PositionedRenderObject2D<TRenderObject>> drawPoints(IEnumerable<Point> points, Symbol2D symbol, Symbol2D highlightSymbol, Symbol2D selectSymbol)
        {
            foreach (Point point in points)
            {
                if (point == null)
                {
                    continue;
                }

                if (symbol == null) //We have no point symbol - Use a default symbol
                {
                    symbol = DefaultSymbol;
                }

                ViewPoint2D pointLocation = new ViewPoint2D(ViewTransform.Transform(point.X, point.Y));
                TRenderObject renderedObject = RenderSymbol(pointLocation, symbol, highlightSymbol, selectSymbol);
                yield return new PositionedRenderObject2D<TRenderObject>(pointLocation, renderedObject);
            }
        }

        private IEnumerable<PositionedRenderObject2D<TRenderObject>> drawLineStrings(IEnumerable<LineString> lines, StylePen fill, StylePen highlightFill, StylePen selectFill,
            StylePen outline, StylePen highlightOutline, StylePen selectOutline)
        {
            GraphicsPath2D gp = new GraphicsPath2D();
            foreach (LineString line in lines)
            {
                gp.NewFigure(convertPointsAndTransform(line.Vertices), false);
            }

            TRenderObject renderedObject;

            if (outline != null && highlightOutline != null && selectOutline != null)
            {
                renderedObject = RenderPath(gp, outline, highlightOutline, selectOutline);
                yield return new PositionedRenderObject2D<TRenderObject>(gp.Bounds.Location, renderedObject);
            }

            renderedObject = RenderPath(gp, fill, highlightFill, selectFill);
            yield return new PositionedRenderObject2D<TRenderObject>(gp.Bounds.Location, renderedObject);
        }

        private IEnumerable<ViewPoint2D> convertPointsAndTransform(IEnumerable<Point> points)
        {
            foreach (Point geoPoint in points)
	        {
                yield return new ViewPoint2D(ViewTransform.Transform(geoPoint.X, geoPoint.Y));
	        }
        }

        private IEnumerable<PositionedRenderObject2D<TRenderObject>> drawPolygons(IEnumerable<Polygon> polygons, StyleBrush fill, StyleBrush highlightFill, StyleBrush selectFill, StylePen outline, StylePen highlightOutline, StylePen selectOutline)
        {
            foreach (Polygon polygon in polygons)
            {
                if (polygon.ExteriorRing == null)
                {
                    continue;
                }

                if (polygon.ExteriorRing.Vertices.Count <= 1)
                {
                    continue;
                }

                GraphicsPath2D gp = new GraphicsPath2D();

                // Add the exterior polygon
                gp.NewFigure(convertPointsAndTransform(polygon.ExteriorRing.Vertices), true);

                // Add the interior polygons (holes)
                foreach (LinearRing ring in polygon.InteriorRings)
                {
                    gp.NewFigure(convertPointsAndTransform(ring.Vertices), true);
                }

                TRenderObject renderedObject;
                renderedObject = RenderPath(gp, fill, highlightFill, selectFill, outline, highlightOutline, selectOutline);
                yield return new PositionedRenderObject2D<TRenderObject>(gp.Bounds.Location, renderedObject);
            }
        }

        //public void Render(BoundingBox region)
        //{
        //    VectorLayer layer = Layer as VectorLayer;

        //    if (layer.CoordinateTransformation != null)
        //        region = GeometryTransform.TransformBox(region, layer.CoordinateTransformation.MathTransform.Inverse());

        //    if (layer.DataSource == null)
        //        throw new InvalidOperationException("DataSource property not set on layer '" + layer.LayerName + "'");

        //    IEnumerable<FeatureDataRow> features = layer.GetFeatures(region);

        //    //If thematics is enabled, we use a slighty different rendering approach
        //    if (this.Theme != null)
        //    {
        //        foreach (FeatureDataRow feature in features)
        //        {
        //            VectorStyle style = Theme.GetStyle(feature) as VectorStyle;
        //            RenderGeometry(view, feature.Geometry, style);
        //        }
        //    }
        //    else
        //    {
        //        foreach (FeatureDataRow feature in features)
        //        {
        //            Geometry g = feature.Geometry;
        //            if (layer.CoordinateTransformation != null)
        //                g = GeometryTransform.TransformGeometry(g, layer.CoordinateTransformation.MathTransform);

        //            if (g != null)
        //                RenderGeometry(view, g, Style);
        //        }
        //    }

        //    // Raises LayerRendered event
        //    //base.Render(view, region);
        //}
    }
}
