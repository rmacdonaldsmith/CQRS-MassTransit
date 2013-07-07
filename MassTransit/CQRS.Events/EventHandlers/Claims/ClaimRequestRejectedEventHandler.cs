using System.Linq;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;
using MHM.WinFlexOne.CQRS.Dtos;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace CQRS.Events.EventHandlers.Claims
{
    public sealed class ClaimRequestRejectedEventHandler : Handles<ClaimRequestRejectedEvent>, Consumes<ClaimRequestRejectedEvent>.All
    {
        private readonly MongoDatabase _mongoDataBase;

        public ClaimRequestRejectedEventHandler(MongoDatabase mongoDataBase)
        {
            _mongoDataBase = mongoDataBase;
        }

        public void Handle(ClaimRequestRejectedEvent args)
        {
            var claimsCollection = _mongoDataBase.GetCollection<ClaimDto>("Claims");
            //var query = Query<ClaimDto>.EQ(claim => claim.Id, args.ClaimRequestId);
            var query = Query.EQ("Id", args.ClaimRequestId);
            var claimDto = claimsCollection.Find(query).FirstOrDefault();
            //check for null here (which should not be able to happen) and do something appropriate, like push a message on to an admin q
            claimDto.ClaimState = ClaimStateEnum.Rejected;
            claimDto.RejectReason = args.RejectReason;

            var safeModeResult = claimsCollection.Save(claimDto);
        }

        public void Consume(ClaimRequestRejectedEvent message)
        {
            Handle(message);
        }
    }
}
