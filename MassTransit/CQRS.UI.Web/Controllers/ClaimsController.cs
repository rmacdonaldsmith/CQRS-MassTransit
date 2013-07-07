using System;
using System.Linq;
using System.Web.Mvc;
using CQRS.Commands;
using CQRS.Common.Client;
using CQRS.Interfaces.Services.ReadModel;
using MHM.WinFlexOne.CQRS.Dtos;

namespace CQRS.UI.Web.EFO.Controllers
{
    public class ClaimsController : Controller
    {
        private const string _participantId = "empnum";
        private readonly IClaimsReadModel _claimsReadModel;
        private readonly ISendCommandsAndWaitForAResponse _commandSender;

        public ClaimsController(IClaimsReadModel claimsReadModel, ISendCommandsAndWaitForAResponse commandSender)
        {
            _claimsReadModel = claimsReadModel;
            _commandSender = commandSender;
        }

        //
        // GET: /Claims/

        public ActionResult Index()
        {
            var claims = _claimsReadModel.GetClaimsForParticipant(_participantId)
                                         .Where(claim => claim.ClaimState == ClaimStateEnum.PendingSubstantiation);

            return View("UnSubstantiatedClaimsView", claims);
        }

        public ActionResult Substantiate(string id)
        {
            var command = new VerifyCardUse
                {
                    ClaimRequestId = id,
                    Comments = "",
                };

            try
            {
                var response = _commandSender.Send(command);

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

        public ActionResult Reject(string id)
        {
            var command = new RejectClaimRequest
            {
                ClaimRequestId = id,
                Reason = "Rejected by EFO claims operative."
            };

            try
            {
                var response = _commandSender.Send(command);

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
    }
}
