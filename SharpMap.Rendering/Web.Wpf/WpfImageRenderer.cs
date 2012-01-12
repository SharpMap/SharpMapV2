using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpMap.Presentation.AspNet;
using SharpMap.Presentation.AspNet.MVP;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Wpf;

namespace SharpMap.Rendering.Web.Wpf
{
    public class WpfImageRenderer : IWebMapRenderer<Visual>
    {
        private bool _disposed;
        private WebMapView _mapView;

        ~WpfImageRenderer()
        {
            Dispose(false);
        }

        public WebMapView MapView
        {
            get { return _mapView; }
            set
            {
                _mapView = value;
                RasterizeSurface = new WpfRasterizeSurface(value);
            }
        }

        public double Dpi
        {
            get { return 96.0; }
        }

        public IRasterizeSurface RasterizeSurface { get; protected set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }

        private static Visual Render(WebMapView mapView, out string mimeType)
        {
            mimeType = "image/png";
            return mapView.RenderedData as Visual;
        }

        Visual IWebMapRenderer<Visual>.Render(WebMapView mapView, out string mimeType)
        {
            return Render(mapView, out mimeType);
        }

        Stream IWebMapRenderer.Render(WebMapView mapView, out string mimeType)
        {
            var rtb = new RenderTargetBitmap(Convert.ToInt32(mapView.ViewSize.Width), Convert.ToInt32(mapView.ViewSize.Height), mapView.Dpi, mapView.Dpi, PixelFormats.Default);

            var visual = Render(mapView, out mimeType);

            //var brush = Brushes.Blue;
            //var visual = new DrawingVisual();
            //using (var drawingConext = visual.RenderOpen())
            //{
            //    drawingConext.DrawRectangle(brush, null, new Rect(new Point(10, 10), new Size(50, 50)));
            //}
            
            rtb.Render(visual);

            var memoryStream = new MemoryStream();
            mimeType = "image/png";
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            encoder.Save(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
