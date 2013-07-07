using System;
using System.Collections.Generic;
using CQRS.Interfaces.Services.ReadModel;
using MHM.WinFlexOne.CQRS.Dtos;
using RestSharp;

namespace CQRS.ReadModel.Client
{
    public class BenefitsReadModel : IBenefitsReadModel
    {
        private readonly RestClient _restClient;
        private readonly Dictionary<Guid, PlanYearBenefitDto> _planYearBenefitMap = new Dictionary<Guid, PlanYearBenefitDto>();

        public BenefitsReadModel(string baseUrl)
        {
            //m_restClient = new RestClient("http://localhost:3000/benefits");
            _restClient = new RestClient(baseUrl);
        }

        public IEnumerable<BenefitDto> GetBenefits()
        {
            var request = new RestRequest("/", Method.GET);
            request.OnBeforeDeserialization = res => res.ContentType = "application/json";
            var response = _restClient.Execute<List<BenefitDto>>(request);

            return response.Data;
        }

        public PlanYearBenefitDto GetPlanYearBenefit(string planYearBenefitId)
        {
            return new PlanYearBenefitDto
            {
                Id = planYearBenefitId,
                AnnualLimit = 1200,
                HasAnnualLimit = true,
                PlanYear = 2013,
            };

            PlanYearBenefitDto itemToReturn = null;
            var guid = Guid.Parse(planYearBenefitId);

            if (_planYearBenefitMap.ContainsKey(guid))
            {
                itemToReturn = _planYearBenefitMap[guid];
            }

            return itemToReturn;
        }
    }
}