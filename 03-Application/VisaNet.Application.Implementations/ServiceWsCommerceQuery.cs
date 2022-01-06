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
    public class ServiceWsCommerceQuery : BaseService<WsCommerceQuery, WsCommerceQueryDto>, IServiceWsCommerceQuery
    {

        public ServiceWsCommerceQuery(IRepositoryWsCommerceQuery repository)
            : base(repository)
        {
        }

        public override IQueryable<WsCommerceQuery> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override WsCommerceQueryDto Converter(WsCommerceQuery entity)
        {
            if (entity == null) return null;

            return new WsCommerceQueryDto
            {
                Id = entity.Id,
                Codresult = entity.Codresult,
                IdApp = entity.IdApp,
                IdOperation = entity.IdOperation,
                WcfVersion = entity.WcfVersion,
            };
        }

        public override WsCommerceQuery Converter(WsCommerceQueryDto entity)
        {
            if (entity == null) return null;

            return new WsCommerceQuery
            {
                Id = entity.Id,
                Codresult = entity.Codresult,
                IdApp = entity.IdApp,
                IdOperation = entity.IdOperation,
                WcfVersion = entity.WcfVersion,
            };
        }

        public override void Edit(WsCommerceQueryDto dto)
        {
            Repository.ContextTrackChanges = true;

            var entity = Repository.GetById(dto.Id);
            entity.Codresult = dto.Codresult;

            Repository.Edit(entity);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public ICollection<WsCommerceQueryDto> GetCommerceQueriesForTable(ReportsIntegrationFilterDto filterDto)
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

            var result = query.Select(t => new WsCommerceQueryDto
            {
                Id = t.Id,
                IdOperation = t.IdOperation,
                IdApp = t.IdApp,
                Codresult = t.Codresult,
                WcfVersion = t.WcfVersion,
                CreationDate = t.CreationDate
            }).ToList();

            return result;
        }

        public int GetCommerceQueriesForTableCount(ReportsIntegrationFilterDto filterDto)
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

            return query.Select(t => new WsCommerceQueryDto
            {
                Id = t.Id
            }).Count();
        }
    }
}