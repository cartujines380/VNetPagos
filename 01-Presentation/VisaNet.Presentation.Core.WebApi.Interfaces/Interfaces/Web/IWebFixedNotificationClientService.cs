using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebFixedNotificationClientService
    {
        Task Create(FixedNotificationDto fixedNotification);
    }
}
