using System;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryBill : IRepository<Bill>
    {
        bool IsBillExlternalIdRepitedByServiceId(string billExternalId, Guid serviceId);
        bool IsBillExlternalIdRepitedByMerchantId(string billExternalId, string merchantId);
    }
}
