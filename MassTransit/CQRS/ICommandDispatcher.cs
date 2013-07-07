using System;
using CQRS.Interfaces.Commands;

namespace CQRS.Common
{
    public interface ICommandDispatcher
    {
        void Register<TCommand>(Handles<TCommand> commandHandler) where TCommand : ICommand;
        void Register(Type commandType, object handlerInstance);

        void Dispatch<TCommand>(TCommand command) where TCommand : ICommand;
        
    }
}


