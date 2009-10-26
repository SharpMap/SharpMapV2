using System.IO;
using System.Text;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.GeoJson
{
    public class GeoJsonRasterRasterizer : GeoJsonRasterizer, IRasterRasterizer<StringBuilder, TextWriter>
    {
        public GeoJsonRasterRasterizer(StringBuilder sb, TextWriter writer)
            : base(sb, writer)
        {
        }
    }
}