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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GeoAPI.IO;
using SharpMap.Presentation.AspNet;
using SharpMap.Presentation.AspNet.MVP;
using SharpMap.Rendering.Wpf;
using SharpMap.Rendering.Rendering2D;
using WpfMatrix = System.Windows.Media.Matrix;
//using GdiColorMatrix = System.Drawing.Imaging.ColorMatrix;

namespace SharpMap.Rendering.Web
{
    public class WpfImageRenderer
        : IWebMapRenderer<BitmapSource>
    {
        private static Type _defaultEncoder;
        private readonly PixelFormat _pixelFormat;
        private readonly RenderQueue _renderQ = new RenderQueue();

        private Type _imageEncoder;

        public WpfImageRenderer(PixelFormat pxfrmt)
        {
            _pixelFormat = pxfrmt;
        }

        public WpfImageRenderer()
            : this(PixelFormats.Pbgra32)
        {
        }

        public PixelFormat PixelFormat
        {
            get { return _pixelFormat; }
        }

        public Type ImageEncoder
        {
            get { return _imageEncoder ?? (_imageEncoder = GetDefaultEncoder()); }
            set { _imageEncoder = value; }
        }

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

        public BitmapSource Render(WebMapView mapView, out string mimeType)
        {
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dvc = dv.RenderOpen())
            {
                while (_renderQ.Count > 0)
                {
                    RenderObject(_renderQ.Dequeue(), dvc);
                }
            }
            RenderTargetBitmap bmp = new RenderTargetBitmap(Width, Height, Dpi, Dpi, PixelFormat);
            bmp.Clear();
            bmp.Render(dv);
            bmp.Freeze();

            mimeType = "";
            return bmp;
        }

        public static void WorkRenderQueue()
        {
            
        }

        public IRasterRenderer2D CreateRasterRenderer()
        {
            return new WpfRasterRenderer();
        }

        public IVectorRenderer2D CreateVectorRenderer()
        {
            return new WpfVectorRenderer();
        }

        public ITextRenderer2D CreateTextRenderer()
        {
            return new WpfTextRenderer();
        }

        public double Dpi { get; set; }

        public Type GetRenderObjectType()
        {
            return typeof(WpfRenderObject);
        }

        public void ClearRenderQueue()
        {
            _renderQ.Clear();
        }

        public void EnqueueRenderObject(object o)
        {
            _renderQ.Enqueue((WpfRenderObject)o);
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

        private void Dispose(bool disposing)
        {
            //nothing to do
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
            BitmapSource im = Render(map, out mimeType);

            if (im == null)
                return null;

            MemoryStream ms = new MemoryStream();
            BitmapEncoder bm = (BitmapEncoder) Activator.CreateInstance(ImageEncoder);
            bm.Frames.Add(BitmapFrame.Create(im));
            bm.Save(ms);

            ms.Position = 0;
            mimeType = bm.CodecInfo.MimeTypes;
            return ms;

        }

        private static void RenderObject(WpfRenderObject ro, DrawingContext g)
        {
            if (ro == null)
                return;

            if (ro.RenderState == RenderState.Unknown)
                return;

            if (ro is WpfVectorRenderObject)
            {
                WpfVectorRenderObject vro = (WpfVectorRenderObject) ro;
                if (vro.Outline != null)
                    g.DrawGeometry(null, vro.Outline, vro.Path);
                g.DrawGeometry(vro.Fill, vro.Line, vro.Path);
            }

            if (ro is WpfPointRenderObject)
            {
                WpfPointRenderObject pro = (WpfPointRenderObject) ro;
                g.DrawImage(pro.Symbol, pro.Bounds);
            }

            if (ro is WpfTextRenderObject<Point>)
            {
                WpfTextRenderObject<Point> trro = (WpfTextRenderObject<Point>) ro;
                g.DrawText(trro.Text, trro.Location);
            }
            if (ro is WpfTextRenderObject<StreamGeometry>)
            {
            }

            if (ro is WpfRasterRenderObject)
            {
                WpfRasterRenderObject rro = (WpfRasterRenderObject) ro;
                g.DrawImage(rro.Raster, rro.Bounds);
            }
        }

        public static Type FindEncoder(string mimeType)
        {
            var encoders = new Reflector().AllSubclassesOf(typeof (BitmapEncoder));
            return (from encoder in encoders
                    select (BitmapEncoder) Activator.CreateInstance(encoder)
                    into bme where bme.CodecInfo.MimeTypes.Contains(mimeType) select bme.GetType()).FirstOrDefault();
        }

        private static Type GetDefaultEncoder()
        {
            if (_defaultEncoder == null)
                _defaultEncoder = typeof(PngBitmapEncoder);
            return _defaultEncoder;
        }

        private static Rect GetSourceRegion(Size sz)
        {
            return new Rect(new Point(0, 0), sz);
        }

        private static Point[] GetPoints(Rect bounds)
        {
            // NOTE: This flips the image along the x-axis at the image's center
            // in order to compensate for the Transform which is in effect 
            // on the Graphics object during OnPaint
            Point location = bounds.Location;
            Point[] symbolTargetPointsTransfer = new Point[3];
            symbolTargetPointsTransfer[0] = new Point(location.X, location.Y + bounds.Height);
            symbolTargetPointsTransfer[1] = new Point(location.X + bounds.Width,
                                                      location.Y + bounds.Height);
            symbolTargetPointsTransfer[2] = location;
            return symbolTargetPointsTransfer;
        }

        //protected static Rect AdjustForLabel(Drawing g, WpfTextRenderObject<Rect> ro)
        //{
        //    // this transform goes from the underlying coordinates to 
        //    // screen coordinates, but for some reason renders text upside down
        //    // we cannot just scale by 1, -1 because offsets are affected also
        //    WpfMatrix m = g.Transform;
        //    // used to scale text size for the current zoom level
        //    float scale = Math.Abs(m.Elements[0]);

        //    // get the bounds of the label in the underlying coordinate space
        //    Point ll = new Point((Int32)ro.Bounds.X, (Int32)ro.Bounds.Y);
        //    Point ur = new Point((Int32)(ro.Bounds.X + ro.Bounds.Width),
        //                         (Int32)(ro.Bounds.Y + ro.Bounds.Height));

        //    Point[] transformedPoints1 =
        //        {
        //            new Point((Int32) ro.Bounds.X, (Int32) ro.Bounds.Y),
        //            new Point((Int32) (ro.Bounds.X + ro.Bounds.Width),
        //                      (Int32) (ro.Bounds.Y + ro.Bounds.Height))
        //        };

        //    // get the label bounds transformed into screen coordinates
        //    // note that if we just render this as-is the label is upside down
        //    m.TransformPoints(transformedPoints1);

        //    // for labels, we're going to use an identity matrix and screen coordinates
        //    GdiMatrix newM = new GdiMatrix();

        //    Boolean scaleText = true;

        //    /*
        //                if (ro.Layer != null)
        //                {
        //                    Double min = ro.Layer.Style.MinVisible;
        //                    Double max = ro.Layer.Style.MaxVisible;
        //                    float scaleMult = Double.IsInfinity(max) ? 2.0f : 1.0f;

        //                    //max = Math.Min(max, _presenter.MaximumWorldWidth);
        //                    max = Math.Min(max, Map.Extents.Width);
        //                    //Double pct = (max - _presenter.WorldWidth) / (max - min);
        //                    Double pct = 1 - (Math.Min(_presenter.WorldWidth, Map.Extents.Width) - min) / (max - min);

        //                    if (scaleMult > 1)
        //                    {
        //                        pct = Math.Max(.5, pct * 2);
        //                    }

        //                    scale = (float)pct*scaleMult;
        //                    labelScale = scale;
        //                }
        //    */

        //    // ok, I lied, if we're scaling labels we need to scale our new matrix, but still no offsets
        //    if (scaleText)
        //        newM.Scale(scale, scale);
        //    else
        //        scale = 1.0f;

        //    g.Transform = newM;

        //    Int32 pixelWidth = ur.X - ll.X;
        //    Int32 pixelHeight = ur.Y - ll.Y;

        //    // if we're scaling text, then x,y position will get multiplied by our 
        //    // scale, so adjust for it here so that we can use actual pixel x,y
        //    // Also center our label on the coordinate instead of putting the label origin on the coordinate
        //    RectangleF newBounds = new RectangleF(transformedPoints1[0].X / scale,
        //                                          (transformedPoints1[0].Y / scale) - pixelHeight,
        //                                          pixelWidth,
        //                                          pixelHeight);
        //    //RectangleF newBounds = new RectangleF(transformedPoints1[0].X / scale - (pixelWidth / 2), transformedPoints1[0].Y / scale - (pixelHeight / 2), pixelWidth, pixelHeight);

        //    return newBounds;
        //}

        #region Nested type: RenderQueue

        private class RenderQueue : Queue<WpfRenderObject>
        {
            public event EventHandler ItemQueued;

            public new void Enqueue(WpfRenderObject o)
            {
                base.Enqueue(o);
                if (ItemQueued != null)
                    ItemQueued(this, EventArgs.Empty);
            }
        }

        #endregion
    }

    class Reflector
    {
        List<Assembly> assemblies = new List<Assembly> { 
            typeof(BitmapEncoder).Assembly
        };
        public IEnumerable<Type> AllSubclassesOf(Type super)
        {
            return from a in assemblies from t in a.GetExportedTypes() where t.IsSubclassOf(super) select t;
        }
    }

}