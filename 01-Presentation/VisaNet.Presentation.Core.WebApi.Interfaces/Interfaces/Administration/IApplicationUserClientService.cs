using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ReportsModel;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IApplicationUserClientService
    {
        Task<ICollection<ApplicationUserDto>> FindAll();
        Task<ICollection<ApplicationUserDto>> FindAll(BaseFilter filtersDto);
        Task<ApplicationUserDto> Find(Guid id);
        Task<ApplicationUserDto> Find(string username);
        Task Edit(ApplicationUserDto entity);
        Task Delete(Guid id);

        Task<bool> ValidateUser(ValidateUserDto entity);
        Task<bool> ConfirmUser(ConfirmUserDto entity);
        Task ResetPassword(string username);
        Task ResetPasswordFromToken(ResetPasswordFromTokenDto entity);
        Task ChangePassword(Guid id, string password);

        Task<IEnumerable<ApplicationUserDto>> GetDashboardData(ReportsDashboardFilterDto filtersDto);

        Task<int> GetDashboardDataCount(ReportsDashboardFilterDto filtersDto);

        Task<ICollection<ApplicationUserDto>> GetDataForReportsUser(ReportsUserFilterDto filterDto);
        Task<int> GetDataForReportsUserCount(ReportsUserFilterDto filterDto);

        Task<bool> ChangeBlockStatusUser(Guid userId);

        Task<ICollection<ReportCardsViewDto>> ReportsCardsData(ReportsCardsFilterDto filters);
        Task<int> ReportsCardsDataCount(ReportsCardsFilterDto filters);

    }
}