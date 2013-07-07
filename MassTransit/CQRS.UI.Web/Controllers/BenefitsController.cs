using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CQRS.Commands;
using CQRS.Common.Client;
using CQRS.Interfaces.Services.ReadModel;
using MHM.WinFlexOne.CQRS.Dtos;

namespace CQRS.UI.Web.EFO.Controllers
{
    public class BenefitsController : Controller
    {
        private readonly ISendCommandsAndWaitForAResponse _commandSender;
        private readonly IBenefitsReadModel _benefitsReadModel;
        private readonly ICompaniesReadModel _companiesReadModel;

        public BenefitsController(ISendCommandsAndWaitForAResponse commandSender, IBenefitsReadModel benefitsReadModel, ICompaniesReadModel companiesReadModel)
        {
            _commandSender = commandSender;
            _benefitsReadModel = benefitsReadModel;
            _companiesReadModel = companiesReadModel;
        }

        //
        // GET: /Benefits/

        public ActionResult List()
        {
            //make a call to our read model to get back the list of benefits for this company
            var benefits = _benefitsReadModel.GetBenefits();

            return View(benefits);
        }

        //
        // GET: /Benefits/Create

        public ActionResult Create()
        {
            //we would get this data from the read model
            var plansOffered = GetPlansOffered();

            //we would get this data from the read model
            var benefitTypes = GetBenefitTypes();
            
            ViewData["PlansOffered"] = new SelectList(plansOffered, "Id", "Name");
            ViewData["BenefitType"] = new SelectList(benefitTypes);

            return View();
        }

        private static List<string> GetBenefitTypes()
        {
            var benefitTypes = new List<string>
                {
                    "Dental",
                    "Medical",
                    "Vision",
                    "Commuter"
                };
            return benefitTypes;
        }

        private IEnumerable<PlanDto> GetPlansOffered()
        {
            return _companiesReadModel.GetPlansForCompany(MvcApplication.CompanyId);
        }

        //
        // POST: /Benefits/Create

        [HttpPost]
        public ActionResult Create(DefineNewBenefit command, FormCollection collection)
        {
            if (ModelState.IsValid == false)
            {
                return View();
            }

            try
            {
                //push our DefineNewBenefit command message on to the bus.
                command.BenefitId = Guid.NewGuid().ToString();
                command.CompanyId = MvcApplication.CompanyId;
                CommandResponse response = _commandSender.Send(command);

                if (response.CommandStatus == CommandStatusEnum.Failed)
                {
                    //set the error message in the ViewData and return to the view
                    ModelState.AddModelError("ResponseError", response.Message);
                    ViewData["PlansOffered"] = new SelectList(GetPlansOffered(), "Id", "Name");
                    ViewData["BenefitType"] = new SelectList(GetBenefitTypes());
                    return View();
                }

                return RedirectToAction("List");
            }
            catch (TimeoutException toe)
            {
                ModelState.AddModelError("TimeOutError", toe.Message);
                ViewData["PlansOffered"] = new SelectList(GetPlansOffered(), "Id", "Name");
                ViewData["BenefitType"] = new SelectList(GetBenefitTypes());
                return View();
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Benefits/ListPlanYearBenefits

        public ActionResult ListPlanYearBenefits(string id)
        {
            throw new NotImplementedException();
        }

        //
        //GET: /Benefits/AssignToYear

        public ActionResult AssignToYear(string benefitId, string planId)
        {
            SetUpBenefitViewData(benefitId);
            SetUpPlanYearsViewData(planId);

            return View();
        }

        private void SetUpBenefitViewData(string benefitId)
        {
            var benefitToAssign = _benefitsReadModel.GetBenefits().FirstOrDefault(benefit => benefit.Id == benefitId);
            ViewData["Benefit"] = benefitToAssign;
            ViewData["CompanyId"] = benefitToAssign.CompanyId;
        }

        private void SetUpPlanYearsViewData(string planId)
        {
            //get the available plan years that have been setup for the plan associated with the incoming plan
            var planYearCollection = _companiesReadModel.GetPlanYearsForPlan(planId);
            //derive a list of years
            var planYears = planYearCollection.Select(planyear => planyear.Year).ToList();
            ViewData["PlanYears"] = new SelectList(planYears);
        }

        //
        //POST: /Benefits/AssignToYear
        [HttpPost]
        public ActionResult AssignToYear(AssignBenefitToYear command, FormCollection formCollection)
        {
            try
            {
                //push our DefineNewBenefit command message on to the bus.
                command.CompanyId = MvcApplication.CompanyId;
                CommandResponse response = _commandSender.Send(command);

                if (response.CommandStatus == CommandStatusEnum.Failed)
                {
                    //set the error message in the ViewData and return to the view
                    ModelState.AddModelError("ResponseError", response.Message);
                    SetUpBenefitViewData(command.BenefitId);
                    SetUpPlanYearsViewData(command.PlanId);
                    return View();
                }

                return RedirectToAction("List");
            }
            catch (TimeoutException toe)
            {
                ModelState.AddModelError("TimeOutError", toe.Message);
                SetUpBenefitViewData(command.BenefitId);
                SetUpPlanYearsViewData(command.PlanId);
                return View();
            }
            catch
            {
                SetUpBenefitViewData(command.BenefitId);
                SetUpPlanYearsViewData(command.PlanId);
                return View();
            }
        }
    }
}
