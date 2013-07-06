using MHM.WinFlexOne.CQRS.Commands;
using MassTransit;

namespace MHM.WinFlexOne.CQRS.TimeoutService
{
    public class StartTimeoutCommandHandler : Interfaces.Commands.Handles<StartTimeout>, Consumes<StartTimeout>.All
    {
        private readonly TimeoutRegistry _timeoutRegistry;

        public StartTimeoutCommandHandler(TimeoutRegistry timeoutRegistry)
        {
            _timeoutRegistry = timeoutRegistry;
        }

        public void Handle(StartTimeout command)
        {
            _timeoutRegistry.RegisterTimeout(command);
        }

        public void Consume(StartTimeout message)
        {
            Handle(message);
        }
    }
}
