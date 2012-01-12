using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Wpf
{
    public class WpfRasterizers : IRasterizers<DrawingVisual, DrawingContext>
    {
        public ITextRasterizer<DrawingVisual, DrawingContext> TextRasterizer { get; internal set; }

        public IRasterRasterizer<DrawingVisual, DrawingContext> RasterRasterizer { get; internal set; }

        public IGeometryRasterizer<DrawingVisual, DrawingContext> GeometryRasterizer { get; internal set; }

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
        
        public void BeginPass()
        {
            TextRasterizer.BeginPass();
            RasterRasterizer.BeginPass();
            GeometryRasterizer.BeginPass();
        }

        public void EndPass()
        {
            TextRasterizer.EndPass();
            RasterRasterizer.EndPass();
            GeometryRasterizer.EndPass();
        }
    }
}
