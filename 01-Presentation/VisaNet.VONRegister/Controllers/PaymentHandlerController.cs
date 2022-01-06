using System;
using System.Collections.Generic;
using System.Web.Mvc;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos.Enums;
using WebGrease.Css.Extensions;

namespace VisaNet.VONRegister.Controllers
{
    public class PaymentHandlerController : BaseController
    {
        [HttpPost]
        public ActionResult Index()
        {
            NLogLogger.LogAppsEvent(NLogType.Info, string.Format("015 - (IdOperacion:({1}) RequestId:{0}", Request.Form["transaction_id"], Request.Form["req_merchant_defined_data27"]));


            var model = new Dictionary<string, string>();
            Request.Form.AllKeys.ForEach(x => model.Add(x, Request.Form[x]));
            try
            {
                var redirectTo = Int32.Parse(Request.Form["req_merchant_defined_data11"]);
                Redirect(redirectTo);
                return View(model);
            }
            catch (Exception e)
            {
                NLogLogger.LogErrorCsEvent(NLogType.Error, "PaymentHandlerController - Error");
                NLogLogger.LogErrorCsEvent(e);
            }
            return RedirectToAction("Index");
        }

        private void Redirect(int value)
        {
            if (value == (int)RedirectEnums.AppAdmission)
            {
                ViewBag.RedirecTo = @Url.Action("TokengenerationCallBack", "Home", new { area = "" });
            }
        }
    }
}