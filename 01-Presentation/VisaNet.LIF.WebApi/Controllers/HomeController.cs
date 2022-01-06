using System.Web.Mvc;

namespace VisaNet.LIF.WebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Help", new { area = "HelpPage" });
        }
    }
}