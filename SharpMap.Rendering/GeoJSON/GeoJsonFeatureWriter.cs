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
using System.Text;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Styles;

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
        public static string WriteFeature(GeoJsonGeometryStyle style, IFeatureDataRecord feature)
        {
            StringBuilder sb = new StringBuilder();
            WriteFeature(sb, style, feature);
            return sb.ToString();
        }

        public static void WriteFeature(StringBuilder sb, GeoJsonGeometryStyle style, IFeatureDataRecord feature)
        {
            IGeometry g = style.PreProcessGeometries
                                          ? style.GeometryPreProcessor(feature.Geometry)
                                          : feature.Geometry;
            sb.Append("{");

            if (feature.HasOid)
            {
                sb.Append(JsonUtility.FormatJsonAttribute("id", feature.GetOid()));
                sb.Append(",");
            }

            sb.Append("\"properties\":");
            WriteFeatureAttributes(sb, style, feature);
            sb.Append(",");

            if (style.IncludeBBox)
            {
                sb.Append("\"bbox\":");
                WriteExtents(sb, feature.Geometry.Extents);
                sb.Append(",");
            }

            /* Ideally this should be higher up e.g at the layer level but we dont currently 
             * have anywhere where we can group features by layer. 
             * It still complies with the spec but will be considerably more verbose */

            if (g.SpatialReference != null || !string.IsNullOrEmpty(g.Srid))
            {
                sb.Append("\"crs\":");
                WriteNamedCrs(sb,
                              g.SpatialReference != null
                                  ? g.SpatialReference.Name
                                  : "EPSG:" + g.Srid);
                sb.Append(",");
            }

            sb.Append("\"geometry\":");
            WriteGeometry(sb, g);

            sb.Append("}");
            sb.AppendLine();
        }

        private static void WriteNamedCrs(StringBuilder sb, string name)
        {
            sb.Append("{");
            sb.Append(JsonUtility.FormatJsonAttribute("type", "name"));
            sb.Append(",");
            sb.Append("\"properties\":");
            sb.Append("{");
            sb.Append(JsonUtility.FormatJsonAttribute("name", name));
            sb.Append("}");
            sb.Append("}");
        }

        public static string WriteGeometry(IGeometry g)
        {
            StringBuilder sb = new StringBuilder();
            WriteGeometry(sb, g);
            return sb.ToString();
        }

        public static void WriteGeometry(StringBuilder sb, IGeometry g)
        {
            sb.Append("{");
            sb.Append(JsonUtility.FormatJsonAttribute("type", GetGeometryType(g)));
            sb.Append(",");


            if (g is IPoint)
            {
                sb.Append("\"coordinates\":");
                WritePointCoordinates(sb, (IPoint)g);
            }
            else if (g is ILineString)
            {
                sb.Append("\"coordinates\":");
                WriteLineStringCoordinates(sb, (ILineString)g);
            }

            else if (g is IPolygon)
            {
                sb.Append("\"coordinates\":");
                WritePolygonCoordinates(sb, (IPolygon)g);
            }
            else if (g is IMultiPoint)
            {
                sb.Append("\"coordinates\":");
                WriteMultiPointCoordinates(sb, (IMultiPoint)g);
            }
            else if (g is IMultiLineString)
            {
                sb.Append("\"coordinates\":");
                WriteMultiLineStringCoordinates(sb, (IMultiLineString)g);
            }
            else if (g is IMultiPolygon)
            {
                sb.Append("\"coordinates\":");
                WriteMultiPolygonCoordinates(sb, (IMultiPolygon)g);
            }
            else if (g is IGeometryCollection)
            {
                sb.Append("\"geometries\":");
                sb.Append("[");
                for (int i = 0; i < ((IGeometryCollection)g).Count; i++)
                {
                    WriteGeometry(sb, ((IGeometryCollection)g)[i]);
                    if (i < ((IGeometryCollection)g).Count - 1)
                        sb.Append(",");
                }
                sb.Append("]");
            }

            sb.Append("}");
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

        private static void WriteExtents(StringBuilder sb, IExtents extents)
        {
            sb.Append("[");
            sb.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, string.Format("{0},{1},{2},{3}", extents.Min[Ordinates.X], extents.Min[Ordinates.Y],
                                          extents.Max[Ordinates.X], extents.Max[Ordinates.Y]));

            sb.Append("]");
        }

        private static void WriteFeatureAttributes(StringBuilder sb, GeoJsonGeometryStyle style,
                                                   IFeatureDataRecord feature)
        {
            sb.Append("{");
            if (style.IncludeAttributes)
            {
                IDictionary<string, object> attributes = style.AttributeExtractionDelegate(feature);
                int i = 0;
                foreach (var kvp in attributes)
                {
                    sb.Append(JsonUtility.FormatJsonAttribute(kvp.Key, kvp.Value));
                    if (i < attributes.Count - 1)
                        sb.Append(",");
                    i++;
                }
            }
            sb.Append("}");
        }


        private static void WriteMultiPolygonCoordinates(StringBuilder sb, IMultiPolygon multiPolygon)
        {
            sb.Append("[");
            for (int i = 0; i < multiPolygon.Count; i++)
            {
                WritePolygonCoordinates(sb, multiPolygon[i]);
                if (i < multiPolygon.Count - 1)
                    sb.Append(",");
            }
            sb.Append("]");
        }

        private static void WriteMultiLineStringCoordinates(StringBuilder sb, IMultiLineString multiLineString)
        {
            sb.Append("[");
            for (int i = 0; i < multiLineString.Count; i++)
            {
                WriteLineStringCoordinates(sb, multiLineString[i]);
                if (i < multiLineString.Count - 1)
                    sb.Append(",");
            }
            sb.Append("]");
        }

        private static void WriteMultiPointCoordinates(StringBuilder sb, IMultiPoint multiPoint)
        {
            sb.Append("[");
            for (int i = 0; i < multiPoint.Count; i++)
            {
                WritePointCoordinates(sb, multiPoint[i]);
                if (i < multiPoint.Count - 1)
                    sb.Append(",");
            }
            sb.Append("]");
        }

        private static void WritePolygonCoordinates(StringBuilder sb, IPolygon polygon)
        {
            sb.Append("[");
            WriteLineStringCoordinates(sb, polygon.ExteriorRing);

            foreach (ILinearRing lr in polygon.InteriorRings)
            {
                sb.Append(",");
                WriteLineStringCoordinates(sb, lr);
            }
            sb.Append("]");
        }


        private static void WriteLineStringCoordinates(StringBuilder sb, ILineString lineString)
        {
            sb.Append("[");
            for (int i = 0; i < lineString.PointCount; i++)
            {
                WritePointCoordinates(sb, lineString.GetPoint(i));
                if (i < lineString.PointCount - 1)
                    sb.Append(",");
            }
            sb.Append("]");
        }

        private static void WritePointCoordinates(StringBuilder sb, IPoint g)
        {
            sb.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "[{0},{1}]", g[Ordinates.X], g[Ordinates.Y]);
        }
    }
}