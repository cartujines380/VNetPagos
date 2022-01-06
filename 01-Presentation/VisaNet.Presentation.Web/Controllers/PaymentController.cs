using System.Web.Mvc;
using System.Web.Routing;

namespace VisaNet.Presentation.Web.Controllers
{
    public class PaymentController : BaseController
    {
        public ActionResult Service()
        {
            return RedirectToAction("Service", "Pay", new RouteValueDictionary() { { "Area", "Pay" } });
        }

        [HttpGet]
        public ActionResult PaymentService(string serviceName)
        {
            return RedirectToAction("PaymentService", "Pay", new RouteValueDictionary() { { "serviceName", serviceName }, { "Area", "Pay" } });
        }
        
        
    }
}
