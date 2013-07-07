using System.Linq;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;
using MHM.WinFlexOne.CQRS.Dtos;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace CQRS.Events.EventHandlers.Elections
{
    public class ElectionTerminatedEventHandler : Handles<ElectionTerminatedEvent>, Consumes<ElectionTerminatedEvent>.All
    {
        private readonly MongoDatabase m_mongoDataBase;

        public ElectionTerminatedEventHandler(MongoDatabase mongoDataBase)
        {
            m_mongoDataBase = mongoDataBase;
        }

        public void Handle(ElectionTerminatedEvent args)
        {
            //update the current record to reflect termination of this election
            var electionCollection = m_mongoDataBase.GetCollection<ElectionDto>("Elections");
            //var query = Query<ElectionDto>.EQ(election => election.Id, args.ElectionId);
            var query = Query.EQ("Id", args.ElectionId);
            var electionDto = electionCollection.Find(query).FirstOrDefault();
            //check for null here (which should not be able to happen) and do something appropriate, like push a message on to an admin q
            electionDto.IsTerminated = true;
            electionDto.TerminationDate = args.TerminatedDate;

            var safeModeResult = electionCollection.Save(electionDto);
        }

        public void Consume(ElectionTerminatedEvent message)
        {
            Handle(message);
        }
    }
}
