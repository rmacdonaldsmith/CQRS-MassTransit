using System;
using System.Linq.Expressions;
using CQRS.Commands;
using CQRS.Interfaces.Commands;
using MassTransit;

namespace CQRS.Domain
{
    public abstract class CommandResponderBase<TCommand> : Consumes<TCommand>.Context where TCommand : class, ICommand
    {
        protected Expression<Func<TCommand, Guid>> _expressionToGetTheMessageId;

        protected CommandResponderBase(Expression<Func<TCommand, Guid>> expressionToGetTheMessageId)
        {
            _expressionToGetTheMessageId = expressionToGetTheMessageId;
        }

        public void Consume(IConsumeContext<TCommand> context)
        {
            var response = new CommandResponse
            {
                CommandId = _expressionToGetTheMessageId.Compile()(context.Message).ToString()
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

        public abstract void Handle(TCommand command);
    }
}
