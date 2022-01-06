using System.Diagnostics;
using System.Reflection;
using System.Web.Mvc;

namespace VisaNet.Presentation.VisaNetOn.Controllers
{
    public class VersionController : BaseController
    {
        [HttpGet]
        public string GetVersion()
        {
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(VersionController)).Location).FileVersion;
            return version;
        }

    }
}