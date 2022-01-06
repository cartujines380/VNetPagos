using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebFaqClientService
    {
        Task<ICollection<FaqDto>> FindAll();
        Task<ICollection<FaqDto>> FindAll(BaseFilter filtersDto);
        Task<FaqDto> Find(Guid id);
    }
}
