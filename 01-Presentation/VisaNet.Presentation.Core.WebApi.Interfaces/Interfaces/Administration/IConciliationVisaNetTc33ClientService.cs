
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IConciliationVisaNetTc33ClientService
    {
        Task<ConciliationVisanetDto> Create(ConciliationVisanetDto dto);
        Task Edit(ConciliationVisanetDto dto);
    }
}
