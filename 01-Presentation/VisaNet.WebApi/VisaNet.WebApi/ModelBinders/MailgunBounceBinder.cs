using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using VisaNet.Common.Logging.NLog;
using VisaNet.WebApi.Models;

namespace VisaNet.WebApi.ModelBinders
{
    public class MailgunBounceBinder : IModelBinder
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
                if (bindingContext.ModelType != typeof(MailgunBounce))
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, "SampleComplexModel type expected, not type: " + bindingContext.ModelType.FullName);
                    return false;
                }
                var obj = new MailgunBounce();
                try
                {
                    obj.MessageId = values["message-id"].Replace(">", "").Replace("<", "");
                    obj.Event = values["event"];
                    obj.Recipient = values["recipient"];
                    obj.Domain = values["domain"];
                    obj.MessageHeaders = values["message-headers"];
                    obj.TimeStamp = (new DateTime(1970, 1, 1).AddSeconds(long.Parse(values["timestamp"])));
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
            var retDicionary = new Dictionary<string, string>();
            using (StringReader reader = new StringReader(content))
            {
                string line;
                var counter = 0;
                var delimiter = string.Empty;

                while ((line = reader.ReadLine()) != null)
                {
                    if (counter == 0)
                    {
                        delimiter = line.Trim();
                        counter++;
                        line = reader.ReadLine();
                    }

                    //Chequeo que sea un key
                    var key = string.Empty;
                    if (line.Contains("Content-Disposition"))
                    {
                        var indexOf = line.LastIndexOf("name=\"") + 6;
                        while (line[indexOf] != '"')
                        {
                            key += line[indexOf];
                            indexOf++;
                        }
                        line = reader.ReadLine();
                    }

                    var value = string.Empty;
                    while (line != null && line != delimiter)
                    {
                        value += line;
                        line = reader.ReadLine();
                    }

                    //Agrego al diccionario
                    retDicionary.Add(key.ToLowerInvariant(), value);

                    // Do something with the line
                    counter++;
                }
            }

            return retDicionary;
        }

    }
}