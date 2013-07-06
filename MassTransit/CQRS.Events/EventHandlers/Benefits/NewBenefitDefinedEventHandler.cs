using MHM.WinFlexOne.CQRS.Dtos;
using MHM.WinFlexOne.CQRS.Interfaces.Events;
using MassTransit;
using MongoDB.Driver;

namespace MHM.WinFlexOne.CQRS.Events.EventHandlers.Benefits
{
    public sealed class NewBenefitDefinedEventHandler : Handles<NewBenefitDefinedEvent>, Consumes<NewBenefitDefinedEvent>.All
    {
        private readonly MongoDatabase _mongoDataBase;

        public NewBenefitDefinedEventHandler(MongoDatabase mongoDataBase)
        {
            _mongoDataBase = mongoDataBase;
        }

        public void Handle(NewBenefitDefinedEvent args)
        {
            if (_mongoDataBase.CollectionExists("Benefits") == false)
            {
                _mongoDataBase.CreateCollection("Benefits");
            }

            var benefitsCollection = _mongoDataBase.GetCollection<BenefitDto>("Benefits");

            var benefit = new BenefitDto
            {
                Id = args.BenefitId,
                BenefitType = args.BenefitType,
                Description = args.BenefitDescription,
                HasMaxElectionAmount = args.HasMaxElectionAmount,
                MaxElectionAmount = args.MaxElectionAmount,
                PlanId = args.PlanId,
                CompanyId = args.CompanyId,
            };

            var safeModeResult = benefitsCollection.Save(benefit);
            if (!safeModeResult.Ok)
            {
                //log!
                //push a message to an admin q?
            }
        }

        public void Consume(NewBenefitDefinedEvent message)
        {
            Handle(message);
        }
    }
}
