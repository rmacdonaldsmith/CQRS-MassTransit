using System.Collections.Generic;
using CQRS.Interfaces.Services.ReadModel;
using RestSharp;

namespace CQRS.ReadModel.Client
{
    public class ClaimTypesReadModel : IClaimTypesReadModel
    {
        private readonly RestClient _restClient;

        public ClaimTypesReadModel(string baseUrl)
        {
            _restClient = new RestClient(baseUrl);
        }

        public IEnumerable<string> GetClaimTypes()
        {
            var request = new RestRequest("/", Method.GET);
            request.OnBeforeDeserialization = res => res.ContentType = "application/json";
            var response = _restClient.Execute<List<string>>(request);

            return response.Data;
        }
    }
}
