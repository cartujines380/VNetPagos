using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebLocationClientService : WebApiClientService, IWebLocationClientService
    {
        public WebLocationClientService(ITransactionContext transactionContext)
            : base("WebLocation", transactionContext)
        {

        }

        public Task<ICollection<LocationDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<LocationDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<ICollection<LocationDto>> GetList(BaseFilter filter)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<LocationDto>>(new WebApiHttpRequestGet(BaseUri + "/GetLocationForList", TransactionContext, filter.GetFilterDictionary()));
        }
    }
}
