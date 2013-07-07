using System;
using System.Linq;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;
using MHM.WinFlexOne.CQRS.Dtos;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace CQRS.Events.EventHandlers.Elections
{
    public class HandlesElectionAmountChangedEventAndUpdatesElectionBalance : Handles<ElectionAmountChangedEvent>, Consumes<ElectionAmountChangedEvent>.All
    {
        private readonly MongoDatabase _mongoDataBase;

        public HandlesElectionAmountChangedEventAndUpdatesElectionBalance(MongoDatabase mongoDataBase)
        {
            _mongoDataBase = mongoDataBase;
        }

        public void Handle(ElectionAmountChangedEvent args)
        {
            var electionBalancesCollection = _mongoDataBase.GetCollection<ElectionBalanceDto>("ElectionBalances");
            //var query = Query<ElectionBalanceDto>.EQ(balanceDto => balanceDto.ElectionId, args.ElectionId);
            var query = Query.EQ("ElectionId", args.ElectionId);
            var electionBalanceDto = electionBalancesCollection.Find(query).FirstOrDefault();
            //check for null here (which should not be able to happen) and do something appropriate, like push a message on to an admin q
            if (electionBalanceDto == null)
            {
                throw new InvalidOperationException(string.Format("There is no election balance document for ElectionId '{0}'", args.ElectionId));
            }

            electionBalanceDto.ElectionAmount = args.NewElectionAmount;

            var safeModeResult = electionBalancesCollection.Save(electionBalanceDto);
        }

        public void Consume(ElectionAmountChangedEvent message)
        {
            Handle(message);
        }
    }
}
