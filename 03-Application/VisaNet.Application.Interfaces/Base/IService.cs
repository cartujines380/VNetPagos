using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace VisaNet.Application.Interfaces.Base
{
    public interface IService<T, Q>
    {
        IEnumerable<Q> All(Expression<Func<T, Q>> select = null, Expression<Func<T, bool>> where = null, params Expression<Func<T, object>>[] properties);
        IEnumerable<Q> AllNoTracking(Expression<Func<T, Q>> select = null, Expression<Func<T, bool>> where = null, params Expression<Func<T, object>>[] properties);
        
        Q GetById(Guid id, params Expression<Func<T, object>>[] properties);
        Q Create(Q entity, bool returnEntity = false);
        void Edit(Q entity);
        
        void Delete(Guid id);
        void Delete(Q entity);
        void DeleteWithOutSaving(Guid id);
        void DeleteWithOutSaving(Q entity);
        void AddEntitiesNoRepository(object entity);
        void DeleteEntitiesNoRepository(object entity);

        void Atach(T entity);
        IQueryable<T> GetDataForTable();
        Q Converter(T entity);
        T Converter(Q entity);
    }
}
