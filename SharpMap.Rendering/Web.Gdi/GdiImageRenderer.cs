/*
 *	This file is part of SharpMap.Rendering.Web.Gdi
 *  SharpMap.Rendering.Web.Gdi is free software © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SharpMap.Presentation.AspNet;
using SharpMap.Presentation.AspNet.MVP;
using SharpMap.Rendering.Gdi;
using SharpMap.Rendering.Rasterize;
using GdiMatrix = System.Drawing.Drawing2D.Matrix;
using GdiColorMatrix = System.Drawing.Imaging.ColorMatrix;

namespace SharpMap.Rendering.Web
{
    public class GdiImageRenderer
        : IWebMapRenderer<Image>
    {
        private static ImageCodecInfo _defaultCodec;
        private readonly PixelFormat _pixelFormat;
        private GdiMatrix _gdiViewMatrix;

        private ImageCodecInfo _imageCodecInfo;
        private WebMapView _mapView;
        private bool disposed;

        public GdiImageRenderer(PixelFormat pxfrmt)
        {
            _pixelFormat = pxfrmt;
        }

        public GdiImageRenderer()
            : this(PixelFormat.Format32bppArgb)
        {
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

        public int Width
        {
            get { return (int)MapView.ViewSize.Width; }
        }

        public int Height
        {
            get { return (int)MapView.ViewSize.Height; }
        }

        #region IWebMapRenderer<Image> Members

        public WebMapView MapView
        {
            get { return _mapView; }
            set
            {
                _mapView = value;
                RasterizeSurface = new GdiRasterizeSurface(value);
            }
        }


        public Image Render(WebMapView mapView, out string mimeType)
        {
            //Bitmap bmp = new Bitmap(Width, Height, PixelFormat);
            //Graphics g = Graphics.FromImage(bmp);
            //g.SmoothingMode = SmoothingMode.AntiAlias;

            ////g.Transform = GetGdiViewTransform();
            //if (!MapView.Presenter.IsRenderingSelection)
            //    g.Clear(ViewConverter.Convert(MapView.BackgroundColor));


            //Image img = mapView.RenderedData as Image;
            //g.DrawImageUnscaled(img, 0, 0);

            //g.Dispose();

            mimeType = "image/bmp";
            return mapView.RenderedData as Image;
        }


        public double Dpi { get; set; }


        Stream IWebMapRenderer.Render(WebMapView map, out string mimeType)
        {
            return RenderStreamInternal(map, out mimeType);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IRasterizeSurface RasterizeSurface { get; protected set; }

        #endregion

        public event EventHandler RenderDone;

        protected virtual Stream RenderStreamInternal(WebMapView map, out string mimeType)
        {
            Image im = Render(map, out mimeType);

            if (im == null)
                return null;

            MemoryStream ms = new MemoryStream();

            im.Save(ms, ImageCodec, EncoderParams);
            im.Dispose();
            ms.Position = 0;
            mimeType = ImageCodec.MimeType;
            return ms;
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

        private static Rectangle GetSourceRegion(Size sz)
        {
            return new Rectangle(new Point(0, 0), sz);
        }

        ~GdiImageRenderer()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
            }
        }
    }
}