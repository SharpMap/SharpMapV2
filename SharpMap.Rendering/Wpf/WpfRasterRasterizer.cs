using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Wpf
{
    public class WpfRasterRasterizer : WpfRasterizer, IRasterRasterizer<DrawingVisual, DrawingContext>
    {
        public WpfRasterRasterizer(DrawingVisual surface, DrawingContext context)
            : base(surface, context)
        {
        }
    }
}
