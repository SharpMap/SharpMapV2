#define SourceDepth24

using System;
using AGG.Buffer;
using AGG.Color;
using AGG.ImageFilter;
using AGG.Interpolation;
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
    public class image_filters_application<T> : AGG.UI.win32.PlatformSupport<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        SliderWidget<T> m_radius;
        SliderWidget<T> m_step;
        rbox_ctrl<T> m_filters;
        cbox_ctrl<T> m_normalize;
        ButtonWidget<T> m_run;
        ButtonWidget<T> m_single_step;
        ButtonWidget<T> m_refresh;
        ScanlinePacked8 m_ScanlinePacked;
        RasterizerScanlineAA<T> m_Rasterizer;
        ScanlineUnpacked8 m_ScanlineUnpacked;
        SpanAllocator m_SpanAllocator;

        double m_cur_angle;
        int m_cur_filter;
        int m_num_steps;
        double m_num_pix;
        double m_time1;
        double m_time2;

        image_filters_application(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            m_step = new SliderWidget<T>(115, 5, 400, 11);
            m_radius = new SliderWidget<T>(115, 5 + 15, 400, 11 + 15);
            m_filters = new rbox_ctrl<T>(0.0, 0.0, 110.0, 210.0);
            m_normalize = new cbox_ctrl<T>(8.0, 215.0, "Normalize Filter");

            m_refresh = new ButtonWidget<T>(8.0, 273.0, "Refresh", 8, 1, 1, 3);
            m_refresh.ButtonClick += RefreshImage;
            m_run = new ButtonWidget<T>(8.0, 253.0, "RUN Test!", 8, 1, 1, 3);
            m_run.ButtonClick += RunTest;
            m_single_step = new ButtonWidget<T>(8.0, 233.0, "Single Step", 8, 1, 1, 3);
            m_single_step.ButtonClick += SingleStep;

            m_cur_angle = (0.0);
            m_cur_filter = (1);
            m_num_steps = (0);
            m_num_pix = (0.0);
            m_time1 = (0);
            m_time2 = (0);
            m_ScanlinePacked = new ScanlinePacked8();
            m_Rasterizer = new RasterizerScanlineAA<T>();
            m_ScanlineUnpacked = new ScanlineUnpacked8();
            m_SpanAllocator = new SpanAllocator();

            AddChild(m_radius);
            AddChild(m_step);
            AddChild(m_filters);
            AddChild(m_run);
            AddChild(m_single_step);
            AddChild(m_normalize);
            AddChild(m_refresh);
            //m_single_step.text_size(7.5);
            m_normalize.SetFontSize(7.5);
            m_normalize.status(true);

            m_radius.label("Filter Radius={0:F2}");
            m_step.label("Step={0:F2}");
            m_radius.range(2.0, 8.0);
            m_radius.value(4.0);
            m_step.range(1.0, 10.0);
            m_step.value(5.0);

            m_filters.add_item("simple (NN)");
            m_filters.add_item("bilinear");
            m_filters.add_item("bicubic");
            m_filters.add_item("spline16");
            m_filters.add_item("spline36");
            m_filters.add_item("hanning");
            m_filters.add_item("hamming");
            m_filters.add_item("hermite");
            m_filters.add_item("kaiser");
            m_filters.add_item("quadric");
            m_filters.add_item("catrom");
            m_filters.add_item("gaussian");
            m_filters.add_item("bessel");
            m_filters.add_item("mitchell");
            m_filters.add_item("sinc");
            m_filters.add_item("lanczos");
            m_filters.add_item("blackman");
            m_filters.cur_item(1);

            m_filters.border_width(0, 0);
            m_filters.background_color(new RGBA_Doubles(0.0, 0.0, 0.0, 0.1));
            m_filters.text_size(6.0);
            m_filters.text_thickness(0.85);
        }

        public override void OnDraw()
        {
#if SourceDepth24
            FormatRGB pixf = new FormatRGB(rbuf_window(), new BlenderBGR());
#else
            pixfmt_alpha_blend_rgba32 pixf = new pixfmt_alpha_blend_rgba32(rbuf_window(), new blender_bgra32());
            pixfmt_alpha_blend_rgba32 pixfImage = new pixfmt_alpha_blend_rgba32(rbuf_img(0), new blender_bgra32());
#endif
            FormatClippingProxy clippingProxy = new FormatClippingProxy(pixf);

            clippingProxy.Clear(new RGBA_Doubles(1.0, 1.0, 1.0));
            clippingProxy.CopyFrom(rbuf_img(0), new RectInt(0, 0, (int)Width.ToInt(), (int)Height.ToInt()), 110, 35);

            string buf = string.Format("NSteps={0:F0}", m_num_steps);
            GsvText<T> t = new GsvText<T>();
            t.StartPoint(10.0, 295.0);
            t.SetFontSize(10.0);
            t.Text = buf;

            ConvStroke<T> pt = new ConvStroke<T>(t);
            pt.Width = M.New<T>(1.5);

            m_Rasterizer.AddPath(pt);
            Renderer<T>.RenderSolid(clippingProxy, m_Rasterizer, m_ScanlinePacked, new RGBA_Bytes(0, 0, 0));

            if (m_time1 != m_time2 && m_num_pix > 0.0)
            {
                buf = string.Format("{0:F2} Kpix/sec", m_num_pix / (m_time2 - m_time1));
                t.StartPoint(10.0, 310.0);
                t.Text = buf;
                m_Rasterizer.AddPath(pt);
                Renderer<T>.RenderSolid(clippingProxy, m_Rasterizer, m_ScanlinePacked, new RGBA_Bytes(0, 0, 0));
            }

            if (m_filters.cur_item() >= 14)
            {
                m_radius.Visible = true;
            }
            else
            {
                m_radius.Visible = true;
            }

            base.OnDraw();
        }

        void transform_image(T angle)
        {
            transform_image(angle.ToDouble());
        }

        void transform_image(double angle)
        {
            double width = rbuf_img(0).Width;
            double height = rbuf_img(0).Height;

#if SourceDepth24
            FormatRGB pixf = new FormatRGB(rbuf_img(0), new BlenderBGR());
            FormatRGB pixf_pre = new FormatRGB(rbuf_img(0), new BlenderPreMultBGR());
#else
            pixfmt_alpha_blend_rgba32 pixf = new pixfmt_alpha_blend_rgba32(rbuf_img(0), new blender_bgra32());
            pixfmt_alpha_blend_rgba32 pixf_pre = new pixfmt_alpha_blend_rgba32(rbuf_img(0), new blender_bgra_pre());
#endif
            FormatClippingProxy rb = new FormatClippingProxy(pixf);
            FormatClippingProxy rb_pre = new FormatClippingProxy(pixf_pre);

            rb.Clear(new RGBA_Doubles(1.0, 1.0, 1.0));

            IAffineTransformMatrix<T> src_mtx = MatrixFactory<T>.NewIdentity(VectorDimension.Two);
            src_mtx.Translate(MatrixFactory<T>.CreateVector2D(-width / 2.0, -height / 2.0));
            src_mtx.RotateAlong(MatrixFactory<T>.CreateVector2D(0, 0), angle * Math.PI / 180.0);
            src_mtx.Translate(MatrixFactory<T>.CreateVector2D(width / 2.0, height / 2.0));

            IAffineTransformMatrix<T> img_mtx = MatrixFactory<T>.CreateAffine(src_mtx);
            img_mtx = img_mtx.Inverse;

            double r = width;
            if (height < r) r = height;

            r *= 0.5;
            r -= 4.0;
            VertexSource.Ellipse<T> ell = new AGG.VertexSource.Ellipse<T>(width / 2.0, height / 2.0, r, r, 200);
            ConvTransform<T> tr = new ConvTransform<T>(ell, src_mtx);

            m_num_pix += r * r * Math.PI;

            SpanInterpolatorLinear<T> interpolator = new SpanInterpolatorLinear<T>(img_mtx);

            ImageFilterLookUpTable<T> filter = new ImageFilterLookUpTable<T>();
            bool norm = m_normalize.status();

#if SourceDepth24
            FormatRGB pixf_img = new FormatRGB(rbuf_img(1), new BlenderBGR());
#else
            pixfmt_alpha_blend_rgba32 pixf_img = new pixfmt_alpha_blend_rgba32(rbuf_img(1), new blender_bgra32());
#endif
            RasterBufferAccessorClip source = new RasterBufferAccessorClip(pixf_img, RGBA_Doubles.RgbaPre(0, 0, 0, 0));

            switch (m_filters.cur_item())
            {
                case 0:
                    {
#if SourceDepth24
                        SpanImageFilterRgbNN<T> sg = new SpanImageFilterRgbNN<T>(source, interpolator);
#else
                    span_image_filter_rgba_nn sg = new span_image_filter_rgba_nn(source, interpolator);
#endif
                        m_Rasterizer.AddPath(tr);
                        Renderer<T>.GenerateAndRender(m_Rasterizer, m_ScanlineUnpacked, rb_pre, m_SpanAllocator, sg);
                    }
                    break;

                case 1:
                    {
#if SourceDepth24
                        //span_image_filter_rgb_bilinear_clip sg = new span_image_filter_rgb_bilinear_clip(pixf_img, rgba.rgba_pre(0, 0.4, 0, 0.5), interpolator);
                        SpanImageFilterRgbBilinear<T> sg = new SpanImageFilterRgbBilinear<T>(source, interpolator);
#else
                    //span_image_filter_rgba_bilinear_clip sg = new span_image_filter_rgba_bilinear_clip(pixf_img, rgba.rgba_pre(0, 0, 0, 0), interpolator);
                    span_image_filter_rgba_bilinear sg = new span_image_filter_rgba_bilinear(source, interpolator);
#endif
                        m_Rasterizer.AddPath(tr);
                        Renderer<T>.GenerateAndRender(m_Rasterizer, m_ScanlineUnpacked, rb_pre, m_SpanAllocator, sg);
                    }
                    break;

                case 5:
                case 6:
                case 7:
                    {
                        switch (m_filters.cur_item())
                        {
                            case 5: filter.Calculate(new ImageFilterHanning<T>(), norm); break;
                            case 6: filter.Calculate(new ImageFilterHamming<T>(), norm); break;
                            case 7: filter.Calculate(new ImageFilterHermite<T>(), norm); break;
                        }

                        SpanImageFilterRgb2x2<T> sg = new SpanImageFilterRgb2x2<T>(source, interpolator, filter);
                        m_Rasterizer.AddPath(tr);
                        Renderer<T>.GenerateAndRender(m_Rasterizer, m_ScanlineUnpacked, rb_pre, m_SpanAllocator, sg);
                    }
                    break;

                case 2:
                case 3:
                case 4:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                    {
                        switch (m_filters.cur_item())
                        {
                            case 2: filter.Calculate(new ImageFilterBicubic<T>(), norm); break;
                            case 3: filter.Calculate(new ImageFilterSpline16<T>(), norm); break;
                            case 4: filter.Calculate(new ImageFilterSpline36<T>(), norm); break;
                            case 8: filter.Calculate(new ImageFilterKaiser<T>(), norm); break;
                            case 9: filter.Calculate(new ImageFilterQuadric<T>(), norm); break;
                            case 10: filter.Calculate(new ImageFilterCatrom<T>(), norm); break;
                            case 11: filter.Calculate(new ImageFilterGaussian<T>(), norm); break;
                            case 12: filter.Calculate(new ImageFilterBessel<T>(), norm); break;
                            case 13: filter.Calculate(new ImageFilterMitchell<T>(), norm); break;
                            case 14: filter.Calculate(new ImageFilterSinc<T>(m_radius.value()), norm); break;
                            case 15: filter.Calculate(new ImageFilterLanczos<T>(m_radius.value()), norm); break;
                            case 16:
                                filter.Calculate(new ImageFilterBlackman<T>(m_radius.value()), norm);
                                break;
                        }

#if SourceDepth24
                        SpanImageFilterRgb<T> sg = new SpanImageFilterRgb<T>(source, interpolator, filter);
#else
                    span_image_filter_rgb sg = new span_image_filter_rgba(source, interpolator, filter);
#endif
                        m_Rasterizer.AddPath(tr);
                        Renderer<T>.GenerateAndRender(m_Rasterizer, m_ScanlineUnpacked, rb_pre, m_SpanAllocator, sg);
                    }
                    break;
            }
        }

        void SingleStep(ButtonWidget<T> button)
        {
            m_cur_angle += m_step.value().ToDouble();
            copy_img_to_img(1, 0);
            transform_image(m_step.value());
            m_num_steps++;
            force_redraw();
        }

        void RunTest(ButtonWidget<T> button)
        {
            start_timer();
            m_time1 = m_time2 = elapsed_time();
            m_num_pix = 0.0;
            wait_mode(false);
        }

        void RefreshImage(ButtonWidget<T> button)
        {
            start_timer();
            m_time1 = m_time2 = 0;
            m_num_pix = 0.0;
            m_cur_angle = 0.0;
            copy_img_to_img(1, 2);
            transform_image(0.0);
            m_cur_filter = m_filters.cur_item();
            m_num_steps = 0;
            force_redraw();
        }

        public override void OnIdle()
        {
            if (m_cur_angle < 360.0)
            {
                m_cur_angle += m_step.value().ToDouble();
                copy_img_to_img(1, 0);
                start_timer();
                transform_image(m_step.value());
                m_time2 += elapsed_time();
                m_num_steps++;
            }
            else
            {
                m_cur_angle = 0.0;
                //m_time2 = clock();
                wait_mode(true);
            }
            force_redraw();
        }

        public static void StartDemo()
        {
#if SourceDepth24
            image_filters_application<T> app = new image_filters_application<T>(PixelFormats.pix_format_bgr24, ERenderOrigin.OriginBottomLeft);
#else
            image_filters_application app = new image_filters_application(pix_format_e.pix_format_rgba32, platform_support_abstract.ERenderOrigin.OriginBottomLeft);
#endif
            app.Caption = "Image transformation filters comparison";

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
                app.copy_img_to_img(1, 0);
                app.copy_img_to_img(2, 0);
                app.transform_image(0.0);

                uint w = app.rbuf_img(0).Width + 110;
                uint h = app.rbuf_img(0).Height + 40;

                if (w < 305) w = 305;
                if (h < 325) h = 325;

                if (app.init(w, h, (uint)WindowFlags.Risizeable))
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
            image_filters_application<DoubleComponent>.StartDemo();
        }
    }

}
