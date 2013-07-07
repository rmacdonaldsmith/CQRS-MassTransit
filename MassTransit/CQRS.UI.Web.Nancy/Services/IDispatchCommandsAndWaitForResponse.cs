using System;
using CQRS.Commands;
using CQRS.Interfaces.Commands;

namespace CQRS.UI.Web.Nancy.Services
{
    public interface IDispatchCommandsAndWaitForResponse
    {
        void Dispatch<TCommand>(TCommand command, Action<CommandResponse> commandcompletedCallback, Action timeoutCallBack) where TCommand : class, ICommand;
    }
}