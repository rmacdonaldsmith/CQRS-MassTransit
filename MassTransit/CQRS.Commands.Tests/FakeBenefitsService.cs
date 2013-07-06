using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MHM.WinFlexOne.CQRS.Dtos;
using MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel;

namespace MHM.WinFlexOne.CQRS.Domain.Tests
{
    public class FakeBenefitsService : IBenefitsReadModel
    {
        private Expression<Func<string, PlanYearBenefitDto>> _fakeGetPlanYearBenefit;

        public Expression<Func<string, PlanYearBenefitDto>> FakeGetPlanYearBenefit
        {
            get { return _fakeGetPlanYearBenefit; }
            set { _fakeGetPlanYearBenefit = value; }
        }

        public IEnumerable<BenefitDto> GetBenefits()
        {
            throw new NotImplementedException();
        }

        public PlanYearBenefitDto GetPlanYearBenefit(string planYearBenefitId)
        {
            return _fakeGetPlanYearBenefit.Compile()(planYearBenefitId);
        }
    }
}
