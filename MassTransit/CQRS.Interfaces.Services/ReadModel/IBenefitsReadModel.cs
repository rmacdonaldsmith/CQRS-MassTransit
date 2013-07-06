using System.Collections.Generic;
using MHM.WinFlexOne.CQRS.Dtos;

namespace MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel
{
    public interface IBenefitsReadModel
    {
        IEnumerable<BenefitDto> GetBenefits();

        PlanYearBenefitDto GetPlanYearBenefit(string planYearBenefitId);
    }
}