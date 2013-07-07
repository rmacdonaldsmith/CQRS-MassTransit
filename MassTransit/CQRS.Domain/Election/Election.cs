using System;
using System.Collections.Generic;
using CQRS.Commands;
using CQRS.Interfaces.Events;
using CQRS.Interfaces.Services.ReadModel;
using CQRS.Messages.Events;
using Common;

namespace CQRS.Domain.Election
{
    public sealed class Election : AggregateRoot
    {
        private ElectionState _state;

        public Election(ElectionState state)
        {
            _state = state;
        }

        public Election()
        {
            _state = new ElectionState(NoEvents);
        }

        public override Guid Id
        {
            get { return _state.Id; }
        }

        protected internal override void InitializeFromHistory(IEnumerable<IEvent> eventHistory)
        {
            _state = new ElectionState(eventHistory);
        }

        public void MakeAnElection(MakeAnElection command, IBenefitsReadModel benefitsService)
        {
            //The method that handles the command is the ONLY place to do any validation
            //If this method thinks all is well, then applying the change MUST succeed.
            //The Apply(...) method can not fail now - the command has been deemed to be
            //valid and the state transition can now go ahead.

            if (ElectionAmountBreachsLimitForPlanYear(command.ElectionAmount.Dollars(), benefitsService, command.PlanYearBenefitId))
                throw new DomainException(
                    string.Format("The new election amount {0} exceeds the annual limit for the plan year benefit.",
                                  command.ElectionAmount.Dollars()));

            ApplyEvent(new ElectionMadeEvent
                {
                    AdministratorCode = command.AdministratorCode,
                    CompanyCode = command.CompanyCode,
                    ParticipantId = command.ParticipantId,
                    PlanYearBenefitId = command.PlanYearBenefitId,
                    ElectionAmount = command.ElectionAmount,
                    ElectionReason = command.ElectionReason,
                    PerPayPeriodAmount = 0.00m,
                    Id = command.Id,
                },
                @event => _state.Apply(@event));
        }

        public void TerminateElection(TerminateElection command)
        {
            //this would be idempotent - applying the terminate election command twice will have
            //no effect on the system after the first command is applied.
            if (_state.TerminationDate == null)
            {
                ApplyEvent(new ElectionTerminatedEvent
                    {
                        ElectionId = Id.ToString(),
                        TerminatedDate = command.TerminationDate,
                    },
                @event => _state.Apply(@event));
            }
        }

        public void ChangeElectionAmount(ChangeElectionAmount command, IBenefitsReadModel benefitsService)
        {
            //check that the new election amount is not greater than the limit for the plan year benefit.
            //this may require injecting a domain service in to this aggregate so that we 
            //can get the max election amount for this benefit year.

            if (_state.TerminationDate != null)
            {
                throw new DomainException(
                    string.Format("This election was terminated on {0}. You can not change the election amount.",
                                  _state.TerminationDate.Value));
            }

            if (ElectionAmountBreachsLimitForPlanYear(command.NewElectionAmount.Dollars(), benefitsService, command.PlanYearBenefitId))
            {
                throw new DomainException(string.Format("The new election amount exceeds the annual limit for the plan year benefit."));
            }

            ApplyEvent(new ElectionAmountChangedEvent
                {
                    ElectionId = Id.ToString(),
                    NewElectionAmount = command.NewElectionAmount,
                    NewPerPayPeriodAmount = 10,
                    QualifyingEvent = command.QualifyingEvent,
                },
                @event => _state.Apply(@event));
            
        }

        private static bool ElectionAmountBreachsLimitForPlanYear(Money electionAmount, IBenefitsReadModel benefitsService, string planYearBenefitId)
        {
            var planYearBenefit = benefitsService.GetPlanYearBenefit(planYearBenefitId);

            if (planYearBenefit.HasAnnualLimit &&
                electionAmount > planYearBenefit.AnnualLimit.Dollars())
            {
                return true;
            }

            return false;
        }
    }
}
