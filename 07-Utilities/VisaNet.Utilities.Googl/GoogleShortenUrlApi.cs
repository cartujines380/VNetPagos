using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.Utilities.Googl
{
    public class GoogleShortenUrlApi
    {
        private readonly string _apiKey = ConfigurationManager.AppSettings["GooglApiKey"];
        private readonly string _googlUrl = ConfigurationManager.AppSettings["GooglUrl"];


        public string Excecute(string longUrl)
        {
            using (var client = new HttpClient())
            {
                var url = _googlUrl + "?key=" + _apiKey;
                var response = client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(new { longUrl = longUrl })
                    , Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
                var responseString = response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("GOOGLE SHORTEN URL - Error {0} al intentar achicar la url {1}. Mensaje: {2} ",
                        response.StatusCode, longUrl, responseString.Result));
                    return string.Empty;
                }
                
                var responseObj = JsonConvert.DeserializeObject<GooglDto>(responseString.Result);
                return responseObj.Id;
            }
        }
    }

    public class GooglDto
    {
        public string Id { get; set; }
        public string LongUrl { get; set; }
        public string Kind { get; set; }
    }
}
