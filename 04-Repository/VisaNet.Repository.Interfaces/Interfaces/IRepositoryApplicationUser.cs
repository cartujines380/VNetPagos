using VisaNet.Common.ChangeTracker.Models;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryChangeTracker : IRepository<AuditLog>
    {
    }
}
