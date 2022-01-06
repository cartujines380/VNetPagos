using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.WebPages;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceWebhookDown : BaseService<WebhookDown, WebhookDownDto>, IServiceWebhookDown
    {
        public ServiceWebhookDown(IRepositoryWebhookDown repository)
            : base(repository)
        {
        }

        public override IQueryable<WebhookDown> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override WebhookDownDto Converter(WebhookDown entity)
        {
            if (entity == null) return null;

            return new WebhookDownDto
            {
                Id = entity.Id,
                IdApp = entity.IdApp,
                IdCard = entity.IdCard,
                IdOperation = entity.IdOperation,
                IdUser = entity.IdUser,
                HttpResponseCode = entity.HttpResponseCode,
                CreationDate = entity.CreationDate
            };
        }

        public override WebhookDown Converter(WebhookDownDto entity)
        {
            if (entity == null) return null;

            return new WebhookDown
            {
                Id = entity.Id,
                IdApp = entity.IdApp,
                IdCard = entity.IdCard,
                IdOperation = entity.IdOperation,
                IdUser = entity.IdUser,
                HttpResponseCode = entity.HttpResponseCode
            };
        }

        //public override WebhookDownDto Create(WebhookDownDto entity, bool returnEntity = false)
        //{
        //    Repository.ContextTrackChanges = true;
        //    var efEntity = Converter(entity);
        //    efEntity.GenerateNewIdentity();
        //    Repository.Create(efEntity);
        //    Repository.Save();
        //    Repository.ContextTrackChanges = false;
        //    return returnEntity ? GetById(efEntity.Id) : null;
        //}

        public ICollection<WebhookDownDto> GetWebhookDownsForTable(ReportsIntegrationFilterDto filterDto)
        {
            var query = Repository.AllNoTracking();

            DateTime from = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filterDto.DateFromString))
            {
                from = DateTime.Parse(filterDto.DateFromString, new CultureInfo("es-UY"));
            }

            DateTime to = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filterDto.DateToString))
            {
                to = DateTime.Parse(filterDto.DateToString, new CultureInfo("es-UY"));
            }

            if (!from.Equals(DateTime.MinValue))
            {
                query = query.Where(p => p.CreationDate.CompareTo(from) >= 0);
            }

            if (!to.Equals(DateTime.MinValue))
            {
                var dateTo = to.AddDays(1);
                query = query.Where(p => p.CreationDate.CompareTo(dateTo) <= 0);
            }

            if (!filterDto.IdOperation.IsEmpty())
                query = query.Where(x => x.IdOperation.StartsWith(filterDto.IdOperation));

            if (!filterDto.IdApp.IsEmpty())
                query = query.Where(x => x.IdApp.Contains(filterDto.IdApp));

            //ordeno, skip y take
            query = filterDto.SortDirection == SortDirection.Desc ? query.OrderByDescending(x => x.CreationDate) : query.OrderBy(x => x.CreationDate);
            query = query.Skip(filterDto.DisplayStart);
            if (filterDto.DisplayLength.HasValue)
                query = query.Take(filterDto.DisplayLength.Value);

            var result = query.Select(t => new WebhookDownDto
            {
                Id = t.Id,
                IdOperation = t.IdOperation,
                IdApp = t.IdApp,
                IdCard = t.IdCard,
                IdUser = t.IdUser,
                HttpResponseCode = t.HttpResponseCode,
                CreationDate = t.CreationDate
            }).ToList();

            return result;
        }

        public int GetWebhookDownsForTableCount(ReportsIntegrationFilterDto filterDto)
        {
            var query = Repository.AllNoTracking();

            DateTime from = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filterDto.DateFromString))
            {
                from = DateTime.Parse(filterDto.DateFromString, new CultureInfo("es-UY"));
            }

            DateTime to = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filterDto.DateToString))
            {
                to = DateTime.Parse(filterDto.DateToString, new CultureInfo("es-UY"));
            }

            if (!from.Equals(DateTime.MinValue))
            {
                query = query.Where(p => p.CreationDate.CompareTo(from) >= 0);
            }

            if (!to.Equals(DateTime.MinValue))
            {
                var dateTo = to.AddDays(1);
                query = query.Where(p => p.CreationDate.CompareTo(dateTo) <= 0);
            }

            if (!filterDto.IdOperation.IsEmpty())
                query = query.Where(x => x.IdOperation.StartsWith(filterDto.IdOperation));

            if (!filterDto.IdApp.IsEmpty())
                query = query.Where(x => x.IdApp.Contains(filterDto.IdApp));

            return query.Select(t => new WebhookDownDto
            {
                Id = t.Id
            }).Count();
        }

        public override void Edit(WebhookDownDto entity)
        {
            Repository.ContextTrackChanges = true;
            var entityDb = Repository.GetById(entity.Id);
            entityDb.HttpResponseCode = entity.HttpResponseCode;
            Repository.Edit(entityDb);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }
    }
}