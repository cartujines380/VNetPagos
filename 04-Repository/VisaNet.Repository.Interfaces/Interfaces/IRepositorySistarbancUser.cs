using System;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositorySistarbancUser : IRepository<SistarbancUser>
    {
        void ActiveUserSistarbanc(Guid id);
    }
}
