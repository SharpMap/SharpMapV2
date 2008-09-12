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
using System.ComponentModel;

namespace MapViewer.Commands
{
    public abstract class CommandComponentSourceBase<TComponent>
        : ICommandComponentSource<TComponent>
        where TComponent : IComponent
    {
        private ICommand _command;
        private TComponent _component;
        protected CommandEnabledChangedDelegate<TComponent> _enabledChangedAction;

        protected CommandComponentSourceBase()
        {
            _enabledChangedAction = DefaultEnabledChangedAction();
        }

        protected CommandComponentSourceBase(ICommand command)
            : this(default(TComponent), command)
        {
        }

        protected CommandComponentSourceBase(TComponent component)
            : this(component, null)
        {
        }

        protected CommandComponentSourceBase(TComponent component, ICommand command)
            : this()
        {
            Command = command;
            Component = component;
        }

        private CommandEnabledChangedDelegate<TComponent> CommandEnabledChangedAction
        {
            get { return _enabledChangedAction; }
            set { _enabledChangedAction = value; }
        }

        #region ICommandComponentSource<TComponent> Members

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

        public TComponent Component
        {
            get { return _component; }
            set
            {
                if (!Equals(_component, default(TComponent)))
                    UnwireComponent(_component);
                if (!Equals(value, default(TComponent)))
                {
                    _component = value;
                    WireComponent(_component);
                    CommandEnabledChanged(null, null);
                }
            }
        }

        #endregion

        protected abstract CommandEnabledChangedDelegate<TComponent> DefaultEnabledChangedAction();

        internal void CommandEnabledChanged(object sender, EventArgs e)
        {
            if (!Equals(Component, default(TComponent)) && Command != null)
                CommandEnabledChangedAction(Component, Command.Enabled);
        }

        protected abstract void WireComponent(TComponent component);
        protected abstract void UnwireComponent(TComponent component);
    }

    public abstract class CommandComponentSourceBase<TComponent, TEventArgs>
        : CommandComponentSourceBase<TComponent>, ICommandComponentSource<TComponent, TEventArgs>
        where TComponent : IComponent
        where TEventArgs : CommandEventArgs
    {
        public CommandComponentSourceBase(ICommand<TEventArgs> command)
            : this(default(TComponent), command)
        {
        }

        public CommandComponentSourceBase(TComponent component)
            : this(component, null)
        {
        }

        public CommandComponentSourceBase(TComponent component, ICommand<TEventArgs> command)
            : base(component, command)
        {
        }

        #region ICommandComponentSource<TComponent,TEventArgs> Members

        public new ICommand<TEventArgs> Command
        {
            get { return (ICommand<TEventArgs>) base.Command; }
            set { base.Command = value; }
        }

        #endregion
    }
}