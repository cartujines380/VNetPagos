using System.Web.Mvc;
using VisaNet.VONRegister.Constants;
using VisaNet.VONRegister.Models;

namespace VisaNet.VONRegister.Controllers
{
    public class ErrorController : BaseController
    {
        //
        // GET: /Error/
        public ActionResult Index()
        {
            var urlCallback = Session[SessionConstants.CallbackUrl] as string;
            
            if (string.IsNullOrEmpty(urlCallback))
            {
                return View("Error");    
            }

            var resultModel = new End
            {
                OperationId = Session[SessionConstants.OperationId] as string,
                UrlCallback = urlCallback,
                ResultCode = "-1",
                ResultDescription = "ERROR GENERAL"
            };

            ClearSessionVariables();
            return RedirectToAction("End", "Home", resultModel);
        }

        public ActionResult OldBrowser()
        {
            return View("Error");
        }
	}
}