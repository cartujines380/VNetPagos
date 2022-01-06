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
    public class ServiceWsBillQuery : BaseService<WsBillQuery, WsBillQueryDto>, IServiceWsBillQuery
    {

        public ServiceWsBillQuery(IRepositoryWsBillQuery repository)
            : base(repository)
        {
        }

        public override IQueryable<WsBillQuery> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override WsBillQueryDto Converter(WsBillQuery entity)
        {
            if (entity == null) return null;

            return new WsBillQueryDto
            {
                Id = entity.Id,
                Codresult = entity.Codresult,
                IdOperation = entity.IdOperation,
                BillNumber = entity.BillNumber,
                CodCommerce = entity.CodCommerce,
                IdMerchant = entity.IdMerchant,
                CodBranch = entity.CodBrunch,
                Date = entity.Date,
                IdApp = entity.IdApp,
                RefClient = entity.RefClient,
                RefClient2 = entity.RefClient2,
                RefClient3 = entity.RefClient3,
                RefClient4 = entity.RefClient4,
                RefClient5 = entity.RefClient5,
                RefClient6 = entity.RefClient6,
                WcfVersion = entity.WcfVersion,
            };
        }

        public override WsBillQuery Converter(WsBillQueryDto entity)
        {
            if (entity == null) return null;

            return new WsBillQuery
            {
                Id = entity.Id,
                Codresult = entity.Codresult,
                IdOperation = entity.IdOperation,
                BillNumber = entity.BillNumber,
                CodCommerce = entity.CodCommerce,
                IdMerchant = entity.IdMerchant,
                CodBrunch = entity.CodBranch,
                Date = entity.Date,
                IdApp = entity.IdApp,
                RefClient = entity.RefClient,
                RefClient2 = entity.RefClient2,
                RefClient3 = entity.RefClient3,
                RefClient4 = entity.RefClient4,
                RefClient5 = entity.RefClient5,
                RefClient6 = entity.RefClient6,
                WcfVersion = entity.WcfVersion,
            };
        }
        
        public override void Edit(WsBillQueryDto dto)
        {
            Repository.ContextTrackChanges = true;

            var entity = Repository.GetById(dto.Id);
            entity.Codresult = dto.Codresult;

            Repository.Edit(entity);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public ICollection<WsBillQueryDto> GetBillQueriesForTable(ReportsIntegrationFilterDto filterDto)
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

            // NO TIENE
            //if (!filterDto.IdApp.IsEmpty())
            //    query = query.Where(x => x.IdApp.Contains(filterDto.IdApp));

            //ordeno, skip y take
            query = filterDto.SortDirection == SortDirection.Desc ? query.OrderByDescending(x => x.CreationDate) : query.OrderBy(x => x.CreationDate);
            query = query.Skip(filterDto.DisplayStart);
            if (filterDto.DisplayLength.HasValue)
                query = query.Take(filterDto.DisplayLength.Value);

            var result = query.Select(t => new WsBillQueryDto
            {
                Id = t.Id,
                IdOperation = t.IdOperation,
                Codresult = t.Codresult,
                BillNumber = t.BillNumber,
                CodBranch = t.CodBrunch,
                CodCommerce = t.CodCommerce,
                Date = t.Date,
                IdApp = t.IdApp,
                RefClient = t.RefClient,
                RefClient2 = t.RefClient2,
                RefClient3 = t.RefClient3,
                RefClient4 = t.RefClient4,
                RefClient5 = t.RefClient5,
                RefClient6 = t.RefClient6,
                IdMerchant = t.IdMerchant,
                CreationDate = t.CreationDate,
                WcfVersion = t.WcfVersion,
            }).ToList();

            return result;
        }

        public int GetBillQueriesForTableCount(ReportsIntegrationFilterDto filterDto)
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

            // NO TIENE
            //if (!filterDto.IdApp.IsEmpty())
            //    query = query.Where(x => x.IdApp.Contains(filterDto.IdApp));

            return query.Select(t => new WsBillQueryDto
            {
                Id = t.Id
            }).Count();
        }
    }
}