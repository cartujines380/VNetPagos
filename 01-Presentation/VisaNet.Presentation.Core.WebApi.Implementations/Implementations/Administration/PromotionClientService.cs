using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class PromotionClientService : WebApiClientService, IPromotionClientService
    {
        public PromotionClientService(ITransactionContext transactionContext)
            : base("Promotion", transactionContext)
        {

        }

        public Task<ICollection<PromotionDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<PromotionDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<ICollection<PromotionDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<PromotionDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<PromotionDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<PromotionDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task Create(PromotionDto promotion)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, promotion));
        }

        public Task Edit(PromotionDto promotion)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, promotion.Id, promotion));
        }

        public Task Delete(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri, TransactionContext, id));
        }

    }
}
