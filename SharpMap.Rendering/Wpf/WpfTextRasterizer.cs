using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpMap.Data;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Wpf
{
    public class WpfTextRasterizer : WpfRasterizer, ITextRasterizer<DrawingVisual, DrawingContext>
    {
        public WpfTextRasterizer(DrawingVisual surface, DrawingContext context)
            : base(surface, context)
        {
        }

        public void Rasterize(IFeatureDataRecord record, string text, LabelStyle style, Matrix2D transform)
        {
            throw new NotImplementedException();
        }
    }
}
