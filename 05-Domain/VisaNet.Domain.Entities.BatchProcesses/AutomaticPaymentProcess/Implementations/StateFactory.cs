using System;
using System.Configuration;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations
{
    public class StateFactory : IStateFactory
    {
        private readonly IServiceProcessHistory _processHistoryService;
        private readonly IServiceServiceAssosiate _serviceServiceAssosiate;
        private readonly ILoggerHelper _loggerHelper;

        public StateFactory(IServiceProcessHistory processHistoryService, IServiceServiceAssosiate serviceServiceAssosiate,
            ILoggerHelper loggerHelper)
        {
            _processHistoryService = processHistoryService;
            _serviceServiceAssosiate = serviceServiceAssosiate;
            _loggerHelper = loggerHelper;
        }

        public RunState GetState()
        {
            var process = SelectProcess();

            if (process == AutomaticPaymentNotificationEnumDto.Both ||
                process == AutomaticPaymentNotificationEnumDto.AutomaticPayment)
            {
                var processMaxTries = Convert.ToInt32(ConfigurationManager.AppSettings["ProcessPaymentMaxTries"]);
                var timesExecuted = _processHistoryService.GetProcessCountForDate(DateTime.Now);

                if (timesExecuted > 0)
                {
                    var executedSuccessfully = _processHistoryService.ProcessExecutedSuccessfully(DateTime.Now);
                    if (executedSuccessfully)
                    {
                        _loggerHelper.LogProcessExecutedSuccessfuly();
                        return new FinishedState();
                    }
                    if (timesExecuted >= processMaxTries)
                    {
                        _loggerHelper.LogProcessReachedMaxTries(processMaxTries);
                        return new FinishedState();
                    }
                    _loggerHelper.LogProcessRunNumber(timesExecuted);
                    if (timesExecuted == processMaxTries - 1)
                    {
                        return new LastRunState(_serviceServiceAssosiate, _processHistoryService);
                    }
                    return new RetryRunState(_serviceServiceAssosiate, _processHistoryService);
                }
                if (process == AutomaticPaymentNotificationEnumDto.Both)
                {
                    //Proceso de pago programado y notificaciones
                    _loggerHelper.LogProcessRunNumber(timesExecuted);
                    return new FirstRunState(_serviceServiceAssosiate, _processHistoryService);
                }

                //Solo proceso de pago programado
                _loggerHelper.LogAutomaticPaymentsSingleProcess(timesExecuted);
                return new AutomaticPaymentsState(_serviceServiceAssosiate, _processHistoryService);
            }

            //Solo proceso de notificaciones
            if (process == AutomaticPaymentNotificationEnumDto.Notification)
            {
                _loggerHelper.LogNotificationsSingleProcess();
                return new NotificationsState(_serviceServiceAssosiate);
            }

            return new FinishedState();
        }

        private AutomaticPaymentNotificationEnumDto SelectProcess()
        {
            var selectProcess = ConfigurationManager.AppSettings["SelectProcess"];

            AutomaticPaymentNotificationEnumDto value;
            if (!Enum.TryParse(selectProcess, true, out value))
            {
                value = AutomaticPaymentNotificationEnumDto.Both;
            }

            return value;
        }

    }
}
