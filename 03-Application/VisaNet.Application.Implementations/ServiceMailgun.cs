using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceMailgun : IServiceMailgun
    {
        public string CreateRoute(string email)
        {
            IRestResponse response = null;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            try
            {
                var client = new RestClient();
                var key = ConfigurationManager.AppSettings["MailgunApiKey"];
                var uri = ConfigurationManager.AppSettings["MailgunUri"];
                var url = ConfigurationManager.AppSettings["MailgunRedirectUrl"];
                var recipient = ConfigurationManager.AppSettings["MailgunHighwayRecipientEmail"];

                client.BaseUrl = new Uri(uri);
                client.Authenticator = new HttpBasicAuthenticator("api", key);

                var request = new RestRequest();
                request.Resource = "routes";
                request.AddParameter("priority", 0);
                request.AddParameter("description", "Route nuevo");
                request.AddParameter("expression", "match_recipient('" + recipient + "') and match_header('from', '(.*)" + email + "(.*)')");
                request.AddParameter("action", "forward('" + url + "')");
                request.AddParameter("action", "stop()");
                request.Method = Method.POST;
                response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    NLogLogger.LogEmailNotificationEvent(NLogType.Error,
                        string.Format("Mailgun error - No se pudo agregar el mail {0}", email));
                    throw new BusinessException(CodeExceptions.MAILGUN_ADD_ERROR);
                }

                dynamic stuff = JsonConvert.DeserializeObject(response.Content);
                string routeId = stuff.route.id;

                return routeId;
            }
            catch (Exception exception)
            {
                if (response != null)
                {
                    NLogLogger.LogEvent(NLogType.Info,
                        string.Format("ServiceMailgun Excepcion - ResponseCode {0}, ResponseContent {1}, ",
                            response.StatusCode, response.Content));
                }
                else
                {
                    NLogLogger.LogEvent(NLogType.Info, "ServiceMailgun Excepcion");
                }

                NLogLogger.LogEvent(exception);
                throw exception;
            }
            
        }
        public void DeleteRoute(string routeId)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            try
            {
                var client = new RestClient();

                var key = ConfigurationManager.AppSettings["MailgunApiKey"];
                var uri = ConfigurationManager.AppSettings["MailgunUri"];

                client.BaseUrl = new Uri(uri);
                client.Authenticator = new HttpBasicAuthenticator("api", key);

                var request = new RestRequest();
                request.Resource = "routes/{id}";
                request.AddUrlSegment("id", routeId);
                request.Method = Method.DELETE;
                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    NLogLogger.LogEmailNotificationEvent(NLogType.Error,
                        string.Format("Mailgun error - No se pudo eliminar la route id {0}", routeId));
                    throw new BusinessException(CodeExceptions.MAILGUN_DELETE_ERROR);
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(NLogType.Error,
                        string.Format("Mailgun error - No se pudo eliminar la route id {0}", routeId));
                NLogLogger.LogEmailNotificationEvent(exception);
                throw;
            }
        }

        public MailgunResponse SendHtml(MailMessage mail)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            try
            {
                var client = new RestClient();

                var key = ConfigurationManager.AppSettings["MailgunApiKey"];
                var domain = ConfigurationManager.AppSettings["MailgunDomain"];
                var uri = ConfigurationManager.AppSettings["MailgunUri"];
                var notiEmail = ConfigurationManager.AppSettings["MailgunNotificationsMail"];
                var email = ConfigurationManager.AppSettings["MailgunRecipientEmail"];


                client.BaseUrl = new Uri(uri);
                client.Authenticator = new HttpBasicAuthenticator("api", key);
                var request = new RestRequest();
                request.AddParameter("domain", domain, ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";
                request.AddParameter("from", "VisaNet Pagos <" + email + ">");
                request.AddParameter("to", mail.To);

                if (!string.IsNullOrEmpty(notiEmail)) request.AddParameter("to", notiEmail);
                request.AddParameter("subject", mail.Subject);
                request.AddParameter(mail.IsBodyHtml ? "html" : "text", mail.Body);

                foreach (var alternateView in mail.AlternateViews)
                {
                    foreach (var linkedResource in alternateView.LinkedResources)
                    {
                        var buffer = new byte[16 * 1024];
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;
                            while ((read = linkedResource.ContentStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, read);
                            }
                            request.AddFile("attachment", ms.ToArray(), linkedResource.ContentId, linkedResource.ContentType.Name);
                        }
                    }

                }

                foreach (var attachment in mail.Attachments)
                {
                    var buffer = new byte[16 * 1024];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int read;
                        while ((read = attachment.ContentStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }
                        request.AddFile("attachment", ms.ToArray(), attachment.Name, attachment.ContentType.Name);
                    }

                }

                request.Method = Method.POST;
                var a = client.Execute(request);
                if (a.StatusCode != HttpStatusCode.OK)
                {
                    var msg = "Cliente " + mail.Subject + " Estado de solicitud de envio " + a.StatusCode;
                    NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("    Error al enviar email al cliente {0}. Msg: {1}. ",mail.To, msg));
                    LogExceptionMessages(a.ErrorException);
                }
                else
                {
                    NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("    Se envio email al cliente {0}. Subject: {1}. ",mail.To, mail.Subject));
                }

                return new MailgunResponse
                {
                    Code = a.StatusCode,
                    Description = a.Content
                };
            }
            catch (Exception e)
            {
                NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Se disprado una excepcion al intentar enviar mail a mailgun. To: {0}, Subject {1}", mail.To, mail.Subject));
                NLogLogger.LogEmailNotificationEvent(e);
            }

            return new MailgunResponse
            {
                Code = HttpStatusCode.InternalServerError,
                Description = "No fue posible enviar el mail a Mailgun"
            };
        }
        public MailgunStatus GetEmailStatus(string mailgunId, string emailTo)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var client = new RestClient();

            var key = ConfigurationManager.AppSettings["MailgunApiKey"];
            var domain = ConfigurationManager.AppSettings["MailgunDomain"];
            var uri = ConfigurationManager.AppSettings["MailgunUri"];


            client.BaseUrl = new Uri(uri);
            client.Authenticator = new HttpBasicAuthenticator("api", key);
            var request = new RestRequest();
            request.AddParameter("domain", domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/events";
            request.AddParameter("message-id", mailgunId);
            request.AddParameter("recipient", emailTo);

            NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Post a Mailgun para obtener el estado MailgunId: {0}, To: {1}", mailgunId, emailTo));
            var response = client.Execute(request);

            NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Response de Mailgun (Code) {0}", response.StatusCode));
            if (response.StatusCode == HttpStatusCode.NotFound)
                return MailgunStatus.NotFound;

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return MailgunStatus.FailureReachingMg;

            var responseJson = JObject.Parse(response.Content);
            var itemsJson = (JArray)responseJson["items"];

            var delivered = itemsJson != null && itemsJson.Any(x => (string)x["event"] == "delivered");
            var failed = itemsJson != null && itemsJson.Any(x => (string)x["event"] == "failed");

            return delivered ? MailgunStatus.Delivered : failed ? MailgunStatus.DroppedHardFail : MailgunStatus.Unknown;
        }
        
        private void LogExceptionMessages(Exception errorException)
        {
            NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Excepcion: {0}", errorException.Message));
            if (errorException.InnerException != null)
            {
                LogExceptionMessages(errorException.InnerException);
            }
        }
    }

    
}
