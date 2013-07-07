using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CQRS.Commands;
using CQRS.DomainTesting;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;

namespace CQRS.TimeoutService.Tests
{
    public class when_registering_a_new_timeout : EventSpecification<StartTimeout>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private readonly int _timeoutMS = 500;

        public override IEnumerable<IEvent> Given()
        {
            return NoEvents;
        }

        public override StartTimeout When()
        {
            return new StartTimeout
                {
                    CorrelationId = _correlationId,
                    ElapsesInMS = _timeoutMS,
                };
        }

        public override global::CQRS.Interfaces.Commands.Handles<StartTimeout> BuildCommandHandler()
        {
            var timeoutRegistry = new TimeoutRegistry(EventStore, @event => { });
            return new StartTimeoutCommandHandler(timeoutRegistry);
        }

        public override IEnumerable<IEvent> Then()
        {
            //we should get a timeoutexpired event after waiting the specified timeout period
            return new[] {new TimeoutElapsedEvent
                {
                    CorrelationId = _correlationId,
                    ElapsesMs = _timeoutMS,
                }};
        }

        public override Expression<Predicate<Exception>> ThenException()
        {
            return NoException;
        }

        public override void Finally()
        {
            //nothing to do
        }
    }
}
