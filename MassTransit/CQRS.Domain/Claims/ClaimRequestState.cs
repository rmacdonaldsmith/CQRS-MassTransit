using System;
using CQRS.Messages.Events;
using Common;

namespace CQRS.Domain.Claims
{
    public sealed class ClaimRequestState : AggregateStateBase
    {
        private Guid _id;
        private Money _claimAmount;
        private string _claimType;
        private ClaimRequestStateEnum _currentClaimState;

        public ClaimRequestState()
        {
            Register<ClaimRequestSubmittedEvent>(When);
            Register<ClaimRequestAutoSubstantiatedEvent>(When);
            Register<ClaimRequestCreatedPendingVerificationEvent>(When);
            Register<CardUseVerifiedEvent>(When);
            Register<ClaimRequestRejectedEvent>(When);
            Register<ClaimDisbursedEvent>(When);
            Register<ClaimNotDisbursedEvent>(When);
        }

        //public ClaimRequestState(IEnumerable<IEvent> eventHistory)
        //{
        //    foreach (var @event in eventHistory)
        //    {
        //        Apply(@event);
        //    }
        //}

        public override Guid Id
        {
            get { return _id; }
        }

        public Money ClaimAmount
        {
            get { return _claimAmount; }
        }

        public string ClaimType
        {
            get { return _claimType; }
        }

        public ClaimRequestStateEnum CurrentClaimState
        {
            get { return _currentClaimState; }
            set { _currentClaimState = value; }
        }

        private void When(ClaimRequestSubmittedEvent @event)
        {
            _id = new Guid(@event.ClaimRequestId);
            _claimAmount = @event.Amount.Dollars();
            _claimType = @event.ClaimType;
            CurrentClaimState = ClaimRequestStateEnum.Substantiated;
        }

        private void When(ClaimRequestAutoSubstantiatedEvent @event)
        {
            _id = new Guid(@event.ClaimRequestId);
            _claimAmount = @event.Amount.Dollars();
            _claimType = @event.ClaimType;
            CurrentClaimState = ClaimRequestStateEnum.Substantiated;
        }

        private void When(ClaimRequestCreatedPendingVerificationEvent @event)
        {
            _id = new Guid(@event.ClaimRequestId);
            _claimAmount = @event.Amount.Dollars();
            _claimType = @event.ClaimType;
            CurrentClaimState = ClaimRequestStateEnum.PendingSubstantiation;
        }

        private void When(CardUseVerifiedEvent @event)
        {
            CurrentClaimState = ClaimRequestStateEnum.Substantiated;
        }

        private void When(ClaimRequestRejectedEvent @event)
        {
            CurrentClaimState = ClaimRequestStateEnum.Rejected;
        }

        private void When(ClaimDisbursedEvent @event)
        {
            CurrentClaimState = ClaimRequestStateEnum.Disbursed;
        }

        private void When(ClaimNotDisbursedEvent @event)
        {
            CurrentClaimState = ClaimRequestStateEnum.NotDisbursed;
        }
    }
}
