using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CQRS.UI.Web.MFO.Models;
using MHM.WinFlexOne.CQRS.Client;
using MHM.WinFlexOne.CQRS.Commands;
using MHM.WinflexOne.CQRS.Interfaces.Services.ReadModel;

namespace CQRS.UI.Web.MFO.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly ISendCommandsAndWaitForAResponse _commandSender;
        private readonly IClaimTypesReadModel _claimTypesReadModel;
        private readonly IClaimsReadModel _claimsReadModel;

        public ClaimsController(ISendCommandsAndWaitForAResponse commandSender, IClaimTypesReadModel claimTypesReadModel, IClaimsReadModel claimsReadModel)
        {
            _commandSender = commandSender;
            _claimTypesReadModel = claimTypesReadModel;
            _claimsReadModel = claimsReadModel;
        }

        //
        // GET: /Claims/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewClaims()
        {
            var claims = _claimsReadModel.GetClaimsForParticipant(MvcApplication.ParticipantId);

            return View(claims);
        }

        //
        // GET: /Claims/SubmitClaim
        public ActionResult SubmitClaim()
        {
            var viewModel = new SubmitClaimViewModel
                {
                    ClaimTypes = BuildClaimTypesSelectList(_claimTypesReadModel),
                };

            return View(viewModel);
        }

        //
        //GET: /Claims/SubmitClaim
        [HttpPost]
        public ActionResult SubmitClaim(SubmitClaimViewModel model)
        {
            try
            {
                var command = model.Command;
                command.ParticipantId = MvcApplication.ParticipantId;
                command.CompanyId = MvcApplication.CompanyId;
                command.ClaimRequestId = Guid.NewGuid().ToString();
                command.Source = "MFO";
                CommandResponse response = _commandSender.Send(command);

                if (response.CommandStatus == CommandStatusEnum.Failed)
                {
                    //set the error message in the ViewData and return to the view
                    ModelState.AddModelError("ResponseError", response.Message);
                    var newModel = new SubmitClaimViewModel
                        {
                            Command = command,
                            ClaimTypes = BuildClaimTypesSelectList(_claimTypesReadModel),
                        };
                    newModel.Command = command;
                    return View(newModel);
                }

                return RedirectToAction("Index");
            }
            catch (TimeoutException toe)
            {
                ModelState.AddModelError("TimeOutError", toe.Message);
                return View(new SubmitClaimViewModel
                    {
                        ClaimTypes = BuildClaimTypesSelectList(_claimTypesReadModel)
                    });
            }
            catch
            {
                return View(new SubmitClaimViewModel
                    {
                        ClaimTypes = BuildClaimTypesSelectList(_claimTypesReadModel)
                    });
            }
        }

        private SelectList BuildClaimTypesSelectList(IClaimTypesReadModel claimTypesReadModel)
        {
            return new SelectList(new List<string>
                {
                    "Dental Copay",
                    "Medical Copay",
                    "Vision Copay",
                    "Medical Coinsurance"
                });
        }
    }
}
