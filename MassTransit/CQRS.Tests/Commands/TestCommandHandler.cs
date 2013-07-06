using System;
using MHM.WinFlexOne.CQRS.Interfaces.Commands;

namespace CQRS.Tests.Commands
{
    public class TestCommandHandler : Handles<TestCommandNoReturn>
    {
        public void Handle(TestCommandNoReturn command)
        {
            throw new NotImplementedException();
        }
    }
}
