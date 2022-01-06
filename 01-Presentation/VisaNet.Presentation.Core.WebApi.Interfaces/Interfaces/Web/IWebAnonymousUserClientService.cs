using System;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebAnonymousUserClientService
    {
        Task<AnonymousUserDto> Find(Guid id);
        Task<AnonymousUserDto> FindByEmail(string email);
        Task<AnonymousUserDto> Create(AnonymousUserDto entity);
        Task Edit(AnonymousUserDto entity);
        Task<AnonymousUserDto> CreateOrEditAnonymousUser(AnonymousUserDto entity);
    }
}
