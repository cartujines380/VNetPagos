using System.Collections.Generic;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryConciliationSucive : IRepository<ConciliationSucive>
    {
        List<ConciliationSucive> GetConciliationsNotInSummary();
    }
}
