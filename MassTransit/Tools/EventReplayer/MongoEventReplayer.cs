using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using CQRS.Interfaces.Events;
using EventStore;
using MongoDB.Driver;

namespace EventReplayer
{
    public class MongoEventReplayer : IEventReplayer
    {
        private readonly Type _eventType;
        private readonly IEnumerable<Type> _eventHandlerTypes;
        private readonly IEnumerable<Handles<IEvent>> _eventHandlerInstances;
        private readonly IStoreEvents _eventStore;
        private readonly MongoDatabase _readModelDatabase;

        public MongoEventReplayer(Type eventType, IEnumerable<Handles<IEvent>> eventHandlerInstances, IStoreEvents eventStore, MongoDatabase readModelDatabase)
        {
            if (eventType == null) throw new ArgumentNullException("eventType");
            if (eventStore == null) throw new ArgumentNullException("eventStore");
            if (readModelDatabase == null) throw new ArgumentNullException("readModelDatabase");

            _eventHandlerInstances = eventHandlerInstances;
            _eventStore = eventStore;
            _readModelDatabase = readModelDatabase;
        }

        public void Replay()
        {
            //flatten the event stream to a single collection of events
            IEnumerable<IEvent> events = _eventStore.Advanced.GetFrom(DateTime.MinValue).SelectMany(commit => commit.Events.Select(evnt => evnt.Body as IEvent));

            foreach (var @event in events)
            {
                foreach (var handlerInstance in _eventHandlerInstances)
                {
                    handlerInstance.Handle(@event);
                }
            }
        }
    }
}
