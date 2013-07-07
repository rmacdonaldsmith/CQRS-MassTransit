using System;
using CQRS.Commands;
using CQRS.Interfaces.Commands;
using MassTransit;

namespace CQRS.Common.Client
{
    public class CommandSender : ISendCommandsAndWaitForAResponse
    {
        private readonly IServiceBus _bus;
        private readonly TimeSpan _timeOut = TimeSpan.FromSeconds(5);

        public CommandSender(IServiceBus bus)
        {
            _bus = bus;
        }

        public CommandResponse Send<TCommand>(TCommand command) where TCommand : class, ICommand
        {
            CommandResponse commandResponse = null;
            bool requestTimedOut = false;
            //do any validation on the command here? Or do we do that in the controller? Probably in here...

            _bus.RequestResponse<TCommand, CommandResponse>(command,
                                                                    response => commandResponse = response,
                                                                    () => requestTimedOut = true,
                                                                    _timeOut);

            if (requestTimedOut)
            {
                throw new TimeoutException(
                    string.Format(
                        "The request timed out after {0} seconds.", _timeOut.Seconds));
            }

            return commandResponse;
        }
    }
}