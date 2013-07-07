using CQRS.Commands;
using CQRS.Interfaces.Commands;
using MassTransit;

namespace CQRS.TimeoutService
{
    public class StartTimeoutCommandHandler : Handles<StartTimeout>, Consumes<StartTimeout>.All
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
