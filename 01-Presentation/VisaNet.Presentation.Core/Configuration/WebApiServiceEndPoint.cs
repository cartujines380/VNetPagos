using System.Configuration;

namespace VisaNet.Presentation.Core.Configuration
{
    /// <summary>
    /// This class represents the structure of the web API service
    /// in the WebConfig.Conf or App.Conf. These are the attributes.
    /// </summary>
    public class WebApiServiceEndpoint : ConfigurationElement
    {
        #region Constants

        private const string KeyName = "name";
        private const string KeyAddress = "address";

        #endregion

        [ConfigurationProperty(KeyName, IsRequired = true)]
        public string Name
        {
            get { return this[KeyName] as string; }
        }

        [ConfigurationProperty(KeyAddress, IsRequired = true)]
        public string Address
        {
            get { return this[KeyAddress] as string; }
        }
    }
}