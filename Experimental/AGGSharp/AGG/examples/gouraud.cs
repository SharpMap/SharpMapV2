//#define SourceDepth24

using System;
using AGG.Color;
using AGG.Gamma;
//using pix_format_e = AGG.UI.PlatformSupportAbstract.PixelFormats;
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
    public class gouraud_application<T> : AGG.UI.win32.PlatformSupport<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T[] m_x = new T[3];
        T[] m_y = new T[3];
        T m_dx;
        T m_dy;
        int m_idx;

        AGG.UI.SliderWidget<T> m_dilation;
        AGG.UI.SliderWidget<T> m_gamma;
        AGG.UI.SliderWidget<T> m_alpha;

        gouraud_application(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            m_idx = (-1);
            m_dilation = new AGG.UI.SliderWidget<T>(5, 5, 400 - 5, 11);
            m_gamma = new AGG.UI.SliderWidget<T>(5, 5 + 15, 400 - 5, 11 + 15);
            m_alpha = new AGG.UI.SliderWidget<T>(5, 5 + 30, 400 - 5, 11 + 30);
            m_x[0] = M.New<T>(57); m_y[0] = M.New<T>(60);
            m_x[1] = M.New<T>(369); m_y[1] = M.New<T>(170);
            m_x[2] = M.New<T>(143); m_y[2] = M.New<T>(310);

            AddChild(m_dilation);
            AddChild(m_gamma);
            AddChild(m_alpha);

            m_dilation.label("Dilation={0:F2}");
            m_gamma.label("Linear gamma={0:F2}");
            m_alpha.label("Opacity={0:F2}");

            m_dilation.value(0.175);
            m_gamma.value(0.809);
            m_alpha.value(1.0);
        }

        //template<class Scanline, class Ras> 
        public void render_gouraud(IScanlineCache sl, IRasterizer<T> ras)
        {
            T alpha = m_alpha.value();
            T brc = M.One<T>();

#if SourceDepth24
            pixfmt_alpha_blend_rgb pf = new pixfmt_alpha_blend_rgb(rbuf_window(), new blender_bgr());
#else
            FormatRGBA pf = new FormatRGBA(rbuf_window(), new BlenderBGRA());
#endif
            FormatClippingProxy ren_base = new FormatClippingProxy(pf);

            AGG.SpanAllocator span_alloc = new SpanAllocator();
            SpanGouraudRgba<T> span_gen = new SpanGouraudRgba<T>();

            ras.Gamma(new GammaLinear(0.0, m_gamma.value().ToDouble()));

            T d = m_dilation.value();

            // Six triangles
            T xc = m_x[0].Add(m_x[1]).Add(m_x[2]).Divide(3.0);
            T yc = m_y[0].Add(m_y[1]).Add(m_y[2]).Divide(3.0);

            T x1 = m_x[1].Add(m_x[0]).Divide(2).Subtract(xc.Subtract(m_x[1].Add(m_x[0]).Divide(2)));
            T y1 = m_y[1].Add(m_y[0]).Divide(2).Subtract(yc.Subtract(m_y[1].Add(m_y[0]).Divide(2)));

            T x2 = m_x[2].Add(m_x[1]).Divide(2).Subtract(xc.Subtract(m_x[2].Add(m_x[1]).Divide(2)));
            T y2 = m_y[2].Add(m_y[1]).Divide(2).Subtract(yc.Subtract(m_y[2].Add(m_y[1]).Divide(2)));

            T x3 = m_x[0].Add(m_x[2]).Divide(2).Subtract(xc.Subtract(m_x[0].Add(m_x[2]).Divide(2)));
            T y3 = m_y[0].Add(m_y[2]).Divide(2).Subtract(yc.Subtract(m_y[0].Add(m_y[2]).Divide(2)));

            span_gen.Colors(new RGBA_Doubles(1, 0, 0, alpha.ToDouble()),
                            new RGBA_Doubles(0, 1, 0, alpha.ToDouble()),
                            new RGBA_Doubles(brc.ToDouble(), brc.ToDouble(), brc.ToDouble(), alpha.ToDouble()));
            span_gen.Triangle(m_x[0], m_y[0], m_x[1], m_y[1], xc, yc, d);
            ras.AddPath(span_gen);
            Renderer<T>.GenerateAndRender(ras, sl, ren_base, span_alloc, span_gen);


            span_gen.Colors(new RGBA_Doubles(0, 1, 0, alpha.ToDouble()),
                            new RGBA_Doubles(0, 0, 1, alpha.ToDouble()),
                            new RGBA_Doubles(brc.ToDouble(), brc.ToDouble(), brc.ToDouble(), alpha.ToDouble()));
            span_gen.Triangle(m_x[1], m_y[1], m_x[2], m_y[2], xc, yc, d);
            ras.AddPath(span_gen);
            Renderer<T>.GenerateAndRender(ras, sl, ren_base, span_alloc, span_gen);


            span_gen.Colors(new RGBA_Doubles(0, 0, 1, alpha.ToDouble()),
                            new RGBA_Doubles(1, 0, 0, alpha.ToDouble()),
                            new RGBA_Doubles(brc.ToDouble(), brc.ToDouble(), brc.ToDouble(), alpha.ToDouble()));
            span_gen.Triangle(m_x[2], m_y[2], m_x[0], m_y[0], xc, yc, d);
            ras.AddPath(span_gen);
            Renderer<T>.GenerateAndRender(ras, sl, ren_base, span_alloc, span_gen);


            brc = M.One<T>().Subtract(brc);
            span_gen.Colors(new RGBA_Doubles(1, 0, 0, alpha.ToDouble()),
                            new RGBA_Doubles(0, 1, 0, alpha.ToDouble()),
                            new RGBA_Doubles(brc.ToDouble(), brc.ToDouble(), brc.ToDouble(), alpha.ToDouble()));
            span_gen.Triangle(m_x[0], m_y[0], m_x[1], m_y[1], x1, y1, d);
            ras.AddPath(span_gen);
            Renderer<T>.GenerateAndRender(ras, sl, ren_base, span_alloc, span_gen);


            span_gen.Colors(new RGBA_Doubles(0, 1, 0, alpha.ToDouble()),
                            new RGBA_Doubles(0, 0, 1, alpha.ToDouble()),
                            new RGBA_Doubles(brc.ToDouble(), brc.ToDouble(), brc.ToDouble(), alpha.ToDouble()));
            span_gen.Triangle(m_x[1], m_y[1], m_x[2], m_y[2], x2, y2, d);
            ras.AddPath(span_gen);
            Renderer<T>.GenerateAndRender(ras, sl, ren_base, span_alloc, span_gen);


            span_gen.Colors(new RGBA_Doubles(0, 0, 1, alpha.ToDouble()),
                            new RGBA_Doubles(1, 0, 0, alpha.ToDouble()),
                            new RGBA_Doubles(brc.ToDouble(), brc.ToDouble(), brc.ToDouble(), alpha.ToDouble()));
            span_gen.Triangle(m_x[2], m_y[2], m_x[0], m_y[0], x3, y3, d);
            ras.AddPath(span_gen);
            Renderer<T>.GenerateAndRender(ras, sl, ren_base, span_alloc, span_gen);
        }

        public override void OnDraw()
        {
#if SourceDepth24
            pixfmt_alpha_blend_rgb pf = new pixfmt_alpha_blend_rgb(rbuf_window(), new blender_bgr());
#else
            FormatRGBA pf = new FormatRGBA(rbuf_window(), new BlenderBGRA());
#endif
            FormatClippingProxy ren_base = new FormatClippingProxy(pf);
            ren_base.Clear(new RGBA_Doubles(1.0, 1.0, 1.0));

            ScanlineUnpacked8 sl = new ScanlineUnpacked8();
            RasterizerScanlineAA<T> ras = new RasterizerScanlineAA<T>();
#if true
            render_gouraud(sl, ras);
#else
            agg.span_allocator span_alloc = new span_allocator();
            span_gouraud_rgba span_gen = new span_gouraud_rgba(new rgba8(255, 0, 0, 255), new rgba8(0, 255, 0, 255), new rgba8(0, 0, 255, 255), 320, 220, 100, 100, 200, 100, 0);
            span_gouraud test_sg = new span_gouraud(new rgba8(0, 0, 0, 255), new rgba8(0, 0, 0, 255), new rgba8(0, 0, 0, 255), 320, 220, 100, 100, 200, 100, 0);
            ras.add_path(test_sg);
            renderer_scanlines.render_scanlines_aa(ras, sl, ren_base, span_alloc, span_gen);
            //renderer_scanlines.render_scanlines_aa_solid(ras, sl, ren_base, new rgba8(0, 0, 0, 255));
#endif


            ras.Gamma(new GammaNone());
            m_dilation.SetTransform(trans_affine_resizing());
            //m_dilation.Render(ras, sl, ren_base);
            m_gamma.SetTransform(trans_affine_resizing());
            //m_gamma.Render(ras, sl, ren_base);
            m_alpha.SetTransform(trans_affine_resizing());
            //m_alpha.Render(ras, sl, ren_base);
            base.OnDraw();
        }

        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            uint i;
            if (mouseEvent.Button == MouseButtons.Right)
            {
                ScanlineUnpacked8 sl = new ScanlineUnpacked8();
                RasterizerScanlineAA<T> ras = new RasterizerScanlineAA<T>();
                start_timer();
                for (i = 0; i < 100; i++)
                {
                    render_gouraud(sl, ras);
                }

                string buf;
                buf = "Time=" + elapsed_time().ToString() + "ms";
                message(buf);
            }

            if (mouseEvent.Button == MouseButtons.Left)
            {
                T x = M.New<T>(mouseEvent.X);
                T y = M.New<T>(mouseEvent.Y);

                for (i = 0; i < 3; i++)
                {
                    T mdx = x.Subtract(m_x[i]), mdy = y.Subtract(m_y[i]);

                    if (M.Length(mdx, mdy).LessThan(10.0))
                    {
                        m_dx = mdx;// x - m_x[i];
                        m_dy = mdy;// y - m_y[i];
                        m_idx = (int)i;
                        break;
                    }
                }
                if (i == 3)
                {
                    if (MathUtil.PointInTriangle(m_x[0], m_y[0],
                                              m_x[1], m_y[1],
                                              m_x[2], m_y[2],
                                              x, y))
                    {
                        m_dx = x.Subtract(m_x[0]);
                        m_dy = y.Subtract(m_y[0]);
                        m_idx = 3;
                    }

                }
            }

            base.OnMouseDown(mouseEvent);
        }


        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            if (mouseEvent.Button == MouseButtons.Left)
            {
                if (m_idx == 3)
                {
                    T dx = x.Subtract(m_dx);
                    T dy = y.Subtract(m_dy);
                    m_x[1].SubtractEquals(m_x[0].Subtract(dx));
                    m_y[1].SubtractEquals(m_y[0].Subtract(dy));
                    m_x[2].SubtractEquals(m_x[0].Subtract(dx));
                    m_y[2].SubtractEquals(m_y[0].Subtract(dy));
                    m_x[0] = dx;
                    m_y[0] = dy;
                    force_redraw();
                    mouseEvent.Handled = true;
                }
                else if (m_idx >= 0)
                {
                    m_x[m_idx] = x.Subtract(m_dx);
                    m_y[m_idx] = y.Subtract(m_dy);
                    force_redraw();
                }
            }
            else
            {
                OnMouseUp(mouseEvent);
            }

            base.OnMouseMove(mouseEvent);
        }

        public override void OnMouseUp(MouseEventArgs mouseEvent)
        {
            m_idx = -1;
            base.OnMouseUp(mouseEvent);
        }

        public override void OnKeyDown(AGG.UI.KeyEventArgs keyEvent)
        {
            T dx = M.Zero<T>();
            T dy = M.Zero<T>();
            switch (keyEvent.KeyCode)
            {
                case Keys.Left: dx = M.New<T>(-0.1); break;
                case Keys.Right: dx = M.New<T>(0.1); break;
                case Keys.Up: dy = M.New<T>(0.1); break;
                case Keys.Down: dy = M.New<T>(-0.1); break;
            }
            m_x[0].AddEquals(dx);
            m_y[0].AddEquals(dy);
            m_x[1].AddEquals(dx);
            m_y[1].AddEquals(dy);
            force_redraw();
            base.OnKeyDown(keyEvent);
        }

        public static void StartDemo()
        {
#if SourceDepth24
            gouraud_application app = new gouraud_application(pix_format_e.pix_format_rgb24, agg.ui.platform_support_abstract.ERenderOrigin.OriginBottomLeft);
#else
            gouraud_application<T> app = new gouraud_application<T>(PixelFormats.pix_format_rgba32, ERenderOrigin.OriginBottomLeft);
#endif
            app.Caption = "AGG Example. Gouraud Shading";

            if (app.init(400, 320, (uint)WindowFlags.Risizeable))
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
            gouraud_application<DoubleComponent>.StartDemo();
        }
    }
}




