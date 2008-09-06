using System;
using AGG.Color;
//using pix_format_e = AGG.UI.PlatformSupportAbstract.PixelFormats;
using AGG.PixelFormat;
using AGG.Rasterizer;
using AGG.Rendering;
using AGG.Scanline;
using AGG.UI;
using NPack;
using NPack.Interfaces;

namespace AGG
{
    public class component_rendering_application<T> : AGG.UI.win32.PlatformSupport<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private AGG.UI.SliderWidget<T> m_alpha;
        AGG.UI.cbox_ctrl<T> m_UseBlackBackground;

        public component_rendering_application(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            m_alpha = new AGG.UI.SliderWidget<T>(5, 5, 320 - 5, 10 + 5);
            m_UseBlackBackground = new UI.cbox_ctrl<T>(5, 5 + 18, "Draw Black Background");
            m_alpha.label("Alpha={0:F0}");
            m_alpha.range(0, 255);
            m_alpha.value(255);
            AddChild(m_alpha);
            AddChild(m_UseBlackBackground);
            m_UseBlackBackground.text_color(new RGBA_Bytes(127, 127, 127));
        }

        public override void OnDraw()
        {
            FormatRGB pf = new FormatRGB(rbuf_window(), new BlenderBGR());

            FormatGray pfr = new FormatGray(rbuf_window(), new BlenderGray(), 3, 2);
            FormatGray pfg = new FormatGray(rbuf_window(), new BlenderGray(), 3, 1);
            FormatGray pfb = new FormatGray(rbuf_window(), new BlenderGray(), 3, 0);

            FormatClippingProxy clippingProxy = new FormatClippingProxy(pf);
            FormatClippingProxy clippingProxyRed = new FormatClippingProxy(pfr);
            FormatClippingProxy clippingProxyGreen = new FormatClippingProxy(pfg);
            FormatClippingProxy clippingProxyBlue = new FormatClippingProxy(pfb);

            RasterizerScanlineAA<T> ras = new RasterizerScanlineAA<T>();
            ScanlinePacked8 sl = new ScanlinePacked8();

            clippingProxy.Clear(m_UseBlackBackground.status() ? new RGBA_Doubles(0, 0, 0) : new RGBA_Doubles(1, 1, 1));

            RGBA_Bytes FillColor = m_UseBlackBackground.status() ? new RGBA_Bytes(255, 255, 255, (uint)(m_alpha.value().ToInt())) : new RGBA_Bytes(0, 0, 0, (uint)(m_alpha.value().ToInt()));

            VertexSource.Ellipse<T> er = new AGG.VertexSource.Ellipse<T>(width().Divide(2).Subtract(0.87 * 50), height().Divide(2).Subtract(0.5 * 50), M.New<T>(100), M.New<T>(100), 100);
            ras.AddPath(er);
            Renderer<T>.RenderSolid(clippingProxyRed, ras, sl, FillColor);

            VertexSource.Ellipse<T> eg = new AGG.VertexSource.Ellipse<T>(width().Divide(2).Add(0.87 * 50), height().Divide(2).Subtract(0.5 * 50), M.New<T>(100), M.New<T>(100), 100);
            ras.AddPath(eg);
            Renderer<T>.RenderSolid(clippingProxyGreen, ras, sl, FillColor);
            //renderer_scanlines.render_scanlines_aa_solid(ras, sl, rbg, new gray8(0, unsigned(m_alpha.Value())));

            VertexSource.Ellipse<T> eb = new AGG.VertexSource.Ellipse<T>(width().Divide(2), height().Divide(2).Add(50), M.New<T>(100), M.New<T>(100), 100);
            ras.AddPath(eb);
            Renderer<T>.RenderSolid(clippingProxyBlue, ras, sl, FillColor);
            //renderer_scanlines.render_scanlines_aa_solid(ras, sl, rbb, new gray8(0, unsigned(m_alpha.Value())));

            //m_alpha.Render(ras, sl, clippingProxy);
            //m_UseBlackBackground.Render(ras, sl, clippingProxy);
            base.OnDraw();
        }

        public static void StartDemo()
        {
            component_rendering_application<T> app = new component_rendering_application<T>(PixelFormats.pix_format_rgb24, ERenderOrigin.OriginBottomLeft);
            app.Caption = "AGG Example. Component Rendering";

            if (app.init(320, 320, (uint)WindowFlags.None))
            {
                app.run();
            }
        }


    };

    public static class App
    {
        [STAThread]
        public static void Main(string[] args)
        {
            component_rendering_application<DoubleComponent>.StartDemo();
        }
    }
}
