using System;
using MassTransit;

namespace CQRS.Sagas.Tests.TestSaga
{
    public class StartMessage : CorrelatedBy<Guid>
    {
        public string MessageId { get; set; }

        public decimal Amount { get; set; }

        public Guid CorrelationId { get; set; }
    }
}
