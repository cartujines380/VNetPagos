using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebLogClientService
    {
        Task<LogDto> Find(Guid id);
        //Task<ICollection<LogDto>> FindAll(BaseFilter filtersDto);
        Task<ICollection<LogDto>> FindAll(LogFilterDto filtersDto);
        Task Put(LogModel model);
        Task Put(LogAnonymousModel model);
        Task Put(Guid id, LogDto log);
    }
}
