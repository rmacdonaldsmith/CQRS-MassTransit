using System.Linq;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;
using MHM.WinFlexOne.CQRS.Dtos;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace CQRS.Events.EventHandlers.Elections
{
    public class ElectionAmountChangedEventHandler : Handles<ElectionAmountChangedEvent>, Consumes<ElectionAmountChangedEvent>.All
    {
        private readonly MongoDatabase _mongoDataBase;

        public ElectionAmountChangedEventHandler(MongoDatabase mongoDataBase)
        {
            _mongoDataBase = mongoDataBase;
        }

        public void Handle(ElectionAmountChangedEvent args)
        {
            var electionCollection = _mongoDataBase.GetCollection<ElectionDto>("Elections");
            //var query = Query<ElectionDto>.EQ(election => election.Id, args.ElectionId);
            var query = Query.EQ("Id", args.ElectionId);
            var electionDto = electionCollection.Find(query).FirstOrDefault();
            //check for null here (which should not be able to happen) and do something appropriate, like push a message on to an admin q
            electionDto.ElectionAmount = args.NewElectionAmount;
            electionDto.QualifyingEvent = args.QualifyingEvent;

            var safeModeResult = electionCollection.Save(electionDto);
        }

        public void Consume(ElectionAmountChangedEvent message)
        {
            Handle(message);
        }
    }
}
