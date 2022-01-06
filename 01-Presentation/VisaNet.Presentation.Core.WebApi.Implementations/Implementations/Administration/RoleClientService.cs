using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Common.Security.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;
using Action = VisaNet.Common.Security.Entities.Action;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class RoleClientService : WebApiClientService, IRoleClientService
    {
        public RoleClientService(ITransactionContext transactionContext)
            : base("Role", transactionContext)
        {

        }

        public Task<ICollection<RoleDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<RoleDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<ICollection<RoleDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<RoleDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<RoleDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<RoleDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task Create(RoleDto serviceCategory)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, serviceCategory));
        }

        public Task Edit(RoleDto serviceCategory)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, serviceCategory.Id, serviceCategory));
        }

        public Task Delete(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri, TransactionContext, id));
        }

        public Task<IEnumerable<FunctionalityGroup>> GetFunctionalityGroups()
        {
            return
                WebApiClient.CallApiServiceAsync<IEnumerable<FunctionalityGroup>>(new WebApiHttpRequestGet(BaseUri + "/GetFunctionalityGroups", TransactionContext));
        }

        public Task<IEnumerable<Action>> GetActions()
        {
            return
                WebApiClient.CallApiServiceAsync<IEnumerable<Action>>(new WebApiHttpRequestGet(BaseUri + "/GetActions", TransactionContext));
        }
    }
}
