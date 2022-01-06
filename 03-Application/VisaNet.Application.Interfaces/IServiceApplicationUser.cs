using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceApplicationUser : IService<ApplicationUser, ApplicationUserDto>
    {
        IEnumerable<ApplicationUserDto> GetDataForTable(ApplicationUserFilterDto filters);
        void EditFromBO(ApplicationUserDto entity);
        void Create(ApplicationUserCreateEditDto entity);
        ApplicationUserDto CreateUserWithoutPassword(ApplicationUserCreateEditDto entity, long? cybersourceIdentifier = null);
        void ChangePassword(Guid id, string email, string oldPassword, string newPassword);
        void ChangePasswordWeb(string email, string oldPassword, string newPassword);
        void ChangePasswordFromBO(Guid id, string newPassword);
        int ResetPassword(string email);
        bool ResetPasswordFromToken(ResetPasswordFromTokenDto model);
        bool ValidateUser(string email, string password);
        ValidateUserResponse ValidateUserWeb(string email, string password);
        ApplicationUserDto SearchUser(Guid id, string identityNumber);
        ApplicationUserDto GetUserByUserName(string username);
        bool ConfirmUser(string email, string token);

        string ResetPasswordForUser(string email);
        void InactivateUser(Guid id);
        IEnumerable<ApplicationUserDto> GetApplicationUserAutoComplete(string contains);
        void ActiveUserSistarbanc(Guid userId);
        CardDto AddCard(CardDto cardDto, Guid userId);
        CybersourceCreateCardDto AddCard(IDictionary<string, string> cybersourceData);

        IEnumerable<ApplicationUserDto> GetDashboardData(ReportsDashboardFilterDto filters);
        int GetDashboardDataCount(ReportsDashboardFilterDto filters);

        List<WebServiceApplicationClientDto> AssosiatedServiceClientUpdate(WebServiceClientInputDto dto);

        ICollection<ApplicationUserDto> GetDataForReportsUser(ReportsUserFilterDto filterDto);
        int GetDataForReportsUserCount(ReportsUserFilterDto filterDto);

        bool ChangeBlockStatusUser(Guid userid);

        long GetNextCyberSourceIdentifier();

    }
}