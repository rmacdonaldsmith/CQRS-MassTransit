using System;
using System.Collections.Generic;
using System.Threading;
using CQRS.Commands;
using CQRS.Common;
using CQRS.DomainTesting;
using CQRS.Interfaces.Events;
using NUnit.Framework;

namespace CQRS.TimeoutService.Tests
{
    [TestFixture]
    public class TimeoutRegistryTests
    {
        [Test]
        public void registering_a_new_timeout()
        {
            Guid correlationId = Guid.NewGuid();
            int timeoutPeriodMS = 500;
            var waitHandle = new ManualResetEvent(false);

            IEventStore eventStore = new FakeEventStore(new List<IEvent>());
            var timeoutRegistry = new TimeoutRegistry(eventStore, @event => { });

            var newTimeoutCommand = new StartTimeout
                {
                    CorrelationId = correlationId.ToString(),
                    ElapsesInMS = timeoutPeriodMS,
                };


        }
    }
}
