using System;
using System.Collections.Generic;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;
using MHM.WinFlexOne.CQRS.Dtos;
using MassTransit;
using MongoDB.Driver;

namespace CQRS.Events.EventHandlers.Elections
{
    public class HandlesElectionCreatedEventAndCreatesNewElectionBalance : Handles<ElectionMadeEvent>, Consumes<ElectionMadeEvent>.All
    {
        private readonly MongoDatabase _mongoDataBase;

        public HandlesElectionCreatedEventAndCreatesNewElectionBalance(MongoDatabase mongoDataBase)
        {
            _mongoDataBase = mongoDataBase;
        }

        public void Handle(ElectionMadeEvent args)
        {
            if (_mongoDataBase.CollectionExists("ElectionBalances") == false)
            {
                _mongoDataBase.CreateCollection("ElectionBalances");
            }

            var electionBalancesCollection = _mongoDataBase.GetCollection<ElectionBalanceDto>("ElectionBalances");

            var newBalanceRecord = new ElectionBalanceDto
                {
                    Id = Guid.NewGuid().ToString(),
                    BalanceRemaining = args.ElectionAmount,
                    Claims = new List<string>(),
                    ElectionAmount = args.ElectionAmount,
                    ElectionDescription = "insert benefit description in here", //Todo: get the planyearbenefit description
                    ElectionId = args.Id,
                    ParticipantId = args.ParticipantId,
                };

            electionBalancesCollection.Insert(newBalanceRecord);
        }

        public void Consume(ElectionMadeEvent message)
        {
            Handle(message);
        }
    }
}
