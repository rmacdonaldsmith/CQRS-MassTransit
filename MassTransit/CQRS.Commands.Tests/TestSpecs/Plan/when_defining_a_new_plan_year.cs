using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CQRS.DomainTesting;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Domain.Plan;
using MHM.WinFlexOne.CQRS.Domain.Repositories;
using MHM.WinFlexOne.CQRS.Events;
using MHM.WinFlexOne.CQRS.Interfaces.Events;

namespace MHM.WinFlexOne.CQRS.Domain.Tests.TestSpecs.Plan
{
    public class when_defining_a_new_plan_year : EventSpecification<DefineYearForPlan>
    {
        private readonly string _companyId = Guid.NewGuid().ToString();
        private readonly string _planId = Guid.NewGuid().ToString();
        private readonly string _planYearId = Guid.NewGuid().ToString();

        public override IEnumerable<IEvent> Given()
        {
            return NoEvents;
        }

        public override DefineYearForPlan When()
        {
            return new DefineYearForPlan
                {
                    CompanyId = _companyId,
                    Ends = new DateTime(2013, 12, 31),
                    Name = "new plan",
                    PlanId = _planId,
                    PlanYearId = _planYearId,
                    Starts = new DateTime(2013, 1, 1),
                    Year = 2013
                };
        }

        public override Interfaces.Commands.Handles<DefineYearForPlan> BuildCommandHandler()
        {
            return new DefineYearForPlanCommandHandler(new Repository<Domain.Plan.Plan>(EventStore));
        }

        public override IEnumerable<IEvent> Then()
        {
            return new List<IEvent>
                {
                    new NewPlanYearAssignedEvent
                        {
                            CompanyId = _companyId,
                            Ends = new DateTime(2013, 12, 31),
                            Name = "new plan",
                            PlanId = _planId,
                            PlanYearId = _planYearId,
                            Starts = new DateTime(2013,1,1),
                            Version = 0,
                            Year = 2013,
                        },
                };
        }

        public override Expression<Predicate<Exception>> ThenException()
        {
            return NoException;
        }

        public override void Finally()
        {
            //do nothing
        }
    }
}
