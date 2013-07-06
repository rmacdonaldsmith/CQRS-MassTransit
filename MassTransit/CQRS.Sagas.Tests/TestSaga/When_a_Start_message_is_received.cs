using System;
using Magnum.TestFramework;

namespace CQRS.Sagas.Tests.TestSaga
{
    [Scenario]
    public class When_a_Start_message_is_received :
        Given_a_simple_saga_does_not_exist
    {
        protected readonly Guid _claimId = Guid.NewGuid();
        protected readonly decimal _amount = 120;

        [When]
        public void A_ClaimSubmitted_message_is_received()
        {
            Message = new StartMessage
                {
                    CorrelationId = SagaId,
                    MessageId = _claimId.ToString(),
                    Amount = _amount
                };

            LocalBus.Publish(Message);
        }

        protected StartMessage Message { get; set; }

        [Then]
        public void A_new_saga_should_be_created()
        {
            Saga.ShouldNotBeNull();
        }

        [Then]
        public void The_claim_id_should_be_set()
        {
            Saga.MessageId.ShouldEqual(_claimId.ToString());
            Saga.Amount.ShouldEqual(_amount);
        }
    }
}
