using System;
using System.Collections.Generic;
using CQRS.Interfaces.Events;

namespace CQRS.Common
{
    public interface IEventStore
    {
        void Save(Guid aggregateId, IEnumerable<IEvent> events, int expectedRevision);

        IEnumerable<IEvent> GetForAggregate(Guid aggregateId);
    }
}
