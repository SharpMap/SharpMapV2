using System;
using Cairo;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Cairo
{
    public class CairoRasterizeSurface : IRasterizeSurface<Surface, Context>
    {
        private readonly object _backLock = new object();
        private readonly object _frontLock = new object();
        private readonly IMapView2D _view;

        private Surface _backSurface;

        private Surface _frontSurface;

        public CairoRasterizeSurface(IMapView2D view)
        {
            _view = view;
        }

        #region IRasterizeSurface<Surface,Context> Members

        public Surface BackSurface
        {
            get
            {
                lock (_backLock)
                    return _backSurface;
            }
            protected set
            {
                lock (_backLock)
                    _backSurface = value;
            }
        }

        public Surface FrontSurface
        {
            get
            {
                lock (_frontLock)
                    return _frontSurface;
            }
            protected set
            {
                lock (_frontLock)
                    _frontSurface = value;
            }
        }

        public IRasterizers<Surface, Context> RetrieveSurface()
        {
            if (BackSurface == null)
                return CreateSurface();
            return CreateRasterizers(BackSurface, new Context(BackSurface));
        }

        public IRasterizers<Surface, Context> CreateSurface()
        {
            Surface sf = CreateCairoSurface((int) MapView.ViewSize.Width, (int) MapView.ViewSize.Height);
            BackSurface = sf;
            Context c = new Context(sf) {FillRule = FillRule.EvenOdd, Antialias = Antialias.Subpixel};
            c.SetSourceRGBA(MapView.BackgroundColor.R, MapView.BackgroundColor.G, MapView.BackgroundColor.B,
                            MapView.BackgroundColor.A);
            c.FillExtents();

            return CreateRasterizers(sf, c);
        }

        public IMapView2D MapView
        {
            get { return _view; }
        }

        public event EventHandler RenderComplete;

        public void RenderCompleted()
        {
            FrontSurface = BackSurface;
            MapView.Display(FrontSurface);
        }

        object IRasterizeSurface.BackSurface
        {
            get { return BackSurface; }
        }

        object IRasterizeSurface.FrontSurface
        {
            get { return FrontSurface; }
        }

        IRasterizers IRasterizeSurface.RetrieveSurface()
        {
            return RetrieveSurface();
        }

        IRasterizers IRasterizeSurface.CreateSurface()
        {
            return CreateSurface();
        }

        #endregion

        private IRasterizers<Surface, Context> CreateRasterizers(Surface surface, Context context)
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