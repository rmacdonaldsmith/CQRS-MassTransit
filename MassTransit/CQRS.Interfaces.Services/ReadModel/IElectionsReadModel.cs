using System.Collections.Generic;
using MHM.WinFlexOne.CQRS.Dtos;

namespace MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel
{
    public interface IElectionsReadModel
    {
        IEnumerable<ElectionDto> GetElectionsForParticipant(string participantId);

        ElectionDto GetElection(string electionId);

        ElectionBalanceDto GetElectionBalance(string electionId);
    }
}
