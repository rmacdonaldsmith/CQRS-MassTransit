using System;
using System.Configuration;
using Autofac;
using MHM.WinFlexOne.CQRS.EventDispatch.AutofacModules;
using MHM.WinFlexOne.CQRS.Events;
using MHM.WinFlexOne.CQRS.Events.EventHandlers;
using MHM.WinFlexOne.CQRS.Events.EventHandlers.Elections;
using MassTransit;
using MongoDB.Driver;
using log4net;
using log4net.Config;

namespace MHM.WinFlexOne.CQRS.EventDispatch
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger("EventDispatch");
        private static string eventQueue = "rabbitmq://localhost/eventprocessor_queue";
        private static string mongoConnectionName = "mongoConnectionString";
        private static IServiceBus m_bus;
        private static MongoServer m_mongoServer;
        private static MongoDatabase m_mongoDatabase;

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            try
            {
                var builder = new ContainerBuilder();
                
                builder.RegisterModule(new EventHandlersInjectionModule
                    {
                        EventQueueName = eventQueue,
                        MongoConnectionStringName = mongoConnectionName,
                    });

                var container = builder.Build();
                var eventBus = container.Resolve<IServiceBus>();
                Logger.InfoFormat("Event dispatcher is listening on " + eventBus.Endpoint.Address);

                //m_mongoServer = MongoServer.Create(ConfigurationManager.ConnectionStrings[mongoConnectionName].ConnectionString);
                //m_mongoDatabase = m_mongoServer.GetDatabase("wf1_read_model", SafeMode.True);
                //m_bus = WireupEventBus(eventQueue);
                //Logger.InfoFormat("Event dispatcher is listening on {0}", eventQueue);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception in the event processor: ", ex);
            }
        }

        private static IServiceBus WireupEventBus(string eventQueue)
        {
            IServiceBus eventBus = ServiceBusFactory.New(configurator =>
            {
                configurator.UseRabbitMq();
                configurator.ReceiveFrom(eventQueue);
                configurator.UseControlBus();
                configurator.Subscribe(subscription =>
                                           {
                                               subscription.Handler<ElectionMadeEvent>(@event =>
                                                                                              {
                                                                                                  Logger.InfoFormat
                                                                                                      ("Received event from bus: {0}:{1}",
                                                                                                       @event, @event.Id);
                                                                                                  new ElectionCreatedEventHandler(m_mongoDatabase).
                                                                                                      Handle(@event);
                                                                                              });
                                               subscription.Handler<ElectionTerminatedEvent>(@event =>
                                                                                                 {
                                                                                                     Logger.InfoFormat
                                                                                                         ("Received event from bus: {0}:{1}",
                                                                                                          @event, @event.ElectionId);
                                                                                                     new ElectionTerminatedEventHandler(m_mongoDatabase).
                                                                                                         Handle(@event);
                                                                                                 });
                                           });
            });
            return eventBus;
        }
    }
}
