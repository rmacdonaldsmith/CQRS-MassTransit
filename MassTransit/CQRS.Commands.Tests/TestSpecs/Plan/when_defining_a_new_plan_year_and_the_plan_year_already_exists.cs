using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CQRS.DomainTesting;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Domain.Plan;
using MHM.WinFlexOne.CQRS.Domain.Repositories;
using MHM.WinFlexOne.CQRS.Events;
using MHM.WinFlexOne.CQRS.Interfaces.Events;

namespace MHM.WinFlexOne.CQRS.Domain.Tests.TestSpecs.Plan
{
    public class when_defining_a_new_plan_year_and_the_plan_year_already_exists : EventSpecification<DefineYearForPlan>
    {
        private readonly string _planId = Guid.NewGuid().ToString();
        private readonly string _companyId = Guid.NewGuid().ToString();

        public override IEnumerable<IEvent> Given()
        {
            return new[]
                {
                    new NewPlanYearAssignedEvent
                        {
                            PlanId = _planId,
                            CompanyId = _companyId,
                            Ends = new DateTime(2013, 12, 31),
                            Name = "2013 plan",
                            PlanYearId = Guid.NewGuid().ToString(),
                            Starts = new DateTime(2013, 1, 1),
                            Year = 2013,
                            Version = 0,
                        },
                };

        }

        public override DefineYearForPlan When()
        {
            return new DefineYearForPlan
                {
                    PlanId = _planId,
                    CompanyId = _companyId,
                    Ends = new DateTime(2013, 12, 31),
                    Name = "Plan Name",
                    PlanYearId = "",
                    Starts = new DateTime(2013, 1, 1),
                    Year = 2013,
                };
        }

        public override Interfaces.Commands.Handles<DefineYearForPlan> BuildCommandHandler()
        {
            return new DefineYearForPlanCommandHandler(new Repository<Domain.Plan.Plan>(EventStore));
        }

        public override IEnumerable<IEvent> Then()
        {
            return NoEvents;
        }

        public override Expression<Predicate<Exception>> ThenException()
        {
            return exception => exception.GetType() == typeof(DomainException) && exception.Message == "There is already a plan year defined for the year '2013'";
        }

        public override void Finally()
        {
            //nothing to do
        }
    }
}
