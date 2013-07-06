using System;
using System.Collections.Generic;
using CQRS.DomainTesting;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Domain.Election;
using MHM.WinFlexOne.CQRS.Domain.Repositories;
using MHM.WinFlexOne.CQRS.Dtos;
using MHM.WinFlexOne.CQRS.Events;
using MHM.WinFlexOne.CQRS.Interfaces.Events;
using MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel;

namespace MHM.WinFlexOne.CQRS.Domain.Tests.TestSpecs.Election
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
            return new ChangeElectionAmountCommandHandler(new Repository<Domain.Election.Election>(EventStore), benefitsService);
        }

        public override IEnumerable<IEvent> Then()
        {
            yield return new ElectionAmountChangedEvent
                {
                    ElectionId = m_electionId.ToString(),
                    NewElectionAmount = 1000,
                    NewPerPayPeriodAmount = 10,
                    QualifyingEvent = "Work",
                    Version = 0,
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
