using System.Threading.Tasks;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceConciliationCybersource : IService<ConciliationCybersource, ConciliationCybersourceDto>
    {
        Task<bool> GetConciliation(ReportsConciliationFilterDto filtersDto, bool? isManualRun = null);
    }
}