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
    public class when_a_claim_request_is_rejected : EventSpecification<RejectClaimRequest>
    {
        private readonly decimal _claimAmount = 20.00m;
        private readonly string _claimRequestId = Guid.NewGuid().ToString();
        private readonly string _companyId = Guid.NewGuid().ToString();
        private readonly string _participantId = Guid.NewGuid().ToString();
        private readonly string _claimType = "Dental Copay";
        private readonly DateTime _transactionDate = new DateTime(2013, 1, 12);

        public override IEnumerable<IEvent> Given()
        {
            return new IEvent[]
                {
                    new ClaimRequestCreatedPendingVerificationEvent
                        {
                            ClaimRequestId = _claimRequestId,
                            Amount = _claimAmount,
                            ClaimType = _claimType,
                            DateOfTransaction = _transactionDate,
                            ProviderName = "provider name",
                            Reason = "Claim request amount is not recognised as a copay amount. Participant must substantiate this card use",
                        },
                };
        }

        public override RejectClaimRequest When()
        {
            return new RejectClaimRequest
                {
                    ClaimRequestId = _claimRequestId,
                    Reason = "The particiapant did not provide any substatiation within the time allowed"
                };
        }

        public override Interfaces.Commands.Handles<RejectClaimRequest> BuildCommandHandler()
        {
            return new RejectClaimRequestCommandHandler(new Repository<ClaimRequest>(EventStore));
        }

        public override IEnumerable<IEvent> Then()
        {
            return new IEvent[]
                {
                    new ClaimRequestRejectedEvent
                        {
                            ClaimRequestId = _claimRequestId,
                            RejectReason = "The particiapant did not provide any substatiation within the time allowed",
                            Version = 1
                        },
                };
        }

        public override Expression<Predicate<Exception>> ThenException()
        {
            return NoException;
        }

        public override void Finally()
        {
            //nothing to do.
        }
    }
}
