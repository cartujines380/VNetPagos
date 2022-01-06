using System.Web.Mvc;
using System.Web.Routing;

namespace VisaNet.Presentation.Web.Controllers
{
    [RoutePrefix("Servicio")]
    public class ServiceController : BaseController
    {
        // GET: Servicio
        [HttpGet]
        [Route("{idapp}")]
        public ActionResult Index(string idapp)
        {
            return RedirectToAction("PaymentService", "Pay", new RouteValueDictionary() { { "serviceName", idapp }, { "Area", "Pay" } });
        }

        // GET: Servicio
        public ActionResult Index()
        {
            var id = Request.Form["serName"];
            return RedirectToAction("PaymentService", "Pay", new RouteValueDictionary() { { "serviceName", id }, { "Area", "Pay" } });
        }
    }
}