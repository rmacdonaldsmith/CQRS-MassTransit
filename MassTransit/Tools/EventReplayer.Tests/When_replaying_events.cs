using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using NUnit.Framework;

namespace EventReplayer.Tests
{
    [TestFixture]
    public class When_replaying_events
    {
        private MongoEventReplayer _replayer = null;

        [SetUp]
        public void Given_we_have_replayed_a_set_of_events()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacModule());
            IContainer container = builder.Build();

            _replayer = new EventReplayerFactory(container).Build("MHM.WinFlexOne.CQRS.Events.EventHandlers.Elections.ElectionAmountChangedEventHandler, MHM.WinFlexOne.CQRS.Events");

            _replayer.Replay();
        }

        [Test]
        public void Then_the_target_eventhandlers_should_update_the_readmodel()
        {

        }

        [Test]
        public void Then_an_error_should_be_thrown_when_there_are_too_many_arguments()
        {

        }
    }
}
