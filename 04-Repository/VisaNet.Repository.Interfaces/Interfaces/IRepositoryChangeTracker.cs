using System;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryApplicationUser: IRepository<ApplicationUser>
    {
        long GetNextCyberSourceIdentifier();
        Card AddCardToUser(Guid userId, Card card);
        IWebApiTransactionContext GetTransactionContext();

    }
}
