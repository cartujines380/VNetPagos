using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.ChangeTracker.Models;
using VisaNet.Common.FrameworkExtensions;
using VisaNet.Common.Resource.Audit;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ChangeTracker;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;
using AuditLog = VisaNet.Common.ChangeTracker.Models.AuditLog;
using AuditLogDto = VisaNet.Domain.EntitiesDtos.ChangeTracker.AuditLogDto;

namespace VisaNet.Application.Implementations
{
    public class ServiceChangeTracker : BaseService<AuditLog, AuditLogDto>, IServiceChangeTracker
    {
        public ServiceChangeTracker(IRepositoryChangeTracker repository)
            : base(repository)
        {

        }


        public override IQueryable<AuditLog> GetDataForTable()
        {
            throw new System.NotImplementedException();
        }

        public override AuditLogDto Converter(AuditLog entity)
        {
            return new AuditLogDto
            {
                AuditLogId = entity.AuditLogId,
                EventDate = entity.EventDate,
                EventType = (EventTypeDto)entity.EventType,
                IP = entity.IP,
                LogDetails = entity.LogDetails.Select(x => new AuditLogDetailDto
                {
                    AuditLogId = x.AuditLogId,
                    ColumnName = x.ColumnName,
                    Id = x.Id,
                    NewValue = x.NewValue,
                    OrginalValue = x.OrginalValue
                }).ToList(),
                RecordId = entity.RecordId,
                TableName = entity.TableName,
                TransactionIdentifier = entity.TransactionIdentifier,
                UserName = entity.UserName,
                AditionalInfo = entity.AditionalInfo
            };
        }

        public override AuditLog Converter(AuditLogDto entity)
        {
            return new AuditLog
            {
                AuditLogId = entity.AuditLogId,
                EventDate = entity.EventDate,
                EventType = (EventType)entity.EventType,
                IP = entity.IP,
                LogDetails = entity.LogDetails.Select(x => new AuditLogDetail
                {
                    AuditLogId = x.AuditLogId,
                    ColumnName = x.ColumnName,
                    Id = x.Id,
                    NewValue = x.NewValue,
                    OrginalValue = x.OrginalValue
                }).ToList(),
                RecordId = entity.RecordId,
                TableName = entity.TableName,
                TransactionIdentifier = entity.TransactionIdentifier,
                UserName = entity.UserName,
                AditionalInfo = entity.AditionalInfo
            };
        }

        public IEnumerable<AuditLogDto> GetDataForTable(ChangeTrackerFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (!string.IsNullOrEmpty(filters.UserName))
                query = query.Where(sc => sc.UserName.ToLower().Contains(filters.UserName.ToLower()));

            if (filters.EventType != null)
                query = query.Where(sc => sc.EventType == (EventType)filters.EventType);

            if (!string.IsNullOrEmpty(filters.TableName))
                query = query.Where(sc => sc.TableName.Equals(filters.TableName, StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrEmpty(filters.AditionalInfo))
                query = query.Where(sc => sc.AditionalInfo.ToLower().Contains(filters.AditionalInfo.ToLower()));

            query = query.Where(x => x.EventDate >= filters.From && x.EventDate <= filters.To);

            query = filters.SortDirection == SortDirection.Asc ? query.OrderByStringProperty(filters.OrderBy) : query.OrderByStringPropertyDescending(filters.OrderBy);

            query = query.Skip(filters.DisplayStart);

            if (filters.DisplayLength.HasValue)
                query = query.Take(filters.DisplayLength.Value);

            return query.Select(Converter).ToList();
        }

        public int[] Count(ChangeTrackerFilterDto filters)
        {
            var query = Repository.AllNoTracking();
            var iTotalRecords = query.Count();

            if (!string.IsNullOrEmpty(filters.UserName))
                query = query.Where(sc => sc.UserName.ToLower().Contains(filters.UserName.ToLower()));

            if (filters.EventType != null)
                query = query.Where(sc => sc.EventType == (EventType)filters.EventType);

            if (!string.IsNullOrEmpty(filters.TableName))
                query = query.Where(sc => sc.TableName.Equals(filters.TableName, StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrEmpty(filters.AditionalInfo))
                query = query.Where(sc => sc.AditionalInfo.ToLower().Contains(filters.AditionalInfo.ToLower()));

            query = query.Where(x => x.EventDate >= filters.From && x.EventDate <= filters.To);

            var iTotalDisplayRecords = query.Count();

            return new[] { iTotalRecords, iTotalDisplayRecords };
        }

        public AuditLogDto GetById(int id)
        {
            return Converter(Repository.AllNoTracking(x => x.AuditLogId == id, x => x.LogDetails).First());
        }

        public IEnumerable<string> GetEntities()
        {
            return Repository.AllNoTracking().Select(x => x.TableName).Distinct();
        }

        public IEnumerable<ChangeTrackerExcelDto> ChangeLogExcelExport(ChangeTrackerFilterDto filters)
        {
            var query = Repository.AllNoTracking(null, x => x.LogDetails);

            if (!string.IsNullOrEmpty(filters.UserName))
                query = query.Where(sc => sc.UserName.ToLower().Contains(filters.UserName.ToLower()));

            if (filters.EventType != null)
                query = query.Where(sc => sc.EventType == (EventType)filters.EventType);

            if (!string.IsNullOrEmpty(filters.TableName))
                query = query.Where(sc => sc.TableName.Equals(filters.TableName, StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrEmpty(filters.AditionalInfo))
                query = query.Where(sc => sc.AditionalInfo.ToLower().Contains(filters.AditionalInfo.ToLower()));

            query = query.Where(x => x.EventDate >= filters.From && x.EventDate <= filters.To);

            if (filters.DisplayLength.HasValue)
                query = query.Take(filters.DisplayLength.Value);

            var ret = new List<ChangeTrackerExcelDto>();

            foreach (var auditLog in query)
            {
                foreach (var auditLogDetail in auditLog.LogDetails)
                {
                    ret.Add(new ChangeTrackerExcelDto
                    {
                        AditionalInfo = auditLog.AditionalInfo,
                        ColumnName = auditLogDetail.ColumnName,
                        Date = auditLog.EventDate.ToString("dd/MM/yyyy HH:mm:ss"),
                        Ip = auditLog.IP,
                        LogType = EnumHelpers.GetName(typeof(EventTypeDto), (int)auditLog.EventType, EnumsStrings.ResourceManager),
                        NewValue = auditLogDetail.NewValue,
                        OriginalValue = auditLogDetail.OrginalValue,
                        TableName = AuditResources.ResourceManager.GetString(auditLog.TableName),
                        UserName = auditLog.UserName
                    });
                }
            }

            return ret;
        }
    }
}
