using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class SystemVersionsClientService : WebApiClientService, ISystemVersionsClientService
    {
        public SystemVersionsClientService(ITransactionContext transactionContext)
            : base("SystemVersions", transactionContext)
        {

        }

        public Task<IDictionary<string, string>> GetSystemVersions()
        {
            return WebApiClient.CallApiServiceAsync<IDictionary<string, string>>(new WebApiHttpRequestGet(
                BaseUri + "/GetSystemVersions", TransactionContext));
        }

    }
}
