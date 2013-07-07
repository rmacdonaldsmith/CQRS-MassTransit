using System;
using CQRS.Interfaces.Events;
using MassTransit;

namespace CQRS.Messages.Events
{
    public partial class ClaimRequestSubmittedEvent : IEvent, CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; private set; }

        public ClaimRequestSubmittedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}
