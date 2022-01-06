using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IHomePageItemClientService
    {
        Task<ICollection<HomePageItemDto>> FindAll();
        Task<HomePageItemDto> Find(Guid id);
        Task Edit(HomePageItemDto page);
    }
}
