using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebAutoCompleteClientService : WebApiClientService, IAutoCompleteClientService
    {
        public WebAutoCompleteClientService(ITransactionContext transactionContext)
            : base("WebAutoComplete", transactionContext)
        {
        }

        public Task<ICollection<ServiceDto>> AutoCompleteServices(string contains)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ServiceDto>>(new WebApiHttpRequestGet(BaseUri + "/AutoCompleteServices",
                                                                                                      TransactionContext,
                                                                                                      new Dictionary<string, string> { { "contains", contains } }
                                                                                                      ));
        }

        public Task<ICollection<ApplicationUserDto>> AutoCompleteApplicationUsers(string contains)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ApplicationUserDto>>(new WebApiHttpRequestGet(BaseUri + "/AutoCompleteApplicationUsers",
                                                                                                      TransactionContext,
                                                                                                      new Dictionary<string, string> { { "contains", contains } }
                                                                                                      ));
        }
    }
}
