using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebPageClientService
    {
        Task<ICollection<PageDto>> FindAll();
        Task<PageDto> Find(Guid id);
        Task<PageDto> FindType(PageTypeDto type);
    }
}
