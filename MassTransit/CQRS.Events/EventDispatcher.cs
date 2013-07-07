using System;
using System.Collections.Generic;
using System.Threading;
using CQRS.Common;
using CQRS.Interfaces.Events;
using MHM.WinFlexOne.CQRS;

namespace CQRS.Events
{
    public interface IEventPublisher
    {
        void RegisterHandler<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
        void Publish<T>(T @event) where T : IEvent;
    }

    public class EventDispatcher : IEventPublisher
    {
        private readonly Dictionary<Type, List<Action<IEvent>>> m_routes = new Dictionary<Type, List<Action<IEvent>>>();

        public void RegisterHandler<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            List<Action<IEvent>> handlers;
            if (!m_routes.TryGetValue(typeof(TEvent), out handlers))
            {
                handlers = new List<Action<IEvent>>();
                m_routes.Add(typeof(TEvent), handlers);
            }
            handlers.Add(DelegateAdjuster.CastArgument<IEvent, TEvent>(x => handler(x)));
        }

        public void Publish<T>(T @event) where T : IEvent
        {
            List<Action<IEvent>> handlers;
            if (!m_routes.TryGetValue(@event.GetType(), out handlers)) return;
            //Parallel.ForEach(handlers, handler => handler(@event));
            foreach (var handler in handlers)
            {
                //dispatch on thread pool
                var handler1 = handler;
                ThreadPool.QueueUserWorkItem(x => handler1(@event));
            }
        }
    }
}
