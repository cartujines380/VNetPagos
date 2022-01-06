using System;
using System.Collections.Generic;
using System.Web.Mvc;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos.Enums;
using WebGrease.Css.Extensions;

namespace VisaNet.Presentation.Web.Controllers
{
    public class PaymentHandlerController : Controller
    {
        [HttpPost]
        public ActionResult Index()
        {
            NLogLogger.LogEvent(NLogType.Info, "PaymentHandlerController - Llega comunicaccion de CS");
            NLogLogger.LogEvent(NLogType.Info, "PaymentHandlerController - RequestId: " + Request.Form["transaction_id"]);

            var model = new Dictionary<string, string>();
            Request.Form.AllKeys.ForEach(x => model.Add(x, Request.Form[x]));
            try
            {
                CheckError(model);
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
            if (value == (int)RedirectEnums.PrivateAssosiate)
            {
                ViewBag.RedirecTo = @Url.Action("TokengenerationCallBack", "Service", new { area = "private" });
            }
            if (value == (int)RedirectEnums.Payment || value == (int)RedirectEnums.VisanetMobilePayment)
            {
                ViewBag.RedirecTo = @Url.Action("TokengenerationCallBack", "Pay", new { area = "Pay" });
            }
            
            if (value == (int)RedirectEnums.PrivateAddCardToUser)
            {
                ViewBag.RedirecTo = @Url.Action("AssociateCardToUser", "Card", new { area = "private" });
            }
            if (value == (int)RedirectEnums.HighwayAdmission)
            {
                ViewBag.RedirecTo = @Url.Action("TokengenerationCallBack", "HighwayAdmission", new { area = "" });
            }
            if (value == (int)RedirectEnums.AppAdmission)
            {
                ViewBag.RedirecTo = @Url.Action("TokengenerationCallBack", "AppAdmission", new { area = "" });
            }
            if (value == (int)RedirectEnums.Debit)
            {
                ViewBag.RedirecTo = @Url.Action("TokengenerationCallBack", "Debit", new { area = "Debit" });
            }
        }

        private void CheckError(Dictionary<string, string> data)
        {
            var code = string.Empty;
            var resultCode = data.TryGetValue("reason_code", out code);

            if (resultCode && !string.IsNullOrEmpty(code) && !code.Equals(100))
            {
                var invalidFields = string.Empty;
                var resultinvalidFields = data.TryGetValue("invalid_fields", out invalidFields);

                var req_bill_to_email = string.Empty;
                var resultreq_bill_to_email = data.TryGetValue("req_bill_to_email", out req_bill_to_email);

                NLogLogger.LogEvent(NLogType.Info, string.Format("reason_code: {0}, invalid_fields: {1},  req_bill_to_email: {2}", code, invalidFields, req_bill_to_email));
            }
        }
    }
}