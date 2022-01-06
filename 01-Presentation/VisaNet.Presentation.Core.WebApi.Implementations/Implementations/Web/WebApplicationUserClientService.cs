using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebApplicationUserClientService : WebApiClientService, IWebApplicationUserClientService
    {
        public WebApplicationUserClientService(ITransactionContext transactionContext)
            : base("WebApplicationUser", transactionContext)
        {

        }

        public Task<ICollection<ApplicationUserDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ApplicationUserDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<ICollection<ApplicationUserDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ApplicationUserDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<ApplicationUserDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<ApplicationUserDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task<ApplicationUserDto> Find(Guid id, string identityNumber)
        {
            return
                WebApiClient.CallApiServiceAsync<ApplicationUserDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() }, { "identityNumber", identityNumber } }));
        }

        public Task<ApplicationUserDto> Find(string username)
        {
            return
                WebApiClient.CallApiServiceAsync<ApplicationUserDto>(new WebApiHttpRequestGet(
                    BaseUri + "/GetUserByUserName", TransactionContext, new Dictionary<string, string> { { "username", username } }));
        }

        public Task Edit(ApplicationUserDto entity)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, entity.Id, entity));
        }

        public Task Delete(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri, TransactionContext, id));
        }

        public Task<bool> ValidateUser(ValidateUserDto entity)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestPost(BaseUri + "/ValidateUser", TransactionContext, entity));
        }

        public Task<ValidateUserResponse> ValidateUserWeb(ValidateUserDto entity)
        {
            return WebApiClient.CallApiServiceAsync<ValidateUserResponse>(new WebApiHttpRequestPost(BaseUri + "/ValidateUserWeb", TransactionContext, entity));
        }

        public Task<bool> ConfirmUser(ConfirmUserDto entity)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestPost(BaseUri + "/ConfirmUser", TransactionContext, entity));
        }

        public Task<int> ResetPassword(string username)
        {
            return
                WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(
                    BaseUri + "/ResetPassword", TransactionContext, new Dictionary<string, string> { { "username", username } }));
        }

        public Task ChangePassword(Guid id, string email, string oldPassword, string newPassword)
        {
            return
                WebApiClient.CallApiServiceAsync(new WebApiHttpRequestGet(
                    BaseUri + "/ChangePassword", TransactionContext,
                    new Dictionary<string, string>
                    {
                        {"id", id.ToString()},
                        {"email", email},
                        {"oldPassword", oldPassword},
                        {"newPassword", newPassword}
                    }));
        }

        public Task ChangePasswordWeb(string email, string oldPassword, string newPassword)
        {
            return
                WebApiClient.CallApiServiceAsync(new WebApiHttpRequestGet(
                    BaseUri + "/ChangePasswordWeb", TransactionContext,
                    new Dictionary<string, string>
                    {
                        {"email", email},
                        {"oldPassword", oldPassword},
                        {"newPassword", newPassword}
                    }));
        }

        public Task ResetPasswordFromToken(ResetPasswordFromTokenDto entity)
        {
            return
                WebApiClient.CallApiServiceAsync(new WebApiHttpRequestGet(
                    BaseUri + "/ResetPasswordFromToken", TransactionContext,
                    new Dictionary<string, string>
                    {
                        {"Token", entity.Token},
                        {"UserName", entity.UserName},
                        {"Password", entity.Password}
                    }));
        }

        public Task<string> ResetPasswordForUser(string username)
        {
            return
                WebApiClient.CallApiServiceAsync<string>(new WebApiHttpRequestGet(
                    BaseUri + "/ResetPasswordForUser", TransactionContext, new Dictionary<string, string> { { "username", username } }));
        }

        public Task InactivateUser(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync(new WebApiHttpRequestGet(
                    BaseUri + "/InactivateUser", TransactionContext,
                    new Dictionary<string, string>
                    {
                        { "id", id.ToString() }
                    }));
        }

        public Task<CardDto> AddCard(Guid id, CardDto cardDto)
        {
            return WebApiClient.CallApiServiceAsync<CardDto>(new WebApiHttpRequestPost(BaseUri + "/AddCard", TransactionContext, id, cardDto));
        }

        public Task<CybersourceCreateCardDto> AddCard(IDictionary<string, string> csData)
        {
            return WebApiClient.CallApiServiceAsync<CybersourceCreateCardDto>(new WebApiHttpRequestPost(BaseUri + "/AddCard", TransactionContext, csData));
        }

        public Task<ApplicationUserDto> GetUserWithCards(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<ApplicationUserDto>(new WebApiHttpRequestGet(
                  BaseUri + "/GetUserWithCards", TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));

        }

        public Task<long> GetNextCyberSourceIdentifier()
        {
            return WebApiClient.CallApiServiceAsync<long>(new WebApiHttpRequestGet(
                    BaseUri + "/GetNextCyberSourceIdentifier", TransactionContext));
        }

    }
}