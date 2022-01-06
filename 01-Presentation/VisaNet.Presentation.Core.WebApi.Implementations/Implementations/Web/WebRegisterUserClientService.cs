using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebRegisterUserClientService : WebApiClientService, IWebRegisterUserClientService
    {
        public WebRegisterUserClientService(ITransactionContext transactionContext)
            : base("WebRegisterUser", transactionContext)
        {

        }

        public Task Create(ApplicationUserCreateEditDto entity)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, entity));
        }

        public Task<ApplicationUserDto> CreateUserWithoutPassword(ApplicationUserCreateEditDto entity)
        {
            return WebApiClient.CallApiServiceAsync<ApplicationUserDto>(new WebApiHttpRequestPut(
                BaseUri + "/CreateUserWithoutPassword", TransactionContext, entity));
        }

    }
}
