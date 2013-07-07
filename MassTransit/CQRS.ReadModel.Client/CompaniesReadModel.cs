using System;
using System.Collections.Generic;
using System.Linq;
using CQRS.Interfaces.Services.ReadModel;
using MHM.WinFlexOne.CQRS.Dtos;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace CQRS.ReadModel.Client
{
    public class CompaniesReadModel : ICompaniesReadModel
    {
        private readonly MongoDatabase _mongoDataBase;

        public CompaniesReadModel(MongoDatabase mongoDataBase)
        {
            _mongoDataBase = mongoDataBase;
        }

        public IEnumerable<CompanyDto> GetCompanies()
        {
            var companiesCollection = _mongoDataBase.GetCollection<CompanyDto>("Companies");
            return companiesCollection.FindAll();
        }

        public CompanyDto GetCompany(string companyId)
        {
            var companiesCollection = _mongoDataBase.GetCollection<CompanyDto>("Companies");
            var query = Query.EQ("_id", companyId);
            var companyDto = companiesCollection.Find(query).FirstOrDefault();

            return companyDto;
        }

        public IEnumerable<PlanDto> GetPlansForCompany(string companyId)
        {
            var plansCollection = _mongoDataBase.GetCollection<PlanDto>("Plans");
            var query = Query.EQ("CompanyId", companyId);
            var found = plansCollection.Find(query);

            return found;
        }

        public IEnumerable<PlanYearDto> GetPlanYearsForPlan(string planId)
        {
            var planYearCollection = _mongoDataBase.GetCollection<PlanYearDto>("PlansYears");
            var query = Query.EQ("PlanId", planId);
            var found = planYearCollection.Find(query);

            return found;
        }

        public IEnumerable<CopayInfo> GetCopayBy(string companyId)
        {
            return GetPlansForCompany(companyId).SelectMany(plan => plan.Copays);
        }

        public IEnumerable<string> GetClaimTypes()
        {
            return new List<string>{"Dental Copay","Medical Copay","Vision Copay","Dental Coinsurance", "Medical Coinsurance"};
        }

        public IEnumerable<PlanYearDto> GetPlanYearsForCompany(string companyId)
        {
            var planYearCollection = _mongoDataBase.GetCollection<PlanYearDto>("PlanYears");
            var query = Query.EQ("CompanyId", companyId);
            var found = planYearCollection.Find(query);

            return found;
        }

        public IEnumerable<PlanYearBenefitDto> GetPlanYearBenefits(string companyId)
        {
            var planYearBenefitCollection = _mongoDataBase.GetCollection<PlanYearBenefitDto>("PlanYearBenefits");
            var query = Query.EQ("CompanyId", companyId);
            var pybFound = planYearBenefitCollection.Find(query);

            return pybFound;
        }
    }
}