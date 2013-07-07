using CQRS.Interfaces.Events;
using CQRS.Messages.Events;
using MassTransit;
using MongoDB.Driver;

namespace CQRS.Events.EventHandlers.Claims
{
    public class ClaimNotDisbursedEventHandler : Handles<ClaimNotDisbursedEvent>, Consumes<ClaimNotDisbursedEvent>.All
    {
        private readonly MongoDatabase _mongoDataBase;

        public void Handle(ClaimNotDisbursedEvent args)
        {
            var claimsNotDisbursedCollection = _mongoDataBase.GetCollection<ClaimNotDisbursedEvent>("ClaimsNotDisbursed");
            claimsNotDisbursedCollection.Insert(args);
        }

        public void Consume(ClaimNotDisbursedEvent message)
        {
            Handle(message);
        }
    }
}
