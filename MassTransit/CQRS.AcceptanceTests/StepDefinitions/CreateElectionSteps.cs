using CQRS.Commands;
using CQRS.Common;
using CQRS.Domain;
using CQRS.Domain.Election;
using CQRS.Domain.Repositories;
using CQRS.Events;
using CQRS.Messages.Events;
using NUnit.Framework;
using Rhino.Mocks;
using TechTalk.SpecFlow;

namespace CQRS.AcceptanceTests.StepDefinitions
{
    [Binding]
    public class CreateElectionSteps
    {
        private ICommandDispatcher m_commandDispatcher;
        private IEventPublisher m_eventPublisher;
        private ElectionMadeEvent m_electionCreatedEvent;

        [When(@"I issue a create election command")]
        public void WhenIIssueACreateElectionCommand()
        {
            //setup the "bus" components
            m_commandDispatcher = new CommandDispatcher();
            m_eventPublisher = new EventDispatcher();

            //register the command handler
            var repository = MockRepository.GenerateStub<IRepository<Election>>();
            m_commandDispatcher.Register(new MakeAnElectionCommandHandler(repository, null));

            //register the event handler
            m_eventPublisher.RegisterHandler<ElectionMadeEvent>(@event => m_electionCreatedEvent = @event);

            //wire-up the domain event to the event publisher
            DomainEvents.Register<ElectionMadeEvent>(@event => m_eventPublisher.Publish(@event));

            //create and send the command
            var command = new MakeAnElection
                              {
                                  AdministratorCode = "AdmCode",
                                  CompanyCode = "CoCode",
                                  ParticipantId = "12345",
                                  ElectionAmount = 1000,
                                  ElectionReason = "election reason",
                              };

            m_commandDispatcher.Dispatch<MakeAnElection>(command);
            
            Assert.Pass();
        }

        [Then(@"I should receive an election created event")]
        public void ThenIShouldReceiveAnElectionCreatedEvent()
        {
            Assert.IsNotNull(m_electionCreatedEvent);
        }
    }
}
