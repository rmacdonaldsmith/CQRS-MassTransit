using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHM.WinFlexOne.CQRS.Dtos;
using MHM.WinFlexOne.CQRS.Interfaces.Events;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MHM.WinFlexOne.CQRS.Events.EventHandlers.Claims
{
    public class ClaimDisbursedEventHandler : Handles<ClaimDisbursedEvent>, Consumes<ClaimDisbursedEvent>.All
    {
        private readonly MongoDatabase _mongoDataBase;

        public ClaimDisbursedEventHandler(MongoDatabase mongoDataBase)
        {
            _mongoDataBase = mongoDataBase;
        }

        public void Handle(ClaimDisbursedEvent args)
        {
            //reduce the election balance record by the claim disbursement amount
            var electionBalanceCollection = _mongoDataBase.GetCollection<ElectionBalanceDto>("ElectionBalances"); 
            //var query = Query<ElectionBalanceDto>.EQ(balance => balance.ElectionId, args.ElectionId);
            var query = Query.EQ("ElectionId", args.ElectionId);
            var electionBalance = electionBalanceCollection.Find(query).FirstOrDefault();

            if (electionBalance != null)
            {
                electionBalance.BalanceRemaining = electionBalance.BalanceRemaining - args.DisbursementAmount;
                electionBalanceCollection.Save(electionBalance);
            }
        }

        public void Consume(ClaimDisbursedEvent message)
        {
            Handle(message);
        }
    }
}
