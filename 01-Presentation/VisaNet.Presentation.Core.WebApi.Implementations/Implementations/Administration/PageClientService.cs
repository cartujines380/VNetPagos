using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class PageClientService : WebApiClientService, IPageClientService
    {
        public PageClientService(ITransactionContext transactionContext)
            : base("Page",transactionContext)
        {

        }

        public Task<ICollection<PageDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<PageDto>>(new WebApiHttpRequestGet(BaseUri,TransactionContext));
        }

        public Task<PageDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<PageDto>(new WebApiHttpRequestGet(
                    BaseUri,TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task<PageDto> FindType(PageTypeDto type)
        {
            return
               WebApiClient.CallApiServiceAsync<PageDto>(new WebApiHttpRequestGet(
                   BaseUri, TransactionContext, new Dictionary<string, string> { { "type", type.ToString() } }));
        }

        public Task Edit(PageDto page)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri,TransactionContext, page.Id, page));
        }
    }
}
