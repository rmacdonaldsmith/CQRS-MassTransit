using CQRS.Commands;
using CQRS.Interfaces.Commands;

namespace CQRS.Interfaces.Services
{
    public interface ICommandHandlerService
    {
        CommandResponse Dispatch(ICommand command);
    }
}
