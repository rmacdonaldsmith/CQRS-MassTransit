using Autofac;
using CQRS.Sagas;
using MassTransit;
using MassTransit.Log4NetIntegration;
using MassTransit.Saga;

namespace MHM.WinFlexOne.CQRS.CommandDispatch
{
    public class MassTransitInjectionModule : Module
    {
        public string CommandQueueName { get; set; }
        public string EventQueueName { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            //command bus
            builder
                .Register(context =>
                    {
                        var bus = ServiceBusFactory.New(configurator =>
                            {
                                configurator.UseLog4Net();
                                configurator.UseRabbitMq();
                                configurator.ReceiveFrom(CommandQueueName);
                                //configurator.UseControlBus();
                                configurator.Subscribe(s => s.LoadFrom(context.Resolve<ILifetimeScope>()));
                            });
                        bus.ControlBus.SubscribeSaga(new InMemorySagaRepository<ClaimRequestSaga>());
                        return bus;
                    })
                .Keyed<IServiceBus>("CommandBus")
                .SingleInstance();

            //event bus
            builder
                .Register(context =>
                          ServiceBusFactory.New(configurator =>
                                                    {
                                                        configurator.UseLog4Net();
                                                        configurator.UseRabbitMq();
                                                        configurator.ReceiveFrom(EventQueueName);
                                                        //configurator.UseControlBus();
                                                    }))
                .Keyed<IServiceBus>("EventBus")
                .SingleInstance();
        }
    }
}
