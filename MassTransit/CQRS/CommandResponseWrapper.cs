using System;
using CQRS.Commands;
using CQRS.Interfaces.Commands;
using MassTransit;

namespace CQRS.Common
{
    public class CommandResponseWrapper<TCommand> : Consumes<TCommand>.Context where TCommand : class, ICommand
    {
        private readonly Handles<TCommand> _commandHandlerToWrap;
        private readonly Func<TCommand, string> _getMessageIdDelegate;

        public CommandResponseWrapper(Handles<TCommand> commandHandlerToWrap, Func<TCommand, string> getMessageIdDelegate)
        {
            _commandHandlerToWrap = commandHandlerToWrap;
            _getMessageIdDelegate = getMessageIdDelegate;
        }

        public void Consume(IConsumeContext<TCommand> context)
        {
            var response = new CommandResponse
                {
                    CommandId = _getMessageIdDelegate(context.Message),
                };

            try
            {
                _commandHandlerToWrap.Handle(context.Message);

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
