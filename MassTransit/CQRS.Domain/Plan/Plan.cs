using System;
using System.Collections.Generic;
using System.Linq;
using CQRS.Commands;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;

namespace CQRS.Domain.Plan
{
    public class Plan : AggregateRoot
    {
        private PlanState _planState;

        public override Guid Id
        {
            get { return _planState.Id; }
        }

        public Plan()
        {
            _planState = new PlanState(NoEvents);
        }

        protected internal override void InitializeFromHistory(IEnumerable<IEvent> eventHistory)
        {
            _planState = new PlanState(eventHistory);
        }

        public void DefineNewPlan(DefineNewPlan command)
        {
            ApplyEvent(new NewPlanDefinedEvent
                {
                    PlanId = command.PlanId,
                    CompanyId = command.CompanyId,
                    Description = command.Description,
                    Name = command.Name,
                    PlanType = command.PlanType,
                    Version = 0,
                }
                , 
                @event => _planState.Apply(@event));
        }

        public void DefineYearForPlan(DefineYearForPlan command)
        {
            //check that there is not already a plan year with the same dates as the incoming command
            if (_planState.PlanYears.Any(year => year.Year == command.Year))
            {
                throw new DomainException(string.Format("There is already a plan year defined for the year '{0}'", command.Year));
            }

            ApplyEvent(new NewPlanYearAssignedEvent
                {
                    CompanyId = command.CompanyId,
                    Ends = command.Ends,
                    Name = command.Name,
                    PlanId = command.PlanId,
                    PlanYearId = command.PlanYearId,
                    Starts = command.Starts,
                    Version = 0,
                    Year = command.Year,
                }
                , @event => _planState.Apply(@event));
        }
    }
}
