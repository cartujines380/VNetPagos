using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebHighwayClientService
    {
        Task ProccessEmail(HighwayEmailDto dto);
    }
}
