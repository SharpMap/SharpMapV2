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

namespace MapViewer.Commands
{
    public class Command : ICommand
    {
        protected readonly string _name;
        private bool _enabled = true;

        public Command(string name)
        {
            _name = name;
        }

        #region ICommand Members

        public event EventHandler CommandFired;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    if (EnabledChanged != null)
                        EnabledChanged(this, EventArgs.Empty);
                }
            }
        }

        public void FireCommand()
        {
            if (CommandFired != null)
                CommandFired(this, EventArgs.Empty);
        }

        public event EventHandler EnabledChanged;

        public string Name
        {
            get { return _name; }
        }

        #endregion
    }
}