using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
            var repository = new Repository<Domain.Election.Election>(EventStore);
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
