using System;
using AGG.Color;
using AGG.Gamma;
using AGG.PixelFormat;
using AGG.Rasterizer;
using AGG.Rendering;
using AGG.Scanline;
using AGG.UI;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;

namespace AGG
{
    public class rounded_rect_application<T> : AGG.UI.win32.PlatformSupport<T>
          where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T[] m_x = new T[2];
        T[] m_y = new T[2];
        T m_dx;
        T m_dy;
        int m_idx;
        AGG.UI.SliderWidget<T> m_radius;
        AGG.UI.SliderWidget<T> m_gamma;
        AGG.UI.SliderWidget<T> m_offset;
        AGG.UI.cbox_ctrl<T> m_white_on_black;
        AGG.UI.cbox_ctrl<T> m_DrawAsOutlineCheckBox;


        public rounded_rect_application(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            m_idx = (-1);
            m_radius = new AGG.UI.SliderWidget<T>(10, 10, 600 - 10, 19);
            m_gamma = new AGG.UI.SliderWidget<T>(10, 10 + 20, 600 - 10, 19 + 20);
            m_offset = new AGG.UI.SliderWidget<T>(10, 10 + 40, 600 - 10, 19 + 40);
            m_white_on_black = new cbox_ctrl<T>(10, 10 + 60, "White on black");
            m_DrawAsOutlineCheckBox = new cbox_ctrl<T>(10 + 180, 10 + 60, "Fill Rounded Rect");

            m_x[0] = M.New<T>(100); m_y[0] = M.New<T>(100);
            m_x[1] = M.New<T>(500); m_y[1] = M.New<T>(350);
            AddChild(m_radius);
            AddChild(m_gamma);
            AddChild(m_offset);
            AddChild(m_white_on_black);
            AddChild(m_DrawAsOutlineCheckBox);
            m_gamma.label("gamma={0:F3}");
            m_gamma.range(0.0, 3.0);
            m_gamma.value(1.8);

            m_radius.label("radius={0:F3}");
            m_radius.range(0.0, 50.0);
            m_radius.value(25.0);

            m_offset.label("subpixel offset={0:F3}");
            m_offset.range(-2.0, 3.0);

            m_white_on_black.text_color(new RGBA_Bytes(127, 127, 127));
            m_white_on_black.inactive_color(new RGBA_Bytes(127, 127, 127));

            m_DrawAsOutlineCheckBox.text_color(new RGBA_Doubles(.5, .5, .5));
            m_DrawAsOutlineCheckBox.inactive_color(new RGBA_Bytes(127, 127, 127));
        }


        public override void OnDraw()
        {
            GammaLut gamma = new GammaLut(m_gamma.value().ToDouble());
            IBlender NormalBlender = new BlenderBGRA();
            IBlender GammaBlender = new BlenderGammaBGRA(gamma);
            FormatRGBA pixf = new FormatRGBA(rbuf_window(), NormalBlender);
            FormatClippingProxy clippingProxy = new FormatClippingProxy(pixf);

            clippingProxy.Clear(m_white_on_black.status() ? new RGBA_Doubles(0, 0, 0) : new RGBA_Doubles(1, 1, 1));

            RasterizerScanlineAA<T> ras = new RasterizerScanlineAA<T>();
            ScanlinePacked8 sl = new ScanlinePacked8();

            Ellipse<T> e = new Ellipse<T>();

            // TODO: If you drag the control circles below the bottom of the window we get an exception.  This does not happen in AGG.
            // It needs to be debugged.  Turning on clipping fixes it.  But standard agg works without clipping.  Could be a bigger problem than this.
            //ras.clip_box(0, 0, width(), height());

            // Render two "control" circles
            e.Init(m_x[0], m_y[0], M.New<T>(3), M.New<T>(3), 16);
            ras.AddPath(e);
            Renderer<T>.RenderSolid(clippingProxy, ras, sl, new RGBA_Bytes(127, 127, 127));
            e.Init(m_x[1], m_y[1], M.New<T>(3), M.New<T>(3), 16);
            ras.AddPath(e);
            Renderer<T>.RenderSolid(clippingProxy, ras, sl, new RGBA_Bytes(127, 127, 127));

            T d = m_offset.value();

            // Creating a rounded rectangle
            RoundedRect<T> r = new RoundedRect<T>(m_x[0].Add(d), m_y[0].Add(d), m_x[1].Add(d), m_y[1].Add(d), m_radius.value());
            r.NormalizeRadius();

            // Drawing as an outline
            if (!m_DrawAsOutlineCheckBox.status())
            {
                ConvStroke<T> p = new ConvStroke<T>(r);
                p.Width = M.One<T>();
                ras.AddPath(p);
            }
            else
            {
                ras.AddPath(r);
            }

            pixf.Blender = GammaBlender;
            Renderer<T>.RenderSolid(clippingProxy, ras, sl, m_white_on_black.status() ? new RGBA_Bytes(1, 1, 1) : new RGBA_Bytes(0, 0, 0));

            // this was in the original demo, but it does nothing because we changed the blender not the gamma function.
            //ras.gamma(new gamma_none());
            // so let's change the blender instead
            pixf.Blender = NormalBlender;

            // Render the controls
            //m_radius.Render(ras, sl, clippingProxy);
            //m_gamma.Render(ras, sl, clippingProxy);
            //m_offset.Render(ras, sl, clippingProxy);
            //m_white_on_black.Render(ras, sl, clippingProxy);
            //m_DrawAsOutlineCheckBox.Render(ras, sl, clippingProxy);
            base.OnDraw();
        }


        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            if (mouseEvent.Button == MouseButtons.Left)
            {
                for (int i = 0; i < 2; i++)
                {
                    T x = M.New<T>(mouseEvent.X);
                    T y = M.New<T>(mouseEvent.Y);

                    T mdx = x.Subtract(m_x[i]), mdy = y.Subtract(m_y[i]);

                    if (M.Length(mdx, mdy).LessThan(5.0))
                    {
                        m_dx = mdx;// x - m_x[i];
                        m_dy = mdy;// y - m_y[i];
                        m_idx = i;
                        break;
                    }
                }
            }

            base.OnMouseDown(mouseEvent);
        }


        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
            if (mouseEvent.Button == MouseButtons.Left)
            {
                if (m_idx >= 0)
                {
                    m_x[m_idx] = M.New<T>(mouseEvent.X).Subtract(m_dx);
                    m_y[m_idx] = M.New<T>(mouseEvent.Y).Subtract(m_dy);
                    force_redraw();
                }
            }

            base.OnMouseMove(mouseEvent);
        }

        override public void OnMouseUp(MouseEventArgs mouseEvent)
        {
            m_idx = -1;
            base.OnMouseUp(mouseEvent);
        }

        public static void StartDemo()
        {
            rounded_rect_application<T> app = new rounded_rect_application<T>(PixelFormats.pix_format_rgba32, ERenderOrigin.OriginBottomLeft);
            app.Caption = "AGG Example. Rounded rectangle with gamma-correction & stuff";

            if (app.init(600, 400, (uint)AGG.UI.PlatformSupportAbstract<T>.WindowFlags.Risizeable))
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
            rounded_rect_application<DoubleComponent>.StartDemo();
        }
    }
}
