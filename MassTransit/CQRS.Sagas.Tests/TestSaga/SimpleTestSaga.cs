using System;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using MassTransit.Services.Timeout.Messages;

namespace CQRS.Sagas.Tests.TestSaga
{
    public class SimpleTestSaga : SagaStateMachine<SimpleTestSaga>, ISaga
    {
        static SimpleTestSaga()
        {
            Define(Saga);
        }

        private static void Saga()
        {
            Correlate(TimeoutScheduled)
                .By((saga, message) => saga.CorrelationId == message.CorrelationId);

            Initially(
                When(Started)
                    .Then((saga, message) =>
                        {
                            saga.MessageId = message.MessageId;
                            saga.Amount = message.Amount;
                        })
                    .TransitionTo(Waiting));

            During(Waiting,
                   When(TimeoutScheduled)
                       .Then((saga, message) => saga.TimeoutAt = message.TimeoutAt)
                       .Complete());
        }

        public SimpleTestSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        protected SimpleTestSaga()
        {
            //empty
        }

        public static State Initial { get; set; }
        public static State Waiting { get; set; }
        public static State Completed { get; set; }

        public static Event<StartMessage> Started { get; set; }
        public static Event<TimeoutScheduled> TimeoutScheduled { get; set; }

        public Guid CorrelationId { get; set; }
        public IServiceBus Bus { get; set; }

        public string MessageId { get; set; }
        public DateTime TimeoutAt { get; set; }
        public decimal Amount { get; set; }
    }
}
