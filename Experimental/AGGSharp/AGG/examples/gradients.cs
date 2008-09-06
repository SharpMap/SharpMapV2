#define SourceDepth24

using System;
using AGG.Color;
using AGG.Gradient;
using AGG.Interpolation;
using AGG.PixelFormat;
using AGG.Rasterizer;
using AGG.Rendering;
using AGG.Scanline;
using AGG.Span;
//using pix_format_e = AGG.UI.PlatformSupportAbstract.PixelFormats;
using AGG.Transform;
using AGG.UI;
using AGG.UI.win32;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;

namespace AGG
{
    public struct ColorFunctionProfile : IColorFunction
    {
        public ColorFunctionProfile(RGBA_Bytes[] colors, byte[] profile)
        {
            m_colors = colors;
            m_profile = profile;
        }

        public int Size { get { return 256; } }
        public RGBA_Bytes this[int v]
        {
            get
            {
                return m_colors[m_profile[v]];
            }
        }

        RGBA_Bytes[] m_colors;
        byte[] m_profile;
    };

    public class GradientsApplication<T> : PlatformSupport<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T center_x = M.New<T>(350);
        T center_y = M.New<T>(280);

        gamma_ctrl<T> m_profile;
        spline_ctrl<T> m_spline_r;
        spline_ctrl<T> m_spline_g;
        spline_ctrl<T> m_spline_b;
        spline_ctrl<T> m_spline_a;
        UI.rbox_ctrl<T> m_GradTypeRBox;
        UI.rbox_ctrl<T> m_GradWrapRBox;

        T m_pdx;
        T m_pdy;

        class SaveData
        {
            internal T m_center_x;
            internal T m_center_y;
            internal T m_scale;
            internal T m_angle;
            internal T[] m_splineRArray = new T[10];
            internal T[] m_splineGArray = new T[10];
            internal T[] m_splineBArray = new T[10];
            internal T[] m_splineAArray = new T[10];
            internal T[] m_profileArray = new T[4];
        };

        SaveData m_SaveData = new SaveData();

        T m_prev_scale;
        T m_prev_angle;
        T m_scale_x;
        T m_prev_scale_x;
        T m_scale_y;
        T m_prev_scale_y;
        bool m_mouse_move;

        /*
        public virtual ~gradients_application()
        {
            FILE* fd = fopen(full_file_name("settings.dat"), "w");
            fprintf(fd, "%f\n", m_center_x);
            fprintf(fd, "%f\n", m_center_y);
            fprintf(fd, "%f\n", m_scale);
            fprintf(fd, "%f\n", m_angle);
            fprintf(fd, "%f\n", m_spline_r.x(0));
            fprintf(fd, "%f\n", m_spline_r.y(0));
            fprintf(fd, "%f\n", m_spline_r.x(1));
            fprintf(fd, "%f\n", m_spline_r.y(1));
            fprintf(fd, "%f\n", m_spline_r.x(2));
            fprintf(fd, "%f\n", m_spline_r.y(2));
            fprintf(fd, "%f\n", m_spline_r.x(3));
            fprintf(fd, "%f\n", m_spline_r.y(3));
            fprintf(fd, "%f\n", m_spline_r.x(4));
            fprintf(fd, "%f\n", m_spline_r.y(4));
            fprintf(fd, "%f\n", m_spline_r.x(5));
            fprintf(fd, "%f\n", m_spline_r.y(5));
            fprintf(fd, "%f\n", m_spline_g.x(0));
            fprintf(fd, "%f\n", m_spline_g.y(0));
            fprintf(fd, "%f\n", m_spline_g.x(1));
            fprintf(fd, "%f\n", m_spline_g.y(1));
            fprintf(fd, "%f\n", m_spline_g.x(2));
            fprintf(fd, "%f\n", m_spline_g.y(2));
            fprintf(fd, "%f\n", m_spline_g.x(3));
            fprintf(fd, "%f\n", m_spline_g.y(3));
            fprintf(fd, "%f\n", m_spline_g.x(4));
            fprintf(fd, "%f\n", m_spline_g.y(4));
            fprintf(fd, "%f\n", m_spline_g.x(5));
            fprintf(fd, "%f\n", m_spline_g.y(5));
            fprintf(fd, "%f\n", m_spline_b.x(0));
            fprintf(fd, "%f\n", m_spline_b.y(0));
            fprintf(fd, "%f\n", m_spline_b.x(1));
            fprintf(fd, "%f\n", m_spline_b.y(1));
            fprintf(fd, "%f\n", m_spline_b.x(2));
            fprintf(fd, "%f\n", m_spline_b.y(2));
            fprintf(fd, "%f\n", m_spline_b.x(3));
            fprintf(fd, "%f\n", m_spline_b.y(3));
            fprintf(fd, "%f\n", m_spline_b.x(4));
            fprintf(fd, "%f\n", m_spline_b.y(4));
            fprintf(fd, "%f\n", m_spline_b.x(5));
            fprintf(fd, "%f\n", m_spline_b.y(5));
            fprintf(fd, "%f\n", m_spline_a.x(0));
            fprintf(fd, "%f\n", m_spline_a.y(0));
            fprintf(fd, "%f\n", m_spline_a.x(1));
            fprintf(fd, "%f\n", m_spline_a.y(1));
            fprintf(fd, "%f\n", m_spline_a.x(2));
            fprintf(fd, "%f\n", m_spline_a.y(2));
            fprintf(fd, "%f\n", m_spline_a.x(3));
            fprintf(fd, "%f\n", m_spline_a.y(3));
            fprintf(fd, "%f\n", m_spline_a.x(4));
            fprintf(fd, "%f\n", m_spline_a.y(4));
            fprintf(fd, "%f\n", m_spline_a.x(5));
            fprintf(fd, "%f\n", m_spline_a.y(5));
            double x1,y1,x2,y2;
            m_profile.values(&x1, &y1, &x2, &y2);
            fprintf(fd, "%f\n", x1);
            fprintf(fd, "%f\n", y1);
            fprintf(fd, "%f\n", x2);
            fprintf(fd, "%f\n", y2);
            fclose(fd);
        }
         */

        GradientsApplication(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            m_profile = new gamma_ctrl<T>(10.0, 10.0, 200.0, 170.0 - 5.0);
            m_spline_r = new spline_ctrl<T>(210, 10, 210 + 250, 5 + 40, 6);
            m_spline_g = new spline_ctrl<T>(210, 10 + 40, 210 + 250, 5 + 80, 6);
            m_spline_b = new spline_ctrl<T>(210, 10 + 80, 210 + 250, 5 + 120, 6);
            m_spline_a = new spline_ctrl<T>(210, 10 + 120, 210 + 250, 5 + 160, 6);
            m_GradTypeRBox = new AGG.UI.rbox_ctrl<T>(10.0, 180.0, 200.0, 300.0);
            m_GradWrapRBox = new rbox_ctrl<T>(10, 310, 200, 375);

            m_pdx = (M.Zero<T>());
            m_pdy = M.Zero<T>();
            m_SaveData.m_center_x = (center_x);
            m_SaveData.m_center_y = (center_y);
            m_SaveData.m_scale = M.One<T>();
            m_prev_scale = M.One<T>();
            m_SaveData.m_angle = M.Zero<T>();
            m_prev_angle = M.Zero<T>();
            m_scale_x = M.One<T>();
            m_prev_scale_x = M.One<T>();
            m_scale_y = M.One<T>();
            m_prev_scale_y = M.One<T>();
            m_mouse_move = (false);


            AddChild(m_profile);
            AddChild(m_spline_r);
            AddChild(m_spline_g);
            AddChild(m_spline_b);
            AddChild(m_spline_a);
            AddChild(m_GradTypeRBox);
            AddChild(m_GradWrapRBox);

            m_profile.border_width(2.0, 2.0);

            m_spline_r.background_color(new RGBA_Bytes(1.0, 0.8, 0.8));
            m_spline_g.background_color(new RGBA_Bytes(0.8, 1.0, 0.8));
            m_spline_b.background_color(new RGBA_Bytes(0.8, 0.8, 1.0));
            m_spline_a.background_color(new RGBA_Bytes(1.0, 1.0, 1.0));

            m_spline_r.border_width(1.0, 2.0);
            m_spline_g.border_width(1.0, 2.0);
            m_spline_b.border_width(1.0, 2.0);
            m_spline_a.border_width(1.0, 2.0);
            m_GradTypeRBox.border_width(2.0, 2.0);
            m_GradWrapRBox.border_width(2.0, 2.0);

            m_spline_r.point(0, 0.0, 1.0);
            m_spline_r.point(1, 1.0 / 5.0, 1.0 - 1.0 / 5.0);
            m_spline_r.point(2, 2.0 / 5.0, 1.0 - 2.0 / 5.0);
            m_spline_r.point(3, 3.0 / 5.0, 1.0 - 3.0 / 5.0);
            m_spline_r.point(4, 4.0 / 5.0, 1.0 - 4.0 / 5.0);
            m_spline_r.point(5, 1.0, 0.0);
            m_spline_r.update_spline();

            m_spline_g.point(0, 0.0, 1.0);
            m_spline_g.point(1, 1.0 / 5.0, 1.0 - 1.0 / 5.0);
            m_spline_g.point(2, 2.0 / 5.0, 1.0 - 2.0 / 5.0);
            m_spline_g.point(3, 3.0 / 5.0, 1.0 - 3.0 / 5.0);
            m_spline_g.point(4, 4.0 / 5.0, 1.0 - 4.0 / 5.0);
            m_spline_g.point(5, 1.0, 0.0);
            m_spline_g.update_spline();

            m_spline_b.point(0, 0.0, 1.0);
            m_spline_b.point(1, 1.0 / 5.0, 1.0 - 1.0 / 5.0);
            m_spline_b.point(2, 2.0 / 5.0, 1.0 - 2.0 / 5.0);
            m_spline_b.point(3, 3.0 / 5.0, 1.0 - 3.0 / 5.0);
            m_spline_b.point(4, 4.0 / 5.0, 1.0 - 4.0 / 5.0);
            m_spline_b.point(5, 1.0, 0.0);
            m_spline_b.update_spline();

            m_spline_a.point(0, 0.0, 1.0);
            m_spline_a.point(1, 1.0 / 5.0, 1.0);
            m_spline_a.point(2, 2.0 / 5.0, 1.0);
            m_spline_a.point(3, 3.0 / 5.0, 1.0);
            m_spline_a.point(4, 4.0 / 5.0, 1.0);
            m_spline_a.point(5, 1.0, 1.0);
            m_spline_a.update_spline();

            m_GradTypeRBox.add_item("Circular");
            m_GradTypeRBox.add_item("Diamond");
            m_GradTypeRBox.add_item("Linear");
            m_GradTypeRBox.add_item("XY");
            m_GradTypeRBox.add_item("sqrt(XY)");
            m_GradTypeRBox.add_item("Conic");
            m_GradTypeRBox.cur_item(0);

            m_GradWrapRBox.add_item("Reflect");
            m_GradWrapRBox.add_item("Repeat");
            m_GradWrapRBox.add_item("Clamp");
            m_GradWrapRBox.cur_item(0);

            /*
            FILE* fd = fopen(full_file_name("settings.dat"), "r");
            if(fd)
            {
                float x;
                float y;
                float x2;
                float y2;
                float t;

                fscanf(fd, "%f\n", &t); m_center_x = t;
                fscanf(fd, "%f\n", &t); m_center_y = t;
                fscanf(fd, "%f\n", &t); m_scale = t;
                fscanf(fd, "%f\n", &t); m_angle = t;
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_r.point(0, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_r.point(1, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_r.point(2, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_r.point(3, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_r.point(4, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_r.point(5, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_g.point(0, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_g.point(1, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_g.point(2, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_g.point(3, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_g.point(4, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_g.point(5, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_b.point(0, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_b.point(1, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_b.point(2, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_b.point(3, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_b.point(4, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_b.point(5, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_a.point(0, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_a.point(1, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_a.point(2, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_a.point(3, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_a.point(4, x, y);
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y); m_spline_a.point(5, x, y);
                m_spline_r.update_spline();
                m_spline_g.update_spline();
                m_spline_b.update_spline();
                m_spline_a.update_spline();
                fscanf(fd, "%f\n", &x);
                fscanf(fd, "%f\n", &y);
                fscanf(fd, "%f\n", &x2);
                fscanf(fd, "%f\n", &y2);
                m_profile.values(x, y, x2, y2);
                fclose(fd);
            }
            */
        }


        public override void OnDraw()
        {
            RasterizerScanlineAA<T> ras = new RasterizerScanlineAA<T>();
            ScanlineUnpacked8 sl = new ScanlineUnpacked8();

#if SourceDepth24
            FormatRGB pixf = new FormatRGB(rbuf_window(), new BlenderBGR());
#else
            FormatRGBA pixf = new FormatRGBA(rbuf_window(), new blender_bgra32());
#endif
            FormatClippingProxy clippingProxy = new FormatClippingProxy(pixf);
            clippingProxy.Clear(new RGBA_Doubles(0, 0, 0));

            m_profile.text_size(8.0);

            //m_profile.Render(ras, sl, clippingProxy);
            //m_spline_r.Render(ras, sl, clippingProxy);
            //m_spline_g.Render(ras, sl, clippingProxy);
            //m_spline_b.Render(ras, sl, clippingProxy);
            //m_spline_a.Render(ras, sl, clippingProxy);
            //m_GradTypeRBox.Render(ras, sl, clippingProxy);
            //m_GradWrapRBox.Render(ras, sl, clippingProxy);

            // draw a background to show how the alpha is working
            int RectWidth = 32;
            int xoffset = 238;
            int yoffset = 171;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if ((i + j) % 2 != 0)
                    {
                        VertexSource.RoundedRect<T> rect = new VertexSource.RoundedRect<T>(i * RectWidth + xoffset, j * RectWidth + yoffset,
                            (i + 1) * RectWidth + xoffset, (j + 1) * RectWidth + yoffset, 2);
                        rect.NormalizeRadius();

                        // Drawing as an outline
                        ras.AddPath(rect);
                        Renderer<T>.RenderSolid(clippingProxy, ras, sl, new RGBA_Bytes(.9, .9, .9));
                    }
                }
            }

            double ini_scale = 1.0;

            IAffineTransformMatrix<T> mtx1 = MatrixFactory<T>.NewIdentity(VectorDimension.Two);
            mtx1.Scale(MatrixFactory<T>.CreateVector2D(ini_scale, ini_scale));
            mtx1.Translate(MatrixFactory<T>.CreateVector2D(center_x, center_y));
            mtx1.Add(trans_affine_resizing());

            VertexSource.Ellipse<T> e1 = new AGG.VertexSource.Ellipse<T>();
            e1.Init(0.0, 0.0, 110.0, 110.0, 64);

            IAffineTransformMatrix<T> mtx_g1 = MatrixFactory<T>.NewIdentity(VectorDimension.Two);
            mtx_g1.Scale(MatrixFactory<T>.CreateVector2D(ini_scale, ini_scale));
            mtx_g1.Scale(MatrixFactory<T>.CreateVector2D(m_SaveData.m_scale, m_SaveData.m_scale));
            mtx_g1.Scale(MatrixFactory<T>.CreateVector2D(m_scale_x, m_scale_y));
            mtx_g1.RotateAlong(MatrixFactory<T>.CreateVector2D(0, 0), m_SaveData.m_angle.ToDouble());
            mtx_g1.Translate(MatrixFactory<T>.CreateVector2D(m_SaveData.m_center_x, m_SaveData.m_center_y));
            mtx_g1.Add(trans_affine_resizing());
            mtx_g1 = mtx_g1.Inverse;


            RGBA_Bytes[] color_profile = new RGBA_Bytes[256]; // color_type is defined in pixel_formats.h
            for (int i = 0; i < 256; i++)
            {
                color_profile[i] = new RGBA_Bytes(m_spline_r.spline()[i].ToInt(),
                                                        m_spline_g.spline()[i].ToInt(),
                                                        m_spline_b.spline()[i].ToInt(),
                                                        m_spline_a.spline()[i].ToInt());
            }

            ConvTransform<T> t1 = new ConvTransform<T>(e1, mtx1);

            IGradient innerGradient = null;
            switch (m_GradTypeRBox.cur_item())
            {
                case 0:
                    innerGradient = new GradientRadial();
                    break;

                case 1:
                    innerGradient = new GradientDiamond();
                    break;

                case 2:
                    innerGradient = new GradientX();
                    break;

                case 3:
                    innerGradient = new GradientXY();
                    break;

                case 4:
                    innerGradient = new GradientSqrtXY();
                    break;

                case 5:
                    innerGradient = new GradientConic();
                    break;
            }

            IGradient outerGradient = null;
            switch (m_GradWrapRBox.cur_item())
            {
                case 0:
                    outerGradient = new GradientReflectAdaptor(innerGradient);
                    break;

                case 1:
                    outerGradient = new GradientRepeatAdaptor(innerGradient);
                    break;

                case 2:
                    outerGradient = new GradientClampAdaptor(innerGradient);
                    break;
            }

            SpanAllocator span_alloc = new SpanAllocator();
            ColorFunctionProfile colors = new ColorFunctionProfile(color_profile, m_profile.gamma());
            SpanInterpolatorLinear<T> inter = new SpanInterpolatorLinear<T>(mtx_g1);
            SpanGradient<T> span_gen = new SpanGradient<T>(inter, outerGradient, colors, 0, 150);

            ras.AddPath(t1);
            Renderer<T>.GenerateAndRender(ras, sl, clippingProxy, span_alloc, span_gen);
            base.OnDraw();
        }

        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
            if (m_mouse_move)
            {
                T x2 = M.New<T>(mouseEvent.X);
                T y2 = M.New<T>(mouseEvent.Y);
                trans_affine_resizing().Inverse.Transform(ref x2, ref y2);

                /*
                if(flags & agg::kbd_ctrl)
                {
                    double dx = x2 - m_center_x;
                    double dy = y2 - m_center_y;
                    m_scale_x = m_prev_scale_x * dx / m_pdx;
                    m_scale_y = m_prev_scale_y * dy / m_pdy;
                    force_redraw();
                }
                else
                 */
                {
                    if (mouseEvent.Button == MouseButtons.Left)
                    {
                        m_SaveData.m_center_x = x2.Add(m_pdx);
                        m_SaveData.m_center_y = y2.Add(m_pdy);
                        force_redraw();
                    }

                    if (mouseEvent.Button == MouseButtons.Right)
                    {
                        T dx = x2.Subtract(m_SaveData.m_center_x);
                        T dy = y2.Subtract(m_SaveData.m_center_y);
                        m_SaveData.m_scale = m_prev_scale.Multiply(
                            M.Length(dx, dy)
                            ).Divide(M.Length(m_pdx, m_pdy))
                                 ;

                        m_SaveData.m_angle = m_prev_angle.Add(M.Atan2(dy, dx)).Subtract(M.Atan2(m_pdy, m_pdx));
                        force_redraw();
                    }
                }
            }

            base.OnMouseMove(mouseEvent);
        }

        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            base.OnMouseDown(mouseEvent);

            if (!mouseEvent.Handled)
            {
                m_mouse_move = true;
                T x2 = M.New<T>(mouseEvent.X);
                T y2 = M.New<T>(mouseEvent.Y);
                trans_affine_resizing().Inverse.Transform(ref x2, ref y2);

                m_pdx = m_SaveData.m_center_x.Subtract(x2);
                m_pdy = m_SaveData.m_center_y.Subtract(y2);
                m_prev_scale = m_SaveData.m_scale;
                m_prev_angle = m_SaveData.m_angle.Add(System.Math.PI);
                m_prev_scale_x = m_scale_x;
                m_prev_scale_y = m_scale_y;
            }
            force_redraw();
        }


        public override void OnMouseUp(MouseEventArgs mouseEvent)
        {
            m_mouse_move = false;
            base.OnMouseUp(mouseEvent);
        }

        public override void OnKeyDown(KeyEventArgs keyEvent)
        {
            if (keyEvent.KeyCode == Keys.F1)
            {
                /*
                FILE* fd = fopen(full_file_name("colors.dat"), "w");
                int i;
                for(i = 0; i < 256; i++)
                {
                    color_type c = agg::rgba(m_spline_r.spline()[i], 
                                             m_spline_g.spline()[i],
                                             m_spline_b.spline()[i],
                                             m_spline_a.spline()[i]);
                    fprintf(fd, "    %3d, %3d, %3d, %3d,\n", c.r, c.g, c.b, c.a);
                }
                fclose(fd);

                fd = fopen(full_file_name("profile.dat"), "w");
                for(i = 0; i < 256; i++)
                {
                    fprintf(fd, "%3d, ", uint(m_profile.gamma()[i]));
                    if((i & 0xF) == 0xF) fprintf(fd, "\n");
                }
                fclose(fd);
                 */
            }
            base.OnKeyDown(keyEvent);
        }

        public static void StartDemo()
        {
#if SourceDepth24
            GradientsApplication<T> app = new GradientsApplication<T>(PixelFormats.pix_format_rgb24, ERenderOrigin.OriginBottomLeft);
#else
            gradients_application app = new gradients_application(pix_format_e.pix_format_rgba32, agg.ui.platform_support_abstract.ERenderOrigin.OriginBottomLeft);
#endif
            app.Caption = "AGG gradients with Mach bands compensation";

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
            GradientsApplication<DoubleComponent>.StartDemo();
        }
    }
}
