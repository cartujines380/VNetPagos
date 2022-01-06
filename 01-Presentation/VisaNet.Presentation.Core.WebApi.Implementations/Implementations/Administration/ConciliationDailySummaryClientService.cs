using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class ConciliationDailySummaryClientService : WebApiClientService, IConciliationDailySummaryClientService
    {
        public ConciliationDailySummaryClientService(ITransactionContext transactionContext)
            : base("ConciliationDailySummary", transactionContext)
        {

        }

        public Task<ICollection<ConciliationDailySummaryDto>> GetConciliationDailySummary(DailyConciliationFilterDto filter)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ConciliationDailySummaryDto>>(new WebApiHttpRequestPost(BaseUri, TransactionContext, filter));
        }
    }
}
