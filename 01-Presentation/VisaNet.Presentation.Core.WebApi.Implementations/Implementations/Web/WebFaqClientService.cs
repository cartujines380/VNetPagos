using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebFaqClientService : WebApiClientService, IWebFaqClientService
    {
        public WebFaqClientService(ITransactionContext transactionContext)
            : base("WebFaq", transactionContext)
        {

        }

        public Task<ICollection<FaqDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<FaqDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<ICollection<FaqDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<FaqDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<FaqDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<FaqDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }
    }
}
