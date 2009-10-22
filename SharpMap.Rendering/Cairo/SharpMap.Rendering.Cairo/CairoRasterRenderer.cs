using System;
using System.Collections.Generic;
using System.IO;
using NPack;
using NPack.Interfaces;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Rendering.Cairo
{
    public class CairoRasterRenderer : RasterRenderer2D<CairoRenderObject>
    {
        public override IEnumerable<CairoRenderObject> RenderRaster(Stream rasterData, Rectangle2D viewBounds,
                                                                    Rectangle2D rasterBounds)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<CairoRenderObject> RenderRaster(Stream rasterData, Rectangle2D viewBounds,
                                                                    Rectangle2D rasterBounds,
                                                                    IMatrix<DoubleComponent> rasterTransform)
        {
            throw new NotImplementedException();
        }
    }
}