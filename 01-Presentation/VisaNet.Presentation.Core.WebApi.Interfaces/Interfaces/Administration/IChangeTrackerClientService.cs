using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using AuditLogDto = VisaNet.Domain.EntitiesDtos.ChangeTracker.AuditLogDto;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IChangeTrackerClientService
    {
        Task<ICollection<AuditLogDto>> FindAll(ChangeTrackerFilterDto filter);
        Task<AuditLogDto> GetChangesDetails(int id);
        Task<int[]> Count(ChangeTrackerFilterDto filter);
        Task<List<string>> GetEntities();
    }
}
