using System.IO;
using System.Text;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.GeoJson
{
    public class GeoJsonRasterizeSurface : RasterizeSurface<StringBuilder, TextWriter>
    {
        public GeoJsonRasterizeSurface(IMapView2D view) : base(view)
        {
        }


        protected override IRasterizers<StringBuilder, TextWriter> CreateRasterizers(StringBuilder builder,
                                                                                     TextWriter writer)
        {
            return new GeoJsonRasterizers
                       {
                           GeometryRasterizer = new GeoJsonGeometryRasterizer(builder, writer),
                           TextRasterizer = new GeoJsonTextRasterizer(builder, writer),
                           RasterRasterizer = new GeoJsonRasterRasterizer(builder, writer)
                       };
        }


        protected override TextWriter CreateNewContextInternal(StringBuilder surface)
        {
            return new StringWriter(surface);
        }

        protected override StringBuilder CreateSurfaceInternal()
        {
            return new StringBuilder();
        }

        protected override TextWriter CreateExistingContext(StringBuilder surface)
        {
            return CreateNewContextInternal(surface);
        }
    }
}