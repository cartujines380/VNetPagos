using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceConciliationBanred : IService<ConciliationBanred, ConciliationBanredDto>
    {
        bool DirectoryConciliation();
        bool SingleFileConciliation(string fileName);
    }
}