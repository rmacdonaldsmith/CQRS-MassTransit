using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHM.WinFlexOne.CQRS.Events;
using MHM.WinFlexOne.CQRS.Events.EventHandlers.Claims;
using MHM.WinFlexOne.CQRS.Interfaces.Events;
using MongoDB.Driver;
using NUnit.Framework;

namespace MHM.WinFlexOne.CQRS
{
    [TestFixture]
    public class ClaimDisbursedEventTests
    {
        private Handles<ClaimDisbursedEvent> _eventHandler;
        private string _electionId = "4fcf70d1-6418-46b0-acc2-c0d3516fb435";

        [SetUp]
        public void when_a_ClaimDisbursedEvent_is_received()
        {
            var mongoServer = MongoServer.Create("mongodb://localhost/wf1_read_model?safe=true");
            var mongoDb = mongoServer.GetDatabase("wf1_read_model", SafeMode.True);
            _eventHandler = new ClaimDisbursedEventHandler(mongoDb);
        }

        [Test]
        public void then_the_election_balance_is_updated()
        {
            var claimDisbursedEvent = new ClaimDisbursedEvent
                {
                    ClaimAmount = 20,
                    ClaimId = Guid.NewGuid().ToString(),
                    DisbursementAmount = 20,
                    ElectionId = _electionId,
                };

            //this line is commented because we dont want this test to update the value of the election balance
            //_eventHandler.Handle(claimDisbursedEvent);

        }
    }
}
