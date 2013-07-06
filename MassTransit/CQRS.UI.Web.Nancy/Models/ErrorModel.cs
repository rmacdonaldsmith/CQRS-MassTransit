using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CQRS.UI.Web.Nancy.Models
{
    public class ErrorModel
    {
        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public string LinkAddress { get; set; }
    }
}