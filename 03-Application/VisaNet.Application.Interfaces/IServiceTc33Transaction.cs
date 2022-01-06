using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceTc33Transaction : IService<Tc33Transaction, Tc33TransactionDto>
    {
        IEnumerable<T> ExecuteSql<T>(string sql, object[] parameters);
    }
}
