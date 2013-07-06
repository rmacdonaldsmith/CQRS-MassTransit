using System;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Domain.Repositories;
using MHM.WinFlexOne.CQRS.Interfaces.Commands;

namespace MHM.WinFlexOne.CQRS.Domain.Plan
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
