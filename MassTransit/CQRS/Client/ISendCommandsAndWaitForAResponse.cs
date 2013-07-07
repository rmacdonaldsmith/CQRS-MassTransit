using CQRS.Commands;
using CQRS.Interfaces.Commands;

namespace CQRS.Common.Client
{
    public interface ISendCommandsAndWaitForAResponse
    {
        CommandResponse Send<TCommand>(TCommand command) where TCommand : class, ICommand;
    }
}
