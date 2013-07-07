using System;
using CQRS.Commands;
using CQRS.Domain.Repositories;
using CQRS.Interfaces.Commands;

namespace CQRS.Domain.Plan
{
    public class DefineYearForPlanCommandHandler : CommandResponderBase<DefineYearForPlan>, Handles<DefineYearForPlan>
    {
        private readonly IRepository<Plan> _repository;

        public DefineYearForPlanCommandHandler(IRepository<Plan> repository) 
            : base(plan => new Guid(plan.PlanId))
        {
            _repository = repository;
        }

        public override void Handle(DefineYearForPlan command)
        {
            var plan = _repository.GetById(new Guid(command.PlanId), int.MaxValue);

            plan.DefineYearForPlan(command);

            _repository.Save(plan, plan.LastEventVersion);
        }
    }
}
