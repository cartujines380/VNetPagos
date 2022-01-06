using System;
using System.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Utilities.SmsApi.Twilio
{
    public class TwilioSmsEndpoint
    {
        private readonly string _twilioId = ConfigurationManager.AppSettings["TwilioId"];
        private readonly string _twilioToken = ConfigurationManager.AppSettings["TwilioToken"];
        private readonly string _twilioPhoneNumer = ConfigurationManager.AppSettings["TwilioPhoneNumber"];

        public SmsStatusDto SendSms(string mobileNumber, string msg)
        {
            try
            {
                TwilioClient.Init(_twilioId, _twilioToken);        
                var response = MessageResource.Create(
                    from: new PhoneNumber(_twilioPhoneNumer), 
                    to: new PhoneNumber(mobileNumber),
                    body: msg);

                var status = response.Status;
                NLogLogger.LogEvent(NLogType.Info, string.Format("TWILIO SMS - Se envio instrucción de envio de SMS al número '{0}'. Respuesta: {1}" ,mobileNumber, status));
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("TWILIO SMS - Mensaje de error en SMS al número '{0}'. Mensaje: ({1}) {2}", 
                        mobileNumber, response.ErrorCode, response.ErrorMessage));    
                }

                return (SmsStatusDto) Enum.Parse(typeof(SmsStatusDto), response.Status.ToString(), true);
                
            }
            catch(Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("TWILIO SMS - Ocurrio una Excepcion. Mensaje al numero '{0}'", mobileNumber));
                NLogLogger.LogEvent(exception);
                return SmsStatusDto.Failed;
            }
        }
    }
}
