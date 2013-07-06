using System.Collections.Generic;
using System.Web.Mvc;
using MHM.WinFlexOne.CQRS.Commands;

namespace CQRS.UI.Web.MFO.Models
{
    public class SubmitClaimViewModel
    {
        public SelectList ClaimTypes { get; set; }

        public SubmitClaimRequest Command { get; set; }
    }
}