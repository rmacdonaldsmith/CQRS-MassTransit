using CQRS.Interfaces.Events;
using CQRS.Messages.Events;
using MHM.WinFlexOne.CQRS.Dtos;
using MassTransit;
using MongoDB.Driver;

namespace CQRS.Events.EventHandlers.Claims
{
    public sealed class ClaimRequestSubmittedEventHandler : Handles<ClaimRequestSubmittedEvent>, Consumes<ClaimRequestSubmittedEvent>.All
    {
        private readonly MongoDatabase _mongoDataBase;

        public ClaimRequestSubmittedEventHandler(MongoDatabase mongoDataBase)
        {
            _mongoDataBase = mongoDataBase;
        }

        public void Handle(ClaimRequestSubmittedEvent args)
        {
            if (_mongoDataBase.CollectionExists("Claims") == false)
            {
                _mongoDataBase.CreateCollection("Claims");
            }

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

        public void Consume(ClaimRequestSubmittedEvent message)
        {
            Handle(message);
        }
    }
}
