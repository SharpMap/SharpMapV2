//#define SourceDepth24

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
    public class image1_application<T> : AGG.UI.win32.PlatformSupport<T>
          where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        AGG.UI.SliderWidget<T> m_angle;
        AGG.UI.SliderWidget<T> m_scale;

        image1_application(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            m_angle = new AGG.UI.SliderWidget<T>(5, 5, 300, 12);
            m_scale = new AGG.UI.SliderWidget<T>(5, 5 + 15, 300, 12 + 15);

            AddChild(m_angle);
            AddChild(m_scale);
            m_angle.label("Angle={0:F2}");
            m_scale.label("Scale={0:F2}");
            m_angle.range(-180.0, 180.0);
            m_angle.value(0.0);
            m_scale.range(0.1, 5.0);
            m_scale.value(1.0);
        }

#if use_timers
        static CNamedTimer AllTimer = new CNamedTimer("All");
        static CNamedTimer image1_100_Times = new CNamedTimer("image1_100_Times");
#endif
        public override void OnDraw()
        {
            //typedef agg::renderer_base<pixfmt>     renderer_base;
            //typedef agg::renderer_base<pixfmt_pre> renderer_base_pre;

#if SourceDepth24
            pixfmt_alpha_blend_rgb pixf = new pixfmt_alpha_blend_rgb(rbuf_window(), new blender_bgr());
            pixfmt_alpha_blend_rgb pixf_pre = new pixfmt_alpha_blend_rgb(rbuf_window(), new blender_bgr_pre());
#else
            FormatRGBA pixf = new FormatRGBA(rbuf_window(), new BlenderBGRA());
            FormatRGBA pixf_pre = new FormatRGBA(rbuf_window(), new BlenderPreMultBGRA());
#endif
            FormatClippingProxy clippingProxy = new FormatClippingProxy(pixf);
            FormatClippingProxy clippingProxy_pre = new FormatClippingProxy(pixf_pre);

            clippingProxy.Clear(new RGBA_Doubles(1.0, 1.0, 1.0));

            IAffineTransformMatrix<T> src_mtx = MatrixFactory<T>.NewIdentity(VectorDimension.Two);
            src_mtx.Translate(MatrixFactory<T>.CreateVector2D(initial_width().Negative().Divide(2).Subtract(10), initial_height().Negative().Divide(2).Subtract(30)));
            src_mtx.RotateAlong(MatrixFactory<T>.CreateVector2D(0, 0), m_angle.value().Multiply(Math.PI / 180.0).ToDouble());
            src_mtx.Scale(m_scale.value());
            src_mtx.Translate(MatrixFactory<T>.CreateVector2D(initial_width().Divide(2), initial_height().Divide(2).Add(20)));
            src_mtx.Multiply(trans_affine_resizing());

            IAffineTransformMatrix<T> img_mtx = MatrixFactory<T>.NewIdentity(VectorDimension.Two);
            img_mtx.Translate(MatrixFactory<T>.CreateVector2D(initial_width().Negative().Divide(2).Add(10), initial_height().Negative().Divide(2).Add(30)));
            img_mtx.RotateAlong(MatrixFactory<T>.CreateVector2D(0, 0), m_angle.value().Multiply(Math.PI / 180.0).ToDouble());
            img_mtx.Scale(m_scale.value());
            img_mtx.Translate(MatrixFactory<T>.CreateVector2D(initial_width().Divide(2), initial_height().Divide(2).Add(20)));
            img_mtx.Multiply(trans_affine_resizing());
            img_mtx = img_mtx.Inverse;

            AGG.SpanAllocator sa = new SpanAllocator();

            SpanInterpolatorLinear<T> interpolator = new SpanInterpolatorLinear<T>(img_mtx);

#if SourceDepth24
            pixfmt_alpha_blend_rgb img_pixf = new pixfmt_alpha_blend_rgb(rbuf_img(0), new blender_bgr());
#else
            FormatRGBA img_pixf = new FormatRGBA(rbuf_img(0), new BlenderBGRA());
#endif

#if SourceDepth24
            span_image_filter_rgb_bilinear_clip sg;
            sg = new span_image_filter_rgb_bilinear_clip(img_pixf, rgba.rgba_pre(0, 0.4, 0, 0.5), interpolator);
#else
            SpanImageFilterRgbaBilinearClip<T> sg;
            RasterBufferAccessorClip source = new RasterBufferAccessorClip(img_pixf, RGBA_Doubles.RgbaPre(0, 0, 0, 0));
            sg = new SpanImageFilterRgbaBilinearClip<T>(source, RGBA_Doubles.RgbaPre(0, 0.4, 0, 0.5), interpolator);
#endif

            RasterizerScanlineAA<T> ras = new RasterizerScanlineAA<T>();
            ras.SetVectorClipBox(M.Zero<T>(), M.Zero<T>(), width(), height());
            //agg.scanline_packed_8 sl = new scanline_packed_8();
            ScanlineUnpacked8 sl = new ScanlineUnpacked8();

            T r = initial_width();
            if (initial_height().Subtract(60).LessThan(r))
            {
                r = initial_height().Subtract(60);
            }

            Ellipse<T> ell = new Ellipse<T>(initial_width().Divide(2.0).Add(10),
                initial_height().Divide(2.0).Add(30),
                r.Divide(2.0).Add(16.0),
                r.Divide(2.0).Add(16.0), 200);

            ConvTransform<T> tr = new ConvTransform<T>(ell, src_mtx);

            ras.AddPath(tr);
#if use_timers
            for (uint j = 0; j < 10; j++)
            {
                Renderer.GenerateAndRender(ras, sl, clippingProxy_pre, sa, sg);
            }
            AllTimer.Start();
            image1_100_Times.Start();
            for(uint i=0; i<500; i++) 
            {
#endif
            //clippingProxy_pre.SetClippingBox(30, 0, (int)width(), (int)height());
            //clippingProxy.SetClippingBox(30, 0, (int)width(), (int)height());
            Renderer<T>.GenerateAndRender(ras, sl, clippingProxy_pre, sa, sg);
#if use_timers
            }
            image1_100_Times.Stop();
#endif

            //m_angle.SetTransform(trans_affine_resizing());
            //m_scale.SetTransform(trans_affine_resizing());
#if use_timers
            AllTimer.Stop();
            CExecutionTimer.Instance.AppendResultsToFile("TimingTest.txt", AllTimer.GetTotalSeconds());
            CExecutionTimer.Instance.Reset();
#endif
            base.OnDraw();
        }

        /*
        E:\agg23\examples\image1.cpp(111) : error C2664: 

          '__thiscall agg::span_image_filter_gray_bilinear<struct agg::gray8,
                                                           struct agg::order_bgra,
                                                           class agg::span_interpolator_linear<class agg::trans_affine,8> >::agg::span_image_filter_gray_bilinear<struct agg::gray8,struct agg::order_bgra,class agg::span_interpolator_linear<class agg::trans_affine,8> >(class agg::span_interpolator_linear<class agg::trans_affine,8> &,const class agg::row_ptr_cache<unsigned char> &,const struct agg::gray8 &,struct agg::order_bgra &)' : 

        cannot convert parameter 1 from 

        'class agg::span_allocator<struct agg::gray8>' to 
        'class agg::span_interpolator_linear<class agg::trans_affine,8> &'
        */
        public static void StartDemo()
        {
#if SourceDepth24
            image1_application app = new image1_application(pix_format_e.pix_format_rgb24, agg.ui.platform_support_abstract.ERenderOrigin.OriginBottomLeft);
#else
            image1_application<T> app = new image1_application<T>(PixelFormats.pix_format_rgba32, ERenderOrigin.OriginBottomLeft);
#endif
            app.Caption = "Image Affine Transformations with filtering";

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
                if (app.init(app.rbuf_img(0).Width + 20, app.rbuf_img(0).Height + 40 + 20, (uint)WindowFlags.Risizeable))
                {

                    // Test the plain/premultiplied issue
                    //-------------------
                    //typedef agg::pixfmt_bgra32         pixfmt; 
                    //typedef agg::renderer_base<pixfmt> renderer_base;
                    //pixfmt        pixf(app.rbuf_img(0));
                    //renderer_base rb(pixf);
                    //for(unsigned i = 0; i < app.rbuf_img(0).height(); i += 2)
                    //{
                    //    // Fully transparent
                    //    rb.copy_hline(0, i, app.rbuf_img(0).width(), agg::rgba(0, 0, 0, 0));  
                    //    if(i + 1 < app.rbuf_img(0).height())
                    //    {
                    //        // Fully opaque white
                    //        rb.copy_hline(0, i + 1, app.rbuf_img(0).width(), agg::rgba(1, 1, 1, 1));  
                    //    }
                    //}

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
            image1_application<DoubleComponent>.StartDemo();
        }
    }
}
