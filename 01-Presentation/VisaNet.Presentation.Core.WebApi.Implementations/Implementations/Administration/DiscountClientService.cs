using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class DiscountClientService : WebApiClientService, IDiscountClientService
    {
        public DiscountClientService(ITransactionContext transactionContext)
            : base("Discount", transactionContext)
        {

        }

        public Task<ICollection<DiscountDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<DiscountDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<DiscountDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<DiscountDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task Edit(DiscountDto discount)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, discount.Id, discount));
        }
    }
}
