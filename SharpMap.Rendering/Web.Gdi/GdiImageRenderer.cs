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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using GeoAPI.IO;
using SharpMap.Presentation.AspNet;
using SharpMap.Presentation.AspNet.MVP;
using SharpMap.Rendering.Gdi;
using SharpMap.Rendering.Rendering2D;
using GdiMatrix = System.Drawing.Drawing2D.Matrix;
using GdiColorMatrix = System.Drawing.Imaging.ColorMatrix;

namespace SharpMap.Rendering.Web
{
    public class GdiImageRenderer
        : IWebMapRenderer<Image>
    {
        private static ImageCodecInfo _defaultCodec;
        private readonly PixelFormat _pixelFormat;
        private readonly RenderQueue _renderQ = new RenderQueue();
        private GdiMatrix _gdiViewMatrix;

        private ImageCodecInfo _imageCodecInfo;
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

        public WebMapView MapView { get; set; }

        public Image Render(out string mimeType)
        {
            Bitmap bmp = new Bitmap(Width, Height, PixelFormat);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                if (!MapView.Presenter.IsRenderingSelection)
                    g.Clear(ViewConverter.Convert(MapView.BackgroundColor));
                while (_renderQ.Count > 0)
                {
                    GdiRenderObject ro = this._renderQ.Dequeue();
                    RenderObject(ro, g);
                }
            }
            mimeType = "image/bmp";
            return bmp;
        }

        public IRasterRenderer2D CreateRasterRenderer()
        {
            return new GdiRasterRenderer();
        }

        public IVectorRenderer2D CreateVectorRenderer()
        {
            return new WebGdiVectorRenderer();
        }

        public ITextRenderer2D CreateTextRenderer()
        {
            return new GdiTextRenderer();
        }

        public double Dpi { get; set; }

        public Type GetRenderObjectType()
        {
            return typeof(GdiRenderObject);
        }

        public void ClearRenderQueue()
        {
            _renderQ.Clear();
        }

        public void EnqueueRenderObject(object o)
        {
            _renderQ.Enqueue((GdiRenderObject)o);
        }

        public event EventHandler RenderDone;

        Stream IWebMapRenderer.Render(WebMapView map, out string mimeType)
        {
            return RenderStreamInternal(map, out mimeType);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Type GeometryRendererType
        {
            get { return typeof(BasicGeometryRenderer2D<>); }
        }

        public Type LabelRendererType
        {
            get { return typeof(BasicLabelRenderer2D<>); }
        }

        #endregion

        protected virtual Stream RenderStreamInternal(WebMapView map, out string mimeType)
        {
            Image im = this.Render(out mimeType);
            if (im == null)
                return null;

            MemoryStream ms = new MemoryStream();
            im.Save(ms, ImageCodec, EncoderParams);
            im.Dispose();
            ms.Position = 0;
            mimeType = ImageCodec.MimeType;
            return ms;

        }

        private static void RenderObject(GdiRenderObject ro, Graphics g)
        {
            RenderState state = ro.State;
            if (state == RenderState.Unknown)
                return;

            switch (state)
            {
                case RenderState.Normal:
                    using (PathParams pn = new PathParams(ro.GdiPath, ro.Line, ro.Fill, ro.Outline, ro.Font))
                        DrawPath(g, ro, pn);
                    break;
                case RenderState.Highlighted:
                    using (PathParams ph = new PathParams(ro.GdiPath, ro.HighlightLine, ro.HighlightFill, ro.HighlightOutline, ro.Font))
                        DrawPath(g, ro, ph);
                    break;
                case RenderState.Selected:
                    using (PathParams ps = new PathParams(ro.GdiPath, ro.SelectLine, ro.SelectFill, ro.SelectOutline, ro.Font))
                        DrawPath(g, ro, ps);
                    break;
                default:
                    break;
            }

            if (ro.Image == null)
                return;

            ImageAttributes imageAttributes = null;
            if (ro.ColorTransform != null)
            {
                imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(ro.ColorTransform);
            }

            if (imageAttributes != null)
            {
                g.DrawImage(ro.Image, GetPoints(ro.Bounds), GetSourceRegion(ro.Image.Size), GraphicsUnit.Pixel, imageAttributes);
            }
            else
            {
                g.DrawImage(ro.Image, GetPoints(ro.Bounds), GetSourceRegion(ro.Image.Size), GraphicsUnit.Pixel);
            }
        }

        public class PathParams : IDisposable
        {
            public PathParams(GraphicsPath path, Pen line, Brush fill, Pen outline, Font font)
            {
                this.Path = path;
                this.Line = CloneOf(line);                
                this.Fill = CloneOf(fill);
                this.Outline = CloneOf(outline);
                this.Font = CloneOf(font);
            }            

            public GraphicsPath Path { get; private set; }

            public Pen Line { get; private set; }            
            public Brush Fill { get; private set; }
            public Pen Outline { get; private set; }
            public Font Font { get; private set; }

            private static T CloneOf<T>(T item) where T : class, ICloneable 
            {
                if (item == null)
                    return null;
                lock (item)
                    return (T)item.Clone();
            }

            public void Dispose()
            {
                if (Path != null)
                {
                    Path.Dispose();
                    Path = null;
                }
                if (Line != null)
                {
                    Line.Dispose();
                    Line = null;
                }
                if (Outline != null)
                {
                    Outline.Dispose();
                    Outline = null;
                }
                if (Fill != null)
                {
                    Fill.Dispose();
                    Fill = null;
                }
                if (Font != null)
                {
                    Font.Dispose();
                    Font = null;
                }
            }
        }

        private static void DrawPath(Graphics g, GdiRenderObject ro, PathParams dpp)
        {
            if (dpp.Path != null)
            {
                if (dpp.Line != null)
                {
                    if (dpp.Outline != null)
                        g.DrawPath(dpp.Outline, dpp.Path);
                    g.DrawPath(dpp.Line, dpp.Path);
                }
                else if (dpp.Fill != null)
                {
                    g.FillPath(dpp.Fill, dpp.Path);
                    if (dpp.Outline != null)
                        g.DrawPath(dpp.Outline, dpp.Path);
                }
            }

            if (ro.Text != null && dpp.Fill != null)
            {
                RectangleF newBounds = AdjustForLabel(g, ro);
                g.DrawString(ro.Text, dpp.Font, dpp.Fill, newBounds);
            }
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

        [Obsolete]
        private GdiMatrix GetGdiViewTransform()
        {
            if (_gdiViewMatrix == null)
            {
                _gdiViewMatrix = MapView.ToViewTransform == null
                                     ? new GdiMatrix()
                                     : ViewConverter.Convert(MapView.ToViewTransform);

                return _gdiViewMatrix;
            }

            Matrix2D viewMatrix = MapView.ToViewTransform ?? new Matrix2D();
            Single[] gdiElements = _gdiViewMatrix.Elements;

            if (gdiElements[0] != (Single)viewMatrix.M11
                || gdiElements[1] != (Single)viewMatrix.M12
                || gdiElements[2] != (Single)viewMatrix.M21
                || gdiElements[3] != (Single)viewMatrix.M22
                || gdiElements[4] != (Single)viewMatrix.OffsetX
                || gdiElements[5] != (Single)viewMatrix.OffsetY)
            {
                Debug.WriteLine(
                    String.Format(
                        "Disposing GDI matrix on values: {0} : {1}; {2} : {3}; {4} : {5}; {6} : {7}; {8} : {9}; {10} : {11}",
                        gdiElements[0],
                        (Single)viewMatrix.M11,
                        gdiElements[1],
                        (Single)viewMatrix.M12,
                        gdiElements[2],
                        (Single)viewMatrix.M21,
                        gdiElements[3],
                        (Single)viewMatrix.M22,
                        gdiElements[4],
                        (Single)viewMatrix.OffsetX,
                        gdiElements[5],
                        (Single)viewMatrix.OffsetY));

                _gdiViewMatrix.Dispose();
                _gdiViewMatrix = ViewConverter.Convert(MapView.ToViewTransform);
            }

            return _gdiViewMatrix;
        }


        private static PointF[] GetPoints(RectangleF bounds)
        {
            // NOTE: This flips the image along the x-axis at the image's center
            // in order to compensate for the Transform which is in effect 
            // on the Graphics object during OnPaint
            PointF location = bounds.Location;
            PointF[] symbolTargetPointsTransfer = new PointF[3];
            symbolTargetPointsTransfer[0] = new PointF(location.X, location.Y + bounds.Height);
            symbolTargetPointsTransfer[1] = new PointF(location.X + bounds.Width,
                                                       location.Y + bounds.Height);
            symbolTargetPointsTransfer[2] = location;
            return symbolTargetPointsTransfer;
        }

        protected static RectangleF AdjustForLabel(Graphics g, GdiRenderObject ro)
        {
            // this transform goes from the underlying coordinates to 
            // screen coordinates, but for some reason renders text upside down
            // we cannot just scale by 1, -1 because offsets are affected also
            GdiMatrix m = g.Transform;
            // used to scale text size for the current zoom level
            float scale = Math.Abs(m.Elements[0]);

            // get the bounds of the label in the underlying coordinate space
            Point ll = new Point((Int32)ro.Bounds.X, (Int32)ro.Bounds.Y);
            Point ur = new Point((Int32)(ro.Bounds.X + ro.Bounds.Width),
                                 (Int32)(ro.Bounds.Y + ro.Bounds.Height));

            Point[] transformedPoints1 =
                {
                    new Point((Int32) ro.Bounds.X, (Int32) ro.Bounds.Y),
                    new Point((Int32) (ro.Bounds.X + ro.Bounds.Width),
                              (Int32) (ro.Bounds.Y + ro.Bounds.Height))
                };

            // get the label bounds transformed into screen coordinates
            // note that if we just render this as-is the label is upside down
            m.TransformPoints(transformedPoints1);

            // for labels, we're going to use an identity matrix and screen coordinates
            GdiMatrix newM = new GdiMatrix();

            Boolean scaleText = true;

            /*
                        if (ro.Layer != null)
                        {
                            Double min = ro.Layer.Style.MinVisible;
                            Double max = ro.Layer.Style.MaxVisible;
                            float scaleMult = Double.IsInfinity(max) ? 2.0f : 1.0f;

                            //max = Math.Min(max, _presenter.MaximumWorldWidth);
                            max = Math.Min(max, Map.Extents.Width);
                            //Double pct = (max - _presenter.WorldWidth) / (max - min);
                            Double pct = 1 - (Math.Min(_presenter.WorldWidth, Map.Extents.Width) - min) / (max - min);

                            if (scaleMult > 1)
                            {
                                pct = Math.Max(.5, pct * 2);
                            }

                            scale = (float)pct*scaleMult;
                            labelScale = scale;
                        }
            */

            // ok, I lied, if we're scaling labels we need to scale our new matrix, but still no offsets
            if (scaleText)
                newM.Scale(scale, scale);
            else
                scale = 1.0f;

            g.Transform = newM;

            Int32 pixelWidth = ur.X - ll.X;
            Int32 pixelHeight = ur.Y - ll.Y;

            // if we're scaling text, then x,y position will get multiplied by our 
            // scale, so adjust for it here so that we can use actual pixel x,y
            // Also center our label on the coordinate instead of putting the label origin on the coordinate
            RectangleF newBounds = new RectangleF(transformedPoints1[0].X / scale,
                                                  (transformedPoints1[0].Y / scale) - pixelHeight,
                                                  pixelWidth,
                                                  pixelHeight);
            //RectangleF newBounds = new RectangleF(transformedPoints1[0].X / scale - (pixelWidth / 2), transformedPoints1[0].Y / scale - (pixelHeight / 2), pixelWidth, pixelHeight);

            return newBounds;
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

        #region Nested type: RenderQueue

        private class RenderQueue : Queue<GdiRenderObject>
        {
            public event EventHandler ItemQueued;

            public new void Enqueue(GdiRenderObject o)
            {
                base.Enqueue(o);
                if (ItemQueued != null)
                    ItemQueued(this, EventArgs.Empty);
            }
        }

        #endregion
    }

    internal class WebGdiVectorRenderer : GdiVectorRenderer
    {
        private readonly Dictionary<SymbolLookupKey, Bitmap> _symbolCache = new Dictionary<SymbolLookupKey, Bitmap>();

        public override IEnumerable<GdiRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbol,
                                                                   Symbol2D highlightSymbol, Symbol2D selectSymbol,
                                                                   RenderState renderState)
        {
            if (renderState == RenderState.Selected) symbol = selectSymbol;
            if (renderState == RenderState.Highlighted) symbol = highlightSymbol;

            foreach (Point2D location in locations)
            {
                Bitmap bitmapSymbol = getSymbol(symbol);
                if (bitmapSymbol.PixelFormat != PixelFormat.Undefined)
                {
                    GdiMatrix transform = ViewConverter.Convert(symbol.AffineTransform);
                    GdiColorMatrix colorTransform = ViewConverter.Convert(symbol.ColorTransform);
                    RectangleF bounds = new RectangleF(ViewConverter.Convert(location), bitmapSymbol.Size);
                    GdiRenderObject holder = new GdiRenderObject(bitmapSymbol, bounds, transform, colorTransform);
                    holder.State = renderState;
                    yield return holder;
                }
                else
                    Debug.WriteLine("Unkbown pixel format");
            }
        }

        private Bitmap getSymbol(Symbol2D symbol2D)
        {
            if (symbol2D == null)
            {
                return null;
            }

            SymbolLookupKey key = new SymbolLookupKey(symbol2D.GetHashCode());
            Bitmap symbol;
            _symbolCache.TryGetValue(key, out symbol);

            if (symbol == null)
            {
                lock (symbol2D)
                {
                    MemoryStream data = new MemoryStream();
                    symbol2D.SymbolData.Position = 0;

                    using (BinaryReader reader = new BinaryReader(new NondisposingStream(symbol2D.SymbolData)))
                    {
                        data.Write(reader.ReadBytes((Int32)symbol2D.SymbolData.Length), 0,
                                   (Int32)symbol2D.SymbolData.Length);
                    }

                    symbol = new Bitmap(data);
                    if (symbol.PixelFormat != PixelFormat.Undefined)
                    {
                        _symbolCache[key] = symbol;
                    }
                    else
                    {
                        symbol = null;
                    }
                }
            }

            return symbol ?? getSymbol(symbol2D);
        }
    }
}