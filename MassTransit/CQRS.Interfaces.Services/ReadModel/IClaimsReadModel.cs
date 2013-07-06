using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHM.WinFlexOne.CQRS.Dtos;

namespace MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel
{
    public interface IClaimsReadModel
    {
        ClaimDto GetClaim(string claimId);

        IEnumerable<ClaimDto> GetClaimsForParticipant(string participantId);
    }
}
