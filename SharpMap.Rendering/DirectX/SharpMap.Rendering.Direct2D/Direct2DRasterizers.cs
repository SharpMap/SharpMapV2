using SharpMap.Rendering.Rasterize;
using SlimDX.Direct2D;

namespace SharpMap.Rendering.Direct2D
{
    public class Direct2DRasterizers : IRasterizers<RenderTarget, RenderTarget>
    {
        #region IRasterizers<RenderTarget,RenderTarget> Members

        public IGeometryRasterizer<RenderTarget, RenderTarget> GeometryRasterizer { get; set; }

        public ITextRasterizer<RenderTarget, RenderTarget> TextRasterizer { get; set; }

        public IRasterRasterizer<RenderTarget, RenderTarget> RasterRasterizer { get; set; }

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