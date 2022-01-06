using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.FrameworkExtensions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Services;
using VisaNet.Components.Sistarbanc.Implementations.Implementations;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceFixedNotification : BaseService<FixedNotification, FixedNotificationDto>, IServiceFixedNotification
    {
        public ServiceFixedNotification(IRepositoryFixedNotification repository)
            : base(repository)
        {

        }

        public override IQueryable<FixedNotification> GetDataForTable()
        {
            throw new NotImplementedException();
        }

        public override FixedNotificationDto Converter(FixedNotification entity)
        {
            return new FixedNotificationDto
            {
                Category = (FixedNotificationCategoryDto)entity.Category,
                DateTime = entity.DateTime,
                Description = entity.Description,
                Detail = entity.Detail,
                Id = entity.Id,
                Level = (FixedNotificationLevelDto)entity.Level,
                Resolved = entity.Resolved,
                Comment = entity.Comment
            };
        }

        public override FixedNotification Converter(FixedNotificationDto entity)
        {
            return new FixedNotification
            {
                Category = (FixedNotificationCategory)entity.Category,
                DateTime = entity.DateTime,
                Description = entity.Description,
                Detail = entity.Detail,
                Id = entity.Id,
                Level = (FixedNotificationLevel)entity.Level,
                Resolved = entity.Resolved,
                Comment = entity.Comment
            };
        }

        public IEnumerable<FixedNotificationDto> GetDataForMenu()
        {
            return Repository.AllNoTracking(x => !x.Resolved && x.Level == FixedNotificationLevel.Error).OrderBy(x => x.DateTime).Select(Converter);
        }

        public IEnumerable<FixedNotificationDto> GetDataForTable(FixedNotificationFilterDto filters)
        {
            bool searchDescription = string.IsNullOrWhiteSpace(filters.Description);
            bool searchDetail = string.IsNullOrWhiteSpace(filters.Detail);

            Expression<Func<FixedNotification, bool>> where = x =>
                (searchDescription || x.Description.ToLower().Contains(filters.Description.ToLower()))
                && (searchDetail || x.Detail.ToLower().Contains(filters.Detail.ToLower()))
                && ((!filters.Level.HasValue) || (int)x.Level == (int)filters.Level.Value)
                && ((!filters.Category.HasValue) || (int)x.Category == (int)filters.Category.Value)
                && ((!filters.Resolved.HasValue) || x.Resolved == filters.Resolved.Value)
                && ((filters.From == DateTime.MinValue) || x.DateTime >= filters.From)
                && ((filters.To == DateTime.MinValue) || x.DateTime <= filters.To)

                ;

            var query = Repository.AllNoTracking(where);

            query = filters.SortDirection == SortDirection.Asc ? query.OrderByStringProperty(filters.OrderBy) : query.OrderByStringPropertyDescending(filters.OrderBy);

            return query.Select(Converter);
        }

        public void ResolveAll(ResolveAllFixedDto resolveAllFixedDto)
        {
            resolveAllFixedDto.Filter.OrderBy = "DateTime";
            var notifications = GetDataForTable(resolveAllFixedDto.Filter);
            Repository.ContextTrackChanges = true;
            foreach (var notification in notifications)
            {
                var entity = Repository.GetById(notification.Id);
                entity.Resolved = true;
                entity.Comment = resolveAllFixedDto.Comment;
            }
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public void Edit(FixedNotificationDto entity)
        {
            Repository.ContextTrackChanges = true;
            var entityInDb = Repository.GetById(entity.Id);
            entityInDb.Comment = entity.Comment;
            entityInDb.Resolved = entity.Resolved;

            Repository.Edit(entityInDb);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public string ExceptionMsg(Exception exception)
        {
            if (exception == null) return string.Empty;

            var str = "Excepción: " + exception.Message + "<br />";
            str = str + "   StackTrace: " + exception.StackTrace + "<br />";
            if (exception.InnerException != null)
            {
                str = str + "   InnerException: <br />" + ExceptionMsg(exception.InnerException);
            }
            return str;
        }
    }
}
