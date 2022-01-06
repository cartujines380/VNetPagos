using System.Web.Mvc;

namespace VisaNet.Testing.VisaNetOn.Controllers
{
    public class DoubleIframeController : Controller
    {
        public ActionResult TestPago()
        {
            return View();
        }

        public ActionResult TestPagoSinUsuario()
        {
            return View();
        }

        public ActionResult TestToken()
        {
            return View();
        }

        public ActionResult TestTokenSinUsuario()
        {
            return View();
        }

    }
}