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
    public class HandlesClaimDisbursedEventUpdatesElection : Handles<ClaimDisbursedEvent>, Consumes<ClaimDisbursedEvent>.All
    {
        private readonly MongoDatabase _mongoDataBase;

        public HandlesClaimDisbursedEventUpdatesElection(MongoDatabase mongoDataBase)
        {
            _mongoDataBase = mongoDataBase;
        }

        public void Consume(ClaimDisbursedEvent message)
        {
            Handle(message);
        }

        public void Handle(ClaimDisbursedEvent args)
        {
            //reduce the election balance by the claim disbursement amount
            var electionsCollection = _mongoDataBase.GetCollection<ElectionDto>("Elections");
            //var query = Query<ElectionDto>.EQ(electionDto => electionDto.Id, args.ElectionId);
            var query = Query.EQ("Id", args.ElectionId);
            var election = electionsCollection.Find(query).FirstOrDefault();

            if (election != null)
            {
                election.Balance = election.Balance - args.DisbursementAmount;
                electionsCollection.Save(election);
            }
        }
    }
}
