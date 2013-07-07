using System.Linq;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;
using MHM.WinFlexOne.CQRS.Dtos;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace CQRS.Events.EventHandlers.Claims
{
    public sealed class CardUseVerifiedEventHandler : Handles<CardUseVerifiedEvent>, Consumes<CardUseVerifiedEvent>.All
    {
        private readonly MongoDatabase _mongoDataBase;

        public CardUseVerifiedEventHandler(MongoDatabase mongoDataBase)
        {
            _mongoDataBase = mongoDataBase;
        }

        public void Handle(CardUseVerifiedEvent args)
        {
            var claimsCollection = _mongoDataBase.GetCollection<ClaimDto>("Claims");
            //var query = Query<ClaimDto>.EQ(claim => claim.Id, args.ClaimRequestId);
            var query = Query.EQ("Id", args.ClaimRequestId);
            var claimDto = claimsCollection.Find(query).FirstOrDefault();
            //check for null here (which should not be able to happen) and do something appropriate, like push a message on to an admin q
            claimDto.ClaimState = ClaimStateEnum.Substantiated;

            var safeModeResult = claimsCollection.Save(claimDto);
        }

        public void Consume(CardUseVerifiedEvent message)
        {
            Handle(message);
        }
    }
}
