using System;
using System.IO;
using System.Text;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.GeoJson
{
    public class GeoJsonTextRasterizer : GeoJsonRasterizer, ITextRasterizer<StringBuilder, TextWriter>
    {
        public GeoJsonTextRasterizer(StringBuilder sb, TextWriter writer)
            : base(sb, writer)
        {
        }

        public void Rasterize(IFeatureDataRecord feature, string text, LabelStyle style, Matrix2D transform)
        {
            GeoJsonLabelStyle geoJsonLabelStyle = style as GeoJsonLabelStyle;

            if (geoJsonLabelStyle == null)
                throw new ArgumentException("style should be an instance of GeoJsonLabelStyle");

            Context.Write("{");
            Context.Write(JsonUtility.FormatJsonAttribute("type", "Label"));
            Context.Write(",");
            Context.Write(JsonUtility.FormatJsonAttribute("id", feature.GetOid()));
            Context.Write(",");
            Context.Write(JsonUtility.FormatJsonAttribute("text", text));
            Context.Write(",");


            IGeometry geom;
            if (geoJsonLabelStyle.IncludeGeometryData)
                geom = geoJsonLabelStyle.PreProcessGeometries
                           ? geoJsonLabelStyle.GeometryPreProcessor(feature.Geometry)
                           : feature.Geometry;
            else
                geom = feature.Geometry.Centroid;

            Context.Write("\"shape\":");
            GeoJsonFeatureWriter.WriteGeometry(Context, geom, geoJsonLabelStyle.CoordinateNumberFormatString);
            Context.Write("}");
            Context.Write(","); 
            
        }
    }
}