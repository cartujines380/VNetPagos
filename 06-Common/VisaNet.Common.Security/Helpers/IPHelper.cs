using System.Web;

namespace VisaNet.Common.Security.Helpers
{
    public static class IPHelper
    {
        public static string GetUserIP(this HttpRequest request)
        {
            var ipList = request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"];
            
            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }
            
            ipList = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }

            return request.ServerVariables["REMOTE_ADDR"];
        }

    }
}
