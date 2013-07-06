using System;
using System.Collections.Generic;
using System.Linq;
using MHM.WinFlexOne.CQRS.Events;
using Magnum.StateMachine;
using MassTransit.Services.Timeout.Messages;
using MassTransit.Testing;
using MassTransit.Testing.TestInstanceConfigurators;
using NUnit.Framework;

namespace CQRS.Sagas.Tests.WithTests
{
    public class when_consuming_CardUseVerifiedEvent_with_saga_in_WaitingForCardUseVerification_state : WithLocalBus<ClaimRequestSaga>
    {
        private readonly Guid _claimRequestId = Guid.NewGuid();
        private readonly Guid _correlationId = Guid.NewGuid();

        protected override IEnumerable<ClaimRequestSaga> Given()
        {
            var saga = CreateSagaInState(_correlationId, ClaimRequestSaga.WaitingForCardUseVerification);
            saga.ClaimRequestId = _claimRequestId;
            yield return saga;
        }

        protected override void When(SagaTestInstanceConfigurator<BusTestScenario, ClaimRequestSaga> sagaConfigurator)
        {
            sagaConfigurator.Send(new CardUseVerifiedEvent()
            {
                ClaimRequestId = _correlationId.ToString(),
                Version = 0,
            });
        }

        [Test]
        public void then_saga_transitions_to_Complete()
        {
            Assert.IsTrue(Test.Saga.Any(s => s.ClaimRequestId == _claimRequestId && s.CurrentState == ClaimRequestSaga.Completed));
        }

        [Test]
        public void then_saga_publishes_a_CancelTimeout_command()
        {
            Assert.AreEqual(1, Test.Scenario.Published.Count(msg => msg.MessageType == typeof (CancelTimeout)));
        }
    }
}
