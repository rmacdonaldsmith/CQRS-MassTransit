using System;
using MHM.WinFlexOne.CQRS.Interfaces.Events;
using MassTransit;

namespace MHM.WinFlexOne.CQRS.Events
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
