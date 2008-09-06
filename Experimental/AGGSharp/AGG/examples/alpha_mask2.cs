#define USE_CLIPPING_ALPHA_MASK

using System;
using AGG.Buffer;
using AGG.Color;
using AGG.PixelFormat;
using AGG.Rasterizer;
using AGG.Rendering;
using AGG.Scanline;
//using pix_format_e = AGG.UI.PlatformSupportAbstract.PixelFormats;
using AGG.Transform;
using AGG.UI;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;

namespace AGG
{
    public class alpha_mask2_application<T> : AGG.UI.win32.PlatformSupport<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        byte[] m_alpha_buf;

        T m_slider_value;

        private AGG.UI.SliderWidget<T> m_num_cb;

        RasterizerScanlineAA<T> g_rasterizer = new RasterizerScanlineAA<T>();
        ScanlinePacked8 g_scanline = new ScanlinePacked8();
        PathStorage<T> g_path = new PathStorage<T>();

        RGBA_Bytes[] g_colors = new RGBA_Bytes[100];
        uint[] g_path_idx = new uint[100];
        uint g_npaths = 0;
        T g_x1 = M.Zero<T>();
        T g_y1 = M.Zero<T>();
        T g_x2 = M.Zero<T>();
        T g_y2 = M.Zero<T>();
        T g_base_dx = M.Zero<T>();
        T g_base_dy = M.Zero<T>();
        T g_angle = M.Zero<T>();
        T g_scale = M.One<T>();
        T g_skew_x = M.Zero<T>();
        T g_skew_y = M.Zero<T>();

        RasterBuffer m_alpha_mask_rbuf;
        IAlphaMask m_alpha_mask;

        public void parse_lion()
        {
            g_npaths = AGG.samples.LionParser.parse_lion(g_path, g_colors, g_path_idx);
            AGG.BoundingRect<T>.GetBoundingRect(g_path, g_path_idx, 0, g_npaths, out g_x1, out g_y1, out g_x2, out g_y2);
            g_base_dx = g_x2.Subtract(g_x1).Divide(2.0);
            g_base_dy = g_y2.Subtract(g_y1).Divide(2.0);
        }

        public alpha_mask2_application(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            m_alpha_mask_rbuf = new RasterBuffer();
#if USE_CLIPPING_ALPHA_MASK
            m_alpha_mask = new AlphaMaskByteClipped(m_alpha_mask_rbuf, 1, 0);
#else
            m_alpha_mask = new AlphaMaskByteUnclipped(m_alpha_mask_rbuf, 1, 0);
#endif

            m_num_cb = new UI.SliderWidget<T>(5, 5, 150, 12);
            m_slider_value = M.Zero<T>();
            parse_lion();
            AddChild(m_num_cb);
            m_num_cb.range(5, 100);
            m_num_cb.value(10);
            m_num_cb.label("N={0:F3}");
            m_num_cb.SetTransform(MatrixFactory<T>.NewIdentity(VectorDimension.Two));
        }

        unsafe void generate_alpha_mask(int cx, int cy)
        {
            m_alpha_buf = new byte[cx * cy];
            fixed (byte* pAlphaBuffer = m_alpha_buf)
            {
#if USE_CLIPPING_ALPHA_MASK
                m_alpha_mask_rbuf.attach(pAlphaBuffer + 20 * cx + 20, (uint)cx - 40, (uint)cy - 40, cx, 1);
#else
                m_alpha_mask_rbuf.attach(pAlphaBuffer, (uint)cx, (uint)cy, cx, 1);
#endif

                FormatGray pixf = new FormatGray(m_alpha_mask_rbuf, new BlenderGray(), 1, 0);
                FormatClippingProxy clippingProxy = new FormatClippingProxy(pixf);
                ScanlinePacked8 sl = new ScanlinePacked8();

                clippingProxy.Clear(new RGBA_Doubles(0));

                VertexSource.Ellipse<T> ell = new AGG.VertexSource.Ellipse<T>();

                System.Random randGenerator = new Random(1432);

                int i;
                int num = (int)m_num_cb.value().ToInt();
                for (i = 0; i < num; i++)
                {
                    if (i == num - 1)
                    {
                        ell.Init(width().Divide(2), height().Divide(2), M.New<T>(110), M.New<T>(110), 100);
                        g_rasterizer.AddPath(ell);
                        Renderer<T>.RenderSolid(clippingProxy, g_rasterizer, sl, new RGBA_Bytes(0, 0, 0, 255));

                        ell.Init(ell.X, ell.Y, ell.RX.Subtract(10), ell.RY.Subtract(10), 100);
                        g_rasterizer.AddPath(ell);
                        Renderer<T>.RenderSolid(clippingProxy, g_rasterizer, sl, new RGBA_Bytes(255, 0, 0, 255));
                    }
                    else
                    {
                        ell.Init(randGenerator.Next() % cx,
                                 randGenerator.Next() % cy,
                                 randGenerator.Next() % 100 + 20,
                                 randGenerator.Next() % 100 + 20,
                                 100);
                        // set the color to draw into the alpha channel.
                        // there is not very much reason to set the alpha as you will get the amount of 
                        // transparency based on the color you draw.  (you might want some type of different edeg effect but it will be minor).
                        g_rasterizer.AddPath(ell);
                        Renderer<T>.RenderSolid(clippingProxy, g_rasterizer, sl, new RGBA_Bytes((uint)((float)i / (float)num * 255), 0, 0, 255));
                    }

                }

                m_alpha_mask_rbuf.dettachBuffer();
            }
        }


        public override void OnResize(int cx, int cy)
        {
            generate_alpha_mask(cx, cy);
        }

        public override void OnDraw()
        {
            int width = (int)rbuf_window().Width;
            int height = (int)rbuf_window().Height;

            if (m_num_cb.value().NotEqual(m_slider_value))
            {
                generate_alpha_mask(width, height);
                m_slider_value = m_num_cb.value();
            }

            g_rasterizer.SetVectorClipBox(0, 0, width, height);

            IPixelFormat pf = new FormatRGB(rbuf_window(), new BlenderBGR());

            unsafe
            {
                fixed (byte* pAlphaBuffer = m_alpha_buf)
                {
                    m_alpha_mask_rbuf.attach(pAlphaBuffer, (uint)width, (uint)height, width, 1);

                    AGG.PixelFormat.AlphaMaskAdaptor pixFormatAlphaMaskAdaptor = new AGG.PixelFormat.AlphaMaskAdaptor(pf, m_alpha_mask);
                    FormatClippingProxy alphaMaskClippingProxy = new FormatClippingProxy(pixFormatAlphaMaskAdaptor);
                    FormatClippingProxy clippingProxy = new FormatClippingProxy(pf);

                    IAffineTransformMatrix<T> mtx = MatrixFactory<T>.NewIdentity(VectorDimension.Two);
                    mtx.Translate(MatrixFactory<T>.CreateVector2D(g_base_dx.Negative(), g_base_dy.Negative()));
                    mtx.Scale(g_scale);
                    mtx.RotateAlong(MatrixFactory<T>.CreateVector2D(0, 0), g_angle.Add(Math.PI).ToDouble());
                    mtx.Shear(MatrixFactory<T>.CreateVector2D(g_skew_x.Divide(1000.0), g_skew_y.Divide(1000.0)));
                    mtx.Translate(MatrixFactory<T>.CreateVector2D(width / 2, height / 2));

                    clippingProxy.Clear(new RGBA_Doubles(1, 1, 1));

                    // draw a background to show how the mask is working better
                    int RectWidth = 30;
                    for (int i = 0; i < 40; i++)
                    {
                        for (int j = 0; j < 40; j++)
                        {
                            if ((i + j) % 2 != 0)
                            {
                                VertexSource.RoundedRect<T> rect = new VertexSource.RoundedRect<T>(i * RectWidth, j * RectWidth, (i + 1) * RectWidth, (j + 1) * RectWidth, 0);
                                rect.NormalizeRadius();

                                // Drawing as an outline
                                g_rasterizer.AddPath(rect);
                                Renderer<T>.RenderSolid(clippingProxy, g_rasterizer, g_scanline, new RGBA_Bytes(.9, .9, .9));
                            }
                        }
                    }

                    //int x, y;

                    // Render the lion
                    ConvTransform<T> trans = new ConvTransform<T>(g_path, mtx);
                    Renderer<T>.RenderSolidAllPaths(alphaMaskClippingProxy, g_rasterizer, g_scanline, trans, g_colors, g_path_idx, g_npaths);

                    /*
                    // Render random Bresenham lines and markers
                    agg::renderer_markers<amask_ren_type> m(r);
                    for(i = 0; i < 50; i++)
                    {
                        m.line_color(agg::rgba8(randGenerator.Next() & 0x7F, 
                                                randGenerator.Next() & 0x7F, 
                                                randGenerator.Next() & 0x7F, 
                                                (randGenerator.Next() & 0x7F) + 0x7F)); 
                        m.fill_color(agg::rgba8(randGenerator.Next() & 0x7F, 
                                                randGenerator.Next() & 0x7F, 
                                                randGenerator.Next() & 0x7F, 
                                                (randGenerator.Next() & 0x7F) + 0x7F));

                        m.line(m.coord(randGenerator.Next() % width), m.coord(randGenerator.Next() % height), 
                               m.coord(randGenerator.Next() % width), m.coord(randGenerator.Next() % height));

                        m.marker(randGenerator.Next() % width, randGenerator.Next() % height, randGenerator.Next() % 10 + 5,
                                 agg::marker_e(randGenerator.Next() % agg::end_of_markers));
                    }


                    // Render random anti-aliased lines
                    double w = 5.0;
                    agg::line_profile_aa profile;
                    profile.width(w);

                    typedef agg::renderer_outline_aa<amask_ren_type> renderer_type;
                    renderer_type ren(r, profile);

                    typedef agg::rasterizer_outline_aa<renderer_type> rasterizer_type;
                    rasterizer_type ras(ren);
                    ras.round_cap(true);

                    for(i = 0; i < 50; i++)
                    {
                        ren.Color = agg::rgba8(randGenerator.Next() & 0x7F, 
                                             randGenerator.Next() & 0x7F, 
                                             randGenerator.Next() & 0x7F, 
                                             //255));
                                             (randGenerator.Next() & 0x7F) + 0x7F); 
                        ras.move_to_d(randGenerator.Next() % width, randGenerator.Next() % height);
                        ras.line_to_d(randGenerator.Next() % width, randGenerator.Next() % height);
                        ras.render(false);
                    }


                    // Render random circles with gradient
                    typedef agg::gradient_linear_color<color_type> grad_color;
                    typedef agg::gradient_circle grad_func;
                    typedef agg::span_interpolator_linear<> interpolator_type;
                    typedef agg::span_gradient<color_type, 
                                              interpolator_type, 
                                              grad_func, 
                                              grad_color> span_grad_type;

                    agg::trans_affine grm;
                    grad_func grf;
                    grad_color grc(agg::rgba8(0,0,0), agg::rgba8(0,0,0));
                    agg::ellipse ell;
                    agg::span_allocator<color_type> sa;
                    interpolator_type inter(grm);
                    span_grad_type sg(inter, grf, grc, 0, 10);
                    agg::renderer_scanline_aa<amask_ren_type, 
                                              agg::span_allocator<color_type>,
                                              span_grad_type> rg(r, sa, sg);
                    for(i = 0; i < 50; i++)
                    {
                        x = randGenerator.Next() % width;
                        y = randGenerator.Next() % height;
                        double r = randGenerator.Next() % 10 + 5;
                        grm.reset();
                        grm *= agg::trans_affine_scaling(r / 10.0);
                        grm *= agg::trans_affine_translation(x, y);
                        grm.invert();
                        grc.colors(agg::rgba8(255, 255, 255, 0),
                                   agg::rgba8(randGenerator.Next() & 0x7F, 
                                              randGenerator.Next() & 0x7F, 
                                              randGenerator.Next() & 0x7F, 
                                              255));
                        sg.color_function(grc);
                        ell.init(x, y, r, r, 32);
                        g_rasterizer.add_path(ell);
                        agg::render_scanlines(g_rasterizer, g_scanline, rg);
                    }
                     */

                    //m_num_cb.Render(g_rasterizer, g_scanline, clippingProxy);
                }
                m_alpha_mask_rbuf.dettachBuffer();
            }
            base.OnDraw();
        }

        void transform(T width, T height, T x, T y)
        {
            x.SubtractEquals(width.Divide(2));
            y.SubtractEquals(height.Divide(2));
            g_angle = M.Atan2(y, x);
            g_scale = M.Length(y, x).Divide(100);// Math.Sqrt(y * y + x * x) / 100.0;
        }

        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            if (mouseEvent.Button == MouseButtons.Left)
            {
                T width = M.New<T>(rbuf_window().Width);
                T height = M.New<T>(rbuf_window().Height);
                transform(width, height, x, y);
                force_redraw();
            }

            if (mouseEvent.Button == MouseButtons.Right)
            {
                g_skew_x = x;
                g_skew_y = y;
                force_redraw();
            }

            base.OnMouseDown(mouseEvent);
        }

        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
            OnMouseDown(mouseEvent);
        }

        public static void StartDemo()
        {
            alpha_mask2_application<T> app = new alpha_mask2_application<T>(PixelFormats.pix_format_bgr24, ERenderOrigin.OriginBottomLeft);
            //alpha_mask2_application app = new alpha_mask2_application(pix_format_e.pix_format_bgra32, agg.ui.platform_support_abstract.ERenderOrigin.OriginBottomLeft);
            //alpha_mask2_application app = new alpha_mask2_application(pix_format_e.pix_format_rgba32, agg.ui.platform_support_abstract.ERenderOrigin.OriginBottomLeft);
            app.Caption = "AGG Example. Clipping to multiple rectangle regions";

            if (app.init(512, 400, (uint)WindowFlags.Risizeable))
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
            alpha_mask2_application<DoubleComponent>.StartDemo();
        }
    }
}
