using System.Drawing;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Gdi
{
    public class GdiRasterizers : IRasterizers<Bitmap, Graphics>
    {
        private IGeometryRasterizer<Bitmap, Graphics> _geometryRasterizer;
        private IRasterRasterizer<Bitmap, Graphics> _rasterRasterizer;

        private ITextRasterizer<Bitmap, Graphics> _textRasterizer;

        #region IRasterizers<Bitmap,Graphics> Members

        public IGeometryRasterizer<Bitmap, Graphics> GeometryRasterizer
        {
            get { return _geometryRasterizer; }
            internal set { _geometryRasterizer = value; }
        }

        public ITextRasterizer<Bitmap, Graphics> TextRasterizer
        {
            get { return _textRasterizer; }
            internal set { _textRasterizer = value; }
        }

        public IRasterRasterizer<Bitmap, Graphics> RasterRasterizer
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