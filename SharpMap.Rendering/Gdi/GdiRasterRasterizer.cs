using System.Drawing;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Gdi
{
    public class GdiRasterRasterizer : GdiRasterizer, IRasterRasterizer<Bitmap, Graphics>
    {
        public GdiRasterRasterizer(Bitmap surface, Graphics context)
            : base(surface, context)
        {
        }
    }
}