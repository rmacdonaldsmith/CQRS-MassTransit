using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Core;
using CQRS.Interfaces.Events;
using EventStore;
using MongoDB.Driver;

namespace EventReplayer
{
    public class EventReplayerFactory
    {
        private readonly IContainer _container;

        public EventReplayerFactory(IContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");

            _container = container;
        }

        public MongoEventReplayer Build(string eventTypeName)
        {
            //Since we are using and IoC container in out PROD pipeline to generate the
            //eventhandlers, we will let our IoC container (Autofac) do all the heavy lifting in terms of
            //creating the eventhandler instances for the replayer. This means that if we change the constructor parameters, etc,
            //we will not have to make changes to the replayer - we will just need to change the IoC configuration.
            var eventType = Type.GetType(eventTypeName);
            var eventHandlersThatHandleOurEvent =
                _container.ComponentRegistry.Registrations
                         .SelectMany(r => r.Services.OfType<IServiceWithType>(), (r, s) => new {r, s})  //search the registered components; should all be of type: Handles<TEvent>
                         .Where(rs => rs.s.ServiceType.GetInterfaces()                                  //get all the interfaces 
                             .Any(i => i.GetGenericArguments()                                          //get all the generic type arguments for each interface
                                 .Any(typeArgs => typeArgs == eventType)))                              //find any that implement the event we are after
                                 .Select(rs => rs.s.ServiceType);                                       //select just the eventhandler type

            //now use this list (of all eventhandlers that handle our event) to get eventhandler instances from the container
            IEnumerable<Handles<IEvent>> eventHandlerInstances = eventHandlersThatHandleOurEvent.Select(handlerType => _container.Resolve(handlerType) as Handles<IEvent>);

            return new MongoEventReplayer(eventType, eventHandlerInstances, _container.Resolve<IStoreEvents>(), _container.Resolve<MongoDatabase>());
        }
    }
}
