using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebLogClientService : WebApiClientService, IWebLogClientService
    {
        public WebLogClientService(ITransactionContext transactionContext)
            : base("WebLog", transactionContext)
        {

        }

        public Task<LogDto> Find(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<LogDto>(new WebApiHttpRequestGet(BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task<ICollection<LogDto>> FindAll(LogFilterDto filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<LogDto>>(new WebApiHttpRequestPost(BaseUri + "/GetForTable", TransactionContext, filtersDto));
        }

        public Task Put(LogModel model)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, model));
        }

        public Task Put(LogAnonymousModel model)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri + "/AnonymousLog", TransactionContext, model));
        }

        public Task Put(Guid id, LogDto model)
        {
            model.TemporaryId = id;
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri + "/CreateWithTemporaryId", TransactionContext, model));
        }
    }
}
