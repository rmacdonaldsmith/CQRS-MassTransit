using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CQRS.UI.Web.Infrastructure;
using MHM.WinFlexOne.CQRS.Client;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel;

namespace CQRS.UI.Web.EFO.Controllers
{
    public class PlansController : Controller
    {
        private readonly ISendCommandsAndWaitForAResponse _commandSender;
        private readonly ICompaniesReadModel _companiesReadModel;

        public PlansController(ICompaniesReadModel companiesReadModel, ISendCommandsAndWaitForAResponse commandSender)
        {
            _companiesReadModel = companiesReadModel;
            _commandSender = commandSender;
        }


        //
        // GET: /Plans/

        public ActionResult Index()
        {
            var plans = _companiesReadModel.GetPlansForCompany(MvcApplication.CompanyId);

            return View(plans);
        }

        //
        // GET: /Plans/PlanYearList/

        public ActionResult PlanYearList(string planId)
        {
            var planYears = _companiesReadModel.GetPlanYearsForPlan(planId);
            
            return View("PlanYearList", planYears);
        }

        //
        // GET: /Plans/DefinePlan

        public ActionResult DefinePlan()
        {
            ViewData["PlanTypes"] = new SelectList(GetPlanTypes());
            return View("DefinePlan");
        }

        //
        //POST: /Plans/DefinePlan

        [HttpPost]
        public ActionResult DefinePlan(DefineNewPlan command, FormCollection formCollection)
        {
            command.PlanId = Guid.NewGuid().ToString();
            command.CompanyId = MvcApplication.CompanyId;

            try
            {
                var response = _commandSender.Send(command);
                if (response.CommandStatus == CommandStatusEnum.Failed)
                {
                    //set the error message in the ViewData and return to the view
                    ModelState.AddModelError("ResponseError", response.Message);
                    ViewData["PlanTypes"] = new SelectList(GetPlanTypes());
                    return View(command);
                }

                return RedirectToAction("Index");
            }
            catch (TimeoutException toe)
            {
                ModelState.AddModelError("TimeOutError", toe.Message);
                ViewData["PlanTypes"] = new SelectList(GetPlanTypes());
                return View(command);
            }
            catch
            {
                return View();
            }
        }

        private IEnumerable<string> GetPlanTypes()
        {
            return new List<string>
                {
                    "Section 125 - FSA",
                    "Section 105 - HRA",
                    "Section 132 - Parking/Trans",
                    "HSA",
                };
        }

        //
        //GET: /Plans/DefinePlanYear

        public ActionResult DefinePlanYear(string planId, string companyId)
        {
            //this command is going to be our model for the view
            //therefore, we can set some properties here and they will be available to the view
            //the planId and companyId came from the selected row in the list of plans view.
            var defineYearCommand = new DefineYearForPlan
                {
                    PlanId = planId,
                    CompanyId = companyId,
                };

            return View(defineYearCommand);
        }

        //
        //POST: /Plans/DefinePlanYear

        [HttpPost]
        public ActionResult DefinePlanYear(DefineYearForPlan command, FormCollection formCollection)
        {
            command.PlanYearId = Guid.NewGuid().ToString();

            try
            {
                var response = _commandSender.Send(command);
                if (response.CommandStatus == CommandStatusEnum.Failed)
                {
                    //set the error message in the ViewData and return to the view
                    ModelState.AddModelError("ResponseError", response.Message);
                    return View(command);
                }

                return RedirectToAction("Index");
            }
            catch (TimeoutException toe)
            {
                ModelState.AddModelError("TimeOutError", toe.Message);
                return View(command);
            }
            catch
            {
                return View();
            }
        }
    }
}
