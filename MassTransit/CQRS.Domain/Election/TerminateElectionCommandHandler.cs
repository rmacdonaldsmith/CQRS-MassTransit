using System;
using CQRS.Commands;
using CQRS.Domain.Repositories;
using CQRS.Interfaces.Commands;

namespace CQRS.Domain.Election
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
