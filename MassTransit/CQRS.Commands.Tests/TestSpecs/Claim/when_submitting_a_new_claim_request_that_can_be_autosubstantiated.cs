using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CQRS.Commands;
using CQRS.Domain.Claims;
using CQRS.Domain.Repositories;
using CQRS.DomainTesting;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;
using MHM.WinFlexOne.CQRS.Dtos;

namespace CQRS.Domain.Tests.TestSpecs.Claim
{
    public sealed class when_submitting_a_new_claim_request_that_can_be_autosubstantiated : EventSpecification<SubmitClaimRequest>
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
                                    CopayAmount = _claimAmount,
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
                    new ClaimRequestAutoSubstantiatedEvent
                        {
                            Amount = _claimAmount,
                            ClaimRequestId = _claimRequestId,
                            ClaimType = _claimType,
                            CompanyId = _companyId,
                            DateOfTransaction = _transactionDate,
                            ParticipantId = _participantId,
                            ProviderName = "provider name",
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
            //nothing to do
        }
    }
}
