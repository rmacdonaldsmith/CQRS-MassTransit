using System;
using CQRS.Interfaces.Events;
using MassTransit;

namespace CQRS.Messages.Events
{
    public partial class ClaimRequestCreatedPendingVerificationEvent : IEvent, CorrelatedBy<Guid>
    {
        //public ClaimRequestCreatedPendingVerificationEvent()
        //{
        //}

        //public ClaimRequestCreatedPendingVerificationEvent(Guid correlationId)
        //{
        //    CorrelationId = correlationId;
        //}

        public Guid CorrelationId { get; set; }
    }
}
