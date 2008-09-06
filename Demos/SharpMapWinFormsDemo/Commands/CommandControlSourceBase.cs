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
using System;
using System.Windows.Forms;

namespace MapViewer.Commands
{
    public delegate void CommandEnabledChangedDelegate<T>(T target, bool commandEnabled);

    public abstract class CommandControlSourceBase<TControl>
        : ICommandControlSource<TControl>
        where TControl : Control
    {
        private ICommand _command;
        private TControl _control;

        protected CommandEnabledChangedDelegate<TControl> _enabledChangedAction
            = delegate(TControl o, bool b) { o.Enabled = b; };

        protected CommandControlSourceBase()
        {
        }

        protected CommandControlSourceBase(ICommand command)
            : this(null, command)
        {
        }

        protected CommandControlSourceBase(TControl control)
            : this(control, null)
        {
        }

        protected CommandControlSourceBase(TControl control, ICommand command)
            : this()
        {
            Command = command;
            Control = control;
        }

        private CommandEnabledChangedDelegate<TControl> CommandEnabledChangedAction
        {
            get { return _enabledChangedAction; }
            set { _enabledChangedAction = value; }
        }

        #region ICommandControlSource<TControl> Members

        public ICommand Command
        {
            get { return _command; }
            set
            {
                if (_command != null)
                    _command.EnabledChanged -= CommandEnabledChanged;
                if (value != null)
                {
                    _command = value;
                    _command.EnabledChanged += CommandEnabledChanged;
                }
            }
        }

        public TControl Control
        {
            get { return _control; }
            set
            {
                if (_control != null)
                    UnwireControl(_control);
                if (value != null)
                {
                    _control = value;
                    WireControl(_control);
                    CommandEnabledChanged(null, null);
                }
            }
        }

        #endregion

        internal void CommandEnabledChanged(object sender, EventArgs e)
        {
            if (Control != null && Command != null)
                CommandEnabledChangedAction(Control, Command.Enabled);
        }

        protected abstract void WireControl(TControl control);
        protected abstract void UnwireControl(TControl control);
    }
}