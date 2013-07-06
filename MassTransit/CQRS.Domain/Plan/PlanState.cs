using System;
using System.Collections.Generic;
using MHM.WinFlexOne.CQRS.Events;
using MHM.WinFlexOne.CQRS.Interfaces.Events;

namespace MHM.WinFlexOne.CQRS.Domain.Plan
{
    public sealed class PlanState : AggregateStateBase
    {
        private Guid _planId;
        private readonly List<PlanYear> _planYears = new List<PlanYear>();

        public List<PlanYear> PlanYears
        {
            get { return _planYears; }
        }

        public PlanState(IEnumerable<IEvent> eventHistory)
        {
            base.Register<NewPlanDefinedEvent>(evnt => When(evnt));
            base.Register<NewPlanYearAssignedEvent>(evnt => When(evnt));

            foreach (var @event in eventHistory)
            {
                Apply(@event);
            }
        }

        public override Guid Id
        {
            get { return _planId; }
        }

        public void When(NewPlanDefinedEvent @event)
        {
            _planId = new Guid(@event.PlanId);
        }

        public void When(NewPlanYearAssignedEvent @event)
        {
            _planYears.Add(new PlanYear
                {
                    Ends = @event.Ends,
                    Starts = @event.Starts,
                    Year = @event.Year,
                });
        }
    }
}
