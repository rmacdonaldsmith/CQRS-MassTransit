using System;
using CQRS.Commands;
using CQRS.Domain.Repositories;
using CQRS.Interfaces.Commands;
using CQRS.Interfaces.Services.ReadModel;

namespace CQRS.Domain.Election
{
    public class MakeAnElectionCommandHandler : CommandResponderBase<MakeAnElection>, Handles<MakeAnElection>
    {
        protected IRepository<Election> _electionRepository;
        protected IBenefitsReadModel _benefitsService;

        public MakeAnElectionCommandHandler(IRepository<Election> electionRepository, IBenefitsReadModel benefitsService)
            :base(command => Guid.Parse(command.Id))
        {
            _electionRepository = electionRepository;
            _benefitsService = benefitsService;
        }

        public override void Handle(MakeAnElection command)
        {
            var election = new Election();
            election.MakeAnElection(command, _benefitsService);

            _electionRepository.Save(election, election.LastEventVersion); //do we need to pass in the actual revision number here?
        }
    }
}
