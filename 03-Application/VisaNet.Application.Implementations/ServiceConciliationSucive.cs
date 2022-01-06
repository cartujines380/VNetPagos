using System;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Components.Sucive.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceConciliationSucive : BaseService<ConciliationSucive, ConciliationSuciveDto>, IServiceConciliationSucive
    {
        private readonly ISuciveAccess _suciveAccess;
        private readonly IServiceConciliationRun _serviceConciliationRun;

        public ServiceConciliationSucive(IRepositoryConciliationSucive repository, ISuciveAccess suciveAccess,
            IServiceConciliationRun serviceConciliationRun)
            : base(repository)
        {
            _suciveAccess = suciveAccess;
            _serviceConciliationRun = serviceConciliationRun;
        }

        public override IQueryable<ConciliationSucive> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override ConciliationSuciveDto Converter(ConciliationSucive entity)
        {
            var dto = new ConciliationSuciveDto
            {
                Id = entity.Id,
                Date = entity.Date,
                BillExternalId = entity.BillExternalId,
                Currency = entity.Currency,
                Amount = entity.Amount,
                Departament = (DepartamentDtoType)entity.Departament
            };

            return dto;
        }

        public override ConciliationSucive Converter(ConciliationSuciveDto entity)
        {
            var cBanred = new ConciliationSucive
            {
                Id = entity.Id,
                Date = entity.Date,
                BillExternalId = entity.BillExternalId,
                Currency = entity.Currency,
                Amount = entity.Amount,
                Departament = (DepartamentType)entity.Departament
            };
            return cBanred;
        }

        public bool GetConciliation(ReportsConciliationFilterDto filtersDto, bool? isManualRun = null)
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSucive - GetConciliation - Inicia.");
            var conciliationRun = CreateConciliationRun(isManualRun, filtersDto.From, filtersDto.To);
            var conciliationRunUpdated = false;

            try
            {
                var dptos = EnumHelpers.AllItems(typeof(DepartamentType));
                var info = _suciveAccess.GetConciliation(filtersDto.From, filtersDto.To, dptos);
                var old = AllNoTracking().Select(c => c.BillExternalId).ToList();

                foreach (var conciliation in info)
                {
                    if (!old.Contains(conciliation.BillExternalId))
                    {
                        Create(conciliation);
                    }
                }

                UpdateConciliationRun(conciliationRun, ConciliationRunStateDto.CompletedOk, null);
                conciliationRunUpdated = true;

                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSucive - GetConciliation - Termina.");
                return true;
            }
            catch (Exception e)
            {
                if (!conciliationRunUpdated)
                {
                    //Se actualiza el estado de la corrida
                    UpdateConciliationRun(conciliationRun, ConciliationRunStateDto.TerminatedWithException, null, e);
                }

                NLogLogger.LogEvent(e);
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSucive - GetConciliation - Error inesperado.");
                throw;
            }
        }

        private ConciliationRunDto CreateConciliationRun(bool? isManualRun, DateTime from, DateTime to)
        {
            try
            {
                var dto = new ConciliationRunDto
                {
                    App = ConciliationAppDto.Sucive,
                    IsManualRun = isManualRun == null ? false : isManualRun.Value,
                    ConciliationDateFrom = from,
                    ConciliationDateTo = to,
                    State = ConciliationRunStateDto.Started
                };

                var newDto = _serviceConciliationRun.Create(dto, true);
                return newDto;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSucive - CreateConciliationRun - Error inesperado");
                NLogLogger.LogEvent(e);
                return null;
            }
        }

        private void UpdateConciliationRun(ConciliationRunDto dto, ConciliationRunStateDto state, string resultDescription, Exception resultException = null)
        {
            if (dto == null)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSucive - UpdateConciliationRun - Dto null");
                return;
            }

            try
            {
                dto.State = state;
                dto.ResultDescription = resultDescription;
                dto.ExceptionMessage = resultException != null ? resultException.InnerException.Message : null;

                _serviceConciliationRun.UpdateConciliationRunResult(dto);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationSucive - UpdateConciliationRun - Error inesperado");
                NLogLogger.LogEvent(e);
            }
        }

    }
}