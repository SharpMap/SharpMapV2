/*
 *	This file is part of SharpMap.Rendering.GeoJson
 *  SharpMap.Rendering.GeoJson is free software © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Styles;
using SharpMap.Utilities.SridUtility;

namespace SharpMap.Rendering.GeoJson
{
    /*
     * 
     * JD: Based on GeoJSON Format Spec 1.0, 16 June 2008
     * 
     * 
     */

    public static class GeoJsonFeatureWriter
    {
        public static string WriteFeature(IGeoJsonFeatureStyle style, IFeatureDataRecord feature)
        {
            StringBuilder sb = new StringBuilder();
            WriteFeature(new StringWriter(sb), style, feature);
            return sb.ToString();
        }

        public static void WriteFeature(TextWriter writer, IGeoJsonFeatureStyle style, IFeatureDataRecord feature)
        {
            IGeometry g = style.PreProcessGeometries
                              ? style.GeometryPreProcessor(feature.Geometry)
                              : feature.Geometry;
            writer.Write("{");
            writer.Write("\"type\":\"Feature\",");
            if (feature.HasOid)
            {
                writer.Write(JsonUtility.FormatJsonAttribute("id", feature.GetOid()));
                writer.Write(",");
            }

            writer.Write("\"properties\":");
            WriteFeatureAttributes(writer, style, feature);
            writer.Write(",");

            if (style.IncludeBBox)
            {
                writer.Write("\"bbox\":");
                WriteExtents(writer, g.Extents, style.CoordinateNumberFormatString);
                writer.Write(",");
            }

            /* Ideally this should be higher up e.g at the layer level but we dont currently 
             * have anywhere where we can group features by layer. 
             * It still complies with the spec but will be considerably more verbose */

            if (g.SpatialReference != null || !string.IsNullOrEmpty(g.Srid))
            {
                writer.Write("\"crs\":");
                WriteNamedCrs(writer,
                              g.SpatialReference != null
                                  ? g.SpatialReference.Name
                                  : "EPSG:" + SridMap.DefaultInstance.Process(g.SpatialReference, 0));
                writer.Write(",");
            }

            writer.Write("\"geometry\":");
            WriteGeometry(writer, g, style.CoordinateNumberFormatString);

            writer.Write("}");
            writer.WriteLine();
        }

        private static void WriteNamedCrs(TextWriter writer, string name)
        {
            writer.Write("{");
            writer.Write(JsonUtility.FormatJsonAttribute("type", "name"));
            writer.Write(",");
            writer.Write("\"properties\":");
            writer.Write("{");
            writer.Write(JsonUtility.FormatJsonAttribute("name", name));
            writer.Write("}");
            writer.Write("}");
        }

        public static string WriteGeometry(IGeometry g, string coordinateNumberFormatString)
        {
            StringBuilder sb = new StringBuilder();
            WriteGeometry(new StringWriter(sb), g, coordinateNumberFormatString);
            return sb.ToString();
        }

        public static void WriteGeometry(TextWriter writer, IGeometry g, string coordinateNumberFormatString)
        {
            writer.Write("{");
            writer.Write(JsonUtility.FormatJsonAttribute("type", GetGeometryType(g)));
            writer.Write(",");


            if (g is IPoint)
            {
                writer.Write("\"coordinates\":");
                WritePointCoordinates(writer, (IPoint)g, coordinateNumberFormatString);
            }
            else if (g is ILineString)
            {
                writer.Write("\"coordinates\":");
                WriteLineStringCoordinates(writer, (ILineString)g, coordinateNumberFormatString);
            }

            else if (g is IPolygon)
            {
                writer.Write("\"coordinates\":");
                WritePolygonCoordinates(writer, (IPolygon)g, coordinateNumberFormatString);
            }
            else if (g is IMultiPoint)
            {
                writer.Write("\"coordinates\":");
                WriteMultiPointCoordinates(writer, (IMultiPoint)g, coordinateNumberFormatString);
            }
            else if (g is IMultiLineString)
            {
                writer.Write("\"coordinates\":");
                WriteMultiLineStringCoordinates(writer, (IMultiLineString)g, coordinateNumberFormatString);
            }
            else if (g is IMultiPolygon)
            {
                writer.Write("\"coordinates\":");
                WriteMultiPolygonCoordinates(writer, (IMultiPolygon)g, coordinateNumberFormatString);
            }
            else if (g is IGeometryCollection)
            {
                writer.Write("\"geometries\":");
                writer.Write("[");
                for (int i = 0; i < ((IGeometryCollection)g).Count; i++)
                {
                    WriteGeometry(writer, ((IGeometryCollection)g)[i], coordinateNumberFormatString);
                    if (i < ((IGeometryCollection)g).Count - 1)
                        writer.Write(",");
                }
                writer.Write("]");
            }

            writer.Write("}");
        }

        private static string GetGeometryType(IGeometry g)
        {
            if (g is IMultiPolygon)
                return "MultiPolygon";
            if (g is IMultiLineString)
                return "MultiLineString";
            if (g is IMultiPoint)
                return "MultiPoint";
            if (g is IGeometryCollection)
                return "GeometryCollection";
            if (g is IPolygon)
                return "Polygon";
            if (g is ILineString)
                return "LineString";
            if (g is IPoint)
                return "Point";

            throw new NotImplementedException();
        }

        private static void WriteExtents(TextWriter writer, IExtents extents, string coordinateFormatString)
        {
            writer.Write("[");
            writer.Write(
                string.Format("{0},{1},{2},{3}",
                              string.Format(CultureInfo.InvariantCulture.NumberFormat, coordinateFormatString,
                                            extents.Min[Ordinates.X]),
                              string.Format(CultureInfo.InvariantCulture.NumberFormat, coordinateFormatString,
                                            extents.Min[Ordinates.Y]),
                              string.Format(CultureInfo.InvariantCulture.NumberFormat, coordinateFormatString,
                                            extents.Max[Ordinates.X]),
                              string.Format(CultureInfo.InvariantCulture.NumberFormat, coordinateFormatString,
                                            extents.Max[Ordinates.Y])));

            writer.Write("]");
        }

        private static void WriteFeatureAttributes(TextWriter writer, IGeoJsonFeatureStyle style,
                                                   IFeatureDataRecord feature)
        {
            writer.Write("{");
            if (style.IncludeAttributes)
            {
                IDictionary<string, object> attributes = style.AttributeExtractionDelegate(feature);
                int i = 0;
                foreach (KeyValuePair<string, object> kvp in attributes)
                {
                    writer.Write(JsonUtility.FormatJsonAttribute(kvp.Key, kvp.Value));
                    if (i < attributes.Count - 1)
                        writer.Write(",");
                    i++;
                }
            }
            writer.Write("}");
        }


        private static void WriteMultiPolygonCoordinates(TextWriter writer, IMultiPolygon multiPolygon,
                                                         string coordinateFormatString)
        {
            writer.Write("[");
            for (int i = 0; i < multiPolygon.Count; i++)
            {
                WritePolygonCoordinates(writer, multiPolygon[i], coordinateFormatString);
                if (i < multiPolygon.Count - 1)
                    writer.Write(",");
            }
            writer.Write("]");
        }

        private static void WriteMultiLineStringCoordinates(TextWriter writer, IMultiLineString multiLineString,
                                                            string coordinateFormatString)
        {
            writer.Write("[");
            for (int i = 0; i < multiLineString.Count; i++)
            {
                WriteLineStringCoordinates(writer, multiLineString[i], coordinateFormatString);
                if (i < multiLineString.Count - 1)
                    writer.Write(",");
            }
            writer.Write("]");
        }

        private static void WriteMultiPointCoordinates(TextWriter writer, IMultiPoint multiPoint,
                                                       string coordinateFormatString)
        {
            writer.Write("[");
            for (int i = 0; i < multiPoint.Count; i++)
            {
                WritePointCoordinates(writer, multiPoint[i], coordinateFormatString);
                if (i < multiPoint.Count - 1)
                    writer.Write(",");
            }
            writer.Write("]");
        }

        private static void WritePolygonCoordinates(TextWriter writer, IPolygon polygon, string coordinateFormatString)
        {
            writer.Write("[");
            WriteLineStringCoordinates(writer, polygon.ExteriorRing, coordinateFormatString);

            foreach (ILinearRing lr in polygon.InteriorRings)
            {
                writer.Write(",");
                WriteLineStringCoordinates(writer, lr, coordinateFormatString);
            }
            writer.Write("]");
        }


        private static void WriteLineStringCoordinates(TextWriter writer, ILineString lineString,
                                                       string coordinateFormatString)
        {
            writer.Write("[");
            for (int i = 0; i < lineString.PointCount; i++)
            {
                WritePointCoordinates(writer, lineString.GetPoint(i), coordinateFormatString);
                if (i < lineString.PointCount - 1)
                    writer.Write(",");
            }
            writer.Write("]");
        }

        private static void WritePointCoordinates(TextWriter writer, IPoint g, string coordinateFormatString)
        {
            writer.Write("[{0},{1}]",
                            string.Format(coordinateFormatString, g[Ordinates.X]),
                            string.Format(coordinateFormatString, g[Ordinates.Y]));
        }
    }
}