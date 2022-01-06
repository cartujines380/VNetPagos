using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class BillClientService : WebApiClientService, IBillClientService
    {
        public BillClientService(ITransactionContext transactionContext)
            : base("Bill", transactionContext)
        {

        }

        public Task<ICollection<BillDto>> TestGatewayGetBills(TestGatewaysFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<BillDto>>(new WebApiHttpRequestPost(
                BaseUri + "/TestGatewayGetBills", TransactionContext, filterDto));
        }
    }
}
