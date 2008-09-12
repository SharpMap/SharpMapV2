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
    public class MenuItemCommandSource
        : CommandComponentSourceBase<MenuItem>
    {
        public MenuItemCommandSource()
            : this(null, null)
        {
        }

        public MenuItemCommandSource(ICommand command)
            : this(null, command)
        {
        }

        public MenuItemCommandSource(MenuItem component)
            : this(component, null)
        {
        }

        public MenuItemCommandSource(MenuItem component, ICommand command)
            : base(component, command)
        {
            Command = command;
            Component = component;
        }

        protected override void WireComponent(MenuItem component)
        {
            component.Click += component_Click;
        }

        private void component_Click(object sender, EventArgs e)
        {
            if (Command != null)
                Command.FireCommand();
        }

        protected override void UnwireComponent(MenuItem component)
        {
            component.Click -= component_Click;
        }

        protected override CommandEnabledChangedDelegate<MenuItem> DefaultEnabledChangedAction()
        {
            return delegate(MenuItem o, bool e) { o.Enabled = e; };
        }
    }
}