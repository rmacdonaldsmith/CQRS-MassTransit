using System;
using CQRS.Commands;
using CQRS.Domain.Repositories;
using CQRS.Interfaces.Commands;
using CQRS.Interfaces.Services.ReadModel;

namespace CQRS.Domain.Claims
{
    public sealed class SubmitClaimRequestCommandHandler : CommandResponderBase<SubmitClaimRequest>, Handles<SubmitClaimRequest>
    {
        private readonly IRepository<ClaimRequest> _repository;
        private readonly ICompaniesReadModel _companiesService;

        public SubmitClaimRequestCommandHandler(IRepository<ClaimRequest> repository, ICompaniesReadModel companiesService)
            : base(command => Guid.Parse(command.ClaimRequestId))
        {
            _repository = repository;
            _companiesService = companiesService;
        }

        public override void Handle(SubmitClaimRequest command)
        {
            var claimRequest = new ClaimRequest();
            claimRequest.SubmitClaimRequest(command, _companiesService);

            _repository.Save(claimRequest, claimRequest.LastEventVersion); 
        }
    }
}
