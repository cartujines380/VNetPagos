using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using VisaNet.Common.Logging.NLog;
using VisaNet.WebApi.Models;

namespace VisaNet.WebApi.ModelBinders
{
    public class MailgunDeliveryBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            try
            {
                var contentTask = actionContext.Request.Content.ReadAsStringAsync();
                var content = contentTask.Result;

                IDictionary<string, string> values = ParseContent(content);

                if (!values.Any())
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, "No input data");
                    return false;
                }

                if (bindingContext.ModelType != typeof(MailgunDelivery))
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, "SampleComplexModel type expected, not type: " + bindingContext.ModelType.FullName);
                    return false;
                }
                var obj = new MailgunDelivery();
                try
                {
                    obj.MessageId = values["message-id"].Replace(">", "").Replace("<", "");
                    obj.Event = values["event"];
                    obj.Recipient = values["recipient"];
                    obj.Domain = values["domain"];
                    obj.MessageHeaders = values["message-headers"];
                    //obj.CustomVariables = values["message-headers"];
                    obj.TimeStamp = (DateTime.ParseExact(ConfigurationManager.AppSettings["MailgunDateOffset"], "yyyy/MM/dd", CultureInfo.InvariantCulture).AddSeconds(long.Parse(values["timestamp"])));
                    obj.Token = values["token"];
                    obj.Signature = values["signature"];
                }
                catch (Exception ex)
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex.Message);
                    NLogLogger.LogEvent(ex);
                    return false;
                }
                bindingContext.Model = obj;
                return true;
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex.Message);
                NLogLogger.LogEvent(ex);
                return false;
            }
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