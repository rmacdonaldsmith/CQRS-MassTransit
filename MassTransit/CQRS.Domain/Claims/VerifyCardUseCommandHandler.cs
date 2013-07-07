using System;
using CQRS.Commands;
using CQRS.Domain.Repositories;
using CQRS.Interfaces.Commands;

namespace CQRS.Domain.Claims
{
    public class VerifyCardUseCommandHandler : CommandResponderBase<VerifyCardUse>, Handles<VerifyCardUse>
    {
        private readonly IRepository<ClaimRequest> _repository;

        public VerifyCardUseCommandHandler(IRepository<ClaimRequest> repository) 
            : base(use => new Guid(use.ClaimRequestId))
        {
            _repository = repository;
        }

        public override void Handle(VerifyCardUse command)
        {
            var claimRequest = _repository.GetById(new Guid(command.ClaimRequestId), int.MaxValue);
            claimRequest.VerifyClaimRequest(command);
            _repository.Save(claimRequest, claimRequest.LastEventVersion);
        }
    }
}
