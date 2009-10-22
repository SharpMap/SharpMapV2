using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Cairo;
using SharpMap.Presentation.AspNet;
using SharpMap.Presentation.AspNet.MVP;
using SharpMap.Rendering.Cairo;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using CairoGraphics = Cairo.Context;
using CairoLineCap = Cairo.LineCap;
using CairoLineJoin = Cairo.LineJoin;
using CairoAntialias = Cairo.Antialias;
using Rectangle = System.Drawing.Rectangle;

namespace SharpMap.Rendering.Web.Cairo
{
    public class CairoImageRenderer : IWebMapRenderer<Image>
    {
        static CairoImageRenderer()
        {
            string bin = ConfigurationManager.AppSettings["CairoUnmanagedBinDir"];
            string path = Environment.GetEnvironmentVariable("path");
            if (!path.Contains(bin))
                Environment.SetEnvironmentVariable("path", path + ";" + bin);

        }

        private static ImageCodecInfo _defaultCodec;
        private readonly RenderQueue _renderQueue = new RenderQueue();
        private ImageCodecInfo _imageCodecInfo;
        private PixelFormat _pixelFormat;
        private bool disposed;

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
            using (Surface s = CreateSurface())
            {
                Context c = new Context(s);
                if (!MapView.Presenter.IsRenderingSelection)
                {
                    SetColour(c, MapView.BackgroundColor);
                    c.FillExtents();
                }


                while (_renderQueue.Count > 0)
                {
                    RenderObject(_renderQueue.Dequeue(), c);
                }


                Bitmap bmp = new Bitmap(Width, Height, PixelFormat);

                using (Win32Surface winSurface = new Win32Surface(bmp.GetHbitmap()))
                using (Context context = new Context(winSurface))
                {
                    context.SetSourceSurface(s, 0, 0);
                    context.Paint();
                }

                mimeType = "image/bmp";
                return bmp;
            }
        }

        public WebMapView MapView { get; set; }

        public double Dpi { get; set; }

        public Type GeometryRendererType
        {
            get { return typeof(BasicGeometryRenderer2D<>); }
        }

        public Type LabelRendererType
        {
            get { return typeof(BasicLabelRenderer2D<>); }
        }

        public Type GetRenderObjectType()
        {
            return typeof(CairoRenderObject);
        }

        public void ClearRenderQueue()
        {
            _renderQueue.Clear();
        }

        public void EnqueueRenderObject(object o)
        {
            _renderQueue.Enqueue((CairoRenderObject)o);
        }


        public event EventHandler RenderDone;

        Stream IWebMapRenderer.Render(WebMapView mapView, out string mimeType)
        {
            return RenderStreamInternal(mapView, out mimeType);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IRasterRenderer2D CreateRasterRenderer()
        {
            return new CairoRasterRenderer();
        }

        public IVectorRenderer2D CreateVectorRenderer()
        {
            return new CairoVectorRenderer();
        }

        public ITextRenderer2D CreateTextRenderer()
        {
            return new CairoTextRenderer();
        }

        #endregion

        protected virtual Surface CreateSurface()
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
            ms.Position = 0;
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

        private void RenderObject(CairoRenderObject ro, Context g)
        {
            if (ro.State == RenderState.Unknown)
                return;

            CreatePath(g, ro.Path);

            switch (ro.State)
            {
                case RenderState.Normal:
                    if (ro.Path != null)
                    {
                        if (ro.Line != null)
                        {
                            if (ro.Outline != null)
                                StrokePath(g, ro.Outline);

                            StrokePath(g, ro.Line);
                        }
                        else if (ro.Fill != null)
                        {
                            FillPath(g, ro.Fill);

                            if (ro.Outline != null)
                                StrokePath(g, ro.Outline);
                        }
                    }

                    if (ro.Text != null)
                    {
                        Rectangle newBounds = AdjustForLabel(g, ro);
                        DrawText(g, ro.Text, ro.Font, ro.Fill, newBounds);
                        //g.DrawString(ro.Text, ro.Font, ro.Fill, newBounds);
                    }

                    break;
                case RenderState.Highlighted:
                    if (ro.Path != null)
                    {
                        if (ro.HighlightLine != null)
                        {
                            if (ro.HighlightOutline != null)
                                StrokePath(g, ro.HighlightOutline);

                            StrokePath(g, ro.HighlightLine);
                        }
                        else if (ro.HighlightFill != null)
                        {
                            FillPath(g, ro.HighlightFill);

                            if (ro.HighlightOutline != null)
                                StrokePath(g, ro.HighlightOutline);
                        }
                    }

                    if (ro.Text != null)
                    {
                        Rectangle newBounds = AdjustForLabel(g, ro);
                        DrawText(g, ro.Text, ro.Font, ro.HighlightFill, newBounds);
                        //g.DrawString(ro.Text, ro.Font, ro.HighlightFill, newBounds);
                    }

                    break;
                case RenderState.Selected:
                    if (ro.Path != null)
                    {
                        if (ro.SelectLine != null)
                        {
                            if (ro.SelectOutline != null)
                                StrokePath(g, ro.SelectOutline);

                            StrokePath(g, ro.SelectLine);
                        }
                        else if (ro.SelectFill != null)
                        {
                            FillPath(g, ro.SelectFill);

                            if (ro.SelectOutline != null)
                                StrokePath(g, ro.SelectOutline);
                        }
                    }

                    if (ro.Text != null)
                    {
                        Rectangle newBounds = AdjustForLabel(g, ro);
                        DrawText(g, ro.Text, ro.Font, ro.SelectFill, newBounds);
                        //g.DrawString(ro.Text, ro.Font, ro.SelectFill, newBounds);
                    }
                    break;
                default:
                    break;
            }

            if (ro.Image == null)
            {
                return;
            }


            PaintImage(g, ro);
            //g.DrawImage(ro.Image,
            //            GetPoints(ro.Bounds),
            //            GetSourceRegion(ro.Image.Size),
            //            GraphicsUnit.Pixel,
            //            imageAttributes);
        }

        private static void DrawText(Context context, string text, FontFace face, StyleBrush brush, Rectangle rectangle)
        {
            throw new NotImplementedException();
        }

        private static void PaintImage(Context context, CairoRenderObject ro)
        {
            throw new NotImplementedException();
        }

        private static Rectangle AdjustForLabel(Context context, CairoRenderObject ro)
        {
            throw new NotImplementedException();
        }

        private static void FillPath(Context context, StyleBrush brush)
        {
            SetColour(context, brush);
            context.FillPreserve();
        }

        private static void StrokePath(Context context, StylePen outline)
        {
            context.SetDash(ConvertDash(outline.DashPattern), outline.DashOffset);
            context.LineCap = ConvertCap(outline.EndCap);
            context.LineJoin = ConvertJoin(outline.LineJoin);
            context.LineWidth = outline.Width;
            context.Antialias = Antialias.Subpixel;
            context.MiterLimit = outline.MiterLimit;

            context.StrokePreserve();
        }

        private static void SetColour(Context context, StyleColor colour)
        {
            context.SetSourceRGBA(colour.R, colour.G,
                                  colour.B, colour.A);
        }

        private static void SetColour(Context context, StyleBrush brush)
        {
            SolidStyleBrush solid = brush as SolidStyleBrush;
            if (solid != null)
                SetColour(context, brush.Color);

            LinearGradientStyleBrush grad = brush as LinearGradientStyleBrush;
            if (grad != null)
                throw new NotImplementedException();
        }

        private static LineJoin ConvertJoin(StyleLineJoin join)
        {
            switch (join)
            {
                case StyleLineJoin.Bevel:
                    return LineJoin.Bevel;
                case StyleLineJoin.MiterClipped:
                case StyleLineJoin.Miter:
                    return LineJoin.Miter;
                default:
                    return LineJoin.Round;
            }
        }

        private static LineCap ConvertCap(StyleLineCap lineDashCap)
        {
            switch (lineDashCap)
            {
                case StyleLineCap.Square:
                    return LineCap.Square;
                case StyleLineCap.Round:
                    return LineCap.Round;
                case StyleLineCap.Flat:
                default:
                    return LineCap.Butt;
            }
        }

        private static double[] ConvertDash(float[] pattern)
        {
            double[] p = new double[pattern.Length];
            for (int i = 0; i < pattern.Length; i++)
                p[i] = pattern[i];
            return p;
        }

        private static void CreatePath(Context g, Path2D path2D)
        {
            g.NewPath();
            foreach (Figure2D figure in path2D)
            {
                g.NewSubPath();

                for (int i = 0; i < figure.Points.Count; i++)
                {
                    Point2D pt = figure.Points[i];
                    if (i == 0)
                        g.MoveTo(pt.X, pt.Y);
                    else
                        g.LineTo(pt.X, pt.Y);
                }
                if (figure.IsClosed)
                    g.ClosePath();
            }
        }

        #region Nested type: RenderQueue

        internal class RenderQueue : Queue<CairoRenderObject>
        {
            public event EventHandler ItemQueued;

            public new void Enqueue(CairoRenderObject o)
            {
                base.Enqueue(o);
                if (ItemQueued != null)
                    ItemQueued(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}