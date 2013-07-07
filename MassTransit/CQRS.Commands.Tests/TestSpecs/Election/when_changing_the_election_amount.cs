using System;
using System.Collections.Generic;
using CQRS.Commands;
using CQRS.Domain.Election;
using CQRS.Domain.Repositories;
using CQRS.DomainTesting;
using CQRS.Interfaces.Events;
using CQRS.Interfaces.Services.ReadModel;
using CQRS.Messages.Events;
using MHM.WinFlexOne.CQRS.Dtos;

namespace CQRS.Domain.Tests.TestSpecs.Election
{
    public class when_changing_the_election_amount : EventSpecification<ChangeElectionAmount>
    {
        private readonly Guid m_electionId = Guid.NewGuid();
        private readonly Guid _planYearBenefitId = Guid.NewGuid();

        public override IEnumerable<IEvent> Given()
        {
            yield return new ElectionMadeEvent
                {
                    AdministratorCode = "mhmrasp",
                    CompanyCode = "mhm",
                    ParticipantId = "empnum",
                    ElectionAmount = 1200,
                    ElectionReason = "reason",
                    Id = m_electionId.ToString(),
                    PerPayPeriodAmount = 100,
                };
        }

        public override ChangeElectionAmount When()
        {
            return new ChangeElectionAmount
                {
                    ElectionId = m_electionId.ToString(),
                    PlanYearBenefitId = _planYearBenefitId.ToString(),
                    NewElectionAmount = 1000,
                    QualifyingEvent = "Work",
                };
        }

        public override Interfaces.Commands.Handles<ChangeElectionAmount> BuildCommandHandler()
        {
            IBenefitsReadModel benefitsService = new FakeBenefitsService
            {
                FakeGetPlanYearBenefit = benefitId => new PlanYearBenefitDto
                {
                    Id = benefitId,
                    HasAnnualLimit = true,
                    AnnualLimit = 1200,
                    BenefitId = "BenefitCode",
                    PlanYear = DateTime.Today.Year,
                },
            };
            return new ChangeElectionAmountCommandHandler(new Repository<global::CQRS.Domain.Election.Election>(EventStore), benefitsService);
        }

        public override IEnumerable<IEvent> Then()
        {
            yield return new ElectionAmountChangedEvent
                {
                    ElectionId = m_electionId.ToString(),
                    NewElectionAmount = 1000,
                    NewPerPayPeriodAmount = 10,
                    QualifyingEvent = "Work",
                    Version = 1,
                };
        }

        public override System.Linq.Expressions.Expression<Predicate<Exception>> ThenException()
        {
            return exception => exception == null;
        }

        public override void Finally()
        {
            //nothing to do
        }
    }
}
