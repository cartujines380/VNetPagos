using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class ConciliationVisaNetTc33ClientService : WebApiClientService, IConciliationVisaNetTc33ClientService
    {
        public ConciliationVisaNetTc33ClientService(ITransactionContext transactionContext)
            : base("ConciliationVisaNetTc33", transactionContext)
        {
        }

        public Task<ConciliationVisanetDto> Create(ConciliationVisanetDto dto)
        {
            return WebApiClient.CallApiServiceAsync<ConciliationVisanetDto>(new WebApiHttpRequestPut(BaseUri + "/Create", TransactionContext, dto));
        }

        public Task Edit(ConciliationVisanetDto dto)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/Edit", TransactionContext, dto));
        }
    }
}