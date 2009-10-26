using System.IO;
using System.Text;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.GeoJson
{
    public class GeoJsonRasterizers : IRasterizers<StringBuilder, TextWriter>
    {
        private IGeometryRasterizer<StringBuilder, TextWriter> _geometryRasterizer;
        private IRasterRasterizer<StringBuilder, TextWriter> _rasterRasterizer;

        private ITextRasterizer<StringBuilder, TextWriter> _textRasterizer;

        #region IRasterizers<StringBuilder,TextWriter> Members

        public IGeometryRasterizer<StringBuilder, TextWriter> GeometryRasterizer
        {
            get { return _geometryRasterizer; }
            internal set { _geometryRasterizer = value; }
        }

        public ITextRasterizer<StringBuilder, TextWriter> TextRasterizer
        {
            get { return _textRasterizer; }
            internal set { _textRasterizer = value; }
        }

        public IRasterRasterizer<StringBuilder, TextWriter> RasterRasterizer
        {
            get { return _rasterRasterizer; }
            internal set { _rasterRasterizer = value; }
        }

        IGeometryRasterizer IRasterizers.GeometryRasterizer
        {
            get { return GeometryRasterizer; }
        }

        ITextRasterizer IRasterizers.TextRasterizer
        {
            get { return TextRasterizer; }
        }

        IRasterRasterizer IRasterizers.RasterRasterizer
        {
            get { return RasterRasterizer; }
        }

        #endregion
    }
}