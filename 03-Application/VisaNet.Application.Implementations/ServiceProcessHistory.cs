using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Implementations.ExtensionMethods;
using VisaNet.Application.Implementations.ExtensionMethods.Filters;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceProcessHistory : BaseService<ProcessHistory, ProcessHistoryDto>, IServiceProcessHistory
    {
        private readonly IRepositoryServiceAsociated _repositoryServiceAsociated;

        public ServiceProcessHistory(IRepositoryProcessHistory repository,
            IRepositoryServiceAsociated repositoryServiceAsociated)
            : base(repository)
        {
            _repositoryServiceAsociated = repositoryServiceAsociated;
        }

        public override IQueryable<ProcessHistory> GetDataForTable()
        {
            throw new NotImplementedException();
        }

        public override ProcessHistoryDto Converter(ProcessHistory entity)
        {
            if (entity == null) return null;

            var dto = new ProcessHistoryDto
            {
                Id = entity.Id,
                Process = (ProcessTypeDto)(int)entity.Process,
                CreationDate = entity.CreationDate,
                Count = entity.Count,
                Status = (ProcessStatusDto)(int)entity.Status,
                ErrorMessages = entity.ErrorMessages,
                Additional = entity.Additional,
                ServicesCount = entity.ServicesCount,
                ServicesWithBillsCount = entity.ServicesWithBillsCount,
                ServicesFailedGetBillsCount = entity.ServicesFailedGetBillsCount,
                BillsCount = entity.BillsCount,
                BillsToPayCount = entity.BillsToPayCount,
                BillsPayedCount = entity.BillsPayedCount,
                BillsFailedAmountCount = entity.BillsFailedAmountCount,
                BillsFailedQuotasCount = entity.BillsFailedQuotasCount,
                ControlledErrorsCount = entity.ControlledErrorsCount,
                RetryErrorsCount = entity.RetryErrorsCount,

                PendingAutomaticPayments = entity.PendingAutomaticPayments != null ?
                        entity.PendingAutomaticPayments.Select(x => new PendingAutomaticPaymentDto
                        {
                            Id = x.Id,
                            Date = x.Date,
                            PendingServiceAssociateId = x.PendingServiceAssociateId,
                            //Status = (PendingRegisterStatusDto)(int)x.Status,
                            Result = (PaymentResultTypeDto)(int)x.Result,
                            ErrorMessage = x.ErrorMessage,
                            ProcessHistoryId = x.ProcessHistoryId,
                            LastProcessHistoryId = x.LastProcessHistoryId
                        }).ToList() : new List<PendingAutomaticPaymentDto>()
            };
            return dto;
        }

        public override ProcessHistory Converter(ProcessHistoryDto entity)
        {
            if (entity == null) return null;

            var process = new ProcessHistory
            {
                Id = entity.Id,
                Process = (ProcessType)(int)entity.Process,
                Count = entity.Count,
                Status = (ProcessStatus)(int)entity.Status,
                ErrorMessages = entity.ErrorMessages,
                Additional = entity.Additional,
                ServicesCount = entity.ServicesCount,
                ServicesWithBillsCount = entity.ServicesWithBillsCount,
                ServicesFailedGetBillsCount = entity.ServicesFailedGetBillsCount,
                BillsCount = entity.BillsCount,
                BillsToPayCount = entity.BillsToPayCount,
                BillsPayedCount = entity.BillsPayedCount,
                BillsFailedAmountCount = entity.BillsFailedAmountCount,
                BillsFailedQuotasCount = entity.BillsFailedQuotasCount,
                ControlledErrorsCount = entity.ControlledErrorsCount,
                RetryErrorsCount = entity.RetryErrorsCount,

                PendingAutomaticPayments = entity.PendingAutomaticPayments != null ?
                        entity.PendingAutomaticPayments.Select(x => new PendingAutomaticPayment
                        {
                            Id = x.Id,
                            Date = x.Date,
                            PendingServiceAssociateId = x.PendingServiceAssociateId,
                            //Status = (PendingRegisterStatus)(int)x.Status,
                            Result = (PaymentResultType)(int)x.Result,
                            ErrorMessage = x.ErrorMessage,
                            ProcessHistoryId = x.ProcessHistoryId,
                            LastProcessHistoryId = x.LastProcessHistoryId
                        }).ToList() : new List<PendingAutomaticPayment>()
            };
            return process;
        }

        public override void Edit(ProcessHistoryDto entity)
        {
            if (entity == null) return;

            Repository.ContextTrackChanges = true;
            var entityDb = Repository.GetById(entity.Id);

            entityDb.Status = (ProcessStatus)(int)entity.Status;
            entityDb.ErrorMessages = entity.ErrorMessages;
            entityDb.Additional = entity.Additional;
            entityDb.ServicesCount = entity.ServicesCount;
            entityDb.ServicesWithBillsCount = entity.ServicesWithBillsCount;
            entityDb.ServicesFailedGetBillsCount = entity.ServicesFailedGetBillsCount;
            entityDb.BillsCount = entity.BillsCount;
            entityDb.BillsToPayCount = entity.BillsToPayCount;
            entityDb.BillsPayedCount = entity.BillsPayedCount;
            entityDb.BillsFailedAmountCount = entity.BillsFailedAmountCount;
            entityDb.BillsFailedQuotasCount = entity.BillsFailedQuotasCount;
            entityDb.ControlledErrorsCount = entity.ControlledErrorsCount;
            entityDb.RetryErrorsCount = entity.RetryErrorsCount;

            if (entityDb.PendingAutomaticPayments != null && entityDb.PendingAutomaticPayments.Any())
            {
                foreach (var pendingAutomaticPayment in entityDb.PendingAutomaticPayments.ToList())
                {
                    DeleteEntitiesNoRepository(pendingAutomaticPayment);
                }
            }

            if (entity.PendingAutomaticPayments != null)
            {
                entityDb.PendingAutomaticPayments =
                    entity.PendingAutomaticPayments.Select(x => new PendingAutomaticPayment
                    {
                        Id = Guid.NewGuid(),
                        Date = x.Date,
                        PendingServiceAssociateId = x.PendingServiceAssociateId,
                        Result = (PaymentResultType)(int)x.Result,
                        ErrorMessage = x.ErrorMessage,
                        ProcessHistoryId = x.ProcessHistoryId,
                        LastProcessHistoryId = x.LastProcessHistoryId
                    }).ToList();
            }
            Repository.Edit(entityDb);
            Repository.Save();

            Repository.ContextTrackChanges = false;
        }

        public int GetProcessCountForDate(DateTime date)
        {
            var query = Repository.AllNoTracking(q =>
                q.CreationDate.Day.Equals(date.Day) &&
                q.CreationDate.Month.Equals(date.Month) &&
                q.CreationDate.Year.Equals(date.Year) &&
                q.Status != ProcessStatus.Start &&
                q.Status != ProcessStatus.FatalError);

            query = query.OrderByDescending(q => q.CreationDate);

            var process = query.ToList().FirstOrDefault();

            return process != null ? process.Count : 0;
        }

        public bool ProcessExecutedSuccessfully(DateTime date)
        {
            var query = Repository.AllNoTracking(q =>
                q.CreationDate.Day.Equals(date.Day) &&
                q.CreationDate.Month.Equals(date.Month) &&
                q.CreationDate.Year.Equals(date.Year));

            query = query.OrderByDescending(q => q.CreationDate);

            var process = query.ToList().FirstOrDefault();

            if (process != null)
            {
                return process.Status == ProcessStatus.Success;
            }
            return false;
        }

        public IEnumerable<ServiceAssociatedDto> GetPendingAutomaticPayments(DateTime date)
        {
            var query = Repository.AllNoTracking(q =>
                q.CreationDate.Day.Equals(date.Day) &&
                q.CreationDate.Month.Equals(date.Month) &&
                q.CreationDate.Year.Equals(date.Year) &&
                q.Count == 1,
                x => x.PendingAutomaticPayments);

            var process = query.ToList().FirstOrDefault();

            var retryErrorCodes = RunState.RetryErrorCodes();

            var pendingRegisters = new List<PendingAutomaticPayment>();
            if (process != null)
            {
                pendingRegisters = process.PendingAutomaticPayments
                    .Where(x => retryErrorCodes.Contains((PaymentResultTypeDto)x.Result)).ToList();
            }

            var result = new List<ServiceAssociatedDto>();

            foreach (var register in pendingRegisters)
            {
                var queryService = _repositoryServiceAsociated.AllNoTracking(x => x.Id == register.PendingServiceAssociateId,
                    s => s.Service,
                    s => s.Service.ServiceCategory,
                    s => s.Service.ServiceGateways,
                    s => s.Service.ServiceGateways.Select(x => x.Gateway),
                    s => s.AutomaticPayment,
                    s => s.DefaultCard,
                    s => s.RegisteredUser,
                    s => s.RegisteredUser.MembershipIdentifierObj,
                    s => s.NotificationConfig,
                    s => s.Service.ServiceContainer);

                var includeFilters = new ServiceAssociatedIncludeFilters
                {
                    IncludeApplicationUserInfo = true,
                    IncludeAutomaticPaymentInfo = true,
                    IncludeCardListInfo = false,
                    IncludeDefaultCardInfo = true,
                    IncludeNotificationConfigInfo = true,
                    IncludeServiceInfo = true,
                    IncludeServiceGatewaysInfo = true,
                    IncludeServiceCategoryInfo = true,
                    IncludeServiceContainerInfo = true
                };

                var service = queryService.ToList().Select(t => t.Convert(includeFilters)).FirstOrDefault();

                result.Add(service);
            }

            return result;
        }

        public void ChangePendingAutomaticPaymentStatus(Guid serviceAssociateId, Guid lastProcessId,
            PaymentResultTypeDto result)
        {
            var query = "UPDATE dbo.PendingAutomaticPayments ";

            query += "SET LastProcessHistoryId = '" + lastProcessId + "', Result = " + (int)result + " ";

            query += "WHERE PendingServiceAssociateId = '" + serviceAssociateId + "' ";
            query += "AND DATEPART(year, Date) = " + DateTime.Today.Year + " ";
            query += "AND DATEPART(month, Date) = " + DateTime.Today.Month + " ";
            query += "AND DATEPART(day, Date) = " + DateTime.Today.Day + ";";

            using (var context = new AppContext())
            {
                context.Database.ExecuteSqlCommand(query);
            }
        }

        public void UpdateProcessHistory(Guid id, List<PendingAutomaticPaymentDto> servicesToRetry,
            AutomaticPaymentStatisticsDto processStatistics, bool firstRun, bool fatalError)
        {
            var processHistory = GetById(id);

            processHistory.Status = ProcessStatusDto.FatalError;
            if (!fatalError)
            {
                processHistory.Status = servicesToRetry.Any() ? ProcessStatusDto.Error : ProcessStatusDto.Success;
            }
            if (firstRun)
            {
                processHistory.PendingAutomaticPayments = servicesToRetry;
            }

            var retryErrorCodes = RunState.RetryErrorCodes();
            var controlledErrorCodes = RunState.ControlledErrorCodes();

            var servicesWithBills =
                processStatistics.ServicesCount -
                processStatistics.ServicesResult.Count(x => x.Value == PaymentResultTypeDto.NoBills) -
                processStatistics.ServicesResult.Count(x => x.Value == PaymentResultTypeDto.GetBillsException);

            processHistory.ServicesCount = processStatistics.ServicesCount;
            processHistory.ServicesWithBillsCount = servicesWithBills;
            processHistory.ServicesFailedGetBillsCount = processStatistics.GetBillsExceptionsCount;
            processHistory.RetryErrorsCount = processStatistics.ServicesResult.Values.Count(retryErrorCodes.Contains);
            processHistory.ControlledErrorsCount = processStatistics.ServicesResult.Values.Count(controlledErrorCodes.Contains);
            processHistory.BillsCount = processStatistics.BillsCount;
            processHistory.BillsToPayCount = processStatistics.BillsToPayCount;
            processHistory.BillsPayedCount = processStatistics.BillsPayedCount;
            processHistory.BillsFailedAmountCount = processStatistics.BillsFailedAmountCount;
            processHistory.BillsFailedQuotasCount = processStatistics.BillsFailedQuotasCount;

            Edit(processHistory);
        }

    }
}