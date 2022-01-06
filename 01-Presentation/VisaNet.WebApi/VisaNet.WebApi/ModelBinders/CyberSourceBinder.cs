using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using VisaNet.Common.Logging.NLog;
using VisaNet.WebApi.Models;

namespace VisaNet.WebApi.ModelBinders
{
    public class CyberSourceBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            //var values = Request.Form.AllKeys.ToDictionary(key => key, key => Request.Form[key]);

            var contentTask = actionContext.Request.Content.ReadAsStringAsync();
            var content = contentTask.Result;

            IDictionary<string, string> values = ParseContent(content);

            //Esto solo lo hago para ver que llega de CS al hacer el post
            //GraylogLogger.LogEvent(new GraylogLog
            //{
            //    Data = string.Join("\n", values.Select(x => string.Format("{0}: {1}", x.Key, x.Value))),
            //    Host = LogPlatform.CSAcknowledgement,
            //    Level = NLogType.Info,
            //    ShortMessage = "Llega post de CS",
            //    OperationType = OperationType.CyberSourcePost
            //});

            if (!values.Any())
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "No input data");
                NLogLogger.LogEvent(NLogType.Info, string.Format("CyberSourceBinder -  No input data"));
                return false;
            }

            if (bindingContext.ModelType != typeof(CyberSourcePostModel))
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "SampleComplexModel type expected, not type: " + bindingContext.ModelType.FullName);
                NLogLogger.LogEvent(NLogType.Info, string.Format("CyberSourceBinder -  SampleComplexModel type expected, not type: {0}", bindingContext.ModelType.FullName));
                return false;
            }

            var obj = new CyberSourcePostModel();
            try
            {
                obj.ReasonCode = values.ContainsKey("reason_code") ? values["reason_code"] : string.Empty;
                obj.TransactionId = values.ContainsKey("transaction_id") ? values["transaction_id"] : string.Empty;
                obj.UserId = values.ContainsKey("req_merchant_defined_data31") ? values["req_merchant_defined_data31"] : string.Empty;
                obj.Decision = values.ContainsKey("decision") ? values["decision"] : string.Empty;
                obj.ReasonCode = values.ContainsKey("reason_code") ? values["reason_code"] : string.Empty;
                obj.Message = values.ContainsKey("message") ? values["message"] : string.Empty;
                obj.BillTransRefNo = values.ContainsKey("bill_trans_ref_no") ? values["bill_trans_ref_no"] : string.Empty;
                obj.ReqCardNumber = values.ContainsKey("req_card_number") ? values["req_card_number"] : string.Empty;
                obj.ReqCardExpiryDate = values.ContainsKey("req_card_expiry_date") ? values["req_card_expiry_date"] : string.Empty;
                obj.ReqProfileId = values.ContainsKey("req_profile_id") ? values["req_profile_id"] : string.Empty;
                obj.ReqCardType = values.ContainsKey("req_card_type") ? values["req_card_type"] : string.Empty;
                obj.ReqPaymentMethod = values.ContainsKey("req_payment_method") ? values["req_payment_method"] : string.Empty;
                obj.ReqTransactionType = values.ContainsKey("req_transaction_type") ? values["req_transaction_type"] : string.Empty;
                obj.ReqTransactionUuid = values.ContainsKey("req_transaction_uuid") ? values["req_transaction_uuid"] : string.Empty;
                obj.ReqCurrency = values.ContainsKey("req_currency") ? values["req_currency"] : string.Empty;
                obj.ReqReferenceNumber = values.ContainsKey("req_reference_number") ? values["req_reference_number"] : string.Empty;
                obj.ReqAmount = values.ContainsKey("req_amount") ? values["req_amount"] : string.Empty;
                obj.AuthAvsCode = values.ContainsKey("auth_avs_code") ? values["auth_avs_code"] : string.Empty;
                obj.AuthCode = values.ContainsKey("auth_code") ? values["auth_code"] : string.Empty;
                obj.AuthAmount = values.ContainsKey("auth_amount") ? values["auth_amount"] : string.Empty;
                obj.AuthTime = values.ContainsKey("auth_time") ? values["auth_time"] : string.Empty;
                obj.AuthResponse = values.ContainsKey("auth_response") ? values["auth_response"] : string.Empty;
                obj.AuthTransRefNo = values.ContainsKey("auth_trans_ref_no") ? values["auth_trans_ref_no"] : string.Empty;
                obj.PaymentToken = values.ContainsKey("req_payment_token") ? values["req_payment_token"] : string.Empty;
                obj.DateTime = DateTime.Now;
                obj.Platform = values.ContainsKey("req_merchant_defined_data28") ? values["req_merchant_defined_data28"] : string.Empty;
                obj.ServiceId = values.ContainsKey("req_merchant_defined_data30") ? values["req_merchant_defined_data30"] : string.Empty;
                obj.OperationId = values.ContainsKey("req_merchant_defined_data27") ? values["req_merchant_defined_data27"] : string.Empty;
                obj.CardId = values.ContainsKey("req_merchant_defined_data32") ? values["req_merchant_defined_data32"] : string.Empty;
                obj.IsUserRegistered = values.ContainsKey("req_merchant_defined_data3") && values["req_merchant_defined_data3"].ToLower() == "y";
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex.Message);
                NLogLogger.LogEvent(NLogType.Info, string.Format("CyberSourceBinder -  Exception - transaction_id: {0}", values != null && 
                    values.ContainsKey("reason_code") ? values["reason_code"] : string.Empty));
                return false;
            }
            bindingContext.Model = obj;
            return true;
        }

        private IDictionary<string, string> ParseContent(string content)
        {
            var values = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(content))
            {
                values = content.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(part => part.Split('='))
               .ToDictionary(split => split[0].ToLower(), split => Uri.UnescapeDataString(split[1]));
            }

            return values;
        }
    }
}