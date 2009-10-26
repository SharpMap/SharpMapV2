using System;
using System.IO;
using System.Text;
using SharpMap.Data;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.GeoJson
{
    public class GeoJsonGeometryRasterizer : GeoJsonRasterizer, IGeometryRasterizer<StringBuilder, TextWriter>
    {
        public GeoJsonGeometryRasterizer(StringBuilder sb, TextWriter writer)
            : base(sb, writer)
        {
        }

        #region IGeometryRasterizer<StringBuilder,TextWriter> Members

        public void Rasterize(IFeatureDataRecord feature, GeometryStyle style, Matrix2D transform)
        {
            if (feature == null) throw new ArgumentNullException("feature");

            IGeoJsonFeatureStyle geoJsonStyle = style as IGeoJsonFeatureStyle;
            if (geoJsonStyle == null) throw new ArgumentNullException("style");
            if (feature.Geometry == null)
            {
                throw new InvalidOperationException("Feature must have a geometry to be rendered.");
            }
            GeoJsonFeatureWriter.WriteFeature(Context, geoJsonStyle, feature);
            Context.Write(",");
        }

        #endregion
    }
}