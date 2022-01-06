using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class SubscriberClientService : WebApiClientService, ISubscriberClientService
    {
        public SubscriberClientService(ITransactionContext transactionContext)
            : base("Subscriber", transactionContext)
        {

        }

        public Task<ICollection<SubscriberDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<SubscriberDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<ICollection<SubscriberDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<SubscriberDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<SubscriberDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<SubscriberDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task Create(SubscriberDto subscriber)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, subscriber));
        }

        public Task Edit(SubscriberDto subscriber)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, subscriber.Id, subscriber));
        }

        public Task Delete(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri, TransactionContext, id));
        }

        public Task<IEnumerable<SubscriberDto>> GetDashboardData(ReportsDashboardFilterDto filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<IEnumerable<SubscriberDto>>(new WebApiHttpRequestPost(BaseUri + "/GetDashboardData", TransactionContext, filtersDto));
        }

        //nuevo
        public Task<int> GetDashboardDataCount(ReportsDashboardFilterDto filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestPost(BaseUri + "/GetDashboardDataCount", TransactionContext, filtersDto));
        }
    }
}
