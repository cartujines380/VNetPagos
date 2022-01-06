using System.Collections.Generic;
using Newtonsoft.Json;

namespace VisaNet.Presentation.Web.Models
{
    public class CaptchaResponseModel
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}