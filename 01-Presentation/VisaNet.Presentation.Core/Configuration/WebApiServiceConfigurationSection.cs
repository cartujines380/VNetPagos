using System.Configuration;

namespace VisaNet.Presentation.Core.Configuration
{
    public class WebApiServiceConfigurationSection : ConfigurationSection
    {
        #region Constants

        private const string ConfigurationSection = "webapiservices";
        private const string ConfigurationCollection = "client";

        #endregion

        /// <summary>
        /// Returns an WebApiServiceConfigurationSection instance
        /// </summary>
        public static WebApiServiceConfigurationSection GetConfiguration()
        {
            return (WebApiServiceConfigurationSection)ConfigurationManager.GetSection(ConfigurationSection) ?? new WebApiServiceConfigurationSection();
        }

        [ConfigurationProperty(ConfigurationCollection)]
        public WebApiServiceEndpointCollection Endpoints
        {
            get { return (WebApiServiceEndpointCollection)this[ConfigurationCollection] ?? new WebApiServiceEndpointCollection(); }
        }
    }
}