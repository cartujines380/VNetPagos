using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ReportsModel;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class ApplicationUserClientService : WebApiClientService, IApplicationUserClientService
    {
        public ApplicationUserClientService(ITransactionContext transactionContext)
            : base("ApplicationUser", transactionContext)
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

        public Task<bool> ConfirmUser(ConfirmUserDto entity)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestPost(BaseUri + "/ConfirmUser", TransactionContext, entity));
        }

        public Task ResetPassword(string username)
        {
            return
                WebApiClient.CallApiServiceAsync(new WebApiHttpRequestGet(
                    BaseUri + "/ResetPassword", TransactionContext, new Dictionary<string, string> { { "username", username } }));
        }

        public Task ChangePassword(Guid id, string password)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestGet(BaseUri + "/ChangePassword", TransactionContext, new Dictionary<string, string> { { "id", id.ToString() }, { "password", password } }));
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

        public Task<IEnumerable<ApplicationUserDto>> GetDashboardData(ReportsDashboardFilterDto filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<IEnumerable<ApplicationUserDto>>(new WebApiHttpRequestPost(BaseUri + "/GetDashboardData", TransactionContext, filtersDto));
        }

        public Task<int> GetDashboardDataCount(ReportsDashboardFilterDto filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestPost(BaseUri + "/GetDashboardDataCount", TransactionContext, filtersDto));
        }

        public Task<ICollection<ApplicationUserDto>> GetDataForReportsUser(ReportsUserFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ApplicationUserDto>>(new WebApiHttpRequestPost(BaseUri + "/GetDataForReportsUser", TransactionContext, filterDto));
        }

        public Task<int> GetDataForReportsUserCount(ReportsUserFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestPost(BaseUri + "/GetDataForReportsUserCount", TransactionContext, filterDto));
        }

        public Task<bool> ChangeBlockStatusUser(Guid userId)
        {
            return
                WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(
                    BaseUri + "/ChangeBlockStatusUser", TransactionContext, new Dictionary<string, string> { { "id", userId.ToString() } }));
        }

        public Task<ICollection<ReportCardsViewDto>> ReportsCardsData(ReportsCardsFilterDto filters)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ReportCardsViewDto>>(new WebApiHttpRequestGet(
                BaseUri + "/ReportsCardsData", TransactionContext, filters.GetFilterDictionary()));
        }

        public Task<int> ReportsCardsDataCount(ReportsCardsFilterDto filters)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(
                BaseUri + "/ReportsCardsDataCount", TransactionContext, filters.GetFilterDictionary()));
        }

    }
}