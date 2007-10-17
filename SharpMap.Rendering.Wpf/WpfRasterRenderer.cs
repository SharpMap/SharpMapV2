using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using NPack;
using NPack.Interfaces;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Rendering.Wpf
{
    public class WpfRasterRenderer : RasterRenderer2D<DependencyObject>
    {
        public override IEnumerable<DependencyObject> RenderRaster(Stream rasterData, Rectangle2D viewBounds,
                                                                   Rectangle2D rasterBounds)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DependencyObject> RenderRaster(Stream rasterData, Rectangle2D viewBounds,
                                                                   Rectangle2D rasterBounds,
                                                                   IMatrix<DoubleComponent> rasterTransform)
        {
            throw new NotImplementedException();
        }
    }
}