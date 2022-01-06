using System.Web.Mvc;

namespace VisaNet.Presentation.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OldBrowser()
        {
            return View();
        }
    }
}