using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VisaNet.Common.ConfigSections;
using VisaNet.Common.Logging.NLog;
using VisaNet.LIF.WebApi.Models;
using VisaNet.Utilities.DigitalSignature;

namespace VisaNet.LIF.WebApi.ActionFilters
{
    public class LIFAuthorizeAttribute : AuthorizeAttribute
    {
        public Type ModelType { get; set; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (AuthorizeRequest(actionContext))
            {
                return;
            }
            HandleUnauthorizedRequest(actionContext);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            //Code to handle unauthorized request
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, new OutModel { Code = 8, Data = "No está autorizado" }, actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }

        private bool AuthorizeRequest(HttpActionContext actionContext)
        {
            try
            {
                var headerValues = actionContext.Request.Headers.GetValues("Signature");
                var signature = headerValues.FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(signature))
                {
                    var preSignedFields = string.Empty;
                    string appId;
                    switch (actionContext.Request.Content.Headers.ContentType.MediaType)
                    {
                        case "application/json":
                        case "text/json":
                            preSignedFields = JsonParser(actionContext.Request.Content.ReadAsStringAsync().Result, out appId);
                            break;
                        case "application/xml":
                        case "text/xml":
                            preSignedFields = XmlParser(actionContext.Request.Content.ReadAsStringAsync().Result, out appId);
                            break;
                        case "application/x-www-form-urlencoded":
                            preSignedFields = XWWWParser(actionContext.Request.Content.ReadAsStringAsync().Result, out appId);
                            break;
                        default:
                            NLogLogger.LogEvent(NLogType.Info, "LIFAuthorizeAttribute - AuthorizeRequest - MediaType Error");
                            throw new ArgumentOutOfRangeException("MediaType");
                    }

                    if (string.IsNullOrEmpty(appId))
                    {
                        NLogLogger.LogEvent(NLogType.Info, "LIFAuthorizeAttribute - AuthorizeRequest - AppId vacio");
                        return false;
                    }

                    var thumbprint = LifClientsConfigurationSection.GetConfiguration().Clients[appId].Thumbprint;
                    var signatureOk = DigitalSignature.CheckSignature(preSignedFields, signature, thumbprint);
                    if (!signatureOk)
                    {
                        NLogLogger.LogEvent(NLogType.Info, "LIFAuthorizeAttribute - AuthorizeRequest - Firma invalida");
                    }
                    return signatureOk;
                }
                NLogLogger.LogEvent(NLogType.Info, "LIFAuthorizeAttribute - AuthorizeRequest - Firma vacia");
                return false;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                NLogLogger.LogEvent(NLogType.Info, "LIFAuthorizeAttribute - AuthorizeRequest - Exception");
                return false;
            }
        }

        private string XWWWParser(string result, out string appId)
        {
            appId = null;
            var queryString = HttpUtility.ParseQueryString(result);
            var obj = Activator.CreateInstance(ModelType);
            var properties = ModelType.GetProperties().OrderBy(x => x.Name);

            foreach (var item in queryString.AllKeys)
            {
                if (properties.Any(p => p.Name == item))
                {
                    var property = properties.First(p => p.Name == item);
                    var value = queryString[item];
                    property.SetValue(obj, value);
                    if (item == "AppId")
                    {
                        appId = value;
                    }
                }
            }

            var concatenatedProperties = string.Empty;
            GetPropertiesValues(obj, ref concatenatedProperties);

            return concatenatedProperties;
        }

        private string XmlParser(string result, out string appId)
        {
            XmlSerializer serializer = new XmlSerializer(ModelType);

            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var obj = Convert.ChangeType(serializer.Deserialize(memStream), ModelType);


            var properties = ModelType.GetProperties().OrderBy(x => x.Name);
            var property = properties.First(p => p.Name == "AppId");
            appId = (property != null) ? property.GetValue(obj, null).ToString() : null;

            var concatenatedProperties = string.Empty;
            GetPropertiesValues(obj, ref concatenatedProperties);

            return concatenatedProperties;

            //XmlSerializer serializer = new XmlSerializer(ModelType);

            //XmlReader reader = XmlReader.Create(new StringReader(result));

            //var kk1 = serializer.Deserialize(reader);

            //var obj1 = Convert.ChangeType(kk1, ModelType);

            //MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(result));
            //var kk = serializer.Deserialize(memStream);
            //kk = result;
            //var obj = Convert.ChangeType(kk, ModelType);
            //var properties = ModelType.GetProperties().OrderBy(x => x.Name);
            //var property = properties.First(p => p.Name == "AppId");
            //appId = (property != null) ? property.GetValue(obj, null).ToString() : null;
            //var concatenatedProperties = string.Empty;
            //GetPropertiesValues(obj, ref concatenatedProperties);

            //return concatenatedProperties;
        }

        private string JsonParser(string content, out string appId)
        {
            var obj = JsonConvert.DeserializeObject(content, ModelType, new JsonSerializerSettings()
            {
                Culture = CultureInfo.CurrentCulture,
            });
            var concatenatedProperties = string.Empty;
            GetPropertiesValues(obj, ref concatenatedProperties);
            var properties = ModelType.GetProperties().OrderBy(x => x.Name);
            var property = properties.First(p => p.Name == "AppId");
            appId = (property != null) ? property.GetValue(obj, null).ToString() : null;
            return concatenatedProperties;
        }

        private void GetPropertiesValues(object obj, ref string asd)
        {
            if (obj == null) return;

            Type objType = obj.GetType();
            var properties = objType.GetProperties().OrderBy(x => x.Name);

            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj, null);
                var elems = propValue as IList;
                if (elems != null)
                {
                    foreach (var item in elems)
                    {
                        GetPropertiesValues(item, ref asd);
                    }
                }
                else
                {
                    // This will not cut-off System.Collections because of the first check
                    if (property.PropertyType.Assembly == objType.Assembly)
                    {
                        GetPropertiesValues(propValue, ref asd);
                    }
                    else
                    {
                        if (property.PropertyType == typeof(double))
                        {
                            var val = (double)propValue;
                            asd += val.ToString(CultureInfo.CurrentCulture);
                        }
                        else
                        {
                            asd += propValue.ToString().ToLower();
                        }
                    }
                }
            }
        }

    }
}