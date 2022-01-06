using System.Collections.Generic;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryTc33Transaction : IRepository<Tc33Transaction>
    {
        IEnumerable<T> ExecuteSql<T>(string sql, object[] parameters);
    }
}
