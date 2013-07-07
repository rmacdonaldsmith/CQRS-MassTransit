using System;
using CQRS.Commands;
using CQRS.Domain.Repositories;
using CQRS.Interfaces.Commands;

namespace CQRS.Domain.Plan
{
    public class DefineNewPlanCommandHandler : CommandResponderBase<DefineNewPlan>, Handles<DefineNewPlan>
    {
        private readonly IRepository<Plan> _repository; 

        public DefineNewPlanCommandHandler(IRepository<Plan> repository) 
            : base(command => new Guid(command.PlanId))
        {
            _repository = repository;
        }

        public override void Handle(DefineNewPlan command)
        {
            var plan = new Plan();

            plan.DefineNewPlan(command);

            _repository.Save(plan, plan.LastEventVersion);
        }
    }
}
