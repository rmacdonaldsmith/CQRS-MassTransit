using System;
using System.Threading;
using MHM.WinFlexOne.CQRS;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Interfaces.Commands;
using MassTransit;

namespace CQRS.UI.Web.Nancy.Services
{
    /// <summary>
    /// The command is dispatched and the calling thread is blocked until either the response is received,
    /// the timeout expires or an exception is thrown.
    /// </summary>
    public class SynchronousCommandDispatcher : IDispatchCommandsAndWaitForResponse
    {
        private readonly TimeSpan m_timeOut;
        private readonly IServiceBus m_bus;

        public SynchronousCommandDispatcher(IServiceBusFactory<IServiceBus> busFactory)
        {
            m_bus = busFactory.Get();
            m_timeOut = TimeSpan.FromSeconds(60);
        }

        public void Dispatch<TCommand>(TCommand command, Action<CommandResponse> commandcompletedCallback, Action timeoutCallBack) where TCommand : class, ICommand
        {
            m_bus.RequestResponse<TCommand, CommandResponse>(command, commandcompletedCallback, timeoutCallBack, m_timeOut);
        }
    }
}