using System;
using System.Collections.Generic;
using CQRS.Interfaces.Events;

namespace CQRS.DomainTesting
{
    public class FakeEventStore : IFakeEventStore
    {
        private readonly List<IEvent> m_events = new List<IEvent>();
        private readonly List<IEvent> m_uncommittedEvents = new List<IEvent>();

        public FakeEventStore(IEnumerable<IEvent> seedEvents)
        {
            m_events.AddRange(seedEvents);
        }

        public void Save(Guid aggregateId, IEnumerable<IEvent> events, int expectedRevision)
        {
            m_uncommittedEvents.AddRange(events);
        }

        public IEnumerable<IEvent> GetForAggregate(Guid aggregateId)
        {
            return m_events;
        }

        public IEnumerable<IEvent> PeakChanges()
        {
            return m_uncommittedEvents;
        }
    }
}
