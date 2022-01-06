using System;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryService : IRepository<Service>
    {
        Service GetService(int codCommerce, int codBranch);
        Service GetService(string merchantId, string appId);
        bool IsBinAssociatedToService(int binValue, Guid serviceId);
    }
}
