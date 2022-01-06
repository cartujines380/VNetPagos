using System;
using System.Web;

namespace VisaNet.Utilities.Helpers
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Extension method for the HttpRequestBase class that 
        /// creates the full base Application Url up to and 
        /// including the virtual root (e.g. "http://qa-server/GiveAQuiz")
        /// </summary>
        /// <param name="request">The HttpRequestBase to extend.</param>
        /// <returns>The full base Application Url</returns>
        public static string ApplicationUrl(this HttpRequestBase request)
        {
            return String.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, request.ApplicationPath);
        }

        /// <summary>
        /// Extension method for the HttpRequestBase class that 
        /// creates the full base Url up to but  
        /// not including the virtual root (e.g. "http://qa-server/")
        /// </summary>
        /// <param name="request">The HttpRequestBase to extend.</param>
        /// <returns>The full base Application Url</returns>
        public static string HostUrl(this HttpRequestBase request)
        {
            return String.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
        }

        public static string GetIPAddress(this HttpRequestBase request)
        {
            string ip;
            try
            {
                ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    if (ip.IndexOf(",") > 0)
                    {
                        string[] ipRange = ip.Split(',');
                        int le = ipRange.Length - 1;
                        ip = ipRange[le];
                    }
                }
                else
                {
                    ip = request.UserHostAddress;
                }
            }
            catch { ip = null; }

            return ip;
        }
    }
}
