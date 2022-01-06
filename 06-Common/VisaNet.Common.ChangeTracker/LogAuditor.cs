using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using VisaNet.Common.ChangeTracker;
using VisaNet.Common.ChangeTracker.Models;
using VisaNet.Common.Security;

namespace TrackerEnabledDbContext
{
    public class LogAuditor : IDisposable
    {
        private readonly DbEntityEntry _dbEntry;
        private readonly IWebApiTransactionContext _transactionContext;

        public LogAuditor(DbEntityEntry dbEntry, IWebApiTransactionContext transactionContext)
        {
            _dbEntry = dbEntry;
            _transactionContext = transactionContext;
        }

        public AuditLog GetLogRecord(object userName, EventType eventType, DbContext context)
        {
            var entityType = Helper.GetEntityType(_dbEntry.Entity.GetType());
            DateTime changeTime = DateTime.Now;

            TrackChanges trackChangesAttr = entityType.GetCustomAttributes(typeof(TrackChanges), false).SingleOrDefault() as TrackChanges;

            if (trackChangesAttr == null || !trackChangesAttr.Enabled) { return null; }

            //Get primary key value (If you have more than one key column, this will need to be adjusted)
            string keyName;
            try
            {
                keyName =
                    entityType.GetProperties()
                        .Single(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any())
                        .Name;
            }
            catch (Exception)
            {
                throw new KeyNotFoundException(string.Format(@"A single primary key attribute is reqiured per entity for tracker to work. 
The entity '{0}' does not contain a primary key attribute or contains more than one.", entityType.Name));
            }

            var newlog = new AuditLog
            {
                TransactionIdentifier = _transactionContext.TransactionIdentifier,
                IP = _transactionContext.IP,
                UserName = userName.ToString(),
                EventDate = changeTime,
                EventType = eventType,
                TableName = GetTableName(entityType, context),
                RecordId = _dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                AditionalInfo = GetTrackChangesAditionalInfo(entityType)
            };

            using (var detailsAuditor = new LogDetailsAuditor(_dbEntry, newlog))
            {
                newlog.LogDetails = detailsAuditor.GetLogDetails().ToList();
            }

            return newlog;
        }

        private string GetTableName(Type entityType, DbContext context)
        {
            TableAttribute tableAttr = entityType.GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

            var dbsetPropertyName = context.GetType().GetProperties().FirstOrDefault(x => x.PropertyType.GenericTypeArguments.Any(y => y == entityType));

            // Get table name (if it has a Table attribute, use that, otherwise dbset property name)
            string tableName = tableAttr != null ? tableAttr.Name : ((dbsetPropertyName != null )? dbsetPropertyName.Name : "[Table Name Not Found]");

            return tableName;
        }

        private string GetTrackChangesAditionalInfo(Type entityType)
        {
            var friendlyNamesList = entityType.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(TrackChangesAditionalInfo)));
            return string.Join(" | ", friendlyNamesList.Select(x => _dbEntry.OriginalValues.GetValue<object>(x.Name)));
        }

        public void Dispose()
        {

        }
    }
}
