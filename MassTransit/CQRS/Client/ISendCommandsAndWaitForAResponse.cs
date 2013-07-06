using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Interfaces.Commands;

namespace MHM.WinFlexOne.CQRS.Client
{
    public interface ISendCommandsAndWaitForAResponse
    {
        CommandResponse Send<TCommand>(TCommand command) where TCommand : class, ICommand;
    }
}
