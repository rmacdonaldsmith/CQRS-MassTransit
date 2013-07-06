using MHM.WinFlexOne.CQRS.Dtos;
using MHM.WinFlexOne.CQRS.Interfaces.Events;
using MassTransit;
using MongoDB.Driver;

namespace MHM.WinFlexOne.CQRS.Events.EventHandlers.Claims
{
    public class ClaimRequestAutoSubstantiatedEventHandler : Handles<ClaimRequestAutoSubstantiatedEvent>, Consumes<ClaimRequestAutoSubstantiatedEvent>.All
    {
        private readonly MongoDatabase _mongoDataBase;

        public ClaimRequestAutoSubstantiatedEventHandler(MongoDatabase mongoDataBase)
        {
            _mongoDataBase = mongoDataBase;
        }

        public void Handle(ClaimRequestAutoSubstantiatedEvent args)
        {
            var claimsCollection = _mongoDataBase.GetCollection<ClaimDto>("Claims");

            var claim = new ClaimDto
                {
                    ClaimAmount = args.Amount,
                    ClaimType = args.ClaimType,
                    Id = args.ClaimRequestId,
                    ParticipantId = args.ParticipantId,
                    Provider = args.ProviderName,
                    ServiceDate = args.DateOfTransaction,
                    AmountPaid = 0.00m,
                    ClaimState = ClaimStateEnum.Substantiated,
                    Source = args.Source,
                };

            var safeModeResult = claimsCollection.Save(claim);
        }

        public void Consume(ClaimRequestAutoSubstantiatedEvent message)
        {
            Handle(message);
        }
    }
}
