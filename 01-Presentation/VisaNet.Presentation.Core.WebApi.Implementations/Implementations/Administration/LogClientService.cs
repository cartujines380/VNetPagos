using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Security;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class LogClientService : WebApiClientService, ILogClientService
    {
        public LogClientService(ITransactionContext transactionContext)
            : base("Log", transactionContext)
        {

        }

        public Task<LogDto> Find(string transactionId)
        {
            return WebApiClient.CallApiServiceAsync<LogDto>(new WebApiHttpRequestGet(BaseUri, TransactionContext, new Dictionary<string, string> { { "transactionId", transactionId } }));
        }
        public Task Put(LogModel model)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, model));
        }

        public Task Put(Guid id, LogDto model)
        {
            model.TemporaryId = id;
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri + "/CreateWithTemporaryId", TransactionContext, model));
        }
    }
}
