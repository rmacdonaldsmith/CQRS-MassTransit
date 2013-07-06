using System;
using MHM.WinFlexOne.CQRS.Interfaces.Commands;

namespace MHM.WinFlexOne.CQRS
{
    public interface ICommandDispatcher
    {
        void Register<TCommand>(Handles<TCommand> commandHandler) where TCommand : ICommand;
        void Register(Type commandType, object handlerInstance);

        void Dispatch<TCommand>(TCommand command) where TCommand : ICommand;
        
    }
}


