using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebGatewayClientService
    {
        Task<ICollection<GatewayDto>> FindAll();
        
    }
}
