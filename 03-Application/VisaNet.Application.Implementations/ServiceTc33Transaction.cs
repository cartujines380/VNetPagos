using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceTc33Transaction : BaseService<Tc33Transaction, Tc33TransactionDto>, IServiceTc33Transaction
    {
        private readonly IRepositoryTc33Transaction _repository;

        public ServiceTc33Transaction(IRepositoryTc33Transaction repository) : base(repository)
        {
            _repository = repository;
        }
        
        public override IQueryable<Tc33Transaction> GetDataForTable()
        {
            throw new System.NotImplementedException();
        }

        public override Tc33TransactionDto Converter(Tc33Transaction entity)
        {
            return new Tc33TransactionDto
                   {
                       RequestId = entity.RequestId,
                       Id = entity.Id,
                       Tc33Id = entity.Tc33Id
                   };
        }

        public override Tc33Transaction Converter(Tc33TransactionDto entity)
        {
            return new Tc33Transaction
            {
                RequestId = entity.RequestId,
                Id = entity.Id,
                Tc33Id = entity.Tc33Id
            };
        }

        public IEnumerable<T> ExecuteSql<T>(string sql, object[] parameters)
        {
            return _repository.ExecuteSql<T>(sql, parameters);
        }

    }
}
