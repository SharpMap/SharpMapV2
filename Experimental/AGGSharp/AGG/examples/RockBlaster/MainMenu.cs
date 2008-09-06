using System;
using AGG.UI;
using NPack.Interfaces;

namespace RockBlaster
{
    /// <summary>
    /// Description of MainMenu.
    /// </summary>
    public class MainMenu<T> : GUIWidget<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public delegate void StartGameEventHandler(GUIWidget<T> button);
        public event StartGameEventHandler StartGame;

        public delegate void ExitGameEventHandler(GUIWidget<T> button);
        public event ExitGameEventHandler ExitGame;

        public MainMenu()
        {
            ButtonWidget<T> StartGameButton = new ButtonWidget<T>(260, 300, "Start Game");
            AddChild(StartGameButton);
            StartGameButton.ButtonClick += new ButtonWidget<T>.ButtonEventHandler(OnStartGameButton);

            ButtonWidget<T> ExitGameButton = new ButtonWidget<T>(300, 180, "Exit");
            AddChild(ExitGameButton);
            ExitGameButton.ButtonClick += new ButtonWidget<T>.ButtonEventHandler(OnExitGameButton);

            //TextEditWidget testEdit = new TextEditWidget("Edit me\r   oaeu.\roeu\r  .aoeu  \r.", new rect_d(200, 140, 440, 160), 9);
            //TextEditWidget testEdit = new TextEditWidget("", new rect_d(200, 140, 440, 160), 9);
            //AddChild(testEdit);
        }

        private void OnStartGameButton(ButtonWidget<T> button)
        {
            if (StartGame != null)
            {
                StartGame(this);
            }
        }

        private void OnExitGameButton(ButtonWidget<T> button)
        {
            if (ExitGame != null)
            {
                ExitGame(this);
            }
        }
    }
}
