using System;
using System.Web;

namespace VisaNet.Utilities.Helpers
{
    public static class WebPathHelper
    {
        /// <summary>
        /// Given a virtual path to a resource, create the full URL
        /// path which incorporates the host name and Application 
        /// virtual root.
        /// </summary>
        /// <param name="virtualPath">The virtual path (e.g. ~/Account/LogOn)</param>
        /// <returns>The full URL path (e.g. 
        /// http://www.give-a-quiz.com/GiveAQuiz/Account/Logon)</returns>
        public static string CreateFullUrl(string virtualPath)
        {
            var request = new HttpRequestWrapper(HttpContext.Current.Request);
            var baseUrl = request.HostUrl();

            return String.Format("{0}{1}", baseUrl, VirtualPathUtility.ToAbsolute(virtualPath));
        }

        /// <summary>
        /// Given a relative path to a resource, create the path 
        /// which incorporates the Application virtual root.
        /// </summary>
        /// <param name="virtualPath">The virtual path (e.g. ~/Account/LogOn)</param>
        /// <returns>The application path (e.g. /GiveAQuiz/Account/Logon)</returns>
        public static string CreateApplicationUrl(string virtualPath)
        {
            return VirtualPathUtility.ToAbsolute(virtualPath);
        }

        /// <summary>
        /// Given a virtual path, return the actual physical
        /// path of the resource on the file system.
        /// </summary>
        /// <param name="virtualPath">The virtual path (e.g. ~/App_Data/foo.txt)</param>
        /// <returns>The physical path (e.g. C:\Inetpub\wwwroot\GiveAQuiz\App_Data\foo.txt)</returns>
        public static string MapPhysicalPath(string virtualPath)
        {
            if (HttpContext.Current == null)
            {
                var t = System.IO.Path.GetFullPath(virtualPath = virtualPath.Replace("~/", ""));
                return t;
            }

            return HttpContext.Current.Server.MapPath(virtualPath);

        }
    }
}
