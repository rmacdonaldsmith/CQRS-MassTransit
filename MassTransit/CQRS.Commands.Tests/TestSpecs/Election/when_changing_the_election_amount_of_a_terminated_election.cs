using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    public class when_changing_the_election_amount_of_a_terminated_election : EventSpecification<ChangeElectionAmount>
    {
        private Guid _electionId = Guid.NewGuid();
        private Guid _planYearBenefitId = Guid.NewGuid();
        private DateTime _electionTerminationDate = DateTime.Today;

        public override IEnumerable<IEvent> Given()
        {
            yield return new ElectionMadeEvent
                {
                    Id = _electionId.ToString(),
                    AdministratorCode = "admcode",
                    CompanyCode = "cocode",
                    ElectionAmount = 1000,
                    ElectionReason = "Work",
                    ParticipantId = "",
                };

            yield return new ElectionTerminatedEvent
                {
                    ElectionId = _electionId.ToString(),
                    TerminatedDate = _electionTerminationDate,
                    Version = 0
                };
        }

        public override ChangeElectionAmount When()
        {
            return new ChangeElectionAmount
                {
                    ElectionId = _electionId.ToString(),
                    NewElectionAmount = 800,
                    PlanYearBenefitId = _planYearBenefitId.ToString(),
                    QualifyingEvent = "Marriage",
                };
        }

        public override Interfaces.Commands.Handles<ChangeElectionAmount> BuildCommandHandler()
        {
            var repository = new Repository<global::CQRS.Domain.Election.Election>(EventStore);
            IBenefitsReadModel benefitsService = new FakeBenefitsService
                {
                    FakeGetPlanYearBenefit = benefitId => new PlanYearBenefitDto
                    {
                        AnnualLimit = 1200,
                        BenefitId = "bebefitcode",
                        HasAnnualLimit = true,
                        Id = _planYearBenefitId.ToString(),
                        PlanYear = 2013,
                    }
                };

            return new ChangeElectionAmountCommandHandler(repository, benefitsService);
        }

        public override IEnumerable<IEvent> Then()
        {
            return NoEvents;
        }

        public override Expression<Predicate<Exception>> ThenException()
        {
            return exception => exception.Message == string.Format("This election was terminated on {0}. You can not change the election amount.", _electionTerminationDate);
        }

        public override void Finally()
        {
            //nothing to do
        }
    }
}
