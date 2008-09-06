using System;
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
    public class lion_outline_application<T> : AGG.UI.win32.PlatformSupport<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private AGG.UI.SliderWidget<T> m_width_slider;
        private AGG.UI.cbox_ctrl<T> m_scanline;

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

        public void parse_lion()
        {
            g_npaths = AGG.samples.LionParser.parse_lion(g_path, g_colors, g_path_idx);
            AGG.BoundingRect<T>.GetBoundingRect(g_path, g_path_idx, 0, g_npaths, out g_x1, out g_y1, out g_x2, out g_y2);
            g_base_dx = g_x2.Subtract(g_x1).Subtract(2.0);
            g_base_dy = g_y2.Subtract(g_y1).Subtract(2.0);
        }

        public lion_outline_application(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            m_width_slider = new AGG.UI.SliderWidget<T>(5, 5, 150, 12);
            m_scanline = new AGG.UI.cbox_ctrl<T>(160, 5, "Use Scanline Rasterizer");
            m_scanline.status(true);
            parse_lion();
            AddChild(m_width_slider);
            m_width_slider.SetTransform(MatrixFactory<T>.NewIdentity(VectorDimension.Two));
            m_width_slider.range(0.0, 4.0);
            m_width_slider.value(1.0);
            m_width_slider.label("Width {0:F2}");

            AddChild(m_scanline);
            m_scanline.SetTransform(MatrixFactory<T>.NewIdentity(VectorDimension.Two));
        }

        public override void OnDraw()
        {
            int width = (int)rbuf_window().Width;
            int height = (int)rbuf_window().Height;


            IPixelFormat pixf = new FormatRGB(rbuf_window(), new BlenderBGR());
            FormatClippingProxy clippingProxy = new FormatClippingProxy(pixf);
            clippingProxy.Clear(new RGBA_Doubles(1, 1, 1));

            IAffineTransformMatrix<T> mtx = MatrixFactory<T>.NewIdentity(VectorDimension.Two);
            mtx.Translate(MatrixFactory<T>.CreateVector2D(g_base_dx.Negative(), g_base_dy.Negative()));
            mtx.Scale(g_scale);
            mtx.RotateAlong(MatrixFactory<T>.CreateVector2D(0, 0), g_angle.Add(Math.PI).ToDouble());
            mtx.Shear(MatrixFactory<T>.CreateVector2D(g_skew_x.Divide(1000.0), g_skew_y.Divide(1000.0)));
            mtx.Translate(MatrixFactory<T>.CreateVector2D(width / 2, height / 2));

            if (m_scanline.status())
            {
                g_rasterizer.SetVectorClipBox(0, 0, width, height);

                ConvStroke<T> stroke = new ConvStroke<T>(g_path);
                stroke.Width = m_width_slider.value();
                stroke.LineJoin = LineJoin.RoundJoin;
                ConvTransform<T> trans = new ConvTransform<T>(stroke, mtx);
                Renderer<T>.RenderSolidAllPaths(clippingProxy, g_rasterizer, g_scanline, trans, g_colors, g_path_idx, g_npaths);
            }
            else
            {
                /*
                double w = m_width_slider.Value() * mtx.scale();

                line_profile_aa profile = new line_profile_aa(w, new gamma_none());
                renderer_outline_aa ren = new renderer_outline_aa(rb, profile);
                rasterizer_outline_aa ras = new rasterizer_outline_aa(ren);

                conv_transform trans = new conv_transform(g_path, mtx);

                ras.render_all_paths(trans, g_colors, g_path_idx, g_npaths);
                 */
            }

            base.OnDraw();
        }

        void transform(double width, double height, double x, double y)
        {
            transform(M.New<T>(width), M.New<T>(height), M.New<T>(x), M.New<T>(y));
        }

        void transform(T width, T height, T x, T y)
        {
            x.SubtractEquals(width.Divide(2));
            y.SubtractEquals(height.Divide(2));
            g_angle = M.Atan2(y, x);
            g_scale = M.Length(y, x).Divide(100);
        }

        protected bool MoveTheLion(MouseEventArgs mouseEvent)
        {
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            if (mouseEvent.Button == MouseButtons.Left)
            {
                T width = M.New<T>(rbuf_window().Width);
                T height = M.New<T>(rbuf_window().Height);
                transform(width, height, x, y);
                force_redraw();
                return true;
            }

            if (mouseEvent.Button == MouseButtons.Right)
            {
                g_skew_x = x;
                g_skew_y = y;
                force_redraw();
                return true;
            }

            return false;
        }


        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            base.OnMouseDown(mouseEvent);
            if (!mouseEvent.Handled)
            {
                if (MoveTheLion(mouseEvent))
                {
                    mouseEvent.Handled = true;
                }
            }
        }

        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
            base.OnMouseMove(mouseEvent);
            if (!mouseEvent.Handled)
            {
                if (MoveTheLion(mouseEvent))
                {
                    mouseEvent.Handled = true;
                }
            }
        }

        public static void StartDemo()
        {
            lion_outline_application<T> app = new lion_outline_application<T>(PixelFormats.pix_format_bgr24, ERenderOrigin.OriginBottomLeft);
            app.Caption = "AGG Example. Lion";

            if (app.init(512, 400, (uint)WindowFlags.Risizeable))
            {
                app.run();
            }
        }


    }

    public static class App
    {
        [STAThread]
        public static void Main(string[] args)
        {
            lion_outline_application<DoubleComponent>.StartDemo();
        }
    }
}
