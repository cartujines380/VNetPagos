using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class HomePageItemClientService : WebApiClientService, IHomePageItemClientService
    {
        public HomePageItemClientService(ITransactionContext transactionContext)
            : base("HomePageItem",transactionContext)
        {

        }

        public Task<ICollection<HomePageItemDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<HomePageItemDto>>(new WebApiHttpRequestGet(BaseUri,TransactionContext));
        }

        public Task<HomePageItemDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<HomePageItemDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task Edit(HomePageItemDto homePageItem)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri,TransactionContext, homePageItem.Id, homePageItem));
        }
    }
}
