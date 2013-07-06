using System;
using MHM.WinFlexOne.CQRS.Interfaces.Commands;

namespace MHM.WinFlexOne.CQRS.Messages.Faults
{
    public class CommandFault<TCommand> where TCommand : ICommand
    {
        public TCommand OriginalCommand { get; set; }

        public Exception Exception { get; set; }
    }
}
