using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Common.Security.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebSystemUserClientService : WebApiClientService, IWebSystemUserClientService
    {
        public WebSystemUserClientService(ITransactionContext transactionContext)
            : base("WebSystemUser", transactionContext)
        {

        }

        public Task<ICollection<SystemUserDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<SystemUserDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<ICollection<SystemUserDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<SystemUserDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<SystemUserDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<SystemUserDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task<SystemUserDto> Find(string username)
        {
            return
                WebApiClient.CallApiServiceAsync<SystemUserDto>(new WebApiHttpRequestGet(
                    BaseUri + "/GetUserByUserName", TransactionContext, new Dictionary<string, string> { { "username", username } }));
        }

        public Task Create(SystemUserDto entity)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, entity));
        }

        public Task Edit(SystemUserDto entity)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, entity.Id, entity));
        }

        public Task Delete(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri, TransactionContext, id));
        }

        public Task<bool> ValidateUserInRole(ValidateUserInRoleDto entity)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestPost(BaseUri + "/ValidateUserInRole", TransactionContext, entity));
        }

        public Task<bool> ValidateUser(ValidateUserDto entity)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestPost(BaseUri + "/ValidateUser", TransactionContext, entity));
        }

        public Task<IEnumerable<FunctionalityGroup>> GetPermissionsFromRoles(IEnumerable<Guid> roleIds)
        {
            return WebApiClient.CallApiServiceAsync<IEnumerable<FunctionalityGroup>>(new WebApiHttpRequestPost(BaseUri + "/GetPermissionsFromRoles", TransactionContext, roleIds));
        }
    }
}
