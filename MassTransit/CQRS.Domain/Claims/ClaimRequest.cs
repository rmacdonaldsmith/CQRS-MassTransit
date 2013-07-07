using System;
using System.Collections.Generic;
using System.Linq;
using CQRS.Commands;
using CQRS.Interfaces.Events;
using CQRS.Interfaces.Services.ReadModel;
using CQRS.Messages.Events;
using Common;
using MHM.WinFlexOne.CQRS.Dtos;

namespace CQRS.Domain.Claims
{
    public enum ClaimRequestStateEnum
    {
        Initial,
        PendingSubstantiation,
        Substantiated,
        Rejected,
        Disbursed,
        NotDisbursed
    }

    //This aggregate is basically a state machine - the aggregate must be in a certain state in order for it to process the relevant command, otherwise
    //it will throw an exception
    public class ClaimRequest : AggregateRoot
    {
        private ClaimRequestState _state;

        public override Guid Id
        {
            get { return _state.Id; }
        }

        public ClaimRequest()
        {
            _state = new ClaimRequestState();
        }

        protected internal override void InitializeFromHistory(IEnumerable<IEvent> eventHistory)
        {
            if(_state == null)
                _state = new ClaimRequestState();

            _state.Initialize(eventHistory);
        }

        public void SubmitClaimRequest(SubmitClaimRequest command, ICompaniesReadModel companyService)
        {
            //if claim amount does not match co-pay amount, then hold pending
            //verification (receive supporting documentation) from the participant.

            IEnumerable<CopayInfo> copayList = companyService.GetCopayBy(command.CompanyId);

            if (copayList.Any(copay => copay.CopayAmount.Dollars() == command.Amount.Dollars() && copay.ClaimType == command.ClaimType))
            {
                //then this is a recognized copay - we dont need any substantiation
                RaiseClaimRequestAutoSubstantiatedEvent(command);
            }
            else
            {
                //else, we need some substantiation from the participant
                RaiseClaimRequestCreatedPendingVerificationEvent(command);
            }
        }

        public void VerifyClaimRequest(VerifyCardUse command)
        {
            if (_state.CurrentClaimState == ClaimRequestStateEnum.PendingSubstantiation)
            {
                RaiseCardUseVerifiedEvent(command.ClaimRequestId);
            }
            else
            {
                throw new DomainException(
                    string.Format(
                        "Received a command to verify card use, but this claim request is not in a state [{0}] to process that command.",
                        _state.CurrentClaimState));
            }
        }

        public void RejectClaimRequest(RejectClaimRequest command)
        {
            if (_state.CurrentClaimState == ClaimRequestStateEnum.PendingSubstantiation)
            {
                RaiseClaimRequestRejectedEvent(command.ClaimRequestId, command.Reason);
            }
            else
            {
                throw new DomainException(
                    string.Format(
                        "Received a command to reject this card use, but this claim request is not in a state [{0}] to process that command.",
                        _state.CurrentClaimState));
            }
        }

        public void DisburseClaim(DisburseClaim command, IElectionsReadModel electionsReadModel)
        {
            var elections = electionsReadModel.GetElectionsForParticipant(command.ParticipantId);
            ElectionDto election = null;
            
            if (TryGetElection(elections, _state.ClaimType, out election))
            {
                var electionBalance = electionsReadModel.GetElectionBalance(election.Id).BalanceRemaining;
                if (electionBalance - _state.ClaimAmount.Amount < 0)
                {
                    RaiseClaimNotDisbursedEvent(command,
                                                string.Format(
                                                    "Your remaining election balance is {0} which is not enough to cover your claim for {1}",
                                                    electionBalance, _state.ClaimAmount.Amount));
                    return;
                }
                RaiseClaimDisbursedEvent(command, election.Id);
            }
            else
            {
                RaiseClaimNotDisbursedEvent(command, string.Format("No election matches the claim type '{0}'", _state.ClaimType));
            }
        }

        private bool TryGetElection(IEnumerable<ElectionDto> elections, string claimTypeToFind, out ElectionDto election)
        {
            bool canFindClaimType = false;
            election = null;

            var electionFound = elections.FirstOrDefault(electionItem => claimTypeToFind.Contains(electionItem.BenefitType));
            if (electionFound != null)
            {
                canFindClaimType = true;
                election = electionFound;
            }

            return canFindClaimType;
        }

        private void RaiseClaimNotDisbursedEvent(DisburseClaim command, string reason)
        {
            var claimNotDisbursedEvent = new ClaimNotDisbursedEvent
            {
                ClaimAmount = _state.ClaimAmount.Amount,
                ClaimId = command.ClaimId,
                ClaimType = _state.ClaimType,
                Reason = reason,
            };
            ApplyEvent(claimNotDisbursedEvent, @event => _state.Apply(@event));
        }

        private void RaiseClaimDisbursedEvent(DisburseClaim command, string electionId)
        {
            var claimDisbursed = new ClaimDisbursedEvent
                {
                    ClaimAmount = _state.ClaimAmount.Amount,
                    ClaimId = command.ClaimId,
                    DisbursementAmount = _state.ClaimAmount.Amount,
                    ElectionId = electionId,
                };
            ApplyEvent(claimDisbursed, @event => _state.Apply(@event));
        }

        private void RaiseClaimRequestAutoSubstantiatedEvent(SubmitClaimRequest command)
        {
            var claimAutoSubstantiated = new ClaimRequestAutoSubstantiatedEvent
            {
                Amount = command.Amount,
                ClaimRequestId = command.ClaimRequestId,
                ClaimType = command.ClaimType,
                CompanyId = command.CompanyId,
                DateOfTransaction = command.DateOfTransaction,
                ParticipantId = command.ParticipantId,
                ProviderName = command.ProviderName,
                Source = command.Source,
            };
            ApplyEvent(claimAutoSubstantiated, @event => _state.Apply(@event));
        }

        private void RaiseCardUseVerifiedEvent(string claimRequestId)
        {
            var cardUseVerifiedEvent = new CardUseVerifiedEvent
                {
                    ClaimRequestId = claimRequestId,
                };
            ApplyEvent(cardUseVerifiedEvent, @event => _state.Apply(@event));
        }

        private void RaiseClaimRequestCreatedPendingVerificationEvent(SubmitClaimRequest command)
        {
            var pendingEvent = new ClaimRequestCreatedPendingVerificationEvent
                {
                    Amount = command.Amount,
                    ClaimRequestId = command.ClaimRequestId,
                    ClaimType = command.ClaimType,
                    DateOfTransaction = command.DateOfTransaction,
                    ProviderName = command.ProviderName,
                    Reason = "Claim request amount is not recognised as a copay amount. Participant must substantiate this card use",
                    Source = command.Source,
                    CompanyId = command.CompanyId,
                    ParticipantId = command.ParticipantId,
                    CorrelationId = Guid.NewGuid(),
                };
            ApplyEvent(pendingEvent, @event => _state.Apply(@event));
        }

        private void RaiseClaimRequestRejectedEvent(string claimRequestId, string rejectReason)
        {
            var rejectedEvent = new ClaimRequestRejectedEvent
                {
                    ClaimRequestId = claimRequestId,
                    RejectReason = rejectReason,
                };
            ApplyEvent(rejectedEvent, @event => _state.Apply(@event));
        }
    }
}
