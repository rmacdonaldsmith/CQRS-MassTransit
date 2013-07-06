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
    public class when_submitting_a_claim_that_needs_substantiation : EventSpecification<SubmitClaimRequest>
    {
        private readonly decimal _claimAmount = 20.00m;
        private readonly string _claimRequestId = Guid.NewGuid().ToString();
        private readonly string _companyId = Guid.NewGuid().ToString();
        private readonly string _participantId = Guid.NewGuid().ToString();
        private readonly string _claimType = "Dental Copay";
        private readonly DateTime _transactionDate = new DateTime(2013, 1, 12);

        public override IEnumerable<IEvent> Given()
        {
            return NoEvents;
        }

        public override SubmitClaimRequest When()
        {
            return new SubmitClaimRequest
            {
                Amount = _claimAmount,
                ClaimRequestId = _claimRequestId,
                ClaimType = _claimType,
                CompanyId = _companyId,
                DateOfTransaction = _transactionDate,
                ProviderName = "provider name",
                ParticipantId = _participantId,
            };
        }

        public override Interfaces.Commands.Handles<SubmitClaimRequest> BuildCommandHandler()
        {
            var companiesService = new FakeCompaniesService
                {
                    FakeGetCopayBy = companyId => new[]
                        {
                            new CopayInfo
                                {
                                    CompanyId = companyId,
                                    ClaimType = _claimType,
                                    CompanyName = companyId,
                                    CopayAmount = 30.00m,
                                    PlanId = "dentalplanid",
                                    PlanName = "dental plan",
                                }
                        }
                };

            return new SubmitClaimRequestCommandHandler(new Repository<ClaimRequest>(EventStore), companiesService);
        }

        public override IEnumerable<IEvent> Then()
        {
            return new IEvent[]
                {
                    new ClaimRequestCreatedPendingVerificationEvent
                        {
                            Amount = _claimAmount,
                            ClaimRequestId = _claimRequestId,
                            ClaimType = _claimType,
                            CompanyId = _companyId,
                            DateOfTransaction = _transactionDate,
                            ParticipantId = _participantId,
                            ProviderName = "provider name",
                            Reason = "Claim request amount is not recognised as a copay amount. Participant must substantiate this card use",
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
