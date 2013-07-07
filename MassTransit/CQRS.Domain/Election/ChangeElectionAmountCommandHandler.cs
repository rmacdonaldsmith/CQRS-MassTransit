using System;
using CQRS.Commands;
using CQRS.Domain.Repositories;
using CQRS.Interfaces.Commands;
using CQRS.Interfaces.Services.ReadModel;
using MassTransit;

namespace CQRS.Domain.Election
{
    public class ChangeElectionAmountCommandHandler : Handles<ChangeElectionAmount>, Consumes<ChangeElectionAmount>.Context
    {
        private readonly IRepository<Election> _repository;
        private readonly IBenefitsReadModel _benefitsService;

        public ChangeElectionAmountCommandHandler(IRepository<Election> repository, IBenefitsReadModel benefitsService)
        {
            _benefitsService = benefitsService;
            _repository = repository;
        }

        public void Handle(ChangeElectionAmount command)
        {
            var election = _repository.GetById(new Guid(command.ElectionId), int.MaxValue);
            election.ChangeElectionAmount(command, _benefitsService);
            _repository.Save(election, election.LastEventVersion);
        }

        public void Consume(IConsumeContext<ChangeElectionAmount> context)
        {
            var response = new CommandResponse
            {
                CommandId = context.Message.ElectionId,
            };

            try
            {
                Handle(context.Message);

                response.CommandStatus = CommandStatusEnum.Succeeded;
                response.ContainsException = false;
                response.ExceptionDetail = string.Empty;
                response.Message = string.Empty;
            }
            catch (Exception exception)
            {
                response.CommandStatus = CommandStatusEnum.Failed;
                response.ContainsException = true;
                response.ExceptionDetail = exception.ToString();
                response.Message = exception.Message;
            }

            context.Respond(response);
        }
    }
}
