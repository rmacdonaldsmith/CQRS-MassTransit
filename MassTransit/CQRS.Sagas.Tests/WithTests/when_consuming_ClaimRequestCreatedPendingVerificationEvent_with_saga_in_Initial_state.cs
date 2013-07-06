using System;
using System.Collections.Generic;
using System.Linq;
using MHM.WinFlexOne.CQRS.Events;
using MassTransit.Services.Timeout.Messages;
using MassTransit.Testing;
using MassTransit.Testing.TestInstanceConfigurators;
using NUnit.Framework;

namespace CQRS.Sagas.Tests.WithTests
{
    [TestFixture]
    public class when_consuming_ClaimRequestCreatedPendingVerificationEvent_with_saga_in_Initial_state : WithLocalBus<ClaimRequestSaga>
    {
        private readonly Guid _claimRequestId = Guid.NewGuid();
        private readonly Guid _participantId = Guid.NewGuid();
        private readonly Guid _correlationId = Guid.NewGuid();

        protected override IEnumerable<ClaimRequestSaga> Given()
        {
            var saga = CreateSagaInState(_correlationId, ClaimRequestSaga.WaitingForCardUseVerification);
            //There is no saga yet - needs to be created by the initiating message: ClaimRequestCreatedPendingVerificationEvent
            yield return saga;
        }

        protected override void When(SagaTestInstanceConfigurator<BusTestScenario, ClaimRequestSaga> sagaConfigurator)
        {
            sagaConfigurator.Send(new ClaimRequestCreatedPendingVerificationEvent()
            {
                CorrelationId = _correlationId,
                ClaimRequestId = _correlationId.ToString(),
                ParticipantId = _participantId.ToString(),
                ClaimType = "claim type",
                Amount = 1000,
                CompanyId = "cocode",
                DateOfTransaction = DateTime.Now,
                ProviderName = "provider name",
                Reason = "reason",
                Source = "test",
                Version = 0,
            });
        }



        [Test]
        public void then_saga_emits_a_ScheduleTimeout_command()
        {
            Assert.IsTrue(Test.Saga.Any(s => s.CorrelationId == _correlationId));
            //var saga = Test.Saga.Where(s => s.Saga.CorrelationId == _correlationId).Select(s => s.Saga).FirstOrDefault();
            //var msgContext = Test.Scenario.Published.FirstOrDefault(message => message.MessageType == typeof(ScheduleTimeout));
            Assert.AreEqual(1, Test.Scenario.Published.Count(message => message.MessageType == typeof(ScheduleTimeout) &&
                ((MassTransit.Context.SendContext<MassTransit.Services.Timeout.Messages.ScheduleTimeout>)(message.Context)).Message.CorrelationId == _correlationId));
        }

        [Test]
        public void then_transitions_to_WaitingForCardUseVerification_state()
        {
            Assert.IsTrue(Test.Saga.Any(s => s.CorrelationId == _correlationId && s.CurrentState == ClaimRequestSaga.WaitingForCardUseVerification));
        }
    }
}
