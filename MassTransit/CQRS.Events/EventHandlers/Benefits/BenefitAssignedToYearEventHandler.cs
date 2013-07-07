using System;
using System.Linq;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;
using MHM.WinFlexOne.CQRS.Dtos;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace CQRS.Events.EventHandlers.Benefits
{
    public class BenefitAssignedToYearEventHandler : Handles<BenefitAssignedToYearEvent>, Consumes<BenefitAssignedToYearEvent>.All
    {
        private readonly MongoDatabase _mongoDataBase;

        public BenefitAssignedToYearEventHandler(MongoDatabase mongoDataBase)
        {
            _mongoDataBase = mongoDataBase;
        }

        public void Handle(BenefitAssignedToYearEvent args)
        {
            var planYearBenefitsCollection = _mongoDataBase.GetCollection<PlanYearBenefitDto>("PlanYearBenefits");
            var benefitsCollection = _mongoDataBase.GetCollection<BenefitDto>("Benefits");
            //var search = Query<BenefitDto>.EQ(dto => dto.Id, args.BenefitId);
            var search = Query.EQ("Id", args.BenefitId);
            var benefit = benefitsCollection.Find(search).FirstOrDefault();

            var planYearBenefitDto = new PlanYearBenefitDto
                {
                    Id = Guid.NewGuid().ToString(),
                    CompanyId = args.CompanyId,
                    PlanId = args.PlanId,
                    BenefitId = args.BenefitId,
                    AnnualLimit = args.MaxAnnualAmount,
                    HasAnnualLimit = args.HasMaxAnnualAmount,
                    PlanYear = args.PlanYear,
                    StartDate = args.StartDate,
                    Name = args.PlanYear + " " + (benefit == null ? "" : benefit.Description),
                };

            planYearBenefitsCollection.Save(planYearBenefitDto);
        }

        public void Consume(BenefitAssignedToYearEvent message)
        {
            Handle(message);
        }
    }
}
