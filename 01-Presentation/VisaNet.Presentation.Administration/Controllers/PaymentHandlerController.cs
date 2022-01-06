using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using WebGrease.Css.Extensions;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class PaymentHandlerController : Controller
    {
        [HttpPost]
        public ActionResult Index()
        {
            NLogLogger.LogEvent(NLogType.Info, "PaymentHandlerController (Admin) - Llega comunicaccion de CS");
            NLogLogger.LogEvent(NLogType.Info, "PaymentHandlerController (Admin) - RequestId: " + Request.Form["transaction_id"]);

            ////SOLO PARA EL TESTING, DEJAR LA PAGINA CON LA EXCEPCION
            //var model = new Dictionary<string, string>();
            //Request.Form.AllKeys.ForEach(x => model.Add(x, Request.Form[x]));
            //return View(model);

            try
            {
                var redirectTo = Int32.Parse(Request.Form["req_merchant_defined_data11"]);
                LoadTempDataFromForm(Request.Form);
                return Redirect(redirectTo);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, "PaymentHandlerController - Error");
                NLogLogger.LogEvent(e);
                //model.Add("Exception", e.Message);
                //model.Add("Track", e.StackTrace);
            }
            return RedirectToAction("Index", "Error");
        }

        private ActionResult Redirect(int value)
        {
            if (value == (int)RedirectEnums.TestCsSecureAcceptance)
            {
                return RedirectToAction("TestCybersourceSecureAcceptanceCallback", "Service");
            }

            return null;
        }

        private void LoadTempDataFromForm(NameValueCollection form)
        {

            TempData.Clear();

            TempData.Add("CsMask", form["req_card_number"]);
            TempData.Add("CsExpiry", form["req_card_expiry_date"]);
            TempData.Add("CsTransaction", form["req_transaction_uuid"]);
            TempData.Add("CsToken", form["payment_token"]);
            TempData.Add("CsTransactionId", form["transaction_id"]);
            TempData.Add("CsMerchantDefinedData18", form["req_merchant_defined_data18"]);
            TempData.Add("CsMerchandId", form["req_merchant_defined_data23"]);
            TempData.Add("CsMerchantDefinedData24", form["req_merchant_defined_data24"]);
            TempData.Add("CsMerchantDefinedData25", form["req_merchant_defined_data25"]);
            TempData.Add("CsMerchantDefinedData26", form["req_merchant_defined_data26"]);
            TempData.Add("CsMerchantDefinedData27", form["req_merchant_defined_data27"]);
            TempData.Add("CsMerchantDefinedData29", form["req_merchant_defined_data29"]);
            
            //ESTO SE SETEA CUANDO SE REALIZA UNA COMPRA CON UN TOKEN PREVIAMENTE CREADO. LO NECESITO PARA ASIGNAR LA TARJETA PERSISITADA EN BD A LA FACTURA.
            TempData.Add("CsUsedToken", form["req_payment_token"]);

            var cyberSourceData = new CyberSourceDataDto
            {
                Decision = form["decision"] ?? string.Empty,
                ReasonCode = form["reason_code"],
                TransactionId = form["transaction_id"] ?? string.Empty,
                Message = form["message"] ?? string.Empty,
                BillTransRefNo = form["bill_trans_ref_no"] ?? string.Empty,
                ReqCardNumber = form["req_card_number"] ?? string.Empty,
                ReqCardExpiryDate = form["req_card_expiry_date"] ?? string.Empty,
                ReqProfileId = form["req_profile_id"] ?? string.Empty,
                ReqCardType = form["req_card_type"] ?? string.Empty,
                ReqPaymentMethod = form["req_payment_method"] ?? string.Empty,
                ReqTransactionType = form["req_transaction_type"] ?? string.Empty,
                ReqTransactionUuid = form["req_transaction_uuid"] ?? string.Empty,
                ReqCurrency = form["req_currency"] ?? string.Empty,
                ReqReferenceNumber = form["req_reference_number"] ?? string.Empty,
                ReqAmount = form["req_amount"] ?? string.Empty,
                AuthAvsCode = form["auth_avs_code"] ?? string.Empty,
                AuthCode = form["auth_code"] ?? string.Empty,
                AuthAmount = form["auth_amount"] ?? string.Empty,
                AuthTime = form["auth_time"] ?? string.Empty,
                AuthResponse = form["auth_response"] ?? string.Empty,
                AuthTransRefNo = form["auth_trans_ref_no"] ?? string.Empty,
                PaymentToken = form["payment_token"] ?? string.Empty,

            };

            TempData.Add("CyberSourceData", cyberSourceData);

            //si el formulario contiene la clave _xid es porque se utilizo verifybyvisa en el proceso
            //se guardan los datos relacionados a verifybyvisa

            var verifyByVisaData = new VerifyByVisaDataDto
            {
                PayerAuthenticationEci = form["payer_authentication_eci"] ?? string.Empty,
                PayerAuthenticationXid = form["payer_authentication_xid"] ?? string.Empty,
                PayerAuthenticationCavv = form["payer_authentication_cavv"] ?? string.Empty,
                PayerAuthenticationProofXml = form["payer_authentication_proof_xml"] ?? string.Empty,
            };

            TempData.Add("VerifyByVisaData", verifyByVisaData);

            if (cyberSourceData.ReasonCode.Equals("101") || cyberSourceData.ReasonCode.Equals("102"))
            {
                NLogLogger.LogErrorCsEvent(NLogType.Info, string.Format("PaymentHandlerController - Error en CS. Codigo: {0}, request id {1}",
                    cyberSourceData.ReasonCode, cyberSourceData.TransactionId));
                Request.Form.AllKeys.ForEach(x =>
                    NLogLogger.LogErrorCsEvent(NLogType.Info, string.Format("PaymentHandlerController - Request id {0} - Campos: {1}, Valor: {2}",
                    cyberSourceData.TransactionId, x, Request.Form[x])));
            }
        }
    }
}