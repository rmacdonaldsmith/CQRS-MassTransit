using System.Linq;
using System.Reflection;
using Autofac;
using CQRS.Common;
using CQRS.Common.EventPersistence;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;
using EventStore;
using EventStore.Dispatcher;
using EventStore.Logging.Log4Net;
using EventStore.Serialization;
using MassTransit;
using MongoDB.Bson.Serialization;
using log4net;
using Module = Autofac.Module;

namespace CQRS.CommandDispatch
{
    public class EventStoreInjectionModule : Module
    {
        public EventStorePersistenceEnum PersistenceType { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(context =>
                          new DispatchCommitsToMassTransit(context.ResolveNamed<IServiceBus>("EventBus"),
                                                           LogManager.GetLogger(typeof(DispatchCommitsToMassTransit))))
                .As<IDispatchCommits>();

            if (PersistenceType == EventStorePersistenceEnum.InMemory)
            {
                builder
                    .Register(context => new ConcreteEventStore(
                                             Wireup.Init()
                                                   .UsingInMemoryPersistence()
                                                   .InitializeStorageEngine()
                                                   .UsingSynchronousDispatchScheduler()
                                                   .Build()))
                    .As<IEventStore>()
                    .SingleInstance();
            }
            else if(PersistenceType == EventStorePersistenceEnum.MongoDb)
            {
                builder.Register(context => new ConcreteEventStore(
                                                Wireup.Init()
                                                      .LogTo(type => new EventStoreLog4NetLogger(type))
                                                      //.LogTo(type => new Log4NetLogger(type)) - need to get assembly redirection working
                                                      .UsingMongoPersistence("mongoConnectionString", new DocumentObjectSerializer())
                                                      .InitializeStorageEngine()
                                                      .UsingSynchronousDispatchScheduler()
                                                      .DispatchTo(context.Resolve<IDispatchCommits>())
                                                      .Build()))
                       .As<IEventStore>()
                       .SingleInstance();

                RegisterEventTypesWithMongo();
            }
            else if (PersistenceType == EventStorePersistenceEnum.SqlServer)
            {
                builder.Register(context => new ConcreteEventStore(
                                                Wireup.Init()
                                                      //.LogTo(type => new EventStoreLog4NetLogger(type))
                                                      .LogTo(type => new Log4NetLogger(type))
                                                      .UsingSqlPersistence("sqlEventStoreConnectionString")
                                                      .InitializeStorageEngine()
                                                      .UsingSynchronousDispatchScheduler()
                                                      .DispatchTo(context.Resolve<IDispatchCommits>())
                                                      .Build()))
                       .As<IEventStore>()
                       .SingleInstance();
            }           
        }

        //We are using Mongo as the backing store to our event store - this may change.
        //The Bson Serializer needs to know about the types that it is going to de/serialize.
        //This code is based on stackoverflow question: http://stackoverflow.com/questions/7451422/unknown-discriminator-value-myevent
        private static void RegisterEventTypesWithMongo()
        {
            var types = Assembly.GetAssembly(typeof(ElectionMadeEvent))
                                .GetTypes()
                                .Where(t => t.GetInterfaces().Contains(typeof(IEvent)));

            foreach (var t in types)
                BsonClassMap.LookupClassMap(t);
        }
    }
}