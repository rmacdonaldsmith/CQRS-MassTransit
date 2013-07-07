using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CQRS.Commands;
using CQRS.Domain.Benefit;
using CQRS.Domain.Repositories;
using CQRS.DomainTesting;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;

namespace CQRS.Domain.Tests.TestSpecs.Benefit
{
    public class when_defining_a_new_benefit : EventSpecification<DefineNewBenefit>
    {
        private Guid _benefitId = Guid.NewGuid();
        private Guid _planId = Guid.NewGuid();

        public override IEnumerable<IEvent> Given()
        {
            return NoEvents;
        }

        public override DefineNewBenefit When()
        {
            return new DefineNewBenefit
                {
                    BenefitDescription = "This is a new vision benefit",
                    BenefitId = _benefitId.ToString(),
                    BenefitType = "Vision",
                    HasMaxAmount = true,
                    MaxBenefitAmount = 1500,
                    PlanId = _planId.ToString(),
                };
        }

        public override Interfaces.Commands.Handles<DefineNewBenefit> BuildCommandHandler()
        {
            var repository = new Repository<global::CQRS.Domain.Benefit.Benefit>(EventStore);

            return new DefineNewBenefitCommandHandler(repository);
        }

        public override IEnumerable<IEvent> Then()
        {
            yield return new NewBenefitDefinedEvent
                {
                    BenefitId = _benefitId.ToString(),
                    BenefitDescription = "This is a new vision benefit",
                    BenefitType = "Vision",
                    HasMaxElectionAmount = true,
                    MaxElectionAmount = 1500,
                    PlanId = _planId.ToString(),
                    Version = 1,
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
