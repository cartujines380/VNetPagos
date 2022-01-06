using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class ReportsClientService : WebApiClientService, IReportsClientService
    {
        public ReportsClientService(ITransactionContext transactionContext)
            : base("Reports", transactionContext)
        {

        }

        public Task<IEnumerable<TransactionsAmountDto>> GetTransactionsAmountData(ReportsTransactionsAmountFilterDto filtersDto)
        {
            return
                WebApiClient.CallApiServiceAsync<IEnumerable<TransactionsAmountDto>>(new WebApiHttpRequestPost(
                    BaseUri + "/GetTransactionsAmountData", TransactionContext, filtersDto));
        }

        public Task<IEnumerable<CybersourceTransactionsDto>> GetCybersourceTransactionsData(ReportsCybersourceTransactionsFilterDto filtersDto)
        {
            return
                WebApiClient.CallApiServiceAsync<IEnumerable<CybersourceTransactionsDto>>(new WebApiHttpRequestPost(
                    BaseUri + "/GetCybersourceTransactionsData", TransactionContext, filtersDto));
        }

        public Task<IEnumerable<LogPaymentCyberSourceDto>> GetCybersourceTransactionsDetails(ReportsCybersourceTransactionsFilterDto filtersDto)
        {
            return
                WebApiClient.CallApiServiceAsync<IEnumerable<LogPaymentCyberSourceDto>>(new WebApiHttpRequestPost(
                    BaseUri + "/GetCybersourceTransactionsDetails", TransactionContext, filtersDto));
        }

        public Task<DashboardDto> GetDashboardSP(ReportsDashboardFilterDto filtersDto)
        {
            return
                WebApiClient.CallApiServiceAsync<DashboardDto>(new WebApiHttpRequestPost(
                    BaseUri + "/GetDashboardSP", TransactionContext, filtersDto));
        }
    }
}
