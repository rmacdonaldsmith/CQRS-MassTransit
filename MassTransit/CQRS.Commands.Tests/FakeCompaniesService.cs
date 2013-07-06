using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MHM.WinFlexOne.CQRS.Dtos;
using MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel;

namespace MHM.WinFlexOne.CQRS.Domain.Tests
{
    public class FakeCompaniesService : ICompaniesReadModel
    {
        private Expression<Func<string, IEnumerable<CopayInfo>>> _fakeGetCopayBy;

        public Expression<Func<string, IEnumerable<CopayInfo>>> FakeGetCopayBy
        {
            get { return _fakeGetCopayBy; }
            set { _fakeGetCopayBy = value; }
        }

        public IEnumerable<CompanyDto> GetCompanies()
        {
            throw new NotImplementedException();
        }

        public CompanyDto GetCompany(string companyId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PlanDto> GetPlansForCompany(string companyId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PlanYearBenefitDto> GetPlanYearBenefits(string companyId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PlanYearDto> GetPlanYearsForCompany(string companyId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PlanYearDto> GetPlanYearsForPlan(string planId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CopayInfo> GetCopayBy(string companyId)
        {
            return _fakeGetCopayBy.Compile()(companyId);
        }

        public IEnumerable<string> GetClaimTypes()
        {
            throw new NotImplementedException();
        }
    }
}
