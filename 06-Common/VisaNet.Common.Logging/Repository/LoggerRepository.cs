using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using VisaNet.Common.Logging.Context;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Security;

namespace VisaNet.Common.Logging.Repository
{
    public class LoggerRepository : ILoggerRepository
    {
        internal readonly DbContext _context;
        private readonly IDbSet<Log> _dbset;
        private readonly Database _db;
        private readonly IWebApiTransactionContext _transactionContext;

        public LoggerRepository(LogContext context, IWebApiTransactionContext transactionContext)
        {
            _context = context;
            _transactionContext = transactionContext;
            _dbset = context.Set<Log>();
            _db = context.Database;

            _context.Configuration.AutoDetectChangesEnabled = false;
            _context.Configuration.LazyLoadingEnabled = false;
            _context.Configuration.ProxyCreationEnabled = false;
        }

        public IQueryable<Log> All(Expression<Func<Log, bool>> includeFilters = null, params Expression<Func<Log, object>>[] includeProperties)
        {
            IQueryable<Log> query = _dbset.AsQueryable();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return includeFilters == null ? query : query.Where(includeFilters);
        }

        public IQueryable<Log> AllNoTracking(Expression<Func<Log, bool>> includeFilters = null, params Expression<Func<Log, object>>[] includeProperties)
        {
            IQueryable<Log> query = _dbset.AsQueryable();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return includeFilters == null ? query.AsQueryable().AsNoTracking() : query.AsQueryable().AsNoTracking().Where(includeFilters);
        }

        public IEnumerable<T> ExecuteSQL<T>(string sql, object[] parameters)
        {
            if (parameters != null)
            {
                return _db.SqlQuery<T>(sql, parameters);
            }
            
            return _db.SqlQuery<T>(sql);
        }

        public void Create(Log entity)
        {
            entity.GenerateNewIdentity();
            
            if (entity.LogPaymentCyberSource != null)
                entity.LogPaymentCyberSource.GenerateNewIdentity();
            
            _dbset.Add(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void SetTrackingStatus(bool track)
        {
            _context.Configuration.AutoDetectChangesEnabled = track;
        }
        
    }


    public class LogCs
    {
        public Guid Id { get; set; }
        public int TransactionType { get; set; }
        public string TransactionId { get; set; }
        public string AuthTime { get; set; }
        public string AuthCode { get; set; }
        public string AuthAmount { get; set; }
        public string ReqCurrency { get; set; }
        public string BillTransRefNo { get; set; }
        public string ReqAmount { get; set; }

        public DateTime TransactionDateTime { get; set; }
    }
}
