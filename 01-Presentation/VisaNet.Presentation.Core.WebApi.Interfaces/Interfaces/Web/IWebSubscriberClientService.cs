using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebSubscriberClientService
    {
        Task Create(SubscriberDto subscriber);
        Task DeleteByEmail(string email);
        Task<bool> ExistsEmail(string email);
    }
}
