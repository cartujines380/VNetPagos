using System;
using System.Linq.Expressions;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryBinGroup : IRepository<BinGroup>
    {
        BinGroup GetByIdTracking(Guid id, params Expression<Func<BinGroup, object>>[] includeProperties);
    }
}
