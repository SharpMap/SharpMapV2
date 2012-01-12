using System;
using System.Windows.Media;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Wpf
{
    public class WpfRasterizer : IRasterizer<DrawingVisual, DrawingContext>
    {
        private readonly DrawingVisual _surface;
        private readonly DrawingContext _context;

        protected WpfRasterizer(DrawingVisual surface, DrawingContext context)
        {
            _surface = surface;
            _context = context;
        }

        public DrawingContext Context
        {
            get { return _context; }
        }

        public DrawingVisual Surface
        {
            get { return _surface; }
        }

        object IRasterizer.Surface
        {
            get { return Surface; }
        }

        object IRasterizer.Context
        {
            get { return Context; }
        }

        public void BeginPass()
        {
        }

        public void EndPass()
        {
            (this.Context as IDisposable).Dispose();
        }
    }
}
