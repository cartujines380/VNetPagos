using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.Application.Implementations
{
    public class ServicePostNotification : IServicePostNotification
    {
        public HttpStatusCode NotifyExternalSourcePostWithSignature(string url, string signature,
            string sigantureField, IDictionary<string, string> signedFields)
        {
            HttpResponseMessage response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    var values = signedFields.ToDictionary(dataAux => dataAux.Key, dataAux => dataAux.Value);
                    values.Add(sigantureField, signature);
                    var content = new FormUrlEncodedContent(values);
                    response = client.PostAsync(url, content).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode(); //Throws an exception if the web server returned an HTTP error status code
                    return response.StatusCode;
                }
            }
            catch (TaskCanceledException e)
            {
                var code = response != null ? response.StatusCode.ToString() : string.Empty;
                NLogLogger.LogEvent(NLogType.Error, string.Format("ServicePostNotification - NotifyExternalSourcePostWithSignature - TaskCanceledException - HttpStatusCode: {0}, Url {1}", code, url));
                NLogLogger.LogEvent(e);
                if (!string.IsNullOrEmpty(code))
                {
                    return response.StatusCode;
                }
                return HttpStatusCode.RequestTimeout;  //Si no hay código para devolver se devuelve el de Timeout
            }
            catch (HttpRequestException e)
            {
                var code = response != null ? response.StatusCode.ToString() : string.Empty;
                NLogLogger.LogEvent(NLogType.Error, string.Format("ServicePostNotification - NotifyExternalSourcePostWithSignature - HttpRequestException - HttpStatusCode: {0}, Url {1}", code, url));
                NLogLogger.LogEvent(e);
                if (!string.IsNullOrEmpty(code))
                {
                    return response.StatusCode;
                }
                throw e; //Si no hay código para devolver se tira la excepción
            }
            catch (Exception e)
            {
                var code = response != null ? response.StatusCode.ToString() : string.Empty;
                NLogLogger.LogEvent(NLogType.Error, string.Format("ServicePostNotification - NotifyExternalSourcePostWithSignature - Exception - HttpStatusCode: {0}, Url {1}", code, url));
                NLogLogger.LogEvent(e);
                if (!string.IsNullOrEmpty(code))
                {
                    return response.StatusCode;
                }
                throw e;  //Si no hay código para devolver se tira la excepción
            }
        }

    }
}