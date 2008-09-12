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
    public class blur_application<T> : AGG.UI.win32.PlatformSupport<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        rbox_ctrl<T> m_method;
        SliderWidget<T> m_radius;
        polygon_ctrl<T> m_shadow_ctrl;
        cbox_ctrl<T> m_channel_r;
        cbox_ctrl<T> m_channel_g;
        cbox_ctrl<T> m_channel_b;
        cbox_ctrl<T> m_FlattenCurves;

        PathStorage<T> m_path;
        ConvCurve<T> m_shape;

        RasterizerScanlineAA<T> m_ras = new RasterizerScanlineAA<T>();
        ScanlinePacked8 m_sl;
        RasterBuffer m_rbuf2;

        //agg::stack_blur    <agg::rgba8, agg::stack_blur_calc_rgb<> >     m_stack_blur;
        RecursiveBlur m_recursive_blur = new RecursiveBlur(new RecursiveBlurCalcRgb());

        RectDouble<T> m_shape_bounds;


        public blur_application(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            m_rbuf2 = new RasterBuffer();
            m_shape_bounds = new RectDouble<T>();
            m_method = new rbox_ctrl<T>(10.0, 10.0, 130.0, 70.0);
            m_radius = new SliderWidget<T>(130 + 10.0, 10.0 + 4.0, 130 + 300.0, 10.0 + 8.0 + 4.0);
            m_shadow_ctrl = new polygon_ctrl<T>(4);
            m_channel_r = new cbox_ctrl<T>(10.0, 80.0, "Red");
            m_channel_g = new cbox_ctrl<T>(10.0, 95.0, "Green");
            m_channel_b = new cbox_ctrl<T>(10.0, 110.0, "Blue");
            m_FlattenCurves = new cbox_ctrl<T>(10, 315, "Convert And Flatten Curves");
            m_FlattenCurves.status(true);

            AddChild(m_method);
            m_method.text_size(8);
            m_method.add_item("Stack Blur");
            m_method.add_item("Recursive Blur");
            m_method.add_item("Channels");
            m_method.cur_item(1);

            AddChild(m_radius);
            m_radius.range(0.0, 40.0);
            m_radius.value(15.0);
            m_radius.label("Blur Radius={0:F2}");

            AddChild(m_shadow_ctrl);

            AddChild(m_channel_r);
            AddChild(m_channel_g);
            AddChild(m_channel_b);
            AddChild(m_FlattenCurves);
            m_channel_g.status(true);

            m_sl = new ScanlinePacked8();
            m_path = new PathStorage<T>();
            m_shape = new ConvCurve<T>(m_path);

            m_path.RemoveAll();
            m_path.MoveTo(28.47, 6.45);
            m_path.Curve3(21.58, 1.12, 19.82, 0.29);
            m_path.Curve3(17.19, -0.93, 14.21, -0.93);
            m_path.Curve3(9.57, -0.93, 6.57, 2.25);
            m_path.Curve3(3.56, 5.42, 3.56, 10.60);
            m_path.Curve3(3.56, 13.87, 5.03, 16.26);
            m_path.Curve3(7.03, 19.58, 11.99, 22.51);
            m_path.Curve3(16.94, 25.44, 28.47, 29.64);
            m_path.LineTo(28.47, 31.40);
            m_path.Curve3(28.47, 38.09, 26.34, 40.58);
            m_path.Curve3(24.22, 43.07, 20.17, 43.07);
            m_path.Curve3(17.09, 43.07, 15.28, 41.41);
            m_path.Curve3(13.43, 39.75, 13.43, 37.60);
            m_path.LineTo(13.53, 34.77);
            m_path.Curve3(13.53, 32.52, 12.38, 31.30);
            m_path.Curve3(11.23, 30.08, 9.38, 30.08);
            m_path.Curve3(7.57, 30.08, 6.42, 31.35);
            m_path.Curve3(5.27, 32.62, 5.27, 34.81);
            m_path.Curve3(5.27, 39.01, 9.57, 42.53);
            m_path.Curve3(13.87, 46.04, 21.63, 46.04);
            m_path.Curve3(27.59, 46.04, 31.40, 44.04);
            m_path.Curve3(34.28, 42.53, 35.64, 39.31);
            m_path.Curve3(36.52, 37.21, 36.52, 30.71);
            m_path.LineTo(36.52, 15.53);
            m_path.Curve3(36.52, 9.13, 36.77, 7.69);
            m_path.Curve3(37.01, 6.25, 37.57, 5.76);
            m_path.Curve3(38.13, 5.27, 38.87, 5.27);
            m_path.Curve3(39.65, 5.27, 40.23, 5.62);
            m_path.Curve3(41.26, 6.25, 44.19, 9.18);
            m_path.LineTo(44.19, 6.45);
            m_path.Curve3(38.72, -0.88, 33.74, -0.88);
            m_path.Curve3(31.35, -0.88, 29.93, 0.78);
            m_path.Curve3(28.52, 2.44, 28.47, 6.45);
            m_path.ClosePolygon();

            m_path.MoveTo(28.47, 9.62);
            m_path.LineTo(28.47, 26.66);
            m_path.Curve3(21.09, 23.73, 18.95, 22.51);
            m_path.Curve3(15.09, 20.36, 13.43, 18.02);
            m_path.Curve3(11.77, 15.67, 11.77, 12.89);
            m_path.Curve3(11.77, 9.38, 13.87, 7.06);
            m_path.Curve3(15.97, 4.74, 18.70, 4.74);
            m_path.Curve3(22.41, 4.74, 28.47, 9.62);
            m_path.ClosePolygon();

            IAffineTransformMatrix<T> shape_mtx = MatrixFactory<T>.NewIdentity(VectorDimension.Two);
            shape_mtx.Scale(M.New<T>(4.0));
            shape_mtx.Translate(MatrixFactory<T>.CreateVector2D(150, 100));
            m_path.Transform(shape_mtx);

            BoundingRect<T>.BoundingRectSingle(m_shape, 0, ref m_shape_bounds);

            m_shadow_ctrl.SetXN(0, m_shape_bounds.x1);
            m_shadow_ctrl.SetYN(0, m_shape_bounds.y1);
            m_shadow_ctrl.SetXN(1, m_shape_bounds.x2);
            m_shadow_ctrl.SetYN(1, m_shape_bounds.y1);
            m_shadow_ctrl.SetXN(2, m_shape_bounds.x2);
            m_shadow_ctrl.SetYN(2, m_shape_bounds.y2);
            m_shadow_ctrl.SetXN(3, m_shape_bounds.x1);
            m_shadow_ctrl.SetYN(3, m_shape_bounds.y2);
            m_shadow_ctrl.line_color(new RGBA_Doubles(0, 0.3, 0.5, 0.3));
        }



        public override void OnDraw()
        {
            //typedef agg::renderer_base<agg::pixfmt_bgr24> ren_base;

            FormatRGB pixf = new FormatRGB(rbuf_window(), new BlenderBGR());
            FormatClippingProxy clippingProxy = new FormatClippingProxy(pixf);
            clippingProxy.Clear(new RGBA_Doubles(1, 1, 1));
            m_ras.SetVectorClipBox(0, 0, width().ToDouble(), height().ToDouble());

            IAffineTransformMatrix<T> move = MatrixFactory<T>.NewTranslation(10, 10);

            Perspective<T> shadow_persp = new Perspective<T>(m_shape_bounds.x1, m_shape_bounds.y1,
                                                m_shape_bounds.x2, m_shape_bounds.y2,
                                                m_shadow_ctrl.polygon());

            IVertexSource<T> shadow_trans;
            if (m_FlattenCurves.status())
            {
                shadow_trans = new ConvTransform<T>(m_shape, shadow_persp);
            }
            else
            {
                shadow_trans = new ConvTransform<T>(m_path, shadow_persp);
                // this will make it very smooth after the Transform
                //shadow_trans = new conv_curve(shadow_trans);
            }


            // Render shadow
            m_ras.AddPath(shadow_trans);
            Renderer<T>.RenderSolid(clippingProxy, m_ras, m_sl, new RGBA_Doubles(0.2, 0.3, 0).GetAsRGBA_Bytes());

            // Calculate the bounding box and extend it by the blur radius
            RectDouble<T> bbox = new RectDouble<T>();
            BoundingRect<T>.BoundingRectSingle(shadow_trans, 0, ref bbox);

            bbox.x1.SubtractEquals(m_radius.value());
            bbox.y1.SubtractEquals(m_radius.value());
            bbox.x2.AddEquals(m_radius.value());
            bbox.y2.AddEquals(m_radius.value());

            if (m_method.cur_item() == 1)
            {
                // The recursive blur method represents the true Gaussian Blur,
                // with theoretically infinite kernel. The restricted window size
                // results in extra influence of edge pixels. It's impossible to
                // solve correctly, but extending the right and top areas to another
                // radius Value produces fair result.
                //------------------
                bbox.x2.AddEquals(m_radius.value());
                bbox.y2.AddEquals(m_radius.value());
            }

            start_timer();

            if (m_method.cur_item() != 2)
            {
                // Create a new pixel renderer and Attach it to the main one as a child image. 
                // It returns true if the attachment succeeded. It fails if the rectangle 
                // (bbox) is fully clipped.
                //------------------
                FormatRGB pixf2 = new FormatRGB(m_rbuf2, new BlenderBGR());
                if (pixf2.Attach(pixf, (int)bbox.x1.ToInt(), (int)bbox.y1.ToInt(), (int)bbox.x2.ToInt(), (int)bbox.y2.ToInt()))
                {
                    // Blur it
                    if (m_method.cur_item() == 0)
                    {
                        // More general method, but 30-40% slower.
                        //------------------
                        //m_stack_blur.blur(pixf2, agg::uround(m_radius.Value()));

                        // Faster, but bore specific. 
                        // Works only for 8 bits per channel and only with radii <= 254.
                        //------------------
                        //                         agg::stack_blur_rgb24(pixf2, agg::uround(m_radius.Value()), 
                        //                                                      agg::uround(m_radius.Value()));
                    }
                    else
                    {
                        // True Gaussian Blur, 3-5 times slower than Stack Blur,
                        // but still constant time of radius. Very sensitive
                        // to precision, doubles are must here.
                        //------------------
                        m_recursive_blur.Blur(pixf2, m_radius.value().ToDouble());
                    }
                }
            }
            else
            {
                /*
                // Blur separate channels
                //------------------
                if(m_channel_r.status())
                {
                    typedef agg::pixfmt_alpha_blend_gray<
                        agg::blender_gray8, 
                        agg::rendering_buffer,
                        3, 2> pixfmt_gray8r;

                    pixfmt_gray8r pixf2r(m_rbuf2);
                    if(pixf2r.Attach(pixf, int(bbox.x1), int(bbox.y1), int(bbox.x2), int(bbox.y2)))
                    {
                        agg::stack_blur_gray8(pixf2r, agg::uround(m_radius.Value()), 
                                                      agg::uround(m_radius.Value()));
                    }
                }

                if(m_channel_g.status())
                {
                    typedef agg::pixfmt_alpha_blend_gray<
                        agg::blender_gray8, 
                        agg::rendering_buffer,
                        3, 1> pixfmt_gray8g;

                    pixfmt_gray8g pixf2g(m_rbuf2);
                    if(pixf2g.Attach(pixf, int(bbox.x1), int(bbox.y1), int(bbox.x2), int(bbox.y2)))
                    {
                        agg::stack_blur_gray8(pixf2g, agg::uround(m_radius.Value()), 
                                                      agg::uround(m_radius.Value()));
                    }
                }

                if(m_channel_b.status())
                {
                    typedef agg::pixfmt_alpha_blend_gray<
                        agg::blender_gray8, 
                        agg::rendering_buffer,
                        3, 0> pixfmt_gray8b;

                    pixfmt_gray8b pixf2b(m_rbuf2);
                    if(pixf2b.Attach(pixf, int(bbox.x1), int(bbox.y1), int(bbox.x2), int(bbox.y2)))
                    {
                        agg::stack_blur_gray8(pixf2b, agg::uround(m_radius.Value()), 
                                                      agg::uround(m_radius.Value()));
                    }
                }
                 */
            }

            double tm = elapsed_time();

            //m_shadow_ctrl.Render(m_ras, m_sl, clippingProxy);

            // Render the shape itself
            //------------------
            if (m_FlattenCurves.status())
            {
                m_ras.AddPath(m_shape);
            }
            else
            {
                m_ras.AddPath(m_path);
            }

            Renderer<T>.RenderSolid(clippingProxy, m_ras, m_sl, new RGBA_Doubles(0.6, 0.9, 0.7, 0.8).GetAsRGBA_Bytes());

            GsvText<T> t = new GsvText<T>();
            t.SetFontSize(10.0);

            ConvStroke<T> st = new ConvStroke<T>(t);
            st.Width = M.New<T>(1.5);

            string buf;
            buf = string.Format("{0:F2} ms", tm);
            t.StartPoint(140.0, 30.0);
            t.Text = buf;

            m_ras.AddPath(st);
            Renderer<T>.RenderSolid(clippingProxy, m_ras, m_sl, new RGBA_Doubles(0, 0, 0).GetAsRGBA_Bytes());


            //m_method.Render(m_ras, m_sl, clippingProxy);
            //m_radius.Render(m_ras, m_sl, clippingProxy);
            //m_channel_r.Render(m_ras, m_sl, clippingProxy);
            //m_channel_g.Render(m_ras, m_sl, clippingProxy);
            //m_channel_b.Render(m_ras, m_sl, clippingProxy);
            //m_FlattenCurves.Render(m_ras, m_sl, clippingProxy);
            base.OnDraw();
        }

        public static void StartDemo()
        {
            blur_application<T> app = new blur_application<T>(PixelFormats.pix_format_bgr24, ERenderOrigin.OriginBottomLeft);
            app.Caption = "AGG Example. Gaussian and Stack Blur";

            if (app.init(440, 330, 0))
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
            blur_application<DoubleComponent>.StartDemo();
        }
    }
}