using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Configuration;

namespace VisaNet.Utilities.CustomProxy
{
    public class CustomWebProxy : IWebProxy
    {
        private IList<string> _useProxyForTheseUrls;

        readonly string _proxyUserName = ConfigurationManager.AppSettings["ProxyUserName"] ?? "";
        readonly string _proxyPassword = ConfigurationManager.AppSettings["ProxyPassword"] ?? "";
        readonly string _proxyDomain = ConfigurationManager.AppSettings["ProxyDomain"] ?? "";
        readonly string _proxyServer = ConfigurationManager.AppSettings["ProxyServer"] ?? "";

        IList<string> UseProxyForTheseUrls
        {
            get 
            {
                return _useProxyForTheseUrls ??
                       (_useProxyForTheseUrls = ConfigurationManager.AppSettings["UseProxyForTheseUrls"].Split('|').ToList());
            }
        }

        public ICredentials Credentials
        {
            get
            {               
                return String.IsNullOrEmpty(_proxyDomain)
                    ? new NetworkCredential(_proxyUserName, _proxyPassword)
                    : new NetworkCredential(_proxyUserName, _proxyPassword, _proxyDomain);
            }
            set {  }
        }

        public Uri GetProxy(Uri destination)
        {            
            var result = new Uri(_proxyServer);
            return result;
        }

        /// <summary>
        /// Solo van por el proxy aquellos paths que están en la lista
        /// </summary>        
        public bool IsBypassed(Uri host)
        {
            if (host.AbsoluteUri.ToLower().Contains("google") || host.AbsolutePath.ToLower().Contains("google"))
            {
                return !UseProxyForTheseUrls.Contains(host.AbsolutePath);    
            }
            return !UseProxyForTheseUrls.Contains(host.AbsoluteUri);
        }       
    }
}
