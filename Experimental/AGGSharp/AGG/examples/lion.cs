#define USE_OPENGL

using System;
using AGG.Color;
//using pix_format_e = AGG.UI.PlatformSupportAbstract.PixelFormats;
using AGG.Transform;
using AGG.UI;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;

namespace AGG
{
    public class lion_application<T> : AGG.UI.win32.PlatformSupport<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private AGG.UI.SliderWidget<T> m_AlphaSlider;

        PathStorage<T> g_PathStorage = new PathStorage<T>();
        RGBA_Bytes[] g_colors = new RGBA_Bytes[100];
        uint[] g_path_idx = new uint[100];
        uint m_NumPaths = 0;
        T g_x1 = M.Zero<T>();
        T g_y1 = M.Zero<T>();
        T g_x2 = M.Zero<T>();
        T g_y2 = M.Zero<T>();
        T g_base_dx = M.Zero<T>();
        T g_base_dy = M.Zero<T>();
        double g_angle = 0;
        T g_scale = M.One<T>();
        T g_skew_x = M.Zero<T>();
        T g_skew_y = M.Zero<T>();

        public void parse_lion()
        {
            m_NumPaths = AGG.samples.LionParser.parse_lion(g_PathStorage, g_colors, g_path_idx);
            AGG.BoundingRect<T>.GetBoundingRect(g_PathStorage, g_path_idx, 0, m_NumPaths, out g_x1, out g_y1, out g_x2, out g_y2);
            g_base_dx = g_x2.Subtract(g_x1).Divide(2.0);
            g_base_dy = g_y2.Subtract(g_y1).Divide(2.0);
        }

        public lion_application(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            m_AlphaSlider = new UI.SliderWidget<T>(M.New<T>(5), M.New<T>(5), M.New<T>(512 - 5), M.New<T>(12));
            parse_lion();
            AddChild(m_AlphaSlider);
            m_AlphaSlider.SetTransform(MatrixFactory<T>.NewIdentity(VectorDimension.Two));
            m_AlphaSlider.label("Alpha {0:F3}");
            m_AlphaSlider.value(M.New<T>(0.1));
        }

#if use_timers
        static CNamedTimer AllTimer = new CNamedTimer("All");
        static CNamedTimer Lion50Timer = new CNamedTimer("Lion 50");
#endif
        public override void OnDraw()
        {
#if use_timers
            AllTimer.Start();
#endif
            int width = (int)rbuf_window().Width;
            int height = (int)rbuf_window().Height;

            uint i;
            for (i = 0; i < m_NumPaths; i++)
            {
                // g_colors[i].A_Byte = (byte)(m_AlphaSlider.value() * 255);
                g_colors[i] = RGBA_Bytes.ModifyComponent(g_colors[i], Component.A, (byte)(m_AlphaSlider.value().ToDouble() * 255));
            }

            IAffineTransformMatrix<T> transform = MatrixFactory<T>.NewIdentity(VectorDimension.Two);
            transform.Translate(MatrixFactory<T>.CreateVector2D(g_base_dx.Negative(), g_base_dy.Negative()));
            transform.Scale(MatrixFactory<T>.CreateVector2D(g_scale, g_scale));
            transform.RotateAlong(MatrixFactory<T>.CreateVector2D(M.Zero<T>(), M.Zero<T>()), g_angle + Math.PI);
            transform.Shear(MatrixFactory<T>.CreateVector2D(g_skew_x.Divide(1000.0), g_skew_y.Divide(1000.0)));
            transform.Translate(MatrixFactory<T>.CreateVector2D(M.New<T>(width).Divide(2), M.New<T>(height).Divide(2)));

            // This code renders the lion:
            ConvTransform<T> transformedPathStorage = new ConvTransform<T>(g_PathStorage, transform);
#if use_timers
            Lion50Timer.Start();
            for (uint j = 0; j < 200; j++)
#endif
            {
                this.GetRenderer().Render(transformedPathStorage, g_colors, g_path_idx, m_NumPaths);
            }
#if use_timers
            Lion50Timer.Stop();
#endif

#if use_timers
            AllTimer.Stop();
            CExecutionTimer.Instance.AppendResultsToFile("TimingTest.txt", AllTimer.GetTotalSeconds());
            CExecutionTimer.Instance.Reset();
#endif
            base.OnDraw();
        }

        void transform(T width, T height, T x, T y)
        {
            x.SubtractEquals(width.Divide(2));
            y.SubtractEquals(height.Divide(2));
            g_angle = M.Atan2(y, x).ToDouble();
            g_scale = M.Length(x, y).Divide(100);// Math.Sqrt(y * y + x * x) / 100.0;
        }

        protected bool MoveTheLion(MouseEventArgs mouseEvent)
        {
            double x = mouseEvent.X;
            double y = mouseEvent.Y;
            if (mouseEvent.Button == MouseButtons.Left)
            {
                uint width = rbuf_window().Width;
                uint height = rbuf_window().Height;
                transform(M.New<T>(width), M.New<T>(height), M.New<T>(x), M.New<T>(y));
                force_redraw();
                return true;
            }

            if (mouseEvent.Button == MouseButtons.Right)
            {
                g_skew_x = M.New<T>(x);
                g_skew_y = M.New<T>(y);
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
            lion_application<T> app = new lion_application<T>(PixelFormats.pix_format_bgr24, ERenderOrigin.OriginBottomLeft);
            app.Caption = "AGG Example. Lion";

#if USE_OPENGL
            if (app.init(512, 400, (uint)(WindowFlags.UseOpenGL | WindowFlags.Risizeable)))
#else
            if (app.init(512, 400, (uint)WindowFlags.Risizeable))
#endif
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
            lion_application<DoubleComponent>.StartDemo();
        }
    }
}
