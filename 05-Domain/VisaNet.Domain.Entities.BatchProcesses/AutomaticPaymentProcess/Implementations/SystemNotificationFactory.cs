using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.Services;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations
{
    public class SystemNotificationFactory : ISystemNotificationFactory
    {
        private readonly ILoggerService _loggerService;
        private readonly IServiceEmailMessage _serviceEmailMessage;

        public SystemNotificationFactory(ILoggerService loggerService, IServiceEmailMessage serviceEmailMessage)
        {
            _loggerService = loggerService;
            _serviceEmailMessage = serviceEmailMessage;
        }

        public void BillExceedsQuotasLoggerNotification(ServiceAssociatedDto serviceAssociatedDto)
        {
            var callcenterMessage = string.Format(
                "El pago programado para el servicio {0} {1} no pudo ser ejecutado, el máximo número {2} de facturas " +
                "a pagar ya fue superado.",
                serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "",
                serviceAssociatedDto.Description,
                serviceAssociatedDto.AutomaticPaymentDto.Quotas);

            var message = string.Format(
                "El pago programado para el servicio {0}-{1} ({2}) no pudo ser ejecutado, el máximo número {3} " +
                "de facturas a pagar ya fue superado.",
                serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "",
                serviceAssociatedDto.Description,
                serviceAssociatedDto.Id,
                serviceAssociatedDto.AutomaticPaymentDto.Quotas);

            LoggerNotification(message, callcenterMessage, serviceAssociatedDto);
        }

        public void BillExceedsAmountLoggerNotification(ServiceAssociatedDto serviceAssociatedDto, BillDto bill)
        {
            var callcenterMessage = string.Format(
                "El pago programado para el servicio {0} {1} no pudo ser ejecutado, el monto máximo ingresado " +
                "{2} {3} es inferior al monto de la factura {4} {5}",
                serviceAssociatedDto.ServiceDto.Name,
                serviceAssociatedDto.Description,
                bill.Currency.Equals("UYU") ? "$" : "U$S",
                serviceAssociatedDto.AutomaticPaymentDto.Maximum,
                bill.Currency.Equals("UYU") ? "$" : "U$S",
                bill.Amount);

            var message = string.Format(
                "El pago programado para el servicio {0}-{1} ({2}) no pudo ser ejecutado, en la factura {3} el monto máximo " +
                "ingresado {4}{5} es inferior al monto de la factura {6}{7}",
                serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "",
                serviceAssociatedDto.Description,
                serviceAssociatedDto.Id,
                bill.BillExternalId,
                bill.Currency,
                serviceAssociatedDto.AutomaticPaymentDto.Maximum,
                bill.Currency,
                bill.Amount);

            LoggerNotification(message, callcenterMessage, serviceAssociatedDto);
        }

        public void NotifyProcessResult(AutomaticPaymentStatisticsDto processStatistics)
        {
            _serviceEmailMessage.SendAutomaticPaymentNotification(processStatistics);
        }

        private void LoggerNotification(string message, string callcenterMessage, ServiceAssociatedDto serviceAssociatedDto)
        {
            _loggerService.CreateLog(LogType.Info,
                                LogOperationType.AutomaticPaymentBatch,
                                LogCommunicationType.VisaNet,
                                serviceAssociatedDto.UserId,
                                message,
                                callcenterMessage);
        }

    }
}