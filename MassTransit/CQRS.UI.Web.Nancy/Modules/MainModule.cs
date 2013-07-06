using Nancy;

namespace CQRS.UI.Web.Nancy.Modules
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = parameters =>
                           {
                               parameters.Title = "MFO CQRS Home Page";
                               return View["index", parameters];
                           };

            Get["/index"] = parameters =>
            {
                parameters.Title = "MFO CQRS Home Page";
                return View["index", parameters];
            };
        }
    }
}