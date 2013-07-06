using Autofac;
using MassTransit;
using NUnit.Framework;

namespace MHM.WinFlexOne.CQRS.AutoFacModuleTests
{
    [TestFixture]
    public class MassTransitModuleTests
    {
        [Test]
        public void Run()
        {
            string commandBusName = "rabbitmq://localhost/commandqueue";
            string eventBusName = "rabbitmq://localhost/event_queue";

            var builder = new ContainerBuilder();
            
            builder.RegisterModule(new MassTransitInjectionModule
                                       {
                                           CommandQueueName = commandBusName,
                                           EventQueueName = eventBusName,
                                       });
            var container = builder.Build();

            var commandBus = container.ResolveNamed<IServiceBus>("CommandBus");
            var eventBus = container.ResolveNamed<IServiceBus>("EventBus");

            Assert.IsNotNull(commandBus);
            Assert.IsNotNull(eventBus);
        }
    }
}
