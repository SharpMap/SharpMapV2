using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using AGGSharp.Drawing.Interface;
using AGG;
using AGG.PixelFormat;
using AGG.RasterizerScanline;
namespace AGGSharp.Drawing
{

    internal static class IoC
    {
        private static IUnityContainer _container;

        static IoC()
        {
            _container = new UnityContainer();
            IoC.ConfigureContainer(_container);
        }


        public static IUnityContainer Instance
        {
            get
            {
                return _container;
            }
        }


        internal static void ConfigureContainer(IUnityContainer container)
        {
            container.RegisterType<IUnityContainer, UnityContainer>();
            container.RegisterType<IGraphicsContext, GraphicsContext>();
            container.RegisterType<IGraphics, Graphics>();
            container.RegisterType<IStroke, Stroke>();
            container.RegisterType<IPixelFormatFactory, PixelFormatFactory>();
            //container.RegisterType<IRasterBuffer, RasterBuffer>(); //use the static factory instead.
            container.RegisterType<IBlender, BlenderBGRA>("bgra");
            container.RegisterType<IBlender, BlenderBGR>("bgr");
            container.RegisterType<IBlenderGray, blender_gray>();
            container.RegisterType<RendererBase, Renderer>();
            container.RegisterType<IRasterizer, rasterizer_scanline_aa>();
            container.RegisterType<IRasterizer, rasterizer_scanline_aa>("scanlineAA");
            container.RegisterType<IScanlineCache, scanline_unpacked_8>();
            container.RegisterType<IScanlineCache, scanline_unpacked_8>("unpacked8");
            container.RegisterType<IScanlineCache, scanline_packed_8>("packed8");


        }

    }
}
