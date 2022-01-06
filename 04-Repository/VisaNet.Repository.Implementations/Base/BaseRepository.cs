using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Implementations.Base
{
    public abstract class BaseRepository<T> : IRepository<T> where T : EntityBase
    {
        protected readonly DbContext _context;
        private readonly IDbSet<T> _dbset;
        protected readonly Database _db;
        private readonly IWebApiTransactionContext _transactionContext;

        public IWebApiTransactionContext TransactionContext
        {
            get { return _transactionContext; }
        }

        public Dictionary<string, object> GetDataLog()
        {
            return new Dictionary<string, object>
            {
                { "TransactionId", _transactionContext.TransactionIdentifier },
                { "UserIP", _transactionContext.IP },
                { "UserEmail", _transactionContext.UserName },
                { "TraceId", _transactionContext.TraceId }
            };
        }

        protected BaseRepository(DbContext context, IWebApiTransactionContext transactionContext)
        {
            _context = context;
            _transactionContext = transactionContext;
            _dbset = context.Set<T>();
            _db = context.Database;
            _context.Configuration.AutoDetectChangesEnabled = false;
            _context.Configuration.LazyLoadingEnabled = false;
            _context.Configuration.ProxyCreationEnabled = false;
        }

        public bool ContextTrackChanges
        {
            get { return _context.Configuration.AutoDetectChangesEnabled; }
            set { _context.Configuration.AutoDetectChangesEnabled = value; }
        }

        public IQueryable<T> All(Expression<Func<T, bool>> includeFilters = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbset.AsQueryable();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            if (includeFilters == null)
                return query;

            return query.Where(includeFilters);
        }

        public IQueryable<T> AllNoTracking(Expression<Func<T, bool>> includeFilters = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbset.AsQueryable();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            if (includeFilters == null)
                return query.AsQueryable().AsNoTracking();

            return query.AsQueryable().AsNoTracking().Where(includeFilters);
        }

        public T GetById(Guid id, params Expression<Func<T, object>>[] properties)
        {
            var query = _dbset.AsQueryable();

            query = properties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return query.FirstOrDefault(x => x.Id == id);
        }

        public void Create(T entity)
        {
            var auditable = entity as IAuditable;
            if (auditable != null)
            {
                auditable.CreationDate = DateTime.Now;
                auditable.CreationUser = TransactionContext.UserName;
                auditable.LastModificationDate = DateTime.Now;
                auditable.LastModificationUser = TransactionContext.UserName;
            }
            entity.GenerateNewIdentity();
            _dbset.Add(entity);
        }

        public void Edit(T entity)
        {
            //If Auditable
            var auditable = entity as IAuditable;
            if (auditable != null)
            {
                auditable.LastModificationDate = DateTime.Now;
                auditable.LastModificationUser = TransactionContext.UserName;
            }

            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(Guid id)
        {
            var entity = GetById(id);
            _dbset.Remove(entity);
        }

        public void Delete(T entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
        }

        public void AddEntitiesNoRepository(object entity)
        {
            if (entity != null)
            {
                if (IsProxy(entity))
                    _context.Set(entity.GetType().BaseType).Add(entity);
                else
                    _context.Set(entity.GetType()).Add(entity);
            }
        }

        public void DeleteEntitiesNoRepository(object entity)
        {
            if (entity != null)
            {
                if (IsProxy(entity))
                    _context.Set(entity.GetType().BaseType).Remove(entity);
                else
                    _context.Set(entity.GetType()).Remove(entity);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Atach(T entity)
        {
            var state = _context.Entry(entity).State;

            if (state == EntityState.Detached)
                _dbset.Attach(entity);
        }

        public int ExecuteSQL(string sql, SqlParameter[] parameters)
        {
            return parameters == null
                ? _db.ExecuteSqlCommand(sql)
                : _db.ExecuteSqlCommand(sql, parameters.ToArray<object>());
        }

        public IEnumerable<T> SqlQuery(string sql, params object[] parameters)
        {
            return _db.SqlQuery<T>(sql, parameters);
        }

        private static bool IsProxy(object type)
        {
            return type != null && ObjectContext.GetObjectType(type.GetType()) != type.GetType();
        }
    }
}
