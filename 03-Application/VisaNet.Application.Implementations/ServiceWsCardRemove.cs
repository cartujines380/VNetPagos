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
    public class ServiceWsCardRemove : BaseService<WsCardRemove, WsCardRemoveDto>, IServiceWsCardRemove
    {

        public ServiceWsCardRemove(IRepositoryWsCardRemove repository)
            : base(repository)
        {
        }

        public override IQueryable<WsCardRemove> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override WsCardRemoveDto Converter(WsCardRemove entity)
        {
            if (entity == null) return null;

            return new WsCardRemoveDto
            {
                IdApp = entity.IdApp,
                IdOperation = entity.IdOperation,
                IdCard = entity.IdCard,
                IdUser = entity.IdUser,
                Codresult = entity.Codresult,
                CreationDate = entity.CreationDate,
                WcfVersion = entity.WcfVersion,
                Id = entity.Id
            };
        }

        public override WsCardRemove Converter(WsCardRemoveDto entity)
        {
            if (entity == null) return null;

            return new WsCardRemove
            {
                IdApp = entity.IdApp,
                IdOperation = entity.IdOperation,
                IdCard = entity.IdCard,
                IdUser = entity.IdUser,
                Codresult = entity.Codresult,
                CreationDate = entity.CreationDate,
                WcfVersion = entity.WcfVersion,
                Id = entity.Id
            };
        }
        
        public override void Edit(WsCardRemoveDto dto)
        {
            Repository.ContextTrackChanges = true;

            var entity = Repository.GetById(dto.Id);
            
            entity.Codresult = dto.Codresult;

            Repository.Edit(entity);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public ICollection<WsCardRemoveDto> GetCardRemovesForTable(ReportsIntegrationFilterDto filterDto)
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

            var result = query.Select(t => new WsCardRemoveDto()
            {
                IdApp = t.IdApp,
                IdOperation = t.IdOperation,
                IdCard = t.IdCard,
                IdUser = t.IdUser,
                Codresult = t.Codresult,
                WcfVersion = t.WcfVersion,
                CreationDate = t.CreationDate
            }).ToList();

            return result;
        }

        public int GetCardRemovesForTableCount(ReportsIntegrationFilterDto filterDto)
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
            
            return query.Count();
        }

        public WsCardRemoveDto GetByIdOperation(string idOperation)
        {
            var entities = Repository.AllNoTracking(x => x.IdOperation.Equals(idOperation, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (entities == null || !entities.Any() || entities.Count > 1)
                return null;

            var entity = entities.FirstOrDefault();

            return Converter(entity);

        }

        public bool IsOperationIdRepited(string idOperation, string idApp)
        {
            var isOperationIdRepited =
               Repository.AllNoTracking(x => x.IdOperation.Equals(idOperation, StringComparison.InvariantCultureIgnoreCase) &&
                   x.IdApp.Equals(idApp, StringComparison.InvariantCultureIgnoreCase))
                   .Any();

            return isOperationIdRepited;
        }
    }
}