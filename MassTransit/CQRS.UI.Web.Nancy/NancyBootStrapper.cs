using CQRS.UI.Web.Nancy.Services;
using MassTransit;
using Nancy;

namespace CQRS.UI.Web.Nancy
{
    public class CustomBootStrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(global::Nancy.TinyIoc.TinyIoCContainer container)
        {
            //we need to do this in order to get Auto Registration working.
            //AutoRegistration: Nancy uses TinyIoC as the IoC container by default. Nancy will tell TinyIoC to scan
            //any Module constructors that take interface arguments and then scan for any
            //classes that implement that interface and automatically inject that class in to 
            //the constructor. TinyIoC is limitedby the fact that it will bind the first implementation 
            //of the interface that it comes accross - so if you have more than one implmentation
            //you may need to do some leg work - there may be a way around this, but not that I am 
            //aware of at the moment.
            //container.Register<IServiceBusFactory<IServiceBus>>((cContainer, overloads) => new MassTransitBusFactory());
            //container
            //    .Register<IServiceBus>((cContainer, overloads) =>
            //                                ServiceBusFactory.New(configurator =>
            //                                                          {
            //                                                              configurator.UseRabbitMq();
            //                                                              configurator.ReceiveFrom("rabbitmq://localhost/command_request_queue");
            //                                                          }));
            base.ConfigureApplicationContainer(container);
        }
    }
}