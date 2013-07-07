using System;
using CQRS.Commands;
using CQRS.Domain.Repositories;
using CQRS.Interfaces.Commands;

namespace CQRS.Domain.Claims
{
    public class RejectClaimRequestCommandHandler : CommandResponderBase<RejectClaimRequest>, Handles<RejectClaimRequest>
    {
        private readonly IRepository<ClaimRequest> _repository;

        public RejectClaimRequestCommandHandler(IRepository<ClaimRequest> repository) 
            : base(request => new Guid(request.ClaimRequestId))
        {
            _repository = repository;
        }

        public override void Handle(RejectClaimRequest command)
        {
            var claimRequest = _repository.GetById(new Guid(command.ClaimRequestId), int.MaxValue);
            claimRequest.RejectClaimRequest(command);
            _repository.Save(claimRequest, claimRequest.LastEventVersion);
        }
    }
}
