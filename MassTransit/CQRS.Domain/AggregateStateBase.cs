using System;
using System.Collections.Generic;
using CQRS.Interfaces.Events;

namespace CQRS.Domain
{
    public abstract class AggregateStateBase
    {
        private readonly Dictionary<Type, Action<IEvent>> _eventRoutes = new Dictionary<Type, Action<IEvent>>();

        public abstract Guid Id { get; }

        public void Initialize(IEnumerable<IEvent> eventHistory)
        {
            foreach (var evnt in eventHistory)
            {
                PlayEvent(evnt);
            }
        }

        protected void Register<TEvent>(Action<TEvent> handlerDelegate) where TEvent : IEvent
        {
            if (handlerDelegate == null) throw new ArgumentNullException("handlerDelegate");

            _eventRoutes.Add(typeof(TEvent), evnt => handlerDelegate((TEvent) evnt));
        }

        public void Apply(IEvent evnt)
        {
            PlayEvent(evnt);
        }

        private void PlayEvent(IEvent evnt)
        {
            Action<IEvent> eventHandlingDelegate;
            if (_eventRoutes.TryGetValue(evnt.GetType(), out eventHandlingDelegate))
            {
                eventHandlingDelegate(evnt);
            }
        }
    }
}