using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceConciliationVisanetCallback : IService<ConciliationVisanetCallback, ConciliationVisanetCallbackDto>
    {
        void DirectoryConciliation();
        void SingleFileConciliation(string fileName);
    }
}