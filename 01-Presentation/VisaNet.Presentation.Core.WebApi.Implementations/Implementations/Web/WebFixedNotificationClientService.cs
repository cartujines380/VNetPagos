using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebFixedNotificationClientService : WebApiClientService, IWebFixedNotificationClientService
    {
        public WebFixedNotificationClientService(ITransactionContext transactionContext)
            : base("WebFixedNotification", transactionContext)
        {

        }


        public Task Create(FixedNotificationDto fixedNotification)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, fixedNotification));
        }
    }
}
