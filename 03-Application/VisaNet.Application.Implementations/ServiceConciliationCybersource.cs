using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Services;
using VisaNet.Components.CyberSource.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceConciliationCybersource : BaseService<ConciliationCybersource, ConciliationCybersourceDto>, IServiceConciliationCybersource
    {
        private readonly ICyberSourceAccess _cyberSourceAccess;
        private readonly IRepositoryConciliationCybersource _repositoryConciliationCybersource;
        private readonly ILoggerService _loggerService;
        private readonly IServiceConciliationRun _serviceConciliationRun;

        public ServiceConciliationCybersource(IRepositoryConciliationCybersource repository, ICyberSourceAccess cyberSourceAccess,
            IRepositoryConciliationCybersource repositoryConciliationCybersource, ILoggerService loggerService,
            IServiceConciliationRun serviceConciliationRun)
            : base(repository)
        {
            _cyberSourceAccess = cyberSourceAccess;
            _repositoryConciliationCybersource = repositoryConciliationCybersource;
            _loggerService = loggerService;
            _serviceConciliationRun = serviceConciliationRun;
        }

        public override IQueryable<ConciliationCybersource> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override ConciliationCybersourceDto Converter(ConciliationCybersource entity)
        {
            var dto = new ConciliationCybersourceDto
            {
                Id = entity.Id,
                Date = entity.Date,
                RequestId = entity.RequestId,
                MerchantReferenceNumber = entity.MerchantReferenceNumber,
                Currency = entity.Currency,
                Amount = entity.Amount,
                DateString = entity.DateString,
                IcsApplications = entity.IcsApplications,
                MerchandId = entity.MerchandId,
                Source = entity.Source,
                PaymentDone = entity.PaymentDone,
                TransactionType = entity.TransactionType
            };

            return dto;
        }

        public override ConciliationCybersource Converter(ConciliationCybersourceDto entity)
        {
            var cCybersource = new ConciliationCybersource
            {
                Id = entity.Id,
                Date = entity.Date,
                RequestId = entity.RequestId,
                MerchantReferenceNumber = entity.MerchantReferenceNumber,
                Currency = entity.Currency,
                Amount = entity.Amount,
                DateString = entity.DateString,
                IcsApplications = entity.IcsApplications,
                MerchandId = entity.MerchandId,
                Source = entity.Source,
                PaymentDone = entity.PaymentDone,
                TransactionType = entity.TransactionType
            };
            return cCybersource;
        }

        public async Task<bool> GetConciliation(ReportsConciliationFilterDto filtersDto, bool? isManualRun = null)
        {
            NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationCybersource - GetConciliation - Inicia.");

            var from = filtersDto.From;
            var conciliationRun = CreateConciliationRun(isManualRun, filtersDto.From, filtersDto.To);
            var conciliationRunUpdated = false;

            try
            {
                while (from.Date.CompareTo(filtersDto.To.Date) < 1)
                {
                    NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationCybersource - GetConciliation - FROM: " + filtersDto.From.ToString("d"));
                    var info = await _cyberSourceAccess.GenerateConciliation(from, from).ConfigureAwait(false);
                    from = from.AddDays(1);
                }

                UpdateConciliationRun(conciliationRun, ConciliationRunStateDto.CompletedOk, null);
                conciliationRunUpdated = true;

                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationCybersource - GetConciliation - Termina.");
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
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationCybersource - GetConciliation - Error inesperado.");
                throw;
            }
        }

        private LogDto CheckPaymentLog(string requestId)
        {
            var log = _loggerService.AllNoTracking(l => new LogDto
            {
                LogPaymentCyberSource = new LogPaymentCyberSourceDto
                {
                    TransactionDateTime = l.LogPaymentCyberSource.TransactionDateTime,
                    CyberSourceLogData = new CyberSourceLogDataDto
                    {
                        ReasonCode = l.LogPaymentCyberSource.CyberSourceLogData.ReasonCode
                    }
                }
            },
            l => l.LogPaymentCyberSource.CyberSourceLogData.TransactionId.Equals(requestId),
            l => l.LogPaymentCyberSource).FirstOrDefault();
            return log;
        }

        private void AnalizeData(ConciliationCybersourceDto conciliation, List<string> old, ref List<String> refund, ref List<String> voidedOrReverse)
        {
            const double token = 0;
            if (conciliation.Amount.CompareTo(token) == 0)
            {
                NLogLogger.LogEvent(NLogType.Info, "Obtuve creacion de un token. Transaction id: " + conciliation.RequestId);
            }
            else
            {
                if (!old.Contains(conciliation.RequestId))
                {
                    conciliation.Amount = Math.Abs(conciliation.Amount);
                    if (conciliation.IcsApplications.Contains("ics_credit"))
                    {
                        //Si no es uno, no se hizo el refund y no tengo que compararlo
                        if (conciliation.CreditRcode == 1)
                        {
                            conciliation.PaymentDone = false;
                            var item = Converter(conciliation);
                            item.TransactionType = TransactionType.Refund;
                            _repositoryConciliationCybersource.Create(item);
                            _repositoryConciliationCybersource.Save();
                        }
                        refund.Add(conciliation.MerchantReferenceNumber);
                    }
                    else if ((conciliation.IcsApplications.Contains("ics_void")) ||
                             conciliation.IcsApplications.Contains("ics_auth_reversal"))
                    {
                        voidedOrReverse.Add(conciliation.MerchantReferenceNumber);
                    }
                    //ESTO ES SI SE QUIERE AGREGAR LA CREACION DE TOKENS
                    //else if (conciliation.IcsApplications.Equals("ics_pay_subscription_create"))
                    //{
                    //    //Esto es solo tokens
                    //    conciliation.PaymentDone = false;
                    //    var item = Converter(conciliation);
                    //    _repositoryConciliationCybersource.Create(item);
                    //    _repositoryConciliationCybersource.Save();    
                    //}
                    else if (conciliation.Correct == 1)
                    {
                        var log = CheckPaymentLog(conciliation.RequestId);
                        if (log == null)
                        {
                            NLogLogger.LogEvent(NLogType.Info,
                                "No hay transaccion en el LOG. RequestId: " + conciliation.RequestId);
                        }
                        else if (!log.LogPaymentCyberSource.CyberSourceLogData.ReasonCode.Equals("100"))
                        {
                            NLogLogger.LogEvent(NLogType.Info,
                                string.Format("Transaccion con ReasonCode {0}. RequestId: {1}",
                                    log.LogPaymentCyberSource.CyberSourceLogData.ReasonCode,
                                    conciliation.RequestId));

                        }
                        else
                        {
                            var item = Converter(conciliation);
                            item.TransactionType = TransactionType.Payment;
                            _repositoryConciliationCybersource.Create(item);
                            _repositoryConciliationCybersource.Save();
                        }
                    }
                }
                else
                {
                    NLogLogger.LogEvent(NLogType.Info,
                               string.Format("Transaccion ya existente en la BD concilicacion. RequestId: {0}",
                                   conciliation.RequestId));
                }
            }
        }

        private ConciliationRunDto CreateConciliationRun(bool? isManualRun, DateTime from, DateTime to)
        {
            try
            {
                var dto = new ConciliationRunDto
                {
                    App = ConciliationAppDto.CyberSource,
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
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationCybersource - CreateConciliationRun - Error inesperado");
                NLogLogger.LogEvent(e);
                return null;
            }
        }

        private void UpdateConciliationRun(ConciliationRunDto dto, ConciliationRunStateDto state, string resultDescription, Exception resultException = null)
        {
            if (dto == null)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationCybersource - UpdateConciliationRun - Dto null");
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
                NLogLogger.LogEvent(NLogType.Info, "ServiceConciliationCybersource - UpdateConciliationRun - Error inesperado");
                NLogLogger.LogEvent(e);
            }
        }

    }
}