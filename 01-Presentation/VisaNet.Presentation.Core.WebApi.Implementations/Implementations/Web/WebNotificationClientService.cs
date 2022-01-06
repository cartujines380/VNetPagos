using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebNotificationClientService : WebApiClientService, IWebNotificationClientService
    {
        public WebNotificationClientService(ITransactionContext transactionContext)
            : base("WebNotification", transactionContext)
        {

        }

        public Task<ICollection<NotificationDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<NotificationDto>>(new WebApiHttpRequestPost(BaseUri, TransactionContext, filtersDto));
        }
    }
}
