using System;
using System.Collections.Generic;
using System.Linq;
using EventStore;
using MHM.WinFlexOne.CQRS.Interfaces.Events;

namespace MHM.WinFlexOne.CQRS.EventPersistence
{
    public class ConcreteEventStore : IEventStore
    {
        private IStoreEvents m_storeEvents;

        public ConcreteEventStore(IStoreEvents configuredEventStore)
        {
            m_storeEvents = configuredEventStore;
        }

        public void Save(Guid aggregateId, IEnumerable<IEvent> events, int expectedRevision)
        {
            //need to think about what is going on with the revision numbers here...
            using (var stream = m_storeEvents.OpenStream(aggregateId, int.MinValue, expectedRevision))
            {
                foreach (var @event in events)
                {
                    stream.Add(new EventMessage { Body = @event });
                }

                stream.CommitChanges(Guid.NewGuid());
            }
        }

        public IEnumerable<IEvent> GetForAggregate(Guid aggregateId)
        {
            using (var stream = m_storeEvents.OpenStream(aggregateId, int.MinValue, int.MaxValue))
            {
                return stream.CommittedEvents.Select(message => message.Body as IEvent);
            }
        }
    }
}
