using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class FixedNotificationClientService : WebApiClientService, IFixedNotificationClientService
    {
        public FixedNotificationClientService(ITransactionContext transactionContext)
            : base("FixedNotification", transactionContext)
        {

        }

        public Task<ICollection<FixedNotificationDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<FixedNotificationDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<IEnumerable<FixedNotificationDto>> FindAll(FixedNotificationFilterDto filters)
        {
            return WebApiClient.CallApiServiceAsync<IEnumerable<FixedNotificationDto>>(new WebApiHttpRequestPost(BaseUri + "/FindAll", TransactionContext, filters));
        }

        public Task Edit(FixedNotificationDto model)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/Edit", TransactionContext, model.Id, model));
        }

        public Task<FixedNotificationDto> GetById(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<FixedNotificationDto>(new WebApiHttpRequestGet(BaseUri + "/GetById", TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task ResolveAll(FixedNotificationFilterDto filter, string comment)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/ResolveAll", TransactionContext, new ResolveAllFixedDto { Filter = filter, Comment = comment }));
        }
    }
}
