using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGG;
using AGG.PixelFormat;
using Microsoft.Practices.Unity;
using AGGSharp.Drawing.Interface;
using AGGSharp.Utility;

namespace AGGSharp.Drawing
{
    public sealed class PixMap : IPixMap, IInternalPixMap
    {
        private IUnityContainer _injector;
        private byte[] imageBytes;
        private RendererBase _renderer;
        private IRasterizer _rasterizer;
        private IScanlineCache _scanlineCache;

        public IScanlineCache ScanlineCache { get { return _scanlineCache; } }
        public IRasterizer Rasterizer { get { return _rasterizer; } }
        public RendererBase Renderer { get { return _renderer; } }

        internal IPixelFormat _pixelFormat;

        internal IRasterBuffer _buffer;

        public IGraphics CreateGraphics()
        {
            IGraphics g = _injector.Resolve<IGraphics>();

            return g;
        }

        public IPixelFormat PixelFormat
        {
            get
            {
                return _pixelFormat;
            }
        }


        public uint Width
        {
            get
            {
                return _pixelFormat.width();
            }
        }

        public uint Height
        {
            get
            {
                return _pixelFormat.height();
            }
        }

        public uint Stride
        {
            get
            {
                return (uint)_pixelFormat.stride();
            }
        }

        public PixMap(IPixelFormat pixFormat)
        {
            _pixelFormat = pixFormat;
            _buffer = pixFormat.GetRenderingBuffer();
        }

        public PixMap(PixelFormat format, uint width, uint height)
            : this()
        {
            Guard.GreaterThan(width, 0u);
            Guard.GreaterThan(height, 0u);
            _pixelFormat = _injector.Resolve<IPixelFormatFactory>().CreatePixelFormat(format, width, height, out imageBytes);
            _buffer = _pixelFormat.GetRenderingBuffer();

            _renderer = this._injector.Resolve<RendererBase>();
            _scanlineCache = this._injector.Resolve<IScanlineCache>();
            _rasterizer = this._injector.Resolve<IRasterizer>();
        }


        private void ConfigureContainer()
        {
            _injector = IoC.Instance.Resolve<IUnityContainer>();
            _injector.RegisterInstance<IPixMap>(this);
            _injector.RegisterInstance<IInternalPixMap>(this);
            IoC.ConfigureContainer(_injector);

        }

        private PixMap()
        {
            ConfigureContainer();

        }



    }
}
