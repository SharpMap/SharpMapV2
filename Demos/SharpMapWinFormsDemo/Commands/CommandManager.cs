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
using System.Collections.Generic;

namespace MapViewer.Commands
{
    public class CommandManager
    {
        private readonly IDictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();
        private readonly IList<ICommandHandler> _handlers = new List<ICommandHandler>();
        private readonly IList<ICommandSource> _sources = new List<ICommandSource>();

        public ICommand this[string commandName]
        {
            get { return _commands[commandName]; }
        }

        public IEnumerable<string> ManagedCommandNames
        {
            get
            {
                foreach (string s in _commands.Keys)
                    yield return s;
            }
        }

        public bool Contains(string commandName)
        {
            return this[commandName] != null;
        }

        public bool Contains(ICommand command)
        {
            return Contains(command.Name);
        }

        public void AddCommand(ICommand command)
        {
            if (string.IsNullOrEmpty(command.Name))
                throw new ArgumentException("Command Name cannot be null");

            if (_commands.ContainsKey(command.Name))
            {
                if (ReferenceEquals(_commands[command.Name], command))
                    return;

                throw new ArgumentException("Command names must be unique");
            }
            _commands.Add(command.Name, command);
        }

        public void AddCommandSource(ICommandSource commandSource)
        {
            if (commandSource.Command == null)
                throw new Exception("Command must be set on the command source");

            if (!_commands.ContainsKey(commandSource.Command.Name))
                AddCommand(commandSource.Command);

            if (!_sources.Contains(commandSource))
                _sources.Add(commandSource);
        }

        public void AddCommandSource(ICommandSource commandSource, string commandName)
        {
            ICommand cmd = _commands[commandName];
            if (cmd == null)
                throw new ArgumentException(string.Format("Command {0} not found", commandName));

            commandSource.Command = cmd;
            if (!_sources.Contains(commandSource))
                _sources.Add(commandSource);
        }

        public void AddCommandHandler(ICommandHandler handler)
        {
            if (handler.Command == null)
                throw new ArgumentException("Command must be set on the command handler");

            if (!_commands.ContainsKey(handler.Command.Name))
                AddCommand(handler.Command);

            if (!_handlers.Contains(handler))
                _handlers.Add(handler);
        }

        public void AddCommandHandler(ICommandHandler handler, string commandName)
        {
            ICommand cmd = _commands[commandName];
            if (cmd == null)
                throw new ArgumentException(string.Format("Command {0} not found", commandName));

            handler.Command = cmd;
            if (!_handlers.Contains(handler))
                _handlers.Add(handler);
        }

        public void RemoveCommand(ICommand command)
        {
            foreach (
                ICommandHandler handler in
                    FindCommandHandlers(
                        delegate(ICommandHandler o) { return ReferenceEquals(o.Command, command); }))
            {
                RemoveCommandHandler(handler);
            }

            foreach (
                ICommandSource source in FindCommandSources(
                    delegate(ICommandSource o) { return ReferenceEquals(o.Command, command); }))
            {
                RemoveCommandSource(source);
            }
            _commands.Remove(command.Name);
        }

        public void RemoveCommandHandler(ICommandHandler handler)
        {
            handler.Command = null;
            _handlers.Remove(handler);
        }

        public void RemoveCommandSource(ICommandSource source)
        {
            source.Command = null;
            _sources.Remove(source);
        }

        public IEnumerable<TCommandSource> FindCommandSources<TCommandSource>(Predicate<TCommandSource> predicate)
            where TCommandSource : ICommandSource
        {
            foreach (ICommandSource source in _sources)
            {
                if (source is TCommandSource)
                {
                    if (predicate((TCommandSource)source))
                        yield return (TCommandSource)source;
                }
            }
        }

        public IEnumerable<TCommandHandler> FindCommandHandlers<TCommandHandler>(Predicate<TCommandHandler> predicate)
            where TCommandHandler : ICommandHandler
        {
            foreach (ICommandHandler handler in _handlers)
            {
                if (handler is TCommandHandler)
                {
                    if (predicate((TCommandHandler)handler))
                        yield return (TCommandHandler)handler;
                }
            }
        }

        public IEnumerable<TCommand> FindCommands<TCommand>(Predicate<TCommand> predicate)
            where TCommand : ICommand
        {
            foreach (ICommand command in _commands.Values)
            {
                if (command is TCommand)
                {
                    if (predicate((TCommand)command))
                        yield return (TCommand)command;
                }
            }
        }
    }
}