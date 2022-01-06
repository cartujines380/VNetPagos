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
    public class AffiliationCardClientService : WebApiClientService, IAffiliationCardClientService
    {
        public AffiliationCardClientService(ITransactionContext transactionContext)
            : base("AffiliationCard", transactionContext)
        {

        }

        public Task<ICollection<AffiliationCardDto>> GetDataForTable(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<AffiliationCardDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }


        public Task<int> GetDataForTableCount(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/GetDataForTableCount", TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<ICollection<AffiliationCardDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<AffiliationCardDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<AffiliationCardDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<AffiliationCardDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task Create(AffiliationCardDto affiliationCard)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, affiliationCard));
        }

        public Task Edit(AffiliationCardDto affiliationCard)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, affiliationCard.Id, affiliationCard));
        }

        public Task Delete(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri, TransactionContext, id));
        }

        public Task ChangeStatus(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri + "/ChangeStatus", TransactionContext, id));
        }
    }
}
