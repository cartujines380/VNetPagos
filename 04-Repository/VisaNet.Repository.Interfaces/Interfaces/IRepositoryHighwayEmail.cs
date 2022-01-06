using VisaNet.Domain.Entities;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryHighwayEmail : IRepository<HighwayEmail>
    {
        long GetNextTransactionNumber();
    }
}
