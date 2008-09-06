using System;

namespace MapViewer.Commands
{
    public abstract class CommandHandlerBase
        : ICommandHandler
    {
        private ICommand _command;

        public CommandHandlerBase()
            : this(null)
        {
        }

        public CommandHandlerBase(ICommand command)
        {
            Command = command;
        }

        #region ICommandHandler Members

        public ICommand Command
        {
            get { return _command; }
            set
            {
                if (_command != null)
                    _command.CommandFired -= CommandFired;
                if (value != null)
                {
                    _command = value;
                    _command.CommandFired += CommandFired;
                }
            }
        }

        public abstract void HandleCommand();

        #endregion

        private void CommandFired(object sender, EventArgs e)
        {
            HandleCommand();
        }
    }
}