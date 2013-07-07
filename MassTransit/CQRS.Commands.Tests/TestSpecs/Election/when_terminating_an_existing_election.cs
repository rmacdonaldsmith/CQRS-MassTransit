using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CQRS.Commands;
using CQRS.Domain.Election;
using CQRS.Domain.Repositories;
using CQRS.DomainTesting;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;
using NUnit.Framework;

namespace CQRS.Domain.Tests.TestSpecs.Election
{
    [TestFixture]
    public class when_terminating_an_existing_election : EventSpecification<TerminateElection>
    {
        private readonly Guid m_electionId = Guid.NewGuid();
        private readonly DateTime m_terminateDate = DateTime.Now.AddDays(-20);
        /// <summary>
        /// Setup - these events need to have occured to get our domain object in to a valid initial state
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IEvent> Given()
        {
            yield return new ElectionMadeEvent
                       {
                           AdministratorCode = "mhmrasp",
                           CompanyCode = "mhm",
                           ParticipantId = "empnum",
                           ElectionAmount = 1200,
                           ElectionReason = "reason",
                           Id = m_electionId.ToString(),
                           PerPayPeriodAmount = 100,
                       };
        }

        /// <summary>
        /// The command to apply to the domain object
        /// </summary>
        /// <returns></returns>
        public override TerminateElection When()
        {
            return new TerminateElection
                       {
                           ElectionId = m_electionId.ToString(),
                           TerminationDate = m_terminateDate,
                       };
        }

        /// <summary>
        /// The command handler that will initialize the domin object and invoke the command on the domain object
        /// </summary>
        /// <returns></returns>
        public override Interfaces.Commands.Handles<TerminateElection> BuildCommandHandler()
        {
            return new TerminateElectionCommandHandler(new Repository<global::CQRS.Domain.Election.Election>(EventStore));
        }

        /// <summary>
        /// The events that we expect to the generated when the command is applied to the domain object
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IEvent> Then()
        {
            yield return new ElectionTerminatedEvent
                             {
                                 ElectionId = m_electionId.ToString(),
                                 TerminatedDate = m_terminateDate,
                                 Version = 1
                             };
        }

        public override Expression<Predicate<Exception>> ThenException()
        {
            return exception => exception == null;
        }

        /// <summary>
        /// Tear down and cleanup when the test has finished
        /// </summary>
        public override void Finally()
        {
            //nothing to do
        }
    }
}
