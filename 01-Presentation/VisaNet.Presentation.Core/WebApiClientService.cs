using System.Configuration;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security;
using VisaNet.Presentation.Core.Configuration;

namespace VisaNet.Presentation.Core
{
    public abstract class WebApiClientService
    {

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiClientService" /> class.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="transactionContext"></param>
        protected WebApiClientService(string serviceName, ITransactionContext transactionContext)
        {
            var configuration = WebApiServiceConfigurationSection.GetConfiguration().Endpoints[serviceName];
            if (configuration == null)
                throw new ConfigurationErrorsException(string.Format(PresentationCoreMessages.WarningEndPointHasNotBeenDefined, serviceName));

            BaseUri = configuration.Address;
            TransactionContext = transactionContext;
        }

        #endregion

        #region Properties

        protected string BaseUri { get; private set; }
        protected ITransactionContext TransactionContext { get; private set; }

        #endregion

        protected StringContent GetHttpContent<TModel>(TModel model)
        {
            var jsonString = JsonConvert.SerializeObject(model); ;
            return new StringContent(jsonString, Encoding.UTF8, "application/json");
        }
    }
}