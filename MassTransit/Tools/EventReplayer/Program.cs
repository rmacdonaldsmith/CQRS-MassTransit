using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Core;
using EventStore;
using MongoDB.Driver;
using NDesk.Options;

namespace EventReplayer
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string eventTypeName = string.Empty;
            string eventHandlerType = string.Empty;
            bool showHelp = false;

            var options = new OptionSet()
                {
                    { "e|event=", "the {EVENT TYPE} to target for replay from the event store.", val => eventTypeName = val },
                    { "eh|eventhandler=", "the {EVENT HANDLER} that will handle the event stream.", val => eventHandlerType = val },
                    { "h|help", "show this help message and exit.", val => showHelp = (val != null) }
                };

            List<string> extra;
            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException oex)
            {
                Console.Write("replayevents: ");
                Console.WriteLine(oex.Message);
                Console.WriteLine("Try 'replayevents --help' for more information.");
                return;
            }

            if (showHelp)
            {
                ShowHelp();
                return;
            }

            if (extra.Count > 0)
            {
                ShowHelp();
                return;
            }
            
            //build the replayer
            var builder = new ContainerBuilder();
            IContainer container = builder.Build();

            var eventType = Type.GetType(eventTypeName);
            var eventHandlersThatHandleOurEvent =
                container.ComponentRegistry.Registrations
                         .SelectMany(r => r.Services.OfType<IServiceWithType>(), (r, s) => new {r, s})  //search the registered components; should all be Handles<TEvent>
                         .Where(rs => rs.s.ServiceType.GetInterfaces()                                  //get all the interfaces 
                             .Any(i => i.GetGenericArguments()                                          //get all the generic type arguments 
                                 .Any(typeArgs => typeArgs == eventType)))                              //find any that implement the event we are after
                                 .Select(rs => rs.s.ServiceType);                                       //select just the eventhandler type

            //now use this list to query eventhandler instances from the container
            IEnumerable<object> eventHandlerInstances = eventHandlersThatHandleOurEvent.Select(handlerType => container.Resolve(handlerType));

            //call the event replayer
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage: replayevents event eventhandlers");
            Console.WriteLine("Replays the stream of events of type 'event' through the specified 'eventhandlers'.");
            Console.WriteLine("Primarily this utility can be used to rebuild a read model projection.");
            Console.WriteLine("version: 0.0.0.1");
        }
    }
}
