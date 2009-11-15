using System.Drawing;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Gdi
{
    public class GdiRasterizeSurface : RasterizeSurface<Bitmap, Graphics>
    {
        public GdiRasterizeSurface(IMapView2D view)
            : base(view)
        {
        }


        protected override IRasterizers<Bitmap, Graphics> CreateRasterizers(Bitmap bmp, Graphics g)
        {
            return new GdiRasterizers
                       {
                           GeometryRasterizer = new GdiGeometryRasterizer(bmp, g),
                           TextRasterizer = new GdiTextRasterizer(bmp, g)
                       };
        }


        protected override Graphics CreateNewContextInternal(Bitmap surface)
        {
            Graphics g = Graphics.FromImage(surface);
            g.Clear(ViewConverter.Convert(MapView.BackgroundColor));
            return g;
        }

        protected override Bitmap CreateSurfaceInternal()
        {
            return new Bitmap((int) MapView.ViewSize.Width, (int) MapView.ViewSize.Height);
        }

        protected override Graphics CreateExistingContext(Bitmap surface)
        {
            return Graphics.FromImage(surface);
        }
    }
}