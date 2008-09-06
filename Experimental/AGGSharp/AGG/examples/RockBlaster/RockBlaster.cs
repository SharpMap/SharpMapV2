//#define USE_OPENGL

using System;
using AGG.Color;
using AGG.UI;
using NPack;
using NPack.Interfaces;
using Reflexive.Game;

namespace RockBlaster
{
    public class RockBlasterGame<T> : Reflexive.Game.GamePlatform<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        MainMenu<T> m_MainMenu;
        PlayfieldView<T> m_Playfield;
        public RockBlasterGame(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(1.0f / 60.0f, 5, format, RenderOrigin)
        {
        }

        public override void OnInitialize()
        {
            Entity<T>.GameWidth = (int)width().ToInt();
            Entity<T>.GameHeight = (int)height().ToInt();

            m_MainMenu = new MainMenu<T>();
            AddChild(m_MainMenu);
            m_MainMenu.StartGame += new MainMenu<T>.StartGameEventHandler(StartGame);
            m_MainMenu.ExitGame += new MainMenu<T>.ExitGameEventHandler(ExitGame);

            m_Playfield = new PlayfieldView<T>();
            AddChild(m_Playfield);
            m_Playfield.Menu += new PlayfieldView<T>.MenuEventHandler(EndGame);
            m_Playfield.Visible = false;

            String PathToUse = "GameData";
            if (!System.IO.Directory.Exists(PathToUse))
            {
                PathToUse = "../../GameData";
                if (!System.IO.Directory.Exists(PathToUse))
                {
                    PathToUse = "../../../../RockBlaster/GameData";
                    if (!System.IO.Directory.Exists(PathToUse))
                    {
                    }
                }
            }

            DataAssetTree DataFolderTree = new DataAssetTree(PathToUse);
            DataAssetCache<T>.Instance.SetAssetTree(DataFolderTree);

            base.OnInitialize();
        }

        public void StartGame(GUIWidget<T> widget)
        {
            m_Playfield.StartGame();
            m_MainMenu.Visible = false;
            m_Playfield.Visible = true;
        }

        public void ExitGame(GUIWidget<T> widget)
        {
            Close();
        }

        public void EndGame(GUIWidget<T> widget)
        {
            m_MainMenu.Visible = true;
            m_Playfield.Visible = false;
        }

        public override void OnDraw()
        {
            this.ShowFrameRate = false;
            GetRenderer().Clear(new RGBA_Doubles(.3, .3, .3));

            base.OnDraw();
        }

        public override void OnKeyDown(AGG.UI.KeyEventArgs keyEvent)
        {
            if (keyEvent.KeyCode == Keys.F6)
            {
                // launch the editor
            }

            base.OnKeyDown(keyEvent);
        }

        public override void OnKeyUp(AGG.UI.KeyEventArgs keyEvent)
        {
            base.OnKeyUp(keyEvent);
        }

        public override void OnUpdate(double NumSecondsPassed)
        {
            if (m_Playfield.Visible)
            {
                m_Playfield.OnUpdate(NumSecondsPassed);
            }

            base.OnUpdate(NumSecondsPassed);
        }

        public static void StartDemo()
        {
            RockBlasterGame<T> app = new RockBlasterGame<T>(PixelFormats.pix_format_rgba32, ERenderOrigin.OriginBottomLeft);
            app.Caption = "Rock blaster is a game a lot like Asteroids.";

#if USE_OPENGL
            if (app.init(640, 480, (uint)WindowFlags.UseOpenGL))
#else
            if (app.init(640, 480, (uint)(WindowFlags.None)))
#endif
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
            RockBlasterGame<DoubleComponent>.StartDemo();
        }
    }
}
