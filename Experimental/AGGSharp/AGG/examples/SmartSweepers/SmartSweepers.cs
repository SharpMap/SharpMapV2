using System;
using AGG.Color;
using AGG.UI;
using NPack;
using NPack.Interfaces;
using Reflexive.Game;

namespace SmartSweeper
{
    public class SmartSweeperApplication<T> : GamePlatform<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        CController<T> m_Controller;
        private static double rtri;                                              // Angle For The Triangle ( NEW )
        private static double rquad;                                             // Angle For The Quad ( NEW )
        AGG.UI.cbox_ctrl<T> m_SuperFast;

        public SmartSweeperApplication(AGG.UI.PlatformSupportAbstract<T>.PixelFormats format, ERenderOrigin RenderOrigin)
            : base(1.0f / 60.0f, 5, format, RenderOrigin)
        {
        }

        public override void OnInitialize()
        {
            m_SuperFast = new AGG.UI.cbox_ctrl<T>(M.New<T>(10), M.New<T>(10), "Run Super Fast");
            AddChild(m_SuperFast);
            m_Controller = new CController<T>(rbuf_window(), 30, 40, .1, .7, .3, 4, 1, 2000);
            base.OnInitialize();
        }

        public override void OnResize(int width, int height)
        {
            base.OnResize(width, height);
        }

        public override void OnDraw()
        {
            GetRenderer().Clear(new RGBA_Doubles(1, 1, 1, 1));
            GetRenderer().Rasterizer.SetVectorClipBox(M.Zero<T>(), M.Zero<T>(), width(), height());
            m_Controller.FastRender(m_SuperFast.status());
            m_Controller.Render(GetRenderer());
            //m_SuperFast.Render(GetRenderer());
            base.OnDraw();
        }

        public override void OnUpdate(double NumSecondsPassed)
        {
            if (m_SuperFast.status())
            {
                for (int i = 0; i < 40; i++)
                {
                    m_Controller.Update();
                }
            }
            m_Controller.Update();
            rtri += 0.2f;                                                       // Increase The Rotation Variable For The Triangle ( NEW )
            rquad -= 0.15f;                                                     // Decrease The Rotation Variable For The Quad ( NEW )
            base.OnUpdate(NumSecondsPassed);
        }

        public static void StartDemo()
        {
            SmartSweeperApplication<T> app = new SmartSweeperApplication<T>(PixelFormats.pix_format_rgba32, ERenderOrigin.OriginBottomLeft);
            app.Caption = "Smart Sweepers";

            if (app.init(640, 480, (uint)(WindowFlags.None)))
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
            SmartSweeperApplication<DoubleComponent>.StartDemo();
        }
    }
}
