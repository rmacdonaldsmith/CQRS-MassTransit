using System.Linq;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;
using MHM.WinFlexOne.CQRS.Dtos;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace CQRS.Events.EventHandlers.Elections
{
    public class ElectionCreatedEventHandler : Handles<ElectionMadeEvent>, Consumes<ElectionMadeEvent>.All
    {
        private readonly MongoDatabase _mongoDataBase;

        public ElectionCreatedEventHandler(MongoDatabase mongoDataBase)
        {
            _mongoDataBase = mongoDataBase;
        }

        public void Handle(ElectionMadeEvent args)
        {
            if(_mongoDataBase.CollectionExists("Elections") == false)
            {
                _mongoDataBase.CreateCollection("Elections");
            }

            //get the PlanId, PlanYear 
            var planYearBenefitCollection = _mongoDataBase.GetCollection<PlanYearBenefitDto>("PlanYearBenefits");
            //var query = Query<PlanYearBenefitDto>.Where(pyb => pyb.Id == args.PlanYearBenefitId);
            var query = Query.EQ("Id", args.PlanYearBenefitId);
            var planYearBenefit = planYearBenefitCollection.Find(query).FirstOrDefault();
            var planId = planYearBenefit.PlanId;
            var planYear = planYearBenefit.PlanYear;
            var benefitId = planYearBenefit.BenefitId;

            //BenefitCode and BenefitType from other collections
            var benefitsCollection = _mongoDataBase.GetCollection<BenefitDto>("Benefits");
            //var benefitQuery = Query<BenefitDto>.Where(benefitDto => benefitDto.Id == benefitId);
            var benefitQuery = Query.EQ("Id", benefitId);
            var benefit = benefitsCollection.Find(benefitQuery).FirstOrDefault();

            var electionCollection = _mongoDataBase.GetCollection<ElectionDto>("Elections");

            var electionDto = new ElectionDto
                                  {
                                      Id = args.Id,
                                      CompanyId = args.CompanyCode,
                                      BenefitCode = benefit.Id,
                                      BenefitType = benefit.BenefitType,
                                      ElectionAmount = args.ElectionAmount,
                                      Balance = args.ElectionAmount,
                                      QualifyingEvent = args.ElectionReason,
                                      ParticipantId = args.ParticipantId,
                                      PerPayPeriodAmount = args.PerPayPeriodAmount,
                                      IsTerminated = false,
                                      PlanCode = planId,
                                      PlanYear = planYear,
                                      PlanYearBenefitId = args.PlanYearBenefitId,
                                      PlanYearBenefitDesc = planYearBenefit.Name,
                                  };

            var safeModeResult = electionCollection.Save(electionDto);
            //use the safe mode result to test if the upsert was ok.
            //if the upsert failed, then we can generate a message (containing this event) and put it back on the bus
            //there could be an admin console that monitors this error q so that people know when upserts fail.
        }

        public void Consume(ElectionMadeEvent message)
        {
            Handle(message);
        }
    }
}
