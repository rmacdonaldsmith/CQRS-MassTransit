using System;
using CQRS.Commands;
using CQRS.Domain.Repositories;
using CQRS.Interfaces.Commands;

namespace CQRS.Domain.Benefit
{
    public class DefineNewBenefitCommandHandler : CommandResponderBase<DefineNewBenefit>, Handles<DefineNewBenefit>
    {
        private readonly IRepository<Benefit> _repository;

        public DefineNewBenefitCommandHandler(IRepository<Benefit> repository)
            : base(command => Guid.Parse(command.BenefitId))
        {
            _repository = repository;
        }

        public override void Handle(DefineNewBenefit command)
        {
            var benefit = new Benefit();

            benefit.DefineNewBenefit(command);

            _repository.Save(benefit, benefit.LastEventVersion);
        }

        //public void Consume(IConsumeContext<DefineNewBenefit> context)
        //{
        //    //use an Expression<Func<DefineNewBenefit, Guid>> to define the ID of the incoming message.
        //    var response = new CommandResponse
        //    {
        //        CommandId = context.Message.Id,
        //    };

        //    try
        //    {
        //        Handle(context.Message);

        //        response.CommandStatus = CommandStatusEnum.Succeeded;
        //        response.ContainsException = false;
        //        response.ExceptionDetail = string.Empty;
        //        response.Message = string.Empty;
        //    }
        //    catch (Exception exception)
        //    {
        //        response.CommandStatus = CommandStatusEnum.Failed;
        //        response.ContainsException = true;
        //        response.ExceptionDetail = exception.ToString();
        //        response.Message = exception.Message;
        //    }

        //    context.Respond(response);
        //}
    }
}
