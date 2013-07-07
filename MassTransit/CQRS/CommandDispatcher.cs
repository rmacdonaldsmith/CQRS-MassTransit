using System;
using System.Collections.Generic;
using CQRS.Interfaces.Commands;

namespace CQRS.Common
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly Dictionary<Type, object> _commandHandlerMap = new Dictionary<Type, object>();

        public void Register<TCommand>(Handles<TCommand> commandHandler) where TCommand : ICommand
        {
            var commandType = typeof (TCommand);

            Register(commandType, commandHandler);
        }

        public void Register(Type commandType, object handlerInstance)
        {
            if (_commandHandlerMap.ContainsKey(commandType))
            {
                throw new InvalidOperationException(string.Format("A command handler is already registered for commands of type '{0}'.", commandType));
            }

            _commandHandlerMap.Add(commandType, handlerInstance);
        }

        public void Dispatch<TCommand>(TCommand command) where TCommand : ICommand
        {
            var commandType = typeof (TCommand);
            if(_commandHandlerMap.ContainsKey(commandType) == false)
            {
                throw new KeyNotFoundException(string.Format("There are no command handlers registered to handle commands of type '{0}'", commandType));
            }

            ((Handles<TCommand>)_commandHandlerMap[commandType]).Handle(command);
        }
    }
}
