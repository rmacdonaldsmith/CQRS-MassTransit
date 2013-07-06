using System;
using MHM.WinFlexOne.CQRS.Commands;
using MassTransit;

namespace MHM.WinFlexOne.CQRS.Domain.Election
{
    public class ChangeElectionAmountResponseDecorator : Consumes<ChangeElectionAmount>.Context
    {
        private readonly ChangeElectionAmountCommandHandler _changeElectionAmountCommandHandler;

        public ChangeElectionAmountResponseDecorator(ChangeElectionAmountCommandHandler changeElectionAmountCommandHandler)
        {
            _changeElectionAmountCommandHandler = changeElectionAmountCommandHandler;
        }

        public void Consume(IConsumeContext<ChangeElectionAmount> context)
        {
            var response = new CommandResponse
                {
                    CommandId = context.Message.ElectionId,
                };

            try
            {
                _changeElectionAmountCommandHandler.Handle(context.Message);

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