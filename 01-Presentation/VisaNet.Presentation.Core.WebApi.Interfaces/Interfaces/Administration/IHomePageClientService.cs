using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IHomePageClientService
    {
        Task<ICollection<HomePageDto>> FindAll();
        Task<HomePageDto> Find(Guid id);
        Task Edit(HomePageDto page);
    }
}
