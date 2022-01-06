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
    public class ContactClientService : WebApiClientService, IContactClientService
    {
        public ContactClientService(ITransactionContext transactionContext)
            : base("Contact", transactionContext)
        {

        }

        public Task<ICollection<ContactDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ContactDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<ICollection<ContactDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ContactDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<ContactDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<ContactDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task Create(ContactDto contact)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, contact));
        }

        public Task Edit(ContactDto contact)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, contact.Id, contact));
        }

        public Task Delete(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri, TransactionContext, id));
        }

        public Task<IEnumerable<ContactDto>> GetDashboardData(ReportsDashboardFilterDto filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<IEnumerable<ContactDto>>(new WebApiHttpRequestPost(BaseUri + "/GetDashboardData", TransactionContext, filtersDto));
        }

        public Task<int[]> GetDashboardDataCount(ReportsDashboardFilterDto filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<int[]>(new WebApiHttpRequestPost(BaseUri + "/GetDashboardDataCount", TransactionContext, filtersDto));
        }
    }
}
