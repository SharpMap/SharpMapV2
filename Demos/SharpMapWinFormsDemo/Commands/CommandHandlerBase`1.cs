/*
 *	This file is part of SharpMapMapViewer
 *  SharpMapMapViewer is free software © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
namespace MapViewer.Commands
{
    public abstract class CommandHandlerBase<TEventArgs>
        : CommandHandlerBase, ICommandHandler<TEventArgs>
        where TEventArgs : CommandEventArgs
    {
        private ICommand<TEventArgs> _command;

        public CommandHandlerBase()
            : this(null)
        {
        }

        public CommandHandlerBase(ICommand<TEventArgs> command)
            : base(command)
        {
        }

        #region ICommandHandler<TEventArgs> Members

        public ICommand<TEventArgs> Command
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

        #endregion

        private void CommandFired(object sender, TEventArgs args)
        {
            HandleCommand(args);
        }

        public abstract void HandleCommand(TEventArgs args);
    }
}