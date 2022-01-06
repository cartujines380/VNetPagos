using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceConciliationRun : BaseService<ConciliationRun, ConciliationRunDto>, IServiceConciliationRun
    {
        public ServiceConciliationRun(IRepositoryConciliationRun repository)
            : base(repository)
        {
        }

        public override ConciliationRunDto Converter(ConciliationRun entity)
        {
            if (entity == null)
            {
                return null;
            }

            var obj = new ConciliationRunDto
            {
                Id = entity.Id,
                App = (ConciliationAppDto)entity.App,
                IsManualRun = entity.IsManualRun,
                InputFileName = entity.InputFileName,
                ConciliationDateFrom = entity.ConciliationDateFrom,
                ConciliationDateTo = entity.ConciliationDateTo,
                State = (ConciliationRunStateDto)entity.State,
                ResultDescription = entity.ResultDescription,
                ExceptionMessage = entity.ExceptionMessage,
                CreationDate = entity.CreationDate,
                CreationUser = entity.CreationUser,
                LastModificationDate = entity.LastModificationDate,
                LastModificationUser = entity.LastModificationUser,
            };

            return obj;
        }

        public override ConciliationRun Converter(ConciliationRunDto entity)
        {
            if (entity == null)
            {
                return null;
            }

            var obj = new ConciliationRun
            {
                Id = entity.Id,
                App = (ConciliationApp)entity.App,
                IsManualRun = entity.IsManualRun,
                InputFileName = entity.InputFileName,
                ConciliationDateFrom = entity.ConciliationDateFrom,
                ConciliationDateTo = entity.ConciliationDateTo,
                State = (ConciliationRunState)entity.State,
                ResultDescription = entity.ResultDescription,
                ExceptionMessage = entity.ExceptionMessage,
                CreationDate = entity.CreationDate,
                CreationUser = entity.CreationUser,
                LastModificationDate = entity.LastModificationDate,
                LastModificationUser = entity.LastModificationUser,
            };

            return obj;
        }

        public override IQueryable<ConciliationRun> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override ConciliationRunDto Create(ConciliationRunDto entity, bool returnEntity = false)
        {
            entity.State = ConciliationRunStateDto.Started;
            return base.Create(entity, returnEntity);
        }

        public IEnumerable<ConciliationRunDto> GetConciliationRunReport(ReportsConciliationRunFilterDto filter)
        {
            //Filters
            var query = ConciliationRunReportQuery(filter);

            //Sort
            switch (filter.OrderBy)
            {
                case "CreationDate":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.CreationDate) : query.OrderByDescending(c => c.CreationDate);
                    break;
                case "App":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.App) : query.OrderByDescending(c => c.App);
                    break;
                case "IsManualRun":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.IsManualRun) : query.OrderByDescending(c => c.IsManualRun);
                    break;
                case "State":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.State) : query.OrderByDescending(c => c.State);
                    break;
                case "InputFileName":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.InputFileName) : query.OrderByDescending(c => c.InputFileName);
                    break;
                case "ConciliationDateFrom":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.ConciliationDateFrom) : query.OrderByDescending(c => c.ConciliationDateFrom);
                    break;
                case "ConciliationDateTo":
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.ConciliationDateTo) : query.OrderByDescending(c => c.ConciliationDateTo);
                    break;
                default:
                    query = filter.SortDirection == SortDirection.Asc ? query.OrderBy(c => c.CreationDate) : query.OrderByDescending(c => c.CreationDate);
                    break;
            }

            //Skip & take
            query = query.Skip(filter.DisplayStart);
            if (filter.DisplayLength.HasValue)
                query = query.Take(filter.DisplayLength.Value);

            //Convert & return
            return query.Select(Converter).ToList();
        }

        public int GetConciliationRunReportCount(ReportsConciliationRunFilterDto filter)
        {
            var query = ConciliationRunReportQuery(filter);
            return query.Count();
        }

        private IQueryable<ConciliationRun> ConciliationRunReportQuery(ReportsConciliationRunFilterDto filter)
        {
            var query = Repository.AllNoTracking();

            if (filter.CreationDateFrom != default(DateTime))
            {
                query = query.Where(c => c.CreationDate > filter.CreationDateFrom);
            }

            if (filter.CreationDateTo != default(DateTime))
            {
                var d = filter.CreationDateTo.AddDays(1);
                query = query.Where(c => c.CreationDate < d);
            }

            if (filter.App != null)
            {
                query = query.Where(c => c.App == (ConciliationApp)filter.App.Value);
            }

            if (filter.IsManualRun != null)
            {
                query = query.Where(c => c.IsManualRun == filter.IsManualRun.Value);
            }

            if (filter.State != null)
            {
                query = query.Where(c => c.State == (ConciliationRunState)filter.State.Value);
            }

            if (!string.IsNullOrEmpty(filter.InputFileName))
            {
                query = query.Where(c => c.InputFileName != null && c.InputFileName.ToLower().Contains(filter.InputFileName.ToLower()));
            }

            if (filter.ConciliationDateFrom != null && filter.ConciliationDateFrom != default(DateTime))
            {
                query = query.Where(c => c.ConciliationDateFrom > filter.ConciliationDateFrom);
            }

            if (filter.ConciliationDateTo != null && filter.ConciliationDateTo != default(DateTime))
            {
                var d = filter.ConciliationDateTo.Value.AddDays(1);
                query = query.Where(c => c.ConciliationDateTo < d);
            }

            return query;
        }

        public void UpdateConciliationRunResult(ConciliationRunDto dto)
        {
            Repository.ContextTrackChanges = true;
            var entityDb = Repository.GetById(dto.Id);

            if (entityDb == null)
            {
                throw new KeyNotFoundException("No se encontró la entidad con el Id indicado.");
            }

            entityDb.State = (ConciliationRunState)dto.State;
            entityDb.ResultDescription = dto.ResultDescription;
            entityDb.ExceptionMessage = dto.ExceptionMessage;

            Repository.Edit(entityDb);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

    }
}