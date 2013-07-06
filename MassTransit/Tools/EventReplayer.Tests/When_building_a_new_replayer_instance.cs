using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using NUnit.Framework;

namespace EventReplayer.Tests
{
    [TestFixture]
    public class When_building_a_new_replayer_instance
    {
        private MongoEventReplayer _replayer;

        [SetUp]
        public void Given()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacModule());
            IContainer container = builder.Build();

            var factory = new EventReplayerFactory(container);
            _replayer = factory.Build("MHM.WinFlexOne.CQRS.Events.EventHandlers.Elections.ElectionAmountChangedEventHandler, MHM.WinFlexOne.CQRS.Events");
        }

        [Test]
        public void then_the_replayer_instance_is_created()
        {
            Assert.IsNotNull(_replayer);
        }

        [Test]
        public void then_the_replayer_contains_the_correct_eventhandlers()
        {

        }
    }
}
