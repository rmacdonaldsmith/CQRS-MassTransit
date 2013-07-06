using MassTransit;

namespace CQRS.UI.Web.Nancy.Services
{
    public class MassTransitBusFactory : IServiceBusFactory<IServiceBus>
    {
        private const string CommandQueueName = "rabbitmq://localhost/commandqueue";

        public IServiceBus Get()
        {
            var bus = ServiceBusFactory.New(configurator =>
            {
                configurator.UseRabbitMq();
                configurator.ReceiveFrom(CommandQueueName);
            });

            return bus;
        }
    }
}