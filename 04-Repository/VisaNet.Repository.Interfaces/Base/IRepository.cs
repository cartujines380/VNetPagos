using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.Repository.Interfaces.Base
{
    public interface IRepository<T>
    {
        IQueryable<T> All(Expression<Func<T, bool>> includeFilters = null, params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> AllNoTracking(Expression<Func<T, bool>> includeFilters = null, params Expression<Func<T, object>>[] includeProperties);

        T GetById(Guid id, params Expression<Func<T, object>>[] includeProperties);
        void Create(T entity);
        void Edit(T entity);

        void Delete(Guid id);
        void Delete(T entity);
        void AddEntitiesNoRepository(object entity);
        void DeleteEntitiesNoRepository(object entity);
        void Save();
        void Atach(T entity);
        int ExecuteSQL(string sql, SqlParameter[] parameters);
        bool ContextTrackChanges { get; set; }

        Dictionary<string, object> GetDataLog();
    }
}
