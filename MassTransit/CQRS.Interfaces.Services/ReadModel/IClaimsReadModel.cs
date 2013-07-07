using System.Collections.Generic;
using MHM.WinFlexOne.CQRS.Dtos;

namespace CQRS.Interfaces.Services.ReadModel
{
    public interface IClaimsReadModel
    {
        ClaimDto GetClaim(string claimId);

        IEnumerable<ClaimDto> GetClaimsForParticipant(string participantId);
    }
}
