//#define SourceDepth24

using System;
using AGG.Buffer;
using AGG.Color;
using AGG.Gamma;
using AGG.ImageFilter;
using AGG.Interpolation;
//using pix_format_e = AGG.UI.PlatformSupportAbstract.PixelFormats;
using AGG.PixelFormat;
using AGG.Rasterizer;
using AGG.Rendering;
using AGG.Scanline;
using AGG.Transform;
using AGG.UI;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;

namespace AGG
{
    public class image_resample_application<T> : AGG.UI.win32.PlatformSupport<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        RasterizerScanlineAA<T> g_rasterizer;
        ScanlineUnpacked8 g_scanline;
        T g_x1 = M.Zero<T>();
        T g_y1 = M.Zero<T>();
        T g_x2 = M.Zero<T>();
        T g_y2 = M.Zero<T>();
        GammaLut m_gamma_lut;
        UI.polygon_ctrl<T> m_quad;
        AGG.UI.rbox_ctrl<T> m_trans_type;
        AGG.UI.SliderWidget<T> m_gamma;
        AGG.UI.SliderWidget<T> m_blur;
        T m_old_gamma;

        image_resample_application(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            m_gamma_lut = new GammaLut(2.0);
            m_quad = new AGG.UI.polygon_ctrl<T>(4, 5.0);
            m_trans_type = new AGG.UI.rbox_ctrl<T>(400, 5.0, 430 + 170.0, 100.0);
            m_gamma = new AGG.UI.SliderWidget<T>(5.0, 5.0 + 15 * 0, 400 - 5, 10.0 + 15 * 0);
            m_blur = new AGG.UI.SliderWidget<T>(5.0, 5.0 + 15 * 1, 400 - 5, 10.0 + 15 * 1);
            m_old_gamma = M.New<T>(2);

            g_rasterizer = new RasterizerScanlineAA<T>();
            g_scanline = new ScanlineUnpacked8();

            m_trans_type.text_size(7);
            m_trans_type.add_item("Affine No Resample");
            m_trans_type.add_item("Affine Resample");
            m_trans_type.add_item("Perspective No Resample LERP");
            m_trans_type.add_item("Perspective No Resample Exact");
            m_trans_type.add_item("Perspective Resample LERP");
            m_trans_type.add_item("Perspective Resample Exact");
            m_trans_type.cur_item(4);
            AddChild(m_trans_type);

            m_gamma.range(0.5, 3.0);
            m_gamma.value(2.0);
            m_gamma.label("Gamma={0:F3}");
            AddChild(m_gamma);

            m_blur.range(0.5, 5.0);
            m_blur.value(1.0);
            m_blur.label("Blur={0:F3}");
            AddChild(m_blur);
        }

        public override void OnInitialize()
        {
            g_x1 = M.Zero<T>();
            g_y1 = M.Zero<T>();
            g_x2 = M.New<T>(rbuf_img(0).Width);
            g_y2 = M.New<T>(rbuf_img(0).Height);

            T x1 = g_x1;// * 100.0;
            T y1 = g_y1;// * 100.0;
            T x2 = g_x2;// * 100.0;
            T y2 = g_y2;// * 100.0;

            T dx = width().Divide(2.0).Subtract(x2.Subtract(x1).Divide(2.0));
            T dy = height().Divide(2.0).Subtract(y2.Subtract(y1).Divide(2.0));
            m_quad.SetXN(0, x1.Add(dx).Floor());
            m_quad.SetYN(0, y1.Add(dy).Floor());// - 150;
            m_quad.SetXN(1, x2.Add(dx).Floor());
            m_quad.SetYN(1, y1.Add(dy).Floor());// - 110;
            m_quad.SetXN(2, x2.Add(dx).Floor());
            m_quad.SetYN(2, y2.Add(dy).Floor());// - 300;
            m_quad.SetXN(3, x1.Add(dx).Floor());
            m_quad.SetYN(3, y2.Add(dy).Floor());// - 200;

#if SourceDepth24
            pixfmt_alpha_blend_rgb pixf = new pixfmt_alpha_blend_rgb(rbuf_img(0), new blender_bgr());
#else
            FormatRGBA pixf = new FormatRGBA(rbuf_img(0), new BlenderBGRA());
#endif
            //pixf.apply_gamma_dir(m_gamma_lut);

            base.OnInitialize();
        }

        public override void OnDraw()
        {
            if (m_gamma.value().NotEqual(m_old_gamma))
            {
                m_gamma_lut.Gamma(m_gamma.value().ToDouble());
                load_img(0, "spheres");
                FormatRGB pixf_change_gamma = new FormatRGB(rbuf_img(0), new BlenderBGR());
                //pixf_change_gamma.apply_gamma_dir(m_gamma_lut);
                m_old_gamma = m_gamma.value();
            }

#if SourceDepth24
            pixfmt_alpha_blend_rgb pixf = new pixfmt_alpha_blend_rgb(rbuf_window(), new blender_bgr());
            pixfmt_alpha_blend_rgb pixf_pre = new pixfmt_alpha_blend_rgb(rbuf_window(), new blender_bgr_pre());
#else
            FormatRGBA pixf = new FormatRGBA(rbuf_window(), new BlenderBGRA());
            FormatRGBA pixf_pre = new FormatRGBA(rbuf_window(), new BlenderPreMultBGRA());
#endif
            FormatClippingProxy clippingProxy = new FormatClippingProxy(pixf);
            FormatClippingProxy clippingProxy_pre = new FormatClippingProxy(pixf_pre);

            clippingProxy.Clear(new RGBA_Doubles(1, 1, 1));

            if (m_trans_type.cur_item() < 2)
            {
                // For the IAffineTransformMatrix<T> parallelogram transformations we
                // calculate the 4-th (implicit) point of the parallelogram
                m_quad.SetXN(3, m_quad.xn(0).Add(m_quad.xn(2).Subtract(m_quad.xn(1))));
                m_quad.SetYN(3, m_quad.yn(0).Add(m_quad.yn(2).Subtract(m_quad.yn(1))));
            }

            //--------------------------
            // Render the "quad" tool and controls
            g_rasterizer.AddPath(m_quad);
            Renderer<T>.RenderSolid(clippingProxy, g_rasterizer, g_scanline, new RGBA_Bytes(0, 0.3, 0.5, 0.1));

            // Prepare the polygon to rasterize. Here we need to fill
            // the destination (transformed) polygon.
            g_rasterizer.SetVectorClipBox(0, 0, width().ToDouble(), height().ToDouble());
            g_rasterizer.Reset();
            int b = 0;
            g_rasterizer.MoveToDbl(m_quad.xn(0).Subtract(b), m_quad.yn(0).Subtract(b));
            g_rasterizer.LineToDbl(m_quad.xn(1).Add(b), m_quad.yn(1).Subtract(b));
            g_rasterizer.LineToDbl(m_quad.xn(2).Add(b), m_quad.yn(2).Add(b));
            g_rasterizer.LineToDbl(m_quad.xn(3).Subtract(b), m_quad.yn(3).Add(b));

            //typedef agg::span_allocator<color_type> span_alloc_type;
            SpanAllocator sa = new SpanAllocator();
            ImageFilterBilinear<T> filter_kernel = new ImageFilterBilinear<T>();
            ImageFilterLookUpTable<T> filter = new ImageFilterLookUpTable<T>(filter_kernel, true);

#if SourceDepth24
            pixfmt_alpha_blend_rgb pixf_img = new pixfmt_alpha_blend_rgb(rbuf_img(0), new blender_bgr());
#else
            FormatRGBA pixf_img = new FormatRGBA(rbuf_img(0), new BlenderBGRA());
#endif
            RasterBufferAccessorClamp source = new RasterBufferAccessorClamp(pixf_img);

            start_timer();

            switch (m_trans_type.cur_item())
            {
                case 0:
                    {
                        /*
                                agg::trans_affine tr(m_quad.polygon(), g_x1, g_y1, g_x2, g_y2);

                                typedef agg::span_interpolator_linear<agg::trans_affine> interpolator_type;
                                interpolator_type interpolator(tr);

                                typedef image_filter_2x2_type<source_type, 
                                                              interpolator_type> span_gen_type;
                                span_gen_type sg(source, interpolator, filter);
                                agg::render_scanlines_aa(g_rasterizer, g_scanline, rb_pre, sa, sg);
                         */
                        break;
                    }

                case 1:
                    {
                        /*
                                agg::trans_affine tr(m_quad.polygon(), g_x1, g_y1, g_x2, g_y2);

                                typedef agg::span_interpolator_linear<agg::trans_affine> interpolator_type;
                                typedef image_resample_affine_type<source_type> span_gen_type;

                                interpolator_type interpolator(tr);
                                span_gen_type sg(source, interpolator, filter);
                                sg.blur(m_blur.Value());
                                agg::render_scanlines_aa(g_rasterizer, g_scanline, rb_pre, sa, sg);
                         */
                        break;
                    }

                case 2:
                    {
                        /*
                                agg::trans_perspective tr(m_quad.polygon(), g_x1, g_y1, g_x2, g_y2);
                                if(tr.is_valid())
                                {
                                    typedef agg::span_interpolator_linear_subdiv<agg::trans_perspective> interpolator_type;
                                    interpolator_type interpolator(tr);

                                    typedef image_filter_2x2_type<source_type,
                                                                  interpolator_type> span_gen_type;
                                    span_gen_type sg(source, interpolator, filter);
                                    agg::render_scanlines_aa(g_rasterizer, g_scanline, rb_pre, sa, sg);
                                }
                         */
                        break;
                    }

                case 3:
                    {
                        /*
                                agg::trans_perspective tr(m_quad.polygon(), g_x1, g_y1, g_x2, g_y2);
                                if(tr.is_valid())
                                {
                                    typedef agg::span_interpolator_trans<agg::trans_perspective> interpolator_type;
                                    interpolator_type interpolator(tr);

                                    typedef image_filter_2x2_type<source_type, 
                                                                  interpolator_type> span_gen_type;
                                    span_gen_type sg(source, interpolator, filter);
                                    agg::render_scanlines_aa(g_rasterizer, g_scanline, rb_pre, sa, sg);
                                }
                         */
                        break;
                    }

                case 4:
                    {
                        //typedef agg::span_interpolator_persp_lerp<> interpolator_type;
                        //typedef agg::span_subdiv_adaptor<interpolator_type> subdiv_adaptor_type;

                        SpanInterpolatorPerspLerp<T> interpolator = new SpanInterpolatorPerspLerp<T>(m_quad.polygon(), g_x1, g_y1, g_x2, g_y2);
                        SpanSubDivAdaptor<T> subdiv_adaptor = new SpanSubDivAdaptor<T>(interpolator);

                        if (interpolator.IsValid())
                        {
#if SourceDepth24
                        span_image_resample_rgb sg = new span_image_resample_rgb(source, subdiv_adaptor, filter);
#else
                            span_image_resample_rgba<T> sg = new span_image_resample_rgba<T>(source, subdiv_adaptor, filter);
#endif
                            sg.Blur = m_blur.value().ToDouble();
                            Renderer<T>.GenerateAndRender(g_rasterizer, g_scanline, clippingProxy_pre, sa, sg);
                        }
                        break;
                    }

                case 5:
                    {
                        /*
                                typedef agg::span_interpolator_persp_exact<> interpolator_type;
                                typedef agg::span_subdiv_adaptor<interpolator_type> subdiv_adaptor_type;

                                interpolator_type interpolator(m_quad.polygon(), g_x1, g_y1, g_x2, g_y2);
                                subdiv_adaptor_type subdiv_adaptor(interpolator);

                                if(interpolator.is_valid())
                                {
                                    typedef image_resample_type<source_type, 
                                                                subdiv_adaptor_type> span_gen_type;
                                    span_gen_type sg(source, subdiv_adaptor, filter);
                                    sg.blur(m_blur.Value());
                                    agg::render_scanlines_aa(g_rasterizer, g_scanline, rb_pre, sa, sg);
                                }
                         */
                        break;
                    }
            }

            double tm = elapsed_time();
            //pixf.apply_gamma_inv(m_gamma_lut);

            GsvText<T> t = new GsvText<T>();
            t.SetFontSize(10.0);

            ConvStroke<T> pt = new ConvStroke<T>(t);
            pt.Width = M.New<T>(1.5);

            string buf = string.Format("{0:F2} ms", tm);
            t.StartPoint(10.0, 70.0);
            t.Text = buf;

            g_rasterizer.AddPath(pt);
            Renderer<T>.RenderSolid(clippingProxy, g_rasterizer, g_scanline, new RGBA_Bytes(0, 0, 0));

            //--------------------------
            //m_trans_type.Render(g_rasterizer, g_scanline, clippingProxy);
            //m_gamma.Render(g_rasterizer, g_scanline, clippingProxy);
            //m_blur.Render(g_rasterizer, g_scanline, clippingProxy);
            base.OnDraw();
        }

        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            if (mouseEvent.Button == MouseButtons.Left)
            {
                m_quad.OnMouseDown(mouseEvent);
                if (mouseEvent.Handled)
                {
                    force_redraw();
                }
            }

            base.OnMouseDown(mouseEvent);
        }

        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
            if (mouseEvent.Button == MouseButtons.Left)
            {
                m_quad.OnMouseMove(mouseEvent);
                if (mouseEvent.Handled)
                {
                    force_redraw();
                }
            }

            base.OnMouseMove(mouseEvent);
        }

        public override void OnMouseUp(MouseEventArgs mouseEvent)
        {
            m_quad.OnMouseUp(mouseEvent);
            if (mouseEvent.Handled)
            {
                force_redraw();
            }

            base.OnMouseUp(mouseEvent);
        }

        public override void OnKeyDown(AGG.UI.KeyEventArgs keyEvent)
        {
            if (keyEvent.KeyCode == Keys.Space)
            {
                T cx = m_quad.xn(0).Add(m_quad.xn(1)).Add(m_quad.xn(2)).Add(m_quad.xn(3)).Divide(4);
                T cy = m_quad.yn(0).Add(m_quad.yn(1)).Add(m_quad.yn(2)).Add(m_quad.yn(3)).Divide(4);
                IAffineTransformMatrix<T> tr = MatrixFactory<T>.NewTranslation(cx.Negative(), cy.Negative());
                tr.RotateAlong(MatrixFactory<T>.CreateVector2D(0, 0), (Math.PI / 2.0));
                tr.Translate(MatrixFactory<T>.CreateVector2D(cx, cy));
                T xn0 = m_quad.xn(0); T yn0 = m_quad.yn(0);
                T xn1 = m_quad.xn(1); T yn1 = m_quad.yn(1);
                T xn2 = m_quad.xn(2); T yn2 = m_quad.yn(2);
                T xn3 = m_quad.xn(3); T yn3 = m_quad.yn(3);
                tr.Transform(ref xn0, ref yn0);
                tr.Transform(ref xn1, ref yn1);
                tr.Transform(ref xn2, ref yn2);
                tr.Transform(ref xn3, ref yn3);
                m_quad.SetXN(0, xn0); m_quad.SetYN(0, yn0);
                m_quad.SetXN(1, xn1); m_quad.SetYN(1, yn1);
                m_quad.SetXN(2, xn2); m_quad.SetYN(2, yn2);
                m_quad.SetXN(3, xn3); m_quad.SetYN(3, yn3);
                force_redraw();
            }

            base.OnKeyDown(keyEvent);
        }

        public static void StartDemo()
        {
#if SourceDepth24
            image_resample_application app = new image_resample_application(pix_format_e.pix_format_bgr24, agg.ui.platform_support_abstract.ERenderOrigin.OriginBottomLeft);
#else
            image_resample_application<T> app = new image_resample_application<T>(PixelFormats.pix_format_bgra32, ERenderOrigin.OriginBottomLeft);
#endif
            app.Caption = "AGG Example. Image Transformations with Resampling";

            string img_name = "spheres";
            if (!app.load_img(0, img_name))
            {
                string buf;
                buf = "File not found: "
                    + img_name
                    + app.img_ext()
                    + ". Download http://www.antigrain.com/" + img_name + app.img_ext() + "\n"
                    + "or copy it from another directory if available.";
                app.message(buf);
            }
            else
            {
                if (app.init(600, 600, (uint)WindowFlags.Risizeable))
                {
                    app.run();
                }
            }
        }


    };

    public static class App
    {
        [STAThread]
        public static void Main(string[] args)
        {
            image_resample_application<DoubleComponent>.StartDemo();
        }
    }
}
