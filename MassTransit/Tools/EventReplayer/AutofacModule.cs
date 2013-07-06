using System.Configuration;
using System.Linq;
using System.Reflection;
using Autofac;
using Common;
using EventStore;
using EventStore.Logging.Log4Net;
using EventStore.Serialization;
using MHM.WinFlexOne.CQRS.Events;
using MHM.WinFlexOne.CQRS.Events.EventHandlers.Elections;
using MHM.WinFlexOne.CQRS.Interfaces.Events;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Module = Autofac.Module;

namespace EventReplayer
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //mongodb read model
            builder.Register(context =>
                {
                    var mongoServer =
                        MongoServer.Create(
                            ConfigurationManager.ConnectionStrings["mongoReadModelConnectionString"].ConnectionString);
                    return mongoServer.GetDatabase("wf1_read_model", SafeMode.True);
                })
                   .AsSelf()
                   .SingleInstance();

            //eventstore
            builder.Register(context => Wireup.Init()
                                              //.LogTo(type => new EventStoreLog4NetLogger(type))
                                              .LogTo(type => new Log4NetLogger(type)) //- need to get assembly redirection working
                                              .UsingMongoPersistence("mongoEventStoreConnectionString",
                                                                     new DocumentObjectSerializer())
                                              .Build())
                   .As<IStoreEvents>()
                   .SingleInstance();

            //do assembly scanning for all event handlers
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(ElectionCreatedEventHandler)))
                   .Where(t => t.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(Handles<>))))
                   .AsImplementedInterfaces();

            RegisterEventTypesWithMongo();
        }

        private static void RegisterEventTypesWithMongo()
        {
            var types = Assembly.GetAssembly(typeof(ElectionMadeEvent))
                                .GetTypes()
                                .Where(t => t.Implements<IEvent>());

            foreach (var t in types)
                BsonClassMap.LookupClassMap(t);
        }
    }
}
