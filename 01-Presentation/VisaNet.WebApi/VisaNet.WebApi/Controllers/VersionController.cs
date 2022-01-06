using System.Diagnostics;
using System.Reflection;
using System.Web.Http;

namespace VisaNet.WebApi.Controllers
{
    public class VersionController : ApiController
    {
        [HttpGet]
        public string Get()
        {
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(VersionController)).Location).FileVersion;
            return version;
        }

    }
}