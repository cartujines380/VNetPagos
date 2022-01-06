using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebLocationClientService
    {
        Task<ICollection<LocationDto>> FindAll();
        Task<ICollection<LocationDto>> GetList(BaseFilter filter);
    }
}
