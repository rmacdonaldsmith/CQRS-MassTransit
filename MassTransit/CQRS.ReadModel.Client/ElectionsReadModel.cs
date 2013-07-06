using System.Collections.Generic;
using System.Linq;
using MHM.WinFlexOne.CQRS.Dtos;
using MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RestSharp;

namespace CQRS.ReadModel.Client
{
    public class ElectionsReadModel : IElectionsReadModel
    {
        private readonly MongoDatabase _mongoDataBase;

        public ElectionsReadModel(MongoDatabase mongoDb)
        {
            _mongoDataBase = mongoDb;
        }

        public IEnumerable<ElectionDto> GetElectionsForParticipant(string participantId)
        {
            var electionsCollection = _mongoDataBase.GetCollection<ElectionDto>("Elections");
            var query = Query.EQ("ParticipantId", participantId);
            var electionDto = electionsCollection.Find(query);

            return electionDto;
        }

        public ElectionDto GetElection(string electionId)
        {
            var electionsCollection = _mongoDataBase.GetCollection<ElectionDto>("Elections");
            var query = Query.EQ("_id", electionId);
            var electionDto = electionsCollection.Find(query).FirstOrDefault();

            return electionDto;
        }

        public ElectionBalanceDto GetElectionBalance(string electionId)
        {
            var balanceCollection = _mongoDataBase.GetCollection<ElectionBalanceDto>("ElectionBalances");
            var query = Query.EQ("ElectionId", electionId);
            var electionDto = balanceCollection.Find(query).FirstOrDefault();

            return electionDto;
        }
    }
}