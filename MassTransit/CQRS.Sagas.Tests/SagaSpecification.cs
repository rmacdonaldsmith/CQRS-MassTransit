using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHM.WinFlexOne.CQRS.Interfaces.Commands;
using MHM.WinFlexOne.CQRS.Interfaces.Events;
using NUnit.Framework;

namespace CQRS.Sagas.Tests
{
    //this test specification is slightly brittle since we are assuming that Given, When and Then will always be
    //a Command or Event collection. It is entirely plausible that e.g. When() can be a command
    public abstract class SagaSpecification
    {
        public abstract IEnumerable<ICommand> Given();

        public abstract IEnumerable<IEvent> When();

        public abstract IEnumerable<ICommand> Then();

        public abstract Exception ExpectedException();

        [Test]
        public void RunTest()
        {
            
        }
    }
}
