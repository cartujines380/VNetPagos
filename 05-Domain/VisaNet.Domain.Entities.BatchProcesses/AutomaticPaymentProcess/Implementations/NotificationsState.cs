using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations
{
    public class NotificationsState : RunState
    {
        private readonly IServiceServiceAssosiate _serviceServiceAssosiate;
        private readonly Object _servicesResultLock = new Object();

        public NotificationsState(IServiceServiceAssosiate serviceServiceAssosiate)
        {
            _serviceServiceAssosiate = serviceServiceAssosiate;
        }

        public override sealed Guid ProcessHistoryId { get; set; }
        public override sealed List<PendingAutomaticPaymentDto> ServicesToRetry { get; set; }
        public override sealed AutomaticPaymentStatisticsDto ProcessStatistics { get; set; }
        public override bool ShouldPay { get; set; }
        public override bool ShouldNotifyUser { get; set; }
        public override bool ShouldNotifySystem { get; set; }

        public override void StartProcess()
        {
            ProcessStatistics = new AutomaticPaymentStatisticsDto
            {
                ProcessRunNumber = 1,
                ServicesResult = new Dictionary<Guid, PaymentResultTypeDto>()
            };
            ShouldPay = false;
            ShouldNotifyUser = true;
            ShouldNotifySystem = false; //VER
        }

        public override List<ServiceAssociatedDto> GetServices()
        {
            return _serviceServiceAssosiate.GetServicesActiveNotification().ToList();
        }

        public override void UpdateProcessHistory(bool fatalError = false)
        {
            //No debe actualizar el process history
        }

        public override void SetNotificationFlag(Dictionary<Guid, PaymentResultTypeDto> billResultDictionary)
        {
            ShouldNotifyUser = true;
        }

        protected override void SuccessResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult)
        {
            lock (_servicesResultLock)
            {
                ProcessStatistics.ServicesResult.Add(serviceAssociatedDto.Id, serviceResult);
            }
        }

        protected override void ControlledErrorResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult)
        {
            lock (_servicesResultLock)
            {
                ProcessStatistics.ServicesResult.Add(serviceAssociatedDto.Id, serviceResult);
            }
        }

        protected override void DeleteAutomaticPaymentControlledErrorResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult)
        {
            lock (_servicesResultLock)
            {
                ProcessStatistics.ServicesResult.Add(serviceAssociatedDto.Id, serviceResult);
            }
        }

        protected override void RetryErrorResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult)
        {
            lock (_servicesResultLock)
            {
                //Verifico si este servicio ya fue agregado por otro error y luego hubo una excepcion
                if (ProcessStatistics.ServicesResult.ContainsKey(serviceAssociatedDto.Id) &&
                    serviceResult == PaymentResultTypeDto.UnhandledException)
                {
                    ProcessStatistics.ServicesResult[serviceAssociatedDto.Id] = serviceResult;
                }
                else
                {
                    ProcessStatistics.ServicesResult.Add(serviceAssociatedDto.Id, serviceResult);
                }
            }
        }

    }
}