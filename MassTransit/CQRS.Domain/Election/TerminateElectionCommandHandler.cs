using System;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Domain.Repositories;
using MHM.WinFlexOne.CQRS.Interfaces.Commands;

namespace MHM.WinFlexOne.CQRS.Domain.Election
{
    public class TerminateElectionCommandHandler : CommandResponderBase<TerminateElection>, Handles<TerminateElection>
    {
        private readonly IRepository<Election> _electionRepository;

        public TerminateElectionCommandHandler(IRepository<Election> electionRepository)
            :base(command => Guid.Parse(command.ElectionId))
        {
            _electionRepository = electionRepository;
        }

        public override void Handle(TerminateElection command)
        {
            var election = _electionRepository.GetById(Guid.Parse(command.ElectionId), int.MaxValue);

            election.TerminateElection(command);

            _electionRepository.Save(election, election.LastEventVersion); //do we need to pass in the real revision number here?
        }
    }
}
