using System;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Domain.Repositories;
using MHM.WinFlexOne.CQRS.Interfaces.Commands;

namespace MHM.WinFlexOne.CQRS.Domain.Benefit
{
    public class AssignBenefitToYearCommandHandler : CommandResponderBase<AssignBenefitToYear>, Handles<AssignBenefitToYear>
    {
        private readonly IRepository<Benefit> _repository;

        public AssignBenefitToYearCommandHandler(IRepository<Benefit> repository) 
            : base(command => Guid.Parse(command.BenefitId))
        {
            _repository = repository;
        }

        public override void Handle(AssignBenefitToYear command)
        {
            var benefit = _repository.GetById(new Guid(command.BenefitId), int.MaxValue);

            benefit.AssignBenefitToYear(command);

            _repository.Save(benefit, int.MaxValue);
        }
    }
}
