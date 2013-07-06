using System.Collections.Generic;
using MHM.WinFlexOne.CQRS.Dtos;
using RestSharp;

namespace CQRS.UI.Web.Nancy.Services
{
    public class RESTReadModel : IReadModelFacade
    {
        private readonly RestClient m_restClient;

        public RESTReadModel()
        {
            m_restClient = new RestClient("http://localhost:3000/elections");
        }

        public List<ElectionDto> GetElectionsForParticipant(string participantId)
        {
            var request = new RestRequest("forparticipant/" + participantId, Method.GET);
            request.OnBeforeDeserialization = res => res.ContentType = "application/json";
            var response = m_restClient.Execute<List<ElectionDto>>(request);

            return response.Data;
        }

        public ElectionDto GetElection(string electionId)
        {
            var request = new RestRequest(electionId, Method.GET);
            request.OnBeforeDeserialization = res => res.ContentType = "application/json";
            var response = m_restClient.Execute<ElectionDto>(request);

            return response.Data;
        }
    }
}