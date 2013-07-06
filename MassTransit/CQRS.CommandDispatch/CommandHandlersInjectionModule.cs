using System.Configuration;
using Autofac;
using CQRS.ReadModel.Client;
using MHM.WinFlexOne.CQRS.Domain.Election;
using MHM.WinFlexOne.CQRS.Domain.Repositories;
using MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel;
using Magnum.Extensions;
using MassTransit;
using MongoDB.Driver;

namespace MHM.WinFlexOne.CQRS.CommandDispatch
{
    public class CommandHandlersInjectionModule : Module
    {
        public string MongoConnectionStringName { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType(typeof(Repository<>))
                .AsImplementedInterfaces();

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

            builder
                .RegisterAssemblyTypes(typeof (MakeAnElectionCommandHandler).Assembly)
                .Where(type => type.Implements(typeof (IConsumer)))
                .AsSelf();

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