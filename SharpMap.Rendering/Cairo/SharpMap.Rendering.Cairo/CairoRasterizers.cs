using Cairo;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Cairo
{
    public class CairoRasterizers : IRasterizers<Surface, Context>
    {
        private IGeometryRasterizer<Surface, Context> _geometryRasterizer;
        private IRasterRasterizer<Surface, Context> _rasterRasterizer;

        private ITextRasterizer<Surface, Context> _textRasterizer;

        #region IRasterizers<Surface,Context> Members

        public IGeometryRasterizer<Surface, Context> GeometryRasterizer
        {
            get { return _geometryRasterizer; }
            internal set { _geometryRasterizer = value; }
        }

        public ITextRasterizer<Surface, Context> TextRasterizer
        {
            get { return _textRasterizer; }
            internal set { _textRasterizer = value; }
        }

        public IRasterRasterizer<Surface, Context> RasterRasterizer
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