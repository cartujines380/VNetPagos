using System;
using System.Web.Mvc;

namespace VisaNet.Presentation.VisaNetOn.Controllers
{
    public class AvailabilityController : Controller
    {
        public ActionResult Check()
        {
            var now = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return Json(now, JsonRequestBehavior.AllowGet);
        }

    }
}