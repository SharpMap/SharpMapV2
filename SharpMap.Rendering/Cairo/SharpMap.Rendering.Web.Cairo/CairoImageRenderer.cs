using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Cairo;
using SharpMap.Presentation.AspNet;
using SharpMap.Presentation.AspNet.MVP;
using SharpMap.Rendering.Cairo;
using SharpMap.Rendering.Rasterize;
using SharpMap.Styles;
using CairoGraphics = Cairo.Context;
using CairoLineCap = Cairo.LineCap;
using CairoLineJoin = Cairo.LineJoin;
using CairoAntialias = Cairo.Antialias;
using Graphics = System.Drawing.Graphics;
using Rectangle = System.Drawing.Rectangle;

namespace SharpMap.Rendering.Web.Cairo
{
    public class CairoImageRenderer : IWebMapRenderer<Image>
    {
        private static ImageCodecInfo _defaultCodec;
        // private readonly RenderQueue _renderQueue = new RenderQueue();
        private ImageCodecInfo _imageCodecInfo;
        private WebMapView _mapView;
        private PixelFormat _pixelFormat;
        private bool disposed;

        static CairoImageRenderer()
        {
            string bin = ConfigurationManager.AppSettings["CairoUnmanagedBinDir"];
            string path = Environment.GetEnvironmentVariable("path");
            if (!path.Contains(bin))
                Environment.SetEnvironmentVariable("path", path + ";" + bin);
        }

        public int Width
        {
            get { return (int)MapView.ViewSize.Width; }
        }

        public int Height
        {
            get { return (int)MapView.ViewSize.Height; }
        }

        public PixelFormat PixelFormat
        {
            get { return _pixelFormat; }
        }

        public ImageCodecInfo ImageCodec
        {
            get
            {
                if (_imageCodecInfo == null)
                    _imageCodecInfo = GetDefaultCodec();

                return _imageCodecInfo;
            }
            set { _imageCodecInfo = value; }
        }

        public ImageFormat ImageFormat
        {
            get { return new ImageFormat(_imageCodecInfo.FormatID); }
            set
            {
                foreach (ImageCodecInfo i in ImageCodecInfo.GetImageEncoders())
                {
                    if (i.FormatID == value.Guid)
                    {
                        _imageCodecInfo = i;
                        break;
                    }
                }
            }
        }

        public EncoderParameters EncoderParams { get; set; }

        #region IWebMapRenderer<Image> Members

        public Image Render(WebMapView mapView, out string mimeType)
        {

            Bitmap bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                IntPtr hdc = graphics.GetHdc();
                using (Win32Surface winSurface = new Win32Surface(hdc))
                using (Context context = new Context(winSurface))
                {
                    BitmapData data = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                    context.SetSourceRGBA(0, 0, 0, 0);
                    context.FillExtents();

                    context.SetSource(MapView.RenderedData as Surface);
                    context.Paint();
                    bmp.UnlockBits(data);
                    graphics.ReleaseHdc(hdc);
                }
            }

            mimeType = "image/bmp";
            return bmp;
        }


        public WebMapView MapView
        {
            get { return _mapView; }
            set
            {
                _mapView = value;
                RasterizeSurface = new CairoRasterizeSurface(value);
            }
        }

        public double Dpi { get; set; }



        Stream IWebMapRenderer.Render(WebMapView mapView, out string mimeType)
        {
            return RenderStreamInternal(mapView, out mimeType);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IRasterizeSurface RasterizeSurface { get; protected set; }

        #endregion

        public event EventHandler RenderDone;

        protected virtual Surface CreateImageSurface()
        {
            return new ImageSurface(Format.Argb32, Width, Height);
        }

        private static ImageCodecInfo GetDefaultCodec()
        {
            if (_defaultCodec == null)
                _defaultCodec = FindCodec("image/png");
            return _defaultCodec;
        }

        public static ImageCodecInfo FindCodec(string mimeType)
        {
            foreach (ImageCodecInfo i in ImageCodecInfo.GetImageEncoders())
            {
                if (i.MimeType == mimeType)
                    return i;
            }

            return null;
        }

        protected virtual Stream RenderStreamInternal(WebMapView map, out string mimeType)
        {
            Image im = Render(map, out mimeType);

            if (im == null)
                return null;

            MemoryStream ms = new MemoryStream();

            im.Save(ms, ImageCodec, EncoderParams);
            im.Dispose();
            ms.Seek(0, SeekOrigin.Begin);
            mimeType = ImageCodec.MimeType;
            return ms;
        }

        ~CairoImageRenderer()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;
            disposed = true;
            if (disposing)
            {
            }
        }
    }
}