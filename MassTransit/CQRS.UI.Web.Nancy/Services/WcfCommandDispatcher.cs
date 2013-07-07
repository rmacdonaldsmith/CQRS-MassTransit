using System;
using CQRS.Commands;
using CQRS.Interfaces.Commands;

namespace CQRS.UI.Web.Nancy.Services
{
    public class WcfCommandDispatcher : IDispatchCommandsAndWaitForResponse
    {
        public void Dispatch<TCommand>(TCommand command, Action<CommandResponse> commandcompletedCallback, Action timeoutCallBack) where TCommand : class, ICommand
        {
            throw new NotImplementedException();

            //1. write shared interface with a method similar to 
            //  CommandResponse Dispatch(ICommand command);

            //2. 
        }
    }
}