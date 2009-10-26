using System;
using System.Drawing;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Rendering.Gdi
{
    public class GdiRasterizeSurface : IRasterizeSurface<Bitmap, Graphics>
    {
        private static readonly object _frontLock = new object();
        private static readonly object _rearLock = new object();
        private readonly IMapView2D _view;

        private Bitmap _backSurface;

        private Bitmap _frontSurface;

        public GdiRasterizeSurface(IMapView2D view)
        {
            _view = view;
        }

        #region IRasterizeSurface<Bitmap,Graphics> Members

        public Bitmap BackSurface
        {
            get
            {
                lock (_rearLock)
                    return _backSurface;
            }
            protected set
            {
                lock (_rearLock)
                    _backSurface = value;
            }
        }

        public Bitmap FrontSurface
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

        public IRasterizers<Bitmap, Graphics> RetrieveSurface()
        {
            lock (_rearLock)
                if (BackSurface == null)
                    return CreateSurface();
                else
                    return CreateRasterizers(BackSurface, Graphics.FromImage(BackSurface));
        }


        public event EventHandler RenderComplete;

        public void RenderCompleted()
        {
            FrontSurface = (Bitmap)BackSurface.Clone();
            MapView.Display(FrontSurface); 
            RaiseRenderComplete();
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

        IRasterizers<Bitmap, Graphics> IRasterizeSurface<Bitmap, Graphics>.CreateSurface()
        {
            return CreateSurface();
        }

        public IMapView2D MapView
        {
            get { return _view; }
        }

        #endregion

        public IRasterizers<Bitmap, Graphics> CreateSurface()
        {
            lock (_rearLock)
            {
                Bitmap bmp = new Bitmap((int)MapView.ViewSize.Width, (int)MapView.ViewSize.Height);
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(ViewConverter.Convert(MapView.BackgroundColor));
                IRasterizers<Bitmap, Graphics> rasterizers = CreateRasterizers(bmp, g);
                BackSurface = bmp;
                return rasterizers;
            }
        }

        private static IRasterizers<Bitmap, Graphics> CreateRasterizers(Bitmap bmp, Graphics g)
        {

            return new GdiRasterizers
                       {
                           GeometryRasterizer = new GdiGeometryRasterizer(bmp, g),
                           TextRasterizer = new GdiTextRasterizer(bmp, g)
                       };
        }

        private void RaiseRenderComplete()
        {
            EventHandler h = RenderComplete;
            if (h != null)
                h(this, EventArgs.Empty);
        }
    }
}