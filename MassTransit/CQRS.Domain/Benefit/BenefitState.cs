using System;
using System.Collections.Generic;
using Common;
using MHM.WinFlexOne.CQRS.Events;
using MHM.WinFlexOne.CQRS.Interfaces.Events;

namespace MHM.WinFlexOne.CQRS.Domain.Benefit
{
    public class BenefitState : AggregateStateBase
    {
        private Guid _id;
        private readonly List<PlanYearBenefit> _planYearBenefits = new List<PlanYearBenefit>();
        private bool _hasMaxAnnualAmount = false;
        private Money _maxAnnualAmount; 

        public override Guid Id
        {
            get { return _id; }
        }

        public List<PlanYearBenefit> PlanYearBenefits
        {
            get { return _planYearBenefits; }
        }

        public bool HasMaxAnnualAmount
        {
            get { return _hasMaxAnnualAmount; }
        }

        public Money MaxAnnualAmount
        {
            get { return _maxAnnualAmount; }
        }

        public BenefitState(IEnumerable<IEvent> eventHistory)
        {
            foreach (var @event in eventHistory)
            {
                Apply(@event);
            }
        }

        public void When(NewBenefitDefinedEvent @event)
        {
            _id = Guid.Parse(@event.BenefitId);
            _hasMaxAnnualAmount = @event.HasMaxElectionAmount;
            _maxAnnualAmount = @event.MaxElectionAmount.Dollars();
        }

        public void When(BenefitAssignedToYearEvent @event)
        {
            _planYearBenefits.Add(new PlanYearBenefit
                {
                    MaxAnnualAmount = @event.MaxAnnualAmount.Dollars(),
                    HasAnnualLimit = @event.HasMaxAnnualAmount,
                    PlanYear = @event.PlanYear,
                    StartDate = @event.StartDate,
                });
        }
    }
}
