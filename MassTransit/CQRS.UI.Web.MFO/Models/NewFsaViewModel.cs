using System.Collections.Generic;
using System.Web.Mvc;
using MHM.WinFlexOne.CQRS.Commands;

namespace CQRS.UI.Web.MFO.Models
{
    public class NewFsaViewModel
    {
        public IEnumerable<SelectListItem> PlanYearBenefitsOffered { get; set; }

        public SelectList ElectionReasons { get; set; }

        public MakeAnElection Command { get; set; }
    }
}