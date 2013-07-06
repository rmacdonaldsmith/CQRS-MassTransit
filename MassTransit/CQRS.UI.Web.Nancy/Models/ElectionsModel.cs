using System.Collections.Generic;
using MHM.WinFlexOne.CQRS.Dtos;

namespace CQRS.UI.Web.Nancy.Models
{
    public class ElectionsModel
    {
        public List<ElectionDto> Elections { get; set; }

        public ElectionDto SelectedElection { get; set; }

        public bool ShowElectionDetail { get; set; }
    }
}