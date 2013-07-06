using System;
using MHM.WinFlexOne.CQRS.Interfaces.Events;
using MassTransit;

namespace MHM.WinFlexOne.CQRS.Events
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
