using System;
using System.Collections.Generic;
using System.Web.Mvc;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos.Enums;
using WebGrease.Css.Extensions;

namespace VisaNet.Presentation.VisaNetOn.Controllers
{
    public class PaymentHandlerController : Controller
    {
        [HttpPost]
        public ActionResult Index()
        {
            NLogLogger.LogEvent(NLogType.Info, "PaymentHandlerController - Index - Llega comunicaccion de CS");
            NLogLogger.LogEvent(NLogType.Info, "PaymentHandlerController - Index - RequestId: " + Request.Form["transaction_id"]);

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
            ViewBag.ActionRedirect = (RedirectEnums)value;
            if (value == (int)RedirectEnums.VisaNetOnPaymentRegisteredWithToken ||
                value == (int)RedirectEnums.VisaNetOnPaymentRegisteredNewToken ||
                value == (int)RedirectEnums.VisaNetOnPaymentRecurrentWithToken ||
                value == (int)RedirectEnums.VisaNetOnPaymentRecurrentNewToken ||
                value == (int)RedirectEnums.VisaNetOnPaymentNewUser ||
                value == (int)RedirectEnums.VisaNetOnPaymentAnonymous)
            {
                ViewBag.RedirectTo = @Url.Action("PaymentTokenGenerationCallback", "Payment");
            }
            if (value == (int)RedirectEnums.VisaNetOnTokenizationRecurrent ||
                value == (int)RedirectEnums.VisaNetOnTokenizationRegistered ||
                value == (int)RedirectEnums.VisaNetOnTokenizationNewUser)
            {
                ViewBag.RedirectTo = @Url.Action("AssociationTokenGenerationCallback", "Association");
            }
        }

        private void CheckError(Dictionary<string, string> data)
        {
            string code;
            var resultCode = data.TryGetValue("reason_code", out code);

            if (resultCode && !string.IsNullOrEmpty(code) && !code.Equals(100))
            {
                var invalidFields = string.Empty;
                data.TryGetValue("invalid_fields", out invalidFields);

                var req_bill_to_email = string.Empty;
                data.TryGetValue("req_bill_to_email", out req_bill_to_email);

                NLogLogger.LogEvent(NLogType.Info, string.Format("reason_code: {0}, invalid_fields: {1},  req_bill_to_email: {2}", code, invalidFields, req_bill_to_email));
            }
        }

    }
}