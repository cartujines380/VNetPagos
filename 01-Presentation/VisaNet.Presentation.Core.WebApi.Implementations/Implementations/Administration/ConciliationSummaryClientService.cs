using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ReportsModel;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class ConciliationSummaryClientService : WebApiClientService, IConciliationSummaryClientService
    {
        public ConciliationSummaryClientService(ITransactionContext transactionContext)
            : base("ConciliationSummary", transactionContext)
        {

        }

        public Task Edit(ConciliationSummaryDto conciliationSummary)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, conciliationSummary.Id, conciliationSummary));
        }

        public Task<ConciliationSummaryDto> GetConciliationSummary(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<ConciliationSummaryDto>(new WebApiHttpRequestGet(BaseUri + "/GetConciliationSummary", TransactionContext, new Dictionary<string, string>
            {
                { "id", id.ToString() }
            }));
        }

        public Task<ConciliationBanredDto> GetConciliationBanred(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<ConciliationBanredDto>(new WebApiHttpRequestGet(BaseUri + "/GetConciliationBanred", TransactionContext, new Dictionary<string, string>
            {
                { "id", id.ToString() }
            }));
        }

        public Task<ConciliationSistarbancDto> GetConciliationSistarbanc(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<ConciliationSistarbancDto>(new WebApiHttpRequestGet(BaseUri + "/GetConciliationSistarbanc", TransactionContext, new Dictionary<string, string>
            {
                { "id", id.ToString() }
            }));
        }

        public Task<ConciliationSuciveDto> GetConciliationSucive(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<ConciliationSuciveDto>(new WebApiHttpRequestGet(BaseUri + "/GetConciliationSucive", TransactionContext, new Dictionary<string, string>
            {
                { "id", id.ToString() }
            }));
        }

        public Task<ConciliationCybersourceDto> GetConciliationCybersource(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<ConciliationCybersourceDto>(new WebApiHttpRequestGet(BaseUri + "/GetConciliationCybersource", TransactionContext, new Dictionary<string, string>
            {
                { "id", id.ToString() }
            }));
        }

        public Task<ConciliationVisanetDto> GetConciliationVisanet(Guid conciliationId)
        {
            return WebApiClient.CallApiServiceAsync<ConciliationVisanetDto>(new WebApiHttpRequestGet(BaseUri + "/GetConciliationVisanet", TransactionContext, new Dictionary<string, string>
            {
                { "id", conciliationId.ToString() }
            }));
        }

        public Task<ConciliationVisanetCallbackDto> GetConciliationBatch(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<ConciliationVisanetCallbackDto>(new WebApiHttpRequestGet(BaseUri + "/GetConciliationBatch", TransactionContext, new Dictionary<string, string>
            {
                { "id", id.ToString() }
            }));
        }

        public Task<ICollection<ReportConciliationDetailDto>> GetConciliationSummary(ReportsConciliationFilterDto filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ReportConciliationDetailDto>>(new WebApiHttpRequestPost(BaseUri + "/GetConciliationSummary", TransactionContext, filtersDto));
        }

    }
}
