using System.Configuration;

namespace VisaNet.Presentation.Core.Configuration
{
    public class WebApiServiceEndpointCollection : ConfigurationElementCollection
    {
        public WebApiServiceEndpoint this[int index]
        {
            get { return base.BaseGet(index) as WebApiServiceEndpoint; }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }

                this.BaseAdd(index, value);
            }
        }

        public new WebApiServiceEndpoint this[string key]
        {
            get { return base.BaseGet(key) as WebApiServiceEndpoint; }
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

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new WebApiServiceEndpoint();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WebApiServiceEndpoint)element).Name;
        }
        
        #endregion
    }
}