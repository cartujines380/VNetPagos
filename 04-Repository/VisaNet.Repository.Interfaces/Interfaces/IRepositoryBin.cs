using System;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryBin : IRepository<Bin>
    {
        Bin GetDefaultBin();

        void ExecuteBinManagerSp_insert(string name, int value, string gatewayId, int cardType, int authorizationAmountType, string country, string issuerBin, string processorBin, string user, string groupBinId, bool editedInBo);
        void ExecuteBinManagerSp_Delete(Guid id);
        void ExecuteBinManagerSp_Update(Guid id, string name, string gatewayId, int cardType, int authorizationAmountType, string country, string issuerBin, string processorBin, string user, string groupBinId, string oldDefaultGroupId, bool editedInBo);
    }
}
