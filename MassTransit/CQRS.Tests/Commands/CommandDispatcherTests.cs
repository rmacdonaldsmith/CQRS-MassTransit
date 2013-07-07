using System;
using CQRS.Common;
using CQRS.Interfaces.Commands;
using NUnit.Framework;
using Rhino.Mocks;

namespace CQRS.Tests.Commands
{
    [TestFixture]
    public class CommandDispatcherTests
    {
        [Test]
        public void Dispatch_Command_With_NoReturnValue()
        {
            //arrange
            var command = new TestCommandNoReturn();
            var commandHandler = MockRepository.GenerateMock<Handles<TestCommandNoReturn>>();
            commandHandler
                .Expect(handle => handle.Handle(command));

            ICommandDispatcher dispatcher = new CommandDispatcher();
            dispatcher.Register(commandHandler);

            //act
            dispatcher.Dispatch(command);

            //assert
            commandHandler.VerifyAllExpectations();

            //if we try to register the same command a second time we get an exception
            Assert.Throws<InvalidOperationException>(() => dispatcher.Register<TestCommandNoReturn>(commandHandler));
        }

        [Test]
        public void Dispatch_Command_With_ReturnValue()
        {
            //arrange
            var command = new TestCommandReturns();
            var commandHandler = MockRepository.GenerateMock<Handles<TestCommandReturns>>();
            commandHandler
                .Expect(handle => handle.Handle(command));

            ICommandDispatcher dispatcher = new CommandDispatcher();
            dispatcher.Register<TestCommandReturns>(commandHandler);

            //act
            dispatcher.Dispatch<TestCommandReturns>(command);

            //assert
            commandHandler.VerifyAllExpectations();

            //if we try to register the same command a second time we get an exception
            Assert.Throws<InvalidOperationException>(() => dispatcher.Register<TestCommandReturns>(commandHandler));
        }
    }
}
