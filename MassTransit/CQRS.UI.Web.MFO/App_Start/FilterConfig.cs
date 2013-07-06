using System.Web;
using System.Web.Mvc;

namespace CQRS.UI.Web.MFO
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}