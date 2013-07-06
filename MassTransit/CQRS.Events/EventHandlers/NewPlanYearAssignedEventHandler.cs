using System.Collections.Generic;
using System.Linq;
using MHM.WinFlexOne.CQRS.Dtos;
using MHM.WinFlexOne.CQRS.Interfaces.Events;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MHM.WinFlexOne.CQRS.Events.EventHandlers
{
    public class NewPlanYearAssignedEventHandler : Handles<NewPlanYearAssignedEvent>, Consumes<NewPlanYearAssignedEvent>.All
    {
        private readonly MongoDatabase _mongoDataBase;

        public NewPlanYearAssignedEventHandler(MongoDatabase mongoDataBase)
        {
            _mongoDataBase = mongoDataBase;
        }

        public void Handle(NewPlanYearAssignedEvent args)
        {
            //first, create the new plan year and add to the collection
            var planYearsCollection = _mongoDataBase.GetCollection<PlanYearDto>("PlanYears");

            var newPlanYear = new PlanYearDto
                {
                    Id = args.PlanYearId,
                    Ends = args.Ends,
                    Name = args.Name,
                    PlanId = args.PlanId,
                    Starts = args.Starts,
                    Year = args.Year,
                    CompanyId = args.CompanyId,
                    Benefits = new List<string>(),
                };

            planYearsCollection.Save(newPlanYear);

            //then, add new plan year id to the PlanYears collection and save this back to mongo
            var planCollection = _mongoDataBase.GetCollection<PlanDto>("Plans");
            //var query = Query<PlanDto>.EQ(plan => plan.Id, args.PlanId);
            var query = Query.EQ("Id", args.PlanId);
            var planDto = planCollection.Find(query).FirstOrDefault();
            if (planDto != null)
            {
                if (planDto.PlanYears == null)
                {
                    planDto.PlanYears = new List<string>();
                }

                planDto.PlanYears.Add(args.PlanYearId);
            }

            planCollection.Save(planDto);
        }

        public void Consume(NewPlanYearAssignedEvent message)
        {
            Handle(message);
        }
    }
}
