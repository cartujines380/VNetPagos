using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;

namespace VisaNet.Services.WebApi.Models
{
    public class CustomModelBinder : IModelBinder
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var content = actionContext.Request.Content;
            string json = content.ReadAsStringAsync().Result;
            var obj = JsonConvert.DeserializeObject(json, bindingContext.ModelType, _settings);
            bindingContext.Model = obj;
            return true;
        }
    }
}