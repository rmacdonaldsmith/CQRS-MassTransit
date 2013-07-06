using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CQRS.DomainTesting;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Domain.Election;
using MHM.WinFlexOne.CQRS.Domain.Repositories;
using MHM.WinFlexOne.CQRS.Dtos;
using MHM.WinFlexOne.CQRS.Events;
using MHM.WinFlexOne.CQRS.Interfaces.Events;
using MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel;
using NUnit.Framework;

namespace MHM.WinFlexOne.CQRS.Domain.Tests.TestSpecs.Election
{
    [TestFixture]
    public class when_making_a_new_election : EventSpecification<MakeAnElection>
    {
        private readonly Guid m_electionId = Guid.NewGuid();

        public override IEnumerable<IEvent> Given()
        {
            //there are no prior events required to set the initial state of our domain object
            return NoEvents;
        }

        public override MakeAnElection When()
        {
            return new MakeAnElection
                       {
                           AdministratorCode = "mhmrasp",
                           CompanyCode = "mhm",
                           ParticipantId = "empnum",
                           ElectionAmount = 1200,
                           ElectionReason = "reason",
                           Id = m_electionId.ToString(),
                       };
        }

        public override Interfaces.Commands.Handles<MakeAnElection> BuildCommandHandler()
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
            return new MakeAnElectionCommandHandler(new Repository<Domain.Election.Election>(EventStore), benefitsService);
        }

        public override IEnumerable<IEvent> Then()
        {
            yield return new ElectionMadeEvent
                             {
                                 AdministratorCode = "mhmrasp",
                                 CompanyCode = "mhm",
                                 ParticipantId = "empnum",
                                 ElectionAmount = 1200,
                                 ElectionReason = "reason",
                                 Id = m_electionId.ToString(),
                                 PerPayPeriodAmount = 0,
                             };
        }

        public override Expression<Predicate<Exception>> ThenException()
        {
            return exception => exception == null;
        }

        public override void Finally()
        {
            //nothing to do
        }
    }
}
