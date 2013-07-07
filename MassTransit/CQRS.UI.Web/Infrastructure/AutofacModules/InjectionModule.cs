using System.Configuration;
using Autofac;
using CQRS.Common.Client;
using CQRS.Interfaces.Services.ReadModel;
using CQRS.ReadModel.Client;
using MassTransit;
using MongoDB.Driver;

namespace CQRS.UI.Web.Infrastructure.AutofacModules
{
    public class InjectionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //command sender
            builder
                .RegisterType<CommandSender>()
                .As<ISendCommandsAndWaitForAResponse>()
                .PropertiesAutowired();

            //masstransit
            builder
                .Register(context =>
                    {
                        var bus = ServiceBusFactory.New(configurator =>
                            {
                                configurator.UseRabbitMq();
                                configurator.ReceiveFrom("rabbitmq://localhost/efo_command");
                            });
                        return bus;
                    })
                .As<IServiceBus>();

            //mongodb
            builder.Register(context =>
            {
                var mongoServer = MongoServer.Create(ConfigurationManager.ConnectionStrings["mongoConnectionString"].ConnectionString);
                return mongoServer.GetDatabase("wf1_read_model", SafeMode.True);
            })
                .AsSelf()
                .SingleInstance();

            //read model sevices
            builder
                .RegisterType<BenefitsReadModel>()
                .WithParameter((info, context) => info.Name == "baseUrl",
                               (info, context) => "http://localhost:3000/benefits")
                .As<IBenefitsReadModel>();

            builder
                .RegisterType<CompaniesReadModel>()
                .WithParameter((info, context) => info.Name == "baseUrl",
                               (info, context) => "http://localhost:3000/companies")
                .As<ICompaniesReadModel>();

            builder
                .RegisterType<ClaimsReadModel>()
                .WithParameter((info, context) => info.Name == "baseUrl",
                               (info, context) => "http://localhost:3000/claims")
                .As<IClaimsReadModel>();
        }
    }
}