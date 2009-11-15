using System;
using System.Drawing;
using System.Drawing.Imaging;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rasterize;
using SlimDX;
using SlimDX.Direct2D;
using SlimDX.DXGI;
using Factory = SlimDX.Direct2D.Factory;
using GdiBitmap = System.Drawing.Bitmap;
using GdiGraphics = System.Drawing.Graphics;
using PixelFormat = SlimDX.Direct2D.PixelFormat;

namespace SharpMap.Rendering.Direct2D
{
    public class Direct2DRasterizeSurface : RasterizeSurface<RenderTarget, RenderTarget>
    {
        public Direct2DRasterizeSurface(IMapView2D view)
            : base(view)
        {
        }

        protected override RenderTarget CreateNewContextInternal(RenderTarget surface)
        {
            surface.Clear(ViewConverter.Convert(MapView.BackgroundColor));
            return surface;
        }

        protected override RenderTarget CreateSurfaceInternal()
        {
            RenderTarget trgt = new DeviceContextRenderTarget(
                new Factory(),
                new RenderTargetProperties
                {
                    PixelFormat = new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Ignore),
                    Type = RenderTargetType.Software
                }).CreateCompatibleRenderTarget(new SizeF((float)MapView.ViewSize.Width,
                                                              (float)MapView.ViewSize.Height));

            return trgt;
        }

        public GdiBitmap GetBitmap(RenderTarget renderTarget)
        {
            throw new NotImplementedException();

            /*BitmapRenderTarget dc = (BitmapRenderTarget) renderTarget;
            GdiBitmap bitmap = new GdiBitmap((int) MapView.ViewSize.Width, (int) MapView.ViewSize.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                                  ImageLockMode.WriteOnly,
                                                  System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                IntPtr hdc = graphics.GetHdc();

                DataStream ds = new DataStream(hdc, bitmap.Width*bitmap.Height*8, true, true);
                Guid g = Guid.NewGuid();
                renderTarget.CreateSharedBitmap(g, ds);

                bitmap.UnlockBits(data);
                graphics.ReleaseHdc(hdc);
            }
            return bitmap;*/
        }

        private ImageCodecInfo GetEncoder()
        {
            foreach (ImageCodecInfo info in ImageCodecInfo.GetImageEncoders())
            {
                if (info.MimeType.Contains("bmp"))
                    return info;
            }
            throw new NotImplementedException();
        }

        protected override RenderTarget CreateExistingContext(RenderTarget surface)
        {
            return surface;
        }

        protected override IRasterizers<RenderTarget, RenderTarget> CreateRasterizers(RenderTarget surface,
                                                                                      RenderTarget context)
        {
            return new Direct2DRasterizers
                       {
                           GeometryRasterizer = new Direct2DGeometryRasterizer(surface, context),
                           RasterRasterizer = new Direct2DRasterRasterizer(surface, context),
                           TextRasterizer = new Direct2DTextRasterizer(surface, context)
                       };
        }
    }
}