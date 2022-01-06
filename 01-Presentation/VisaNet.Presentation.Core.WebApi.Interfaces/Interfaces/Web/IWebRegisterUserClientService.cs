using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebRegisterUserClientService
    {
        Task Create(ApplicationUserCreateEditDto entity);
        Task<ApplicationUserDto> CreateUserWithoutPassword(ApplicationUserCreateEditDto entity);
    }
}