using System;

using AGG.UI;

using NPack.Interfaces;


namespace RockBlaster
{
    public class PlayfieldView<T> : GUIWidget<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        Playfield<T> m_Playfield;
        public TextWidget<T> m_ScoreWidget;

        public delegate void MenuEventHandler(GUIWidget<T> button);
        public event MenuEventHandler Menu;

        public PlayfieldView()
        {
            ButtonWidget<T> MenuButton = new ButtonWidget<T>(20, 20, "Menu");
            AddChild(MenuButton);
            MenuButton.ButtonClick += new ButtonWidget<T>.ButtonEventHandler(EscapeMenu);

            m_ScoreWidget = new TextWidget<T>("0", 550, 450, 16);
            AddChild(m_ScoreWidget);
        }

        private void EscapeMenu(GUIWidget<T> widget)
        {
            if (Menu != null)
            {
                Menu(this);
            }
        }

        public void StartGame()
        {
            Focus();

            m_Playfield = new Playfield<T>();

            m_Playfield.StartGame();

            //m_Playfield.SaveXML("Test");
        }

        public override void OnDraw()
        {
            m_Playfield.Draw(GetRenderer());

            m_ScoreWidget.Text = PlayerSaveInfo<T>.GetPlayerInfo().Score.ToString();

            base.OnDraw();
        }

        public override void OnKeyDown(AGG.UI.KeyEventArgs keyEvent)
        {
            m_Playfield.Player.KeyDown(keyEvent);

            if (keyEvent.Control && keyEvent.KeyCode == Keys.S)
            {
                m_Playfield.SaveXML("TestSave");
            }

            base.OnKeyDown(keyEvent);
        }

        public override void OnKeyUp(AGG.UI.KeyEventArgs keyEvent)
        {
            m_Playfield.Player.KeyUp(keyEvent);
            base.OnKeyUp(keyEvent);
        }

        public void OnUpdate(double NumSecondsPassed)
        {
            m_Playfield.Update(NumSecondsPassed);
        }
    }
}
