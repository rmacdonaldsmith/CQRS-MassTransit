using System;
using Autofac;
using MassTransit;
using log4net;
using log4net.Config;

namespace MHM.WinFlexOne.CQRS.CommandDispatch
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger("CommandDispatch");

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            IContainer container = null;

            try
            {
                //var bootStrapper = new CommandDispatchBootStrapper();
                //var commandBus = bootStrapper.ConfigureCommandBus();
                //var eventBus = bootStrapper.ConfigureEventBus();

                var builder = new ContainerBuilder();
                var eventStoreInjectionModule = new EventStoreInjectionModule
                    {
                        PersistenceType = EventStorePersistenceEnum.MongoDb,
                    };

                var massTransitModule = new MassTransitInjectionModule
                    {
                        CommandQueueName = "rabbitmq://localhost/commandprocessor_queue",
                        EventQueueName = "rabbitmq://localhost/eventprocessor_queue",
                    };

                var commandHandlerModule = new CommandHandlersInjectionModule
                    {
                        MongoConnectionStringName = "mongoReadModelConnectionString",
                    };

                builder.RegisterModule(eventStoreInjectionModule);
                builder.RegisterModule(massTransitModule);
                builder.RegisterModule(commandHandlerModule);
                //builder.RegisterModule<LogInjectionModule>();

                container = builder.Build();
                var commandBus = container.ResolveKeyed<IServiceBus>("CommandBus");
                var eventBus = container.ResolveKeyed<IServiceBus>("EventBus");

                Console.WriteLine("Command listener initialized:");
                Console.WriteLine("Listening for commands on " + commandBus.Endpoint.Address);
                Console.WriteLine("Publishing events on " + eventBus.Endpoint.Address);
                Console.WriteLine();

                Console.ReadLine();
                container.Dispose();
                commandBus.Dispose();
                eventBus.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error("There was an error while initialising the command dispatch.", ex);
                throw;
            }
            finally
            {
                if (container != null)  container.Dispose();
            }
        }
    }
}
