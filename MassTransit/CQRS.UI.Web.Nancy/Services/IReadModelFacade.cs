using System;
using System.Collections.Generic;
using MHM.WinFlexOne.CQRS.Dtos;

namespace CQRS.UI.Web.Nancy.Services
{
    public interface IReadModelFacade
    {
        List<ElectionDto> GetElectionsForParticipant(string participantId);
        ElectionDto GetElection(string electionId);

    }
}