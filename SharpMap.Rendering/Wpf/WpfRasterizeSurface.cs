using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Wpf
{
    public class WpfRasterizeSurface : RasterizeSurface<DrawingVisual, DrawingContext>
    {
        private DrawingContext currentSurfaceContext;

        public WpfRasterizeSurface(IMapView2D view) 
            : base(view)
        {
        }

        protected override DrawingContext CreateNewContextInternal(DrawingVisual surface)
        {
            if (this.currentSurfaceContext != null) throw new ApplicationException("Context already exists.");

            this.currentSurfaceContext = surface.RenderOpen();
            return this.currentSurfaceContext;
        }

        protected override DrawingVisual CreateSurfaceInternal()
        {
            var surface = new DrawingVisual();
            RenderOptions.SetEdgeMode(surface, EdgeMode.Aliased);
            return surface;
        }

        protected override DrawingContext CreateExistingContext(DrawingVisual surface)
        {
            if (this.currentSurfaceContext != null) throw new ApplicationException("Context already exists.");

            this.currentSurfaceContext = surface.RenderOpen();
            return this.currentSurfaceContext;
        }

        protected override IRasterizers<DrawingVisual, DrawingContext> CreateRasterizers(DrawingVisual surface, DrawingContext context)
        {
            return new WpfRasterizers
                       {
                           GeometryRasterizer = new WpfGeometryRasterizer(surface, context),
                           RasterRasterizer = new WpfRasterRasterizer(surface, context),
                           TextRasterizer = new WpfTextRasterizer(surface, context)
                       };
        }

        public override void RenderCompleted()
        {
            this.currentSurfaceContext.Close();

            (this.currentSurfaceContext as IDisposable).Dispose();
            this.currentSurfaceContext = null;

            base.RenderCompleted();
        }
    }
}
