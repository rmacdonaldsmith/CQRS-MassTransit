using System;
using System.Linq;
using System.Threading;
using Autofac;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Domain.Election;
using Magnum.Extensions;
using MassTransit;
using MassTransit.Testing;
using NUnit.Framework;

namespace MHM.WinFlexOne.CQRS.AutoFacModuleTests
{
    [TestFixture]
    public class CommandHandlersModuleTests
    {
        [Test]
        public void commandhandlers_are_registered_from_autofac()
        {
            var builder = new ContainerBuilder();
            //builder.RegisterModule(new CommandHandlersInjectionModule());
            builder
                .RegisterAssemblyTypes(typeof (MakeAnElectionCommandHandler).Assembly)
                .Where(type => type.Implements<IConsumer>())
                .AsSelf();
            var container = builder.Build();

            var localBus = ServiceBusFactory.New(configurator =>
                                                     {
                                                         //configurator.ReceiveFrom("loopback://localhost/testqueue");
                                                         configurator.ReceiveFrom("rabbitmq://localhost/commandqueue");
                                                         configurator.UseRabbitMq();
                                                         configurator.Subscribe(sbc => sbc.LoadFrom(container));
                                                     });

            localBus.Publish(new MakeAnElection());
            localBus.Publish(new TerminateElection());
            localBus.AutoDispose().ElementAt(0).Dispose(); //prevents unhandled ThreadAbortedException from reaching Resharper test runner

            Assert.AreEqual(1, localBus.HasSubscription<MakeAnElection>().Count());
            Assert.AreEqual(1, localBus.HasSubscription<TerminateElection>().Count());
            Assert.IsTrue(container.IsRegistered<MakeAnElectionCommandHandler>());
            Assert.IsTrue(container.IsRegistered<TerminateElectionCommandHandler>());
        }

        [Test]
        public void Send_message_to_remote_queue()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new CommandHandlersInjectionModule());
            builder.RegisterModule(new EventStoreInjectionModule{PersistenceType = EventStorePersistenceEnum.InMemory});
            builder.RegisterModule(new MassTransitInjectionModule
                {
                    CommandQueueName = "rabbitmq://localhost/commandqueue",
                    EventQueueName = "rabbitmq://localhost/event_queue",
                });

            IServiceBus commandbus;
            IServiceBus eventBus;

            using (var container = builder.Build())
            {
                //var builder2 = new ContainerBuilder();
                //builder2.RegisterModule(new MassTransitInjectionModule
                //    {
                //        CommandQueueName = "rabbitmq://localhost/commandqueue",
                //        EventQueueName = "rabbitmq://localhost/event_queue",
                //    });
                //builder2.Update(container);

                commandbus = container.ResolveNamed<IServiceBus>("CommandBus");
                eventBus = container.ResolveNamed<IServiceBus>("EventBus");

                Assert.IsNotNull(commandbus);
                Assert.IsNotNull(eventBus);
                //Assert.AreEqual(1, commandbus.HasSubscription<MakeAnElection>().Count());
                //Assert.AreEqual(1, commandbus.HasSubscription<TerminateElection>().Count());
                Assert.IsTrue(container.IsRegistered<MakeAnElectionCommandHandler>());
                Assert.IsTrue(container.IsRegistered<TerminateElectionCommandHandler>());
            }

            var resetEvent = new ManualResetEvent(false);

            eventBus.SubscribeHandler<MakeAnElection>(election => resetEvent.Set());
            commandbus.Publish(new MakeAnElection
                                   {
                                       Id = Guid.NewGuid().ToString(),
                                   });

            var waitResult = resetEvent.WaitOne(5.Seconds());

            Assert.IsTrue(waitResult);
        }
    }
}
