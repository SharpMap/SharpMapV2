using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SharpMap.Presentation.AspNet;
using SharpMap.Presentation.AspNet.MVP;
using SharpMap.Rendering.Direct2D;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Web.Direct2D
{
    public class Direct2DImageRenderer : IWebMapRenderer<Image>
    {
        private static ImageCodecInfo _defaultCodec;
        private ImageCodecInfo _imageCodecInfo;
        private WebMapView _mapView;
        private PixelFormat _pixelFormat;

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

        public IRasterizeSurface RasterizeSurface { get; protected set; }

        public Image Render(WebMapView mapView, out string mimeType)
        {
            mimeType = "image/bmp";
            return (RasterizeSurface as Direct2DRasterizeSurface).GetBitmap((RasterizeSurface as Direct2DRasterizeSurface).FrontSurface);

        }

        public WebMapView MapView
        {
            get { return _mapView; }
            set
            {
                _mapView = value;
                RasterizeSurface = new Direct2DRasterizeSurface(value);
            }
        }

        public double Dpi
        {
            get { return MapView.Dpi; }
        }

        Stream IWebMapRenderer.Render(WebMapView mapView, out string mimeType)
        {
            return RenderStreamInternal(mapView, out mimeType);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

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

        private Stream RenderStreamInternal(WebMapView map, out string mimeType)
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
    }
}