using System;
using System.Collections.Generic;
using Common;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Events;
using MHM.WinFlexOne.CQRS.Interfaces.Events;

namespace MHM.WinFlexOne.CQRS.Domain.Benefit
{
    public class Benefit : AggregateRoot
    {
        private BenefitState _state;

        public override Guid Id
        {
            get { return _state.Id; }
        }

        public Benefit()
        {
            _state = new BenefitState(new IEvent[0]);
        }

        protected internal override void InitializeFromHistory(IEnumerable<IEvent> eventHistory)
        {
            _state = new BenefitState(eventHistory);
        }

        public void DefineNewBenefit(DefineNewBenefit command)
        {
            ApplyEvent(new NewBenefitDefinedEvent
                {
                    BenefitId = command.BenefitId,
                    BenefitDescription = command.BenefitDescription,
                    BenefitType = command.BenefitType,
                    HasMaxElectionAmount = command.HasMaxAmount,
                    MaxElectionAmount = command.MaxBenefitAmount,
                    PlanId = command.PlanId,
                    CompanyId = command.CompanyId,
                }, 
                @event => _state.Apply(@event));
        }

        public void AssignBenefitToYear(AssignBenefitToYear command)
        {
            if (_state.PlanYearBenefits.Exists(pyb => pyb.PlanYear == command.PlanYear))
            {
                throw new DomainException(string.Format("This benefit is already assigned to the {0} plan year.", command.PlanYear));
            }

            if (command.HasMaxAnnualAmount == false && _state.HasMaxAnnualAmount)
            {
                throw new DomainException(
                    "You are attempting to create a plan year benefit with no limit on the annual amount, but the benefit has a limit on the annual amount.");
            }

            if (command.HasMaxAnnualAmount && _state.HasMaxAnnualAmount &&
                command.MaxAnnualAmount.Dollars() > _state.MaxAnnualAmount)
            {
                throw new DomainException(
                    string.Format(
                        "You are attempting to create a plan year benefit with a maximum amount that is greater than the maximum amount for the benefit."));
            }

            ApplyEvent(new BenefitAssignedToYearEvent
                {
                    BenefitId = command.BenefitId,
                    HasMaxAnnualAmount = command.HasMaxAnnualAmount,
                    MaxAnnualAmount = command.MaxAnnualAmount,
                    PlanYear = command.PlanYear,
                    StartDate = command.StartDate,
                    PlanId = command.PlanId,
                    CompanyId = command.CompanyId,
                    
                },
                @event => _state.Apply(@event));
        }
    }
}
