using System;
using System.Configuration;

namespace VisaNet.Common.ConfigSections
{
    public class LifClientsConfigurationSection : ConfigurationSection
    {
        #region Constants

        private const string ConfigurationSection = "lifClients";
        private const string ConfigurationCollection = "clients";

        #endregion

        public static LifClientsConfigurationSection GetConfiguration()
        {
            return (LifClientsConfigurationSection)ConfigurationManager.GetSection(ConfigurationSection) ?? new LifClientsConfigurationSection();
        }

        [ConfigurationProperty(ConfigurationCollection)]
        public LifClientsCollection Clients
        {
            get { return (LifClientsCollection)this[ConfigurationCollection] ?? new LifClientsCollection(); }
        }
    }

    public class LifClientsCollection : ConfigurationElementCollection
    {
        public LifClient this[int index]
        {
            get { return base.BaseGet(index) as LifClient; }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }

                this.BaseAdd(index, value);
            }
        }

        public new LifClient this[string key]
        {
            get { return base.BaseGet(key) as LifClient; }
            set
            {
                if (base.BaseGet(key) != null)
                {
                    base.BaseRemove(key);
                }

                this.BaseAdd(value);
            }
        }

        #region ConfigurationElementCollection Members

        protected override ConfigurationElement CreateNewElement()
        {
            return new LifClient();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LifClient)element).AppId;
        }

        #endregion
    }

    public class LifClient : ConfigurationElement
    {
        [ConfigurationProperty("appId", IsRequired = true)]
        public string AppId
        {
            get { return this["appId"] as string; }
        }

        [ConfigurationProperty("thumbprint", IsRequired = true)]
        public string Thumbprint
        {
            get { return this["thumbprint"] as string; }
        }
    }
}
