using Cairo;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Cairo
{
    public class CairoRasterizeSurface : RasterizeSurface<Surface, Context>
    {
        public CairoRasterizeSurface(IMapView2D view)
            : base(view)
        {
        }


        protected override Surface CreateSurfaceInternal()
        {
            return CreateCairoSurface((int)MapView.ViewSize.Width, (int)MapView.ViewSize.Height);
        }

        protected override Context CreateNewContextInternal(Surface surface)
        {
            Context c = new Context(surface) { FillRule = FillRule.EvenOdd, Antialias = Antialias.Subpixel };
            c.SetSourceRGBA(MapView.BackgroundColor.R, MapView.BackgroundColor.G, MapView.BackgroundColor.B,
                            MapView.BackgroundColor.A);
            c.FillExtents();
            return c;
        }


        protected override Context CreateExistingContext(Surface surface)
        {
            return new Context(surface);
        }


        protected override IRasterizers<Surface, Context> CreateRasterizers(Surface surface, Context context)
        {
            return new CairoRasterizers
                       {
                           GeometryRasterizer = new CairoGeometryRasterizer(surface, context),
                           TextRasterizer = new CairoTextRasterizer(surface, context),
                           RasterRasterizer = new CairoRasterRasterizer(surface, context)
                       };
        }

        protected virtual Surface CreateCairoSurface(int width, int height)
        {
            return new ImageSurface(Format.Argb32, width, height);
        }
    }
}