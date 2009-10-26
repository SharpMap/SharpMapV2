using System;
using System.Collections.Generic;
using System.Text;
using Cairo;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Cairo
{
    public class CairoRasterRasterizer : CairoRasterizer, IRasterRasterizer<Surface, Context>
    {
        public CairoRasterRasterizer(Surface surface, Context context)
            : base(surface, context)
        {
        }
    }
}
