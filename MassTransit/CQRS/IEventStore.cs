using System;
using System.Collections.Generic;
using MHM.WinFlexOne.CQRS.Interfaces.Events;

namespace MHM.WinFlexOne.CQRS
{
    public interface IEventStore
    {
        void Save(Guid aggregateId, IEnumerable<IEvent> events, int expectedRevision);

        IEnumerable<IEvent> GetForAggregate(Guid aggregateId);
    }
}
