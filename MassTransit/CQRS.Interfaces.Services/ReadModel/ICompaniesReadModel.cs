using System.Collections.Generic;
using MHM.WinFlexOne.CQRS.Dtos;

namespace CQRS.Interfaces.Services.ReadModel
{
    public interface ICompaniesReadModel
    {
        IEnumerable<CompanyDto> GetCompanies();

        CompanyDto GetCompany(string companyId);

        IEnumerable<PlanDto> GetPlansForCompany(string companyId);

        IEnumerable<PlanYearBenefitDto> GetPlanYearBenefits(string companyId);

        IEnumerable<PlanYearDto> GetPlanYearsForCompany(string companyId);

        IEnumerable<PlanYearDto> GetPlanYearsForPlan(string planId);

        IEnumerable<CopayInfo> GetCopayBy(string companyId);

        IEnumerable<string> GetClaimTypes();
    }
}