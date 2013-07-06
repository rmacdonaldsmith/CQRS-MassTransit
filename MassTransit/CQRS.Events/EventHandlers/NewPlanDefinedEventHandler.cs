using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHM.WinFlexOne.CQRS.Dtos;
using MHM.WinFlexOne.CQRS.Interfaces.Events;
using MassTransit;
using MongoDB.Driver;

namespace MHM.WinFlexOne.CQRS.Events.EventHandlers
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
