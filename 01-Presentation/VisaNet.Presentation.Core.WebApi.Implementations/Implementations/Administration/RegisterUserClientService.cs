using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class RegisterUserClientService : WebApiClientService, IRegisterUserClientService
    {
        public RegisterUserClientService(ITransactionContext transactionContext)
            : base("RegisterUser",transactionContext)
        {

        }

        public Task Create(ApplicationUserCreateEditDto entity)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, entity));
        }
    }
}
