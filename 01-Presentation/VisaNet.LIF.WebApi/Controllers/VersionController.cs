using System.Diagnostics;
using System.Reflection;
using System.Web.Mvc;

namespace VisaNet.LIF.WebApi.Controllers
{
    public class VersionController : Controller
    {
        [HttpGet]
        public string GetVersion()
        {
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(VersionController)).Location).FileVersion;
            return version;
        }

    }
}