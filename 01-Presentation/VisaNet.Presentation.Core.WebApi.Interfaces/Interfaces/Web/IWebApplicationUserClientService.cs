using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebApplicationUserClientService
    {
        Task<ICollection<ApplicationUserDto>> FindAll();
        Task<ICollection<ApplicationUserDto>> FindAll(BaseFilter filtersDto);
        Task<ApplicationUserDto> Find(Guid id);
        Task<ApplicationUserDto> Find(Guid id, string identityNumber);
        Task<ApplicationUserDto> Find(string username);
        Task Edit(ApplicationUserDto entity);
        Task Delete(Guid id);

        Task<bool> ValidateUser(ValidateUserDto entity);
        Task<ValidateUserResponse> ValidateUserWeb(ValidateUserDto entity);
        Task<bool> ConfirmUser(ConfirmUserDto entity);
        /// <summary>
        /// Envia email de cambio de contraseña o activacion de cuenta. Devuelve 1 = cambio de contraseña, 2 = activacion de usuario
        /// </summary>
        /// <param name="username"></param>
        /// <returns>
        /// 1 = cambio de contraseña
        /// 2 = activacion de usuario 
        /// </returns>
        Task<int> ResetPassword(string username);
        Task ResetPasswordFromToken(ResetPasswordFromTokenDto entity);

        Task ChangePassword(Guid id, string email, string oldPassword, string newPassword);
        Task ChangePasswordWeb(string email, string oldPassword, string newPassword);

        Task<string> ResetPasswordForUser(string username);
        Task InactivateUser(Guid id);

        Task<CardDto> AddCard(Guid id, CardDto cardDto);
        Task<CybersourceCreateCardDto> AddCard(IDictionary<string, string> csData);

        Task<ApplicationUserDto> GetUserWithCards(Guid id);

        Task<long> GetNextCyberSourceIdentifier();

    }
}