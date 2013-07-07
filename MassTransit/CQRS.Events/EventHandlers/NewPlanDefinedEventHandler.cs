using System.Collections.Generic;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;
using MHM.WinFlexOne.CQRS.Dtos;
using MassTransit;
using MongoDB.Driver;

namespace CQRS.Events.EventHandlers
{
    public class NewPlanDefinedEventHandler : Handles<NewPlanDefinedEvent>, Consumes<NewPlanDefinedEvent>.All
    {
        private readonly MongoDatabase _mongoDataBase;

        public NewPlanDefinedEventHandler(MongoDatabase mongoDataBase)
        {
            _mongoDataBase = mongoDataBase;
        }

        public void Handle(NewPlanDefinedEvent args)
        {
            var plansCollection = _mongoDataBase.GetCollection<PlanDto>("Plans");

            var newPlan = new PlanDto
                {
                    CompanyId = args.CompanyId,
                    Description = args.Description,
                    Id = args.PlanId,
                    Name = args.Name,
                    PlanType = args.PlanType,
                    PlanYears = new List<string>(),
                };

            plansCollection.Save(newPlan);
        }

        public void Consume(NewPlanDefinedEvent message)
        {
            Handle(message);
        }
    }
}
