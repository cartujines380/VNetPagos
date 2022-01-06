using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class ConciliationVNPClientService : WebApiClientService, IConciliationVNPClientService
    {
        public ConciliationVNPClientService(ITransactionContext transactionContext)
            : base("ConciliationVNP", transactionContext)
        {
        }

        public Task RunConciliation(RunConciliationDto dto)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/RunConciliation", TransactionContext, dto));
        }

        public Task<IEnumerable<ConciliationRunDto>> GetConciliationRunReport(ReportsConciliationRunFilterDto filter)
        {
            return WebApiClient.CallApiServiceAsync<IEnumerable<ConciliationRunDto>>(new WebApiHttpRequestGet(BaseUri + "/GetConciliationRunReport",
                 TransactionContext, filter.GetFilterDictionary()));
        }

        public Task<int> GetConciliationRunReportCount(ReportsConciliationRunFilterDto filter)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/GetConciliationRunReportCount",
                 TransactionContext, filter.GetFilterDictionary()));
        }

        public Task<ConciliationRunDto> GetConciliationRun(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<ConciliationRunDto>(new WebApiHttpRequestGet(
                BaseUri + "/GetConciliationRun", TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

    }
}