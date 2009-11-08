using SharpMap.Rendering.Rasterize;
using SlimDX.Direct2D;

namespace SharpMap.Rendering.Direct2D
{
    public class Direct2DRasterRasterizer : Direct2DRasterizer, IRasterRasterizer<RenderTarget, RenderTarget>
    {
        public Direct2DRasterRasterizer(RenderTarget surface, RenderTarget context)
            : base(surface, context)
        {
        }
    }
}