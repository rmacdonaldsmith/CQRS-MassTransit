using System;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Interfaces.Commands;
using MHM.WinFlexOne.CQRS.Interfaces.Services;
using log4net;

namespace MHM.WinflexOne.CQRS.CommandHandlerService
{
    public class CommandHandlerService : ICommandHandlerService
    {
        private readonly ILog Logger = LogManager.GetLogger(typeof (CommandHandlerService));
        private readonly ICommandDispatcher m_commandDispatcher;

        public CommandHandlerService(ICommandDispatcher commandDispatcher)
        {
            m_commandDispatcher = commandDispatcher;
        }

        public CommandResponse Dispatch(ICommand command)
        {
            try
            {
                m_commandDispatcher.Dispatch(command);

                return new CommandResponse
                           {
                               CommandStatus = CommandStatusEnum.Succeeded,
                               ContainsException = false,
                           };
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("There was an error while attempting to handle a command of type '{0}'.", command.GetType()), ex);
                return new CommandResponse
                           {
                               CommandStatus = CommandStatusEnum.Failed,
                               ContainsException = true,
                               Exception = ex.ToString(),
                               Message = ex.Message
                           };
            }
        }
    }
}
