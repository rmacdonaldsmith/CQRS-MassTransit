using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MHM.WinFlexOne.CQRS.Dtos;
using MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel;

namespace MHM.WinFlexOne.CQRS.Domain.Tests
{
    public class FakeElectionsService : IElectionsReadModel
    {
        private readonly Expression<Func<string, IEnumerable<ElectionDto>>> _fakeGetElectionsForParticipant;
        private readonly Expression<Func<string, ElectionBalanceDto>> _fakeGetElectionbalance;

        public FakeElectionsService(Expression<Func<string, IEnumerable<ElectionDto>>> fakeGetElectionsForParticipant, Expression<Func<string, ElectionBalanceDto>> fakeGetElectionbalance)
        {
            _fakeGetElectionsForParticipant = fakeGetElectionsForParticipant;
            _fakeGetElectionbalance = fakeGetElectionbalance;
        }

        public IEnumerable<ElectionDto> GetElectionsForParticipant(string participantId)
        {
            return _fakeGetElectionsForParticipant.Compile().Invoke(participantId);
        }

        public ElectionDto GetElection(string electionId)
        {
            throw new NotImplementedException();
        }

        public ElectionBalanceDto GetElectionBalance(string electionId)
        {
            return _fakeGetElectionbalance.Compile()(electionId);
        }
    }
}
