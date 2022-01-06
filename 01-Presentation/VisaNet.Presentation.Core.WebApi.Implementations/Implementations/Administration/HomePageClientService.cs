using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class HomePageClientService : WebApiClientService, IHomePageClientService
    {
        public HomePageClientService(ITransactionContext transactionContext)
            : base("HomePage",transactionContext)
        {

        }

        public Task<ICollection<HomePageDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<HomePageDto>>(new WebApiHttpRequestGet(BaseUri,TransactionContext));
        }

        public Task<HomePageDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<HomePageDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task Edit(HomePageDto homePage)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri,TransactionContext, homePage.Id, homePage));
        }
    }
}
