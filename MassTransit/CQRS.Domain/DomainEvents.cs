using System;
using System.Collections.Generic;
using CQRS.Interfaces.Events;
using Ninject;

namespace CQRS.Domain
{
    public static class DomainEvents
    {
        [ThreadStatic] //so that each thread has its own callbacks
        private static List<Delegate> actions;

        public static IKernel Container { get; set; }

        //Registers a callback for the given domain event
        public static void Register<T>(Action<T> callback) where T : IEvent
        {
            if (actions == null)
                actions = new List<Delegate>();

            actions.Add(callback);
        }

        //Clears callbacks passed to Register on the current thread
        public static void ClearCallbacks()
        {
            actions = null;
        }

        //Raises the given domain event
        public static void Raise<T>(T args) where T : IEvent
        {
            //we can use the IoC container
            if (Container != null)
                foreach (var handler in Container.GetAll<Handles<T>>())
                    handler.Handle(args);

            //and / or the delegates that have been "manually" registered - this block is great for unit testing
            if (actions != null)
                foreach (var action in actions)
                    if (action is Action<T>)
                        ((Action<T>) action)(args);
        }
    }
}
