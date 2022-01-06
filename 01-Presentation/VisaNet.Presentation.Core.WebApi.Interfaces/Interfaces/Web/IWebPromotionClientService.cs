using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebPromotionClientService
    {
        Task<ICollection<PromotionDto>> FindActive();
    }
}
