using System;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Domain.Repositories;
using MHM.WinFlexOne.CQRS.Interfaces.Commands;
using MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel;

namespace MHM.WinFlexOne.CQRS.Domain.Election
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
