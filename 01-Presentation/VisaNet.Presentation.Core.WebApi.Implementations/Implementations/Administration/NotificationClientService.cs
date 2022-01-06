using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class NotificationClientService : WebApiClientService, INotificationClientService
    {
        public NotificationClientService(ITransactionContext transactionContext)
            : base("Notification", transactionContext)
        {

        }

        public Task<ICollection<NotificationDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<NotificationDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<IEnumerable<NotificationDto>> GetDashboardData(ReportsDashboardFilterDto filters)
        {
            return WebApiClient.CallApiServiceAsync<IEnumerable<NotificationDto>>(new WebApiHttpRequestPost(BaseUri + "/GetDashboardData", TransactionContext, filters));
        }

        //nuevo
        public Task<int> GetDashboardDataCount(ReportsDashboardFilterDto filters)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestPost(BaseUri + "/GetDashboardDataCount", TransactionContext, filters));
        }
    }
}
