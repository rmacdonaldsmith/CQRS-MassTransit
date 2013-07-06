using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Interfaces.Commands;

namespace MHM.WinFlexOne.CQRS.Interfaces.Services
{
    public interface ICommandHandlerService
    {
        CommandResponse Dispatch(ICommand command);
    }
}
