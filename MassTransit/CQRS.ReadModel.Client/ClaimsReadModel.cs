using System.Collections.Generic;
using MHM.WinFlexOne.CQRS.Dtos;
using MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel;
using RestSharp;

namespace CQRS.ReadModel.Client
{
    public class ClaimsReadModel : IClaimsReadModel
    {
        private readonly RestClient _restClient;

        public ClaimsReadModel(string baseUrl)
        {
            //todo: probably better adherence to the Dependency Inversion Principle if the constructor takes an instance of RestClient instead of the base Url.
            //That way we the constructor makes it explicit what dependencies this class has. 
            _restClient = new RestClient(baseUrl);
        }

        public ClaimDto GetClaim(string claimId)
        {
            var request = new RestRequest("/" + claimId, Method.GET);
            request.OnBeforeDeserialization = res => res.ContentType = "application/json";
            var response = _restClient.Execute<ClaimDto>(request);

            return response.Data;
        }

        public IEnumerable<ClaimDto> GetClaimsForParticipant(string participantId)
        {
            var request = new RestRequest("forparticipant/" + participantId, Method.GET);
            request.OnBeforeDeserialization = res => res.ContentType = "application/json";
            var response = _restClient.Execute<List<ClaimDto>>(request);

            return response.Data;
        }
    }
}
