using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CQRS.DomainTesting;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Domain.Benefit;
using MHM.WinFlexOne.CQRS.Domain.Repositories;
using MHM.WinFlexOne.CQRS.Events;
using MHM.WinFlexOne.CQRS.Interfaces.Events;

namespace MHM.WinFlexOne.CQRS.Domain.Tests.TestSpecs.Benefit
{
    public class when_assigning_a_benefit_to_a_plan_year : EventSpecification<AssignBenefitToYear>
    {
        private Guid _benefitId = Guid.NewGuid();
        private Guid _planId = Guid.NewGuid();

        public override IEnumerable<IEvent> Given()
        {
            yield return new NewBenefitDefinedEvent
                {
                    BenefitId = _benefitId.ToString(),
                    BenefitDescription = "This is a new vision benefit",
                    BenefitType = "Vision",
                    HasMaxElectionAmount = true,
                    MaxElectionAmount = 1500,
                    PlanId = _planId.ToString(),
                    Version = 0
                };
        }

        public override AssignBenefitToYear When()
        {
            return new AssignBenefitToYear
                {
                    BenefitId = _benefitId.ToString(),
                    HasMaxAnnualAmount = true,
                    MaxAnnualAmount = 1000,
                    PlanYear = 2013,
                    StartDate = new DateTime(2013, 1, 1),
                };
        }

        public override Interfaces.Commands.Handles<AssignBenefitToYear> BuildCommandHandler()
        {
            var repository = new Repository<Domain.Benefit.Benefit>(EventStore);

            return new AssignBenefitToYearCommandHandler(repository);
        }

        public override IEnumerable<IEvent> Then()
        {
            yield return new BenefitAssignedToYearEvent
                {
                    BenefitId = _benefitId.ToString(),
                    HasMaxAnnualAmount = true,
                    MaxAnnualAmount = 1000,
                    PlanYear = 2013,
                    StartDate = new DateTime(2013, 1, 1),
                };
        }

        public override Expression<Predicate<Exception>> ThenException()
        {
            return exception => exception == null;
        }

        public override void Finally()
        {
            //nothing
        }
    }
}
