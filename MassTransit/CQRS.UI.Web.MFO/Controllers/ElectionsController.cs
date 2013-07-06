using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CQRS.UI.Web.MFO.Models;
using MHM.WinFlexOne.CQRS.Client;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinFlexOne.CQRS.Dtos;
using MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel;

namespace CQRS.UI.Web.MFO.Controllers
{
    public class ElectionsController : Controller
    {
        private readonly ISendCommandsAndWaitForAResponse _commandSender;
        private readonly IElectionsReadModel _electionsReadModel;
        private readonly ICompaniesReadModel _companiesReadModel;

        public ElectionsController(IElectionsReadModel electionsReadModel, ICompaniesReadModel companiesReadModel, ISendCommandsAndWaitForAResponse commandSender)
        {
            _electionsReadModel = electionsReadModel;
            _companiesReadModel = companiesReadModel;
            _commandSender = commandSender;
        }

        //
        // GET: /Elections/

        public ActionResult Index()
        {
            var elections = _electionsReadModel.GetElectionsForParticipant(MvcApplication.ParticipantId);

            return View(elections);
        }

        //
        // GET: /Elections/NewHsa/

        public ActionResult NewHsa()
        {
            var model = BuildNewFsaViewModel();
            //SetElectionReasonsViewData();
            //SetPlanYearBenefitsViewData();

            return View();
        }

        //
        // POST: /Elections/NewHsa/
        [HttpPost]
        public ActionResult NewHsa(MakeAnElection command, FormCollection collection)
        {
            return View();
        }

        //
        // GET: /Elections/NewFsa/

        public ActionResult NewFsa()
        {
            var model = BuildNewFsaViewModel();
            //SetElectionReasonsViewData();

            return View(model);
        }

        //
        // POST: /Elections/NewFsa/
        [HttpPost]
        public ActionResult NewFsa(NewFsaViewModel model)
        {
            try
            {
                var command = model.Command;
                //push our command message on to the bus.
                command.ParticipantId = MvcApplication.ParticipantId;
                command.CompanyCode = MvcApplication.CompanyId;
                command.AdministratorCode = "MHMRASP";
                command.Id = Guid.NewGuid().ToString();
                CommandResponse response = _commandSender.Send(command);

                if (response.CommandStatus == CommandStatusEnum.Failed)
                {
                    //set the error message in the ViewData and return to the view
                    ModelState.AddModelError("ResponseError", response.Message);
                    var newModel = BuildNewFsaViewModel();
                    newModel.Command = command;
                    return View(newModel);
                }

                return RedirectToAction("Index");
            }
            catch (TimeoutException toe)
            {
                ModelState.AddModelError("TimeOutError", toe.Message);
                return View(BuildNewFsaViewModel());
            }
            catch
            {
                return View(BuildNewFsaViewModel());
            }
        }

        //
        //Get /Elections/Terminate

        public ActionResult Terminate(string electionId)
        {
            var terminateCommand = new TerminateElection
                {
                    ElectionId = electionId,
                    TerminationDate = DateTime.Now,
                };

            try
            {
                var response = _commandSender.Send(terminateCommand);

                if (response.CommandStatus == CommandStatusEnum.Failed)
                {
                    ModelState.AddModelError("ResponseError", response.Message);
                }

                return RedirectToAction("Index");
            }
            catch (TimeoutException toe)
            {
                ModelState.AddModelError("TimeOutError", toe.Message);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("Index");
            }
        }

        public ActionResult Details(string electionId)
        {
            var electionDto = _electionsReadModel.GetElection(electionId);

            return View(electionDto);
        }

        private NewFsaViewModel BuildNewFsaViewModel()
        {
            IEnumerable<PlanYearBenefitDto> planYearBenefits = _companiesReadModel.GetPlanYearBenefits(MvcApplication.CompanyId);
            var pybSelectList = planYearBenefits.Select(pyb => new SelectListItem { Value = pyb.Id, Text = pyb.Name });
            var electionReasons = new SelectList(new List<string>
                {
                    "Original Election",
                    "Change in Employment Status",
                    "Change in Marital Status",
                    "Change in Dependent Status",
                });

            return new NewFsaViewModel
            {
                ElectionReasons = electionReasons,
                PlanYearBenefitsOffered = pybSelectList
            };
        }

        private void SetPlanYearBenefitsViewData()
        {
            IEnumerable<PlanYearBenefitDto> planYearBenefits = _companiesReadModel.GetPlanYearBenefits(MvcApplication.CompanyId);
            var pybKeyValueList = planYearBenefits.Select(pyb => new SelectListItem{Value = pyb.Id, Text = pyb.Name});

            //ViewData["PlanYearBenefitsOffered"] = new SelectList(planYearBenefits, "Id", "Name");
        }

        private void SetElectionReasonsViewData()
        {
            ViewData["ElectionReasons"] = new SelectList(new List<string>
                {
                    "Original Election",
                    "Change in Employment Status",
                    "Change in Marital Status",
                    "Change in Dependent Status",
                });
        }
    }
}
