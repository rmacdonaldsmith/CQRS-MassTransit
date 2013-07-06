using Autofac;
using CQRS.ReadModel.Client;
using MHM.WinFlexOne.CQRS.Client;
using MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel;
using MassTransit;
using MongoDB.Driver;

namespace CQRS.UI.Web.MFO.AutofacModules
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
                                configurator.ReceiveFrom("rabbitmq://localhost/mfo_command");
                            });
                        return bus;
                    })
                .As<IServiceBus>();

            //mongodb
            builder.Register(context =>
            {
                var mongoServer = MongoServer.Create("mongodb://localhost/wf1_read_model?safe=true");
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

            builder.RegisterType<ElectionsReadModel>()
                .WithParameter((info, context) => info.Name == "baseUrl",
                               (info, context) => "http://localhost:3000/elections")
                .As<IElectionsReadModel>();

            builder.RegisterType<ClaimTypesReadModel>()
                   .WithParameter((info, context) => info.Name == "baseUrl",
                                  (info, context) => "http://localhost:3000/claimtypes")
                   .As<IClaimTypesReadModel>();

            builder.RegisterType<ClaimsReadModel>()
                   .WithParameter((info, context) => info.Name == "baseUrl",
                                  (info, context) => "http://localhost:3000/claims")
                   .As<IClaimsReadModel>();
        }
    }
}