using System;
using CQRS.Commands;
using CQRS.Messages.Events;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using MassTransit.Services.Timeout.Messages;
using log4net;

namespace CQRS.Sagas
{
    public class ClaimRequestSaga : SagaStateMachine<ClaimRequestSaga>, ISaga
    {
        static ClaimRequestSaga()
        {
            Define(() =>
                {
                    //if the incomming INITIAL message is CorrelatedBy<Guid> then MT will create the saga using the CorrelatedBy property of the message.
                    //when the message does not implemente CorrelatedBy<Guid> then MT will create the saga and give it a Guid for the correlation ID (via the constructor?).
                    Correlate(ClaimRequestCreated)
                        .By((saga, message) => saga.CorrelationId.ToString() == message.ClaimRequestId)
                        .UseId(message => new Guid(message.ClaimRequestId));

                    Correlate(CardUseVerified)
                        .By((saga, message) =>
                            saga.CorrelationId.ToString() == message.ClaimRequestId);

                    Correlate((ClaimAutoSubstantiated))
                        .By((saga, message) => saga.CorrelationId.ToString() == message.ClaimRequestId)
                        .UseId(message => new Guid(message.ClaimRequestId));

                    //Correlate(TimeoutExpired)
                        //.By((saga, timeout) =>
                            //saga.ClaimRequestId == timeout.CorrelationId);

                    Initially(
                            When(ClaimRequestCreated)
                                .Call((saga, message) => saga.Handle(message))
                                .TransitionTo(WaitingForCardUseVerification),
                            When(ClaimAutoSubstantiated)
                                .Call((saga, message) => saga.Handle(message))
                                .TransitionTo(Completed));

                    During(WaitingForCardUseVerification,
                           When(CardUseVerified)
                               .Call((saga, message) => saga.Handle(message))
                               .Complete(),
                           When(TimeoutExpired)
                               .Call((saga, message) => saga.Handle(message))
                               .Complete());
                });
        }

        public static State Initial { get; set; }
        public static State WaitingForCardUseVerification { get; set; }
        //public static State Substantiated { get; set; }
        public static State Completed { get; set; }

        public static Event<ClaimRequestCreatedPendingVerificationEvent> ClaimRequestCreated { get; set; }
        public static Event<CardUseVerifiedEvent> CardUseVerified { get; set; }
        public static Event<TimeoutExpired> TimeoutExpired { get; set; }
        public static Event<ClaimRequestAutoSubstantiatedEvent> ClaimAutoSubstantiated { get; set; }

        public Guid CorrelationId { get; private set; }
        public Guid ClaimRequestId { get; set; }
        public string ParticipantId { get; set; }
        public IServiceBus Bus { get; set; }

        public ClaimRequestSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public ClaimRequestSaga()
        {
            //empty
        }

        private ILog logger = LogManager.GetLogger(typeof (ClaimRequestSaga));
        private void Handle(ClaimRequestCreatedPendingVerificationEvent @event)
        {
            //send a message to the timeout service to start a timeout
            //CorrelationId = new Guid(@event.ClaimRequestId); //- can not reset the correlation id because the saga has already been created with the original ID and
            //that is how it is persisted in the saga repository. If you reset the ID then the original saga state (in the saga repository) will not be retrieved and 
            //the new ID you just specified does not exist in the saga repository.
            ClaimRequestId = new Guid(@event.ClaimRequestId);
            ParticipantId = @event.ParticipantId;
            var timeoutAt = DateTime.Now.Add(TimeSpan.FromSeconds(30));
            var newTimeout = new ScheduleTimeout(CorrelationId, timeoutAt);
            logger.DebugFormat("ClaimRequestSaga: handle pending verification event: ClaimId [{0}] - CorrrelationId [{1}]", ClaimRequestId, CorrelationId);
            Bus.Publish(newTimeout);
        }

        private void Handle(ClaimRequestAutoSubstantiatedEvent autoSubstantiatedEvent)
        {
            ClaimRequestId = new Guid(autoSubstantiatedEvent.ClaimRequestId);
            ParticipantId = autoSubstantiatedEvent.ParticipantId;
            Bus.Publish(new DisburseClaim
                {
                    ClaimId = ClaimRequestId.ToString(),
                    ParticipantId = ParticipantId,
                });
        }

        private void Handle(CardUseVerifiedEvent cardUseVerified)
        {
            //cancel the timeout request
            var cancelTimeout = new CancelTimeout
                {
                    CorrelationId = CorrelationId,
                };

            Bus.Publish(cancelTimeout);
            //publish a new DisburseClaim command
            Bus.Publish(new DisburseClaim
                {
                    ClaimId = ClaimRequestId.ToString(),
                    ParticipantId = ParticipantId,
                });
        }

        private void Handle(TimeoutExpired timeOutEvent)
        {
            //send a RejectClaimRequest command to the bus
            var reject = new RejectClaimRequest
                {
                    ClaimRequestId = timeOutEvent.CorrelationId.ToString(),
                    Reason = string.Format("Rejected because the participant did not substantiate the claim within the time allowed.")
                };

            Bus.Publish(reject);
        }
    }
}
