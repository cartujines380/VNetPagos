using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VisaNet.Common.Logging.Entities;

namespace VisaNet.Common.Logging.Repository
{
    public interface ILoggerRepository
    {
        IQueryable<Log> All(Expression<Func<Log, bool>> includeFilters = null, params Expression<Func<Log, object>>[] includeProperties);
        IQueryable<Log> AllNoTracking(Expression<Func<Log, bool>> includeFilters = null, params Expression<Func<Log, object>>[] includeProperties);
        IEnumerable<T> ExecuteSQL<T>(string sql, object[] parameters);
        void Create(Log entity);
        void Save();
        void SetTrackingStatus(bool track);
    }
}
