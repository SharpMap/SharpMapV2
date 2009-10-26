using System;
using System.IO;
using System.Text;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.GeoJson
{
    public class GeoJsonRasterizeSurface : IRasterizeSurface<StringBuilder, TextWriter>
    {
        private IMapView2D _view;
        public GeoJsonRasterizeSurface(IMapView2D view)
        {
            _view = view;
        }

        private readonly object _backLock = new object();
        private readonly object _frontLock = new object();

        private StringBuilder _backSurface;

        private StringBuilder _frontSurface;

        #region IRasterizeSurface<StringBuilder,TextWriter> Members

        public StringBuilder BackSurface
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

        public StringBuilder FrontSurface
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

        public IRasterizers<StringBuilder, TextWriter> RetrieveSurface()
        {
            if (BackSurface == null)
                return CreateSurface();
            return CreateRasterizers(BackSurface, new StringWriter(BackSurface));
        }

        public IRasterizers<StringBuilder, TextWriter> CreateSurface()
        {
            StringBuilder sb = new StringBuilder();
            TextWriter w = new StringWriter(sb);
            BackSurface = sb;
            return CreateRasterizers(sb, w);
        }

        public IMapView2D MapView
        {
            get { return _view; }
        }

        public event EventHandler RenderComplete;

        public void RenderCompleted()
        {
            FrontSurface = BackSurface;
            BackSurface = null;
            MapView.Display(FrontSurface);
            OnRenderComplete();
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

        private static IRasterizers<StringBuilder, TextWriter> CreateRasterizers(StringBuilder builder, TextWriter writer)
        {
            return new GeoJsonRasterizers
                       {
                           GeometryRasterizer = new GeoJsonGeometryRasterizer(builder, writer),
                           TextRasterizer = new GeoJsonTextRasterizer(builder, writer),
                           RasterRasterizer = new GeoJsonRasterRasterizer(builder, writer)
                       };
        }

        private void OnRenderComplete()
        {
            if (RenderComplete != null)
                RenderComplete(this, EventArgs.Empty);
        }
    }
}