// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
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

// SOURCECODE IS MODIFIED FROM ANOTHER WORK AND IS ORIGINALLY BASED ON GeoTools.NET:
/*
 *  Copyright (C) 2002 Urban Science Applications, Inc. 
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 */

using System;
using System.IO;
using GeoAPI.Geometries;

namespace SharpMap.Converters.WellKnownText
{
    /// <summary>
    /// Outputs the textual representation of a <see cref="SharpMap.Geometries.Geometry"/> instance.
    /// </summary>
    /// <remarks>
    /// <para>The Well-Known Text (WKT) representation of Geometry is designed to exchange geometry data in ASCII form.</para>
    /// Examples of WKT representations of geometry objects are:
    /// <list type="table">
    /// <listheader><term>Geometry </term><description>WKT Representation</description></listheader>
    /// <item><term>A Point</term>
    /// <description>POINT(15 20)<br/> Note that point coordinates are specified with no separating comma.</description></item>
    /// <item><term>A LineString with four points:</term>
    /// <description>LINESTRING(0 0, 10 10, 20 25, 50 60)</description></item>
    /// <item><term>A Polygon with one exterior ring and one interior ring:</term>
    /// <description>POLYGON((0 0,10 0,10 10,0 10,0 0),(5 5,7 5,7 7,5 7, 5 5))</description></item>
    /// <item><term>A MultiPoint with three Point values:</term>
    /// <description>MULTIPOINT(0 0, 20 20, 60 60)</description></item>
    /// <item><term>A MultiLineString with two LineString values:</term>
    /// <description>MULTILINESTRING((10 10, 20 20), (15 15, 30 15))</description></item>
    /// <item><term>A MultiPolygon with two Polygon values:</term>
    /// <description>MULTIPOLYGON(((0 0,10 0,10 10,0 10,0 0)),((5 5,7 5,7 7,5 7, 5 5)))</description></item>
    /// <item><term>A GeometryCollection consisting of two Point values and one LineString:</term>
    /// <description>GEOMETRYCOLLECTION(POINT(10 10), POINT(30 30), LINESTRING(15 15, 20 20))</description></item>
    /// </list>
    /// </remarks>
    public class GeometryToWkt
    {
        #region Methods

        /// <summary>
        /// Converts a Geometry to its Well-Known Text representation.
        /// </summary>
        /// <param name="geometry">A Geometry to write.</param>
        /// <returns>A &lt;Geometry Tagged Text&gt; String (see the OpenGIS Simple
        ///  Features Specification)</returns>
        public static String Write(IGeometry geometry)
        {
            StringWriter sw = new StringWriter();
            Write(geometry, sw);
            return sw.ToString();
        }

        /// <summary>
        /// Converts a Geometry to its Well-Known Text representation.
        /// </summary>
        /// <param name="geometry">A geometry to process.</param>
        /// <param name="writer">Stream to write out the geometry's text representation.</param>
        /// <remarks>
        /// Geometry is written to the output stream as &lt;Gemoetry Tagged Text&gt; String (see the OpenGIS
        /// Simple Features Specification).
        /// </remarks>
        public static void Write(IGeometry geometry, StringWriter writer)
        {
            appendGeometryTaggedText(geometry, writer);
        }

        /// <summary>
        /// Converts a Geometry to &lt;Geometry Tagged Text &gt; format, then Appends it to the writer.
        /// </summary>
        /// <param name="geometry">The Geometry to process.</param>
        /// <param name="writer">The output stream to Append to.</param>
        private static void appendGeometryTaggedText(IGeometry geometry, StringWriter writer)
        {
            if (geometry == null) throw new ArgumentNullException("geometry");

            if (geometry is Point)
            {
                Point point = geometry as Point;
                appendPointTaggedText(point, writer);
            }
            else if (geometry is LineString)
            {
                appendLineStringTaggedText(geometry as LineString, writer);
            }
            else if (geometry is Polygon)
            {
                appendPolygonTaggedText(geometry as Polygon, writer);
            }
            else if (geometry is MultiPoint)
            {
                appendMultiPointTaggedText(geometry as MultiPoint, writer);
            }
            else if (geometry is MultiLineString)
            {
                appendMultiLineStringTaggedText(geometry as MultiLineString, writer);
            }
            else if (geometry is MultiPolygon)
            {
                appendMultiPolygonTaggedText(geometry as MultiPolygon, writer);
            }
            else if (geometry is GeometryCollection)
            {
                appendGeometryCollectionTaggedText(geometry as GeometryCollection, writer);
            }
            else
            {
                throw new NotSupportedException("Unsupported Geometry implementation:" + geometry.GetType().Name);
            }
        }

        /// <summary>
        /// Converts a Coordinate to &lt;Point Tagged Text&gt; format,
        /// then Appends it to the writer.
        /// </summary>
        /// <param name="coordinate">the <code>Coordinate</code> to process</param>
        /// <param name="writer">the output writer to Append to</param>
        private static void appendPointTaggedText(Point coordinate, StringWriter writer)
        {
            writer.Write("POINT ");
            appendPointText(coordinate, writer);
        }

        /// <summary>
        /// Converts a LineString to LineString tagged text format, 
        /// </summary>
        /// <param name="lineString">The LineString to process.</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void appendLineStringTaggedText(LineString lineString, StringWriter writer)
        {
            writer.Write("LINESTRING ");
            appendLineStringText(lineString, writer);
        }

        /// <summary>
        ///  Converts a Polygon to &lt;Polygon Tagged Text&gt; format,
        ///  then Appends it to the writer.
        /// </summary>
        /// <param name="polygon">Th Polygon to process.</param>
        /// <param name="writer">The stream writer to Append to.</param>
        private static void appendPolygonTaggedText(Polygon polygon, StringWriter writer)
        {
            writer.Write("POLYGON ");
            appendPolygonText(polygon, writer);
        }

        /// <summary>
        /// Converts a MultiPoint to &lt;MultiPoint Tagged Text&gt;
        /// format, then Appends it to the writer.
        /// </summary>
        /// <param name="multipoint">The MultiPoint to process.</param>
        /// <param name="writer">The output writer to Append to.</param>
        private static void appendMultiPointTaggedText(MultiPoint multipoint, StringWriter writer)
        {
            writer.Write("MULTIPOINT ");
            appendMultiPointText(multipoint, writer);
        }

        /// <summary>
        /// Converts a MultiLineString to &lt;MultiLineString Tagged
        /// Text&gt; format, then Appends it to the writer.
        /// </summary>
        /// <param name="multiLineString">The MultiLineString to process</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void appendMultiLineStringTaggedText(MultiLineString multiLineString, StringWriter writer)
        {
            writer.Write("MULTILINESTRING ");
            appendMultiLineStringText(multiLineString, writer);
        }

        /// <summary>
        /// Converts a MultiPolygon to &lt;MultiPolygon Tagged
        /// Text&gt; format, then Appends it to the writer.
        /// </summary>
        /// <param name="multiPolygon">The MultiPolygon to process</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void appendMultiPolygonTaggedText(MultiPolygon multiPolygon, StringWriter writer)
        {
            writer.Write("MULTIPOLYGON ");
            AppendMultiPolygonText(multiPolygon, writer);
        }

        /// <summary>
        /// Converts a GeometryCollection to &lt;GeometryCollection Tagged
        /// Text&gt; format, then Appends it to the writer.
        /// </summary>
        /// <param name="geometryCollection">The GeometryCollection to process</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void appendGeometryCollectionTaggedText(GeometryCollection geometryCollection,
                                                               StringWriter writer)
        {
            writer.Write("GEOMETRYCOLLECTION ");
            appendGeometryCollectionText(geometryCollection, writer);
        }


        /// <summary>
        /// Converts a Coordinate to Point Text format then Appends it to the writer.
        /// </summary>
        /// <param name="coordinate">The Coordinate to process.</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void appendPointText(Point coordinate, StringWriter writer)
        {
            if (coordinate == null || coordinate.IsEmpty())
            {
                writer.Write("EMPTY");
            }
            else
            {
                writer.Write("(");
                appendCoordinate(coordinate, writer);
                writer.Write(")");
            }
        }

        /// <summary>
        /// Converts a Coordinate to &lt;Point&gt; format, then Appends
        /// it to the writer. 
        /// </summary>
        /// <param name="coordinate">The Coordinate to process.</param>
        /// <param name="writer">The output writer to Append to.</param>
        private static void appendCoordinate(Point coordinate, StringWriter writer)
        {
            for (UInt32 i = 0; i < coordinate.NumOrdinates; i++)
            {
                writer.Write(writeNumber(coordinate[i]) + (i < coordinate.NumOrdinates - 1 ? " " : ""));
            }
        }

        /// <summary>
        /// Converts a Double to a String, not in scientific notation.
        /// </summary>
        /// <param name="d">The Double to convert.</param>
        /// <returns>The Double as a String, not in scientific notation.</returns>
        private static String writeNumber(Double d)
        {
            return d.ToString(WktStreamTokenizer.NumberFormat_enUS);
        }

        /// <summary>
        /// Converts a LineString to &lt;LineString Text&gt; format, then
        /// Appends it to the writer.
        /// </summary>
        /// <param name="lineString">The LineString to process.</param>
        /// <param name="writer">The output stream to Append to.</param>
        private static void appendLineStringText(LineString lineString, StringWriter writer)
        {
            if (lineString == null || lineString.IsEmpty())
            {
                writer.Write("EMPTY");
            }
            else
            {
                writer.Write("(");

                for (Int32 i = 0; i < lineString.NumPoints; i++)
                {
                    if (i > 0)
                    {
                        writer.Write(", ");
                    }

                    appendCoordinate(lineString.Vertices[i], writer);
                }

                writer.Write(")");
            }
        }

        /// <summary>
        /// Converts a Polygon to &lt;Polygon Text&gt; format, then
        /// Appends it to the writer.
        /// </summary>
        /// <param name="polygon">The Polygon to process.</param>
        /// <param name="writer"></param>
        private static void appendPolygonText(Polygon polygon, StringWriter writer)
        {
            if (polygon == null || polygon.IsEmpty())
            {
                writer.Write("EMPTY");
            }
            else
            {
                writer.Write("(");
                appendLineStringText(polygon.ExteriorRing, writer);

                for (Int32 i = 0; i < polygon.InteriorRings.Count; i++)
                {
                    writer.Write(", ");
                    appendLineStringText(polygon.InteriorRings[i], writer);
                }

                writer.Write(")");
            }
        }

        /// <summary>
        /// Converts a MultiPoint to &lt;MultiPoint Text&gt; format, then
        /// Appends it to the writer.
        /// </summary>
        /// <param name="multiPoint">The MultiPoint to process.</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void appendMultiPointText(MultiPoint multiPoint, StringWriter writer)
        {
            if (multiPoint == null || multiPoint.IsEmpty())
            {
                writer.Write("EMPTY");
            }
            else
            {
                writer.Write("(");

                for (Int32 i = 0; i < multiPoint.Points.Count; i++)
                {
                    if (i > 0)
                    {
                        writer.Write(", ");
                    }
                    appendCoordinate(multiPoint[i], writer);
                }

                writer.Write(")");
            }
        }

        /// <summary>
        /// Converts a MultiLineString to &lt;MultiLineString Text&gt;
        /// format, then Appends it to the writer.
        /// </summary>
        /// <param name="multiLineString">The MultiLineString to process.</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void appendMultiLineStringText(MultiLineString multiLineString, StringWriter writer)
        {
            if (multiLineString == null || multiLineString.IsEmpty())
            {
                writer.Write("EMPTY");
            }
            else
            {
                writer.Write("(");

                for (Int32 i = 0; i < multiLineString.LineStrings.Count; i++)
                {
                    if (i > 0)
                    {
                        writer.Write(", ");
                    }
                    appendLineStringText(multiLineString[i], writer);
                }

                writer.Write(")");
            }
        }

        /// <summary>
        /// Converts a MultiPolygon to &lt;MultiPolygon Text&gt; format, then Appends to it to the writer.
        /// </summary>
        /// <param name="multiPolygon">The MultiPolygon to process.</param>
        /// <param name="writer">The output stream to Append to.</param>
        private static void AppendMultiPolygonText(MultiPolygon multiPolygon, StringWriter writer)
        {
            if (multiPolygon == null || multiPolygon.IsEmpty())
            {
                writer.Write("EMPTY");
            }
            else
            {
                writer.Write("(");

                for (Int32 i = 0; i < multiPolygon.Polygons.Count; i++)
                {
                    if (i > 0)
                    {
                        writer.Write(", ");
                    }
                    appendPolygonText(multiPolygon[i], writer);
                }

                writer.Write(")");
            }
        }

        /// <summary>
        /// Converts a GeometryCollection to &lt;GeometryCollection Text &gt; format, then Appends it to the writer.
        /// </summary>
        /// <param name="geometryCollection">The GeometryCollection to process.</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void appendGeometryCollectionText(GeometryCollection geometryCollection, StringWriter writer)
        {
            if (geometryCollection == null || geometryCollection.IsEmpty())
            {
                writer.Write("EMPTY");
            }
            else
            {
                writer.Write("(");

                for (Int32 i = 0; i < geometryCollection.Collection.Count; i++)
                {
                    if (i > 0)
                    {
                        writer.Write(", ");
                    }
                    appendGeometryTaggedText(geometryCollection[i], writer);
                }

                writer.Write(")");
            }
        }

        #endregion
    }
}