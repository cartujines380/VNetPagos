using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using TrackerEnabledDbContext;
using VisaNet.Common.ChangeTracker.Models;
using VisaNet.Common.Logging.NLog;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryChangeTracker : IRepositoryChangeTracker
    {
        protected readonly DbContext _context;
        private readonly IDbSet<AuditLog> _dbset;

        public RepositoryChangeTracker(TrackerContext context)
        {
            _dbset = context.Set<AuditLog>();
            _context = context;
        }
        public IQueryable<AuditLog> All(Expression<System.Func<AuditLog, bool>> includeFilters = null, params Expression<System.Func<AuditLog, object>>[] includeProperties)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<AuditLog> AllNoTracking(Expression<System.Func<AuditLog, bool>> includeFilters = null, params Expression<System.Func<AuditLog, object>>[] includeProperties)
        {
            var query = _dbset.AsQueryable();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return includeFilters == null ? query.AsQueryable().AsNoTracking() : query.AsQueryable().AsNoTracking().Where(includeFilters);
        }

        public AuditLog GetById(Guid id, params Expression<Func<AuditLog, object>>[] properties)
        {
            var entity = _dbset.Find(id);

            if (entity != null)
            {
                foreach (var includeProperty in properties)
                {
                    bool cargado;
                    try
                    {
                        _context.Entry(entity).Collection(includeProperty.ToString().Split('.')[1]).Load();
                        cargado = true;
                    }
                    catch (Exception)
                    {
                        cargado = false;
                    }

                    if (!cargado)
                    {
                        _context.Entry(entity).Reference(includeProperty).Load();
                    }
                }
            }
            return entity;
        }

        public void Create(AuditLog entity)
        {
            throw new System.NotImplementedException();
        }

        public void Edit(AuditLog entity)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(System.Guid id)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(AuditLog entity)
        {
            throw new System.NotImplementedException();
        }

        public void AddEntitiesNoRepository(object entity)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteEntitiesNoRepository(object entity)
        {
            throw new System.NotImplementedException();
        }

        public void Save()
        {
            throw new System.NotImplementedException();
        }

        public void Atach(AuditLog entity)
        {
            throw new System.NotImplementedException();
        }

        public int ExecuteSQL(string sql, System.Data.SqlClient.SqlParameter[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public bool ContextTrackChanges
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public object ExecuteNonQuery(Type type, string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, object> GetDataLog()
        {
            throw new NotImplementedException();
        }
    }
}