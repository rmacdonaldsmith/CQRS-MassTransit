namespace MHM.WinFlexOne.CQRS.Interfaces.Commands
{
    public interface Handles<in TCommand, out TResult> where TCommand : ICommand
    {
        TResult Handle(TCommand command);
    }

    public interface Handles<in TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
}