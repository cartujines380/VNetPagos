using System;
using System.Configuration;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using VisaNet.Common.Logging.NLog;
using VisaNet.WebApi.Models;

namespace VisaNet.WebApi.Controllers
{
    public class MailgunController : ApiController
    {
        protected bool ValidateCredentials(MailgunResponse request)
        {
            var timestamp = (request.TimeStamp - DateTime.ParseExact(ConfigurationManager.AppSettings["MailgunDateOffset"], "yyyy/MM/dd", CultureInfo.InvariantCulture)).TotalSeconds;
            var token = request.Token;
            var signature = request.Signature;
            var apikey = ConfigurationManager.AppSettings["MailgunApiKey"];

            var encoding = Encoding.ASCII;
            var hmacSha256 = new HMACSHA256(encoding.GetBytes(apikey));
            var cleartext = encoding.GetBytes(timestamp + token);
            var hash = hmacSha256.ComputeHash(cleartext);
            var computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return computedSignature == signature;
        }

        public void LogEmailNotificationReception(MailgunResponse request, ApiMethodEnum method)
        {
            var recipient = request.Recipient;

            if (!recipient.Contains("@visanetpagos") && !recipient.Contains("vnet.uy"))
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("Metodo: {0}, Llego email id: {1}, para Recipient: {2}",method.ToString(), request.MessageId, recipient));
            }
        }

        public void LogEmailNotificationForbbiden(MailgunResponse request)
        {
            var recipient = request.Recipient;
            if (!recipient.Contains("@visanetpagos") && !recipient.Contains("vnet.uy"))
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("El email id: {0} fallo la validación.", request.MessageId));
            }
        }

    }
}