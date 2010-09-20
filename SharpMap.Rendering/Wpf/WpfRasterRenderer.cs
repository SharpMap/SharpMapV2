using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using NPack;
using NPack.Interfaces;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Rendering.Wpf
{
    public class WpfRasterRenderer : RasterRenderer2D<WpfRenderObject>
    {
        public override IEnumerable<WpfRenderObject> RenderRaster(Stream rasterData, Rectangle2D viewBounds,
                                                                   Rectangle2D rasterBounds)
        {
            yield return null;
        }

        public override IEnumerable<WpfRenderObject> RenderRaster(Stream rasterData, Rectangle2D viewBounds,
                                                                   Rectangle2D rasterBounds,
                                                                   IMatrix<DoubleComponent> rasterTransform)
        {
            yield return null;
        }
    }
}