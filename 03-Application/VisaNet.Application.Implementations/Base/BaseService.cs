using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Application.Implementations.Base
{
    public abstract class BaseService<T, Q> : IService<T, Q>
        where T : class
        where Q : class
    {
        protected readonly IRepository<T> Repository;

        protected BaseService(IRepository<T> repository)
        {
            Repository = repository;
        }

        public virtual IEnumerable<Q> All(Expression<Func<T, Q>> select = null, Expression<Func<T, bool>> where = null, params Expression<Func<T, object>>[] properties)
        {
            if (@select != null)
                return Repository.All(@where, properties).Select(@select).ToList();
            return Repository.All(@where, properties).Select(Converter).ToList();
        }

        public virtual IEnumerable<Q> AllNoTracking(Expression<Func<T, Q>> select = null, Expression<Func<T, bool>> where = null, params Expression<Func<T, object>>[] properties)
        {
            if (@select != null)
                return Repository.AllNoTracking(@where, properties).Select(@select).ToList();
            return Repository.AllNoTracking(@where, properties).Select(Converter).ToList();
        }

        public virtual Q GetById(Guid id, params Expression<Func<T, object>>[] properties)
        {
            return Converter(Repository.GetById(id, properties));
        }

        public virtual Q Create(Q entity, bool returnEntity = false)
        {
            var efEntity = Converter(entity);
            //If Auditable



            Repository.Create(efEntity);
            Repository.Save();
            return returnEntity ? Converter(efEntity) : null;
        }

        public virtual void Edit(Q entity)
        {
            var efEntity = Converter(entity);

            Repository.Edit(efEntity);
            Repository.Save();
        }

        public virtual void Delete(Guid id)
        {
            Repository.Delete(id);
            Repository.Save();
        }

        public virtual void Delete(Q entity)
        {
            Repository.Delete(Converter(entity));
            Repository.Save();
        }

        public virtual void DeleteWithOutSaving(Guid id)
        {
            Repository.Delete(id);
        }

        public virtual void DeleteWithOutSaving(Q entity)
        {
            Repository.Delete(Converter(entity));
        }
        public virtual void AddEntitiesNoRepository(object entity)
        {
            Repository.AddEntitiesNoRepository(entity);
        }

        public virtual void DeleteEntitiesNoRepository(object entity)
        {
            Repository.DeleteEntitiesNoRepository(entity);
        }

        public virtual void Atach(T entity)
        {
            Repository.Atach(entity);
        }


        public abstract IQueryable<T> GetDataForTable();
        public abstract Q Converter(T entity);
        public abstract T Converter(Q entity);
    }
}
