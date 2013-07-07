using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CQRS.Commands;
using CQRS.Domain.Claims;
using CQRS.Domain.Repositories;
using CQRS.DomainTesting;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;

namespace CQRS.Domain.Tests.TestSpecs.Claim
{
    public class when_manually_substantiating_a_claim_request : EventSpecification<VerifyCardUse>
    {
        private readonly decimal _claimAmount = 20.00m;
        private readonly string _claimRequestId = Guid.NewGuid().ToString();
        private readonly string _claimType = "Dental Copay";
        private readonly DateTime _transactionDate = new DateTime(2013, 1, 12);

        public override IEnumerable<IEvent> Given()
        {
            return new IEvent[]
                {
                    new ClaimRequestCreatedPendingVerificationEvent
                        {
                            Amount = _claimAmount,
                            ClaimRequestId = _claimRequestId,
                            ClaimType = _claimType,
                            DateOfTransaction = _transactionDate,
                            ProviderName = "provider name",
                            Reason = "Claim request amount is not recognised as a copay amount. Participant must substantiate this card use"
                        }
                };
        }

        public override VerifyCardUse When()
        {
            return new VerifyCardUse
                {
                    ClaimRequestId = _claimRequestId,
                };
        }

        public override Interfaces.Commands.Handles<VerifyCardUse> BuildCommandHandler()
        {
            return new VerifyCardUseCommandHandler(new Repository<ClaimRequest>(EventStore));
        }

        public override IEnumerable<IEvent> Then()
        {
            return new IEvent[]
                {
                    new CardUseVerifiedEvent
                        {
                            ClaimRequestId = _claimRequestId,
                            Version = 1
                        }
                };
        }

        public override Expression<Predicate<Exception>> ThenException()
        {
            return NoException;
        }

        public override void Finally()
        {
            //nothing to do
        }
    }
}
