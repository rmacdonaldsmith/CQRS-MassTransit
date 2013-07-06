using System;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Domain.Repositories;
using MHM.WinFlexOne.CQRS.Interfaces.Commands;
using MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel;

namespace MHM.WinFlexOne.CQRS.Domain.Claims
{
    public class DisburseClaimCommandHandler : CommandResponderBase<DisburseClaim>, Handles<DisburseClaim>
    {
        private readonly IRepository<ClaimRequest> _repository;
        private readonly IElectionsReadModel _electionsReadModel;

        public DisburseClaimCommandHandler(IRepository<ClaimRequest> repository, IElectionsReadModel electionsReadModel)
            : base(claim => new Guid(claim.ClaimId))
        {
            _repository = repository;
            _electionsReadModel = electionsReadModel;
        }

        public override void Handle(DisburseClaim command)
        {
            var claimRequest = _repository.GetById(new Guid(command.ClaimId), int.MaxValue);
            claimRequest.DisburseClaim(command, _electionsReadModel);
            _repository.Save(claimRequest, claimRequest.LastEventVersion);
        }
    }
}
