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
using MapViewer.Commands;

namespace MapViewer
{
    public class ActionCommandHandler
        : CommandHandlerBase
    {
        public ActionCommandHandler()
            : this(null, null)
        {
        }

        public ActionCommandHandler(ICommand command)
            : this(command, null)
        {
        }

        public ActionCommandHandler(Action action)
            : this(null, action)
        {
        }

        public ActionCommandHandler(ICommand command, Action action)
            : base(command)
        {
            Action = action;
        }

        public Action Action { get; set; }

        public override void HandleCommand()
        {
            if (Action != null)
                Action();
        }
    }

    public class ActionCommandHandler<TEventArgs>
        : CommandHandlerBase<TEventArgs>
        where TEventArgs : CommandEventArgs
    {
        public ActionCommandHandler()
            : this(null, null)
        {
        }

        public ActionCommandHandler(ICommand<TEventArgs> command)
            : this(command, null)
        {
        }

        public ActionCommandHandler(Action<TEventArgs> action)
            : this(null, action)
        {
        }

        public ActionCommandHandler(ICommand<TEventArgs> command, Action<TEventArgs> action)
            : base(command)
        {
            Action = action;
        }

        public Action<TEventArgs> Action { get; set; }

        public override void HandleCommand(TEventArgs args)
        {
            if (Action != null)
                Action(args);
        }

        public override void HandleCommand()
        {
            throw new InvalidOperationException();
        }
    }
}