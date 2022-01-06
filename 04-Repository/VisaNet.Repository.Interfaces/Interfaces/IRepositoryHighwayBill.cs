using System;
using System.Collections.Generic;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryHighwayBill : IRepository<HighwayBill>
    {
        
        string UserAction();

        List<String> MasiveInsert(string[] lines, int codCommerce, int codBranch, Guid emailId, Guid serviceId, out int countN,
            out int countD, out double valueN, out double valueD);
    }
}
