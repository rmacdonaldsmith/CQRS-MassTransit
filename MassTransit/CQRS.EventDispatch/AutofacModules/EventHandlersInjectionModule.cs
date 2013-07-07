using System.Configuration;
using Autofac;
using CQRS.Events.EventHandlers.Elections;
using Magnum.Extensions;
using MassTransit;
using MassTransit.Log4NetIntegration;
using MongoDB.Driver;

namespace CQRS.EventDispatch.AutofacModules
{
    public class EventHandlersInjectionModule : Module
    {
        public string EventQueueName { get; set; }
        public string MongoConnectionStringName { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            //scan assemblies to find all event consumers
            builder
                .RegisterAssemblyTypes(typeof (ElectionCreatedEventHandler).Assembly)
                .Where(type => type.Implements(typeof (IConsumer)))
                .AsSelf();

            //event bus
            builder
                .Register(context =>
                          ServiceBusFactory.New(configurator =>
                          {
                              configurator.UseLog4Net();
                              configurator.UseRabbitMq();
                              configurator.ReceiveFrom(EventQueueName);
                              //configurator.UseControlBus();
                              configurator.Subscribe(subscription => subscription.LoadFrom(context.Resolve<ILifetimeScope>()));
                          }))
                .As<IServiceBus>()
                .SingleInstance();

            //mongodb
            builder.Register(context =>
                {
                    var mongoServer = MongoServer.Create(ConfigurationManager.ConnectionStrings[MongoConnectionStringName].ConnectionString);
                    return mongoServer.GetDatabase("wf1_read_model", SafeMode.True);
                })
                .AsSelf()
                .SingleInstance();
            
        }
    }
}