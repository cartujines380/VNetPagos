using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Web.WebPages;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceWsPaymentCancellation : BaseService<WsPaymentCancellation, WsPaymentCancellationDto>, IServiceWsPaymentCancellation
    {

        public ServiceWsPaymentCancellation(IRepositoryWsPaymentCancellation repository)
            : base(repository)
        {
        }

        public override IQueryable<WsPaymentCancellation> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override WsPaymentCancellationDto Converter(WsPaymentCancellation entity)
        {
            if (entity == null) return null;

            return new WsPaymentCancellationDto
            {
                Id = entity.Id,
                Codresult = entity.Codresult,
                IdApp = entity.IdApp,
                IdOperacionCobro = entity.IdOperacionCobro,
                IdOperation = entity.IdOperation,
                WcfVersion = entity.WcfVersion,
            };
        }

        public override WsPaymentCancellation Converter(WsPaymentCancellationDto entity)
        {
            if (entity == null) return null;

            return new WsPaymentCancellation
            {
                Id = entity.Id,
                Codresult = entity.Codresult,
                IdApp = entity.IdApp,
                IdOperacionCobro = entity.IdOperacionCobro,
                IdOperation = entity.IdOperation,
                WcfVersion = entity.WcfVersion,
            };
        }

        public override WsPaymentCancellationDto Create(WsPaymentCancellationDto entity, bool returnEntity = false)
        {
            Repository.ContextTrackChanges = true;
            var efEntity = Converter(entity);

            try
            {
                efEntity.GenerateNewIdentity();
                Repository.Create(efEntity);
                Repository.Save();

            }
            catch (DbException exception)
            {
                NLogLogger.LogEvent(exception);
                if (exception.Message.Contains("IX_IdApp_IdOperation"))
                {
                    //IDOPERACION REPETIDO
                    throw new BusinessException(CodeExceptions.OPERATION_ID_REPETED);
                }
                //TODO subo bussinesexception ?
            }

            Repository.ContextTrackChanges = false;
            return returnEntity ? GetById(efEntity.Id) : null;
        }

        public override void Edit(WsPaymentCancellationDto dto)
        {
            Repository.ContextTrackChanges = true;

            var entity = Repository.GetById(dto.Id);

            entity.Codresult = dto.Codresult;

            Repository.Edit(entity);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public ICollection<WsPaymentCancellationDto> GetPaymentCancellationsForTable(ReportsIntegrationFilterDto filterDto)
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

            var result = query.Select(t => new WsPaymentCancellationDto
            {
                Id = t.Id,
                IdOperation = t.IdOperation,
                IdApp = t.IdApp,
                Codresult = t.Codresult,
                IdOperacionCobro = t.IdOperacionCobro,
                WcfVersion = t.WcfVersion,
                CreationDate = t.CreationDate
            }).ToList();

            return result;
        }

        public int GetPaymentCancellationsForTableCount(ReportsIntegrationFilterDto filterDto)
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

            return query.Select(t => new WsPaymentCancellationDto
            {
                Id = t.Id
            }).Count();
        }

        public WsPaymentCancellationDto GetByIdOperation(string idOperation)
        {
            var entities = Repository.AllNoTracking(x => x.IdOperation.Equals(idOperation, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (!entities.Any() || entities.Count > 1)
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