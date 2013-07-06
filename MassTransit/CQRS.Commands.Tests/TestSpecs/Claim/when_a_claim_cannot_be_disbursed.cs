using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CQRS.DomainTesting;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Domain.Claims;
using MHM.WinFlexOne.CQRS.Domain.Repositories;
using MHM.WinFlexOne.CQRS.Dtos;
using MHM.WinFlexOne.CQRS.Events;
using MHM.WinFlexOne.CQRS.Interfaces.Events;

namespace MHM.WinFlexOne.CQRS.Domain.Tests.TestSpecs.Claim
{
    public class when_a_claim_cannot_be_disbursed : EventSpecification<DisburseClaim>
    {
        private readonly string _claimId = Guid.NewGuid().ToString();
        private readonly string _participantId = Guid.NewGuid().ToString();
        private readonly decimal _claimAmount = 1200;
        private readonly string _claimType = "Vision Copay";

        public override IEnumerable<IEvent> Given()
        {
            return new IEvent[]
                {
                    new ClaimRequestAutoSubstantiatedEvent
                        {
                            ClaimRequestId = _claimId,
                            ClaimType = _claimType,
                            Amount = _claimAmount,
                            ParticipantId = _participantId,
                        },
                };
        }

        public override DisburseClaim When()
        {
            return new DisburseClaim
            {
                ClaimId = _claimId,
                ParticipantId = _participantId,
            };
        }

        public override Interfaces.Commands.Handles<DisburseClaim> BuildCommandHandler()
        {
            var fakeElectionService = new FakeElectionsService(participantId => new ElectionDto[]
                {
                    new ElectionDto {Id = "election1", BenefitType = "Medical"},
                    new ElectionDto {Id = "election2", BenefitType = "Dental"},
                },
                electionId => new ElectionBalanceDto
                    {
                        BalanceRemaining = 10000,
                        ElectionId = "election2",
                        ParticipantId = _participantId,
                    }
            );

            return new DisburseClaimCommandHandler(new Repository<ClaimRequest>(EventStore), fakeElectionService);
        }

        public override IEnumerable<IEvent> Then()
        {
            return new IEvent[]
                {
                    new ClaimNotDisbursedEvent
                        {
                            ClaimAmount = _claimAmount,
                            ClaimId = _claimId,
                            ClaimType = _claimType,
                            Reason = string.Format("No election matches the claim type '{0}'", _claimType)
                        }
                };
        }

        public override Expression<Predicate<Exception>> ThenException()
        {
            return NoException;
        }

        public override void Finally()
        {
            //nought to do
        }
    }
}
