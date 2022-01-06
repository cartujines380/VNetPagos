using System;
using System.Configuration;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces;

namespace VisaNet.Testing.AutomaticPayment
{
    public class TestStateFactory : IStateFactory
    {
        private readonly IServiceProcessHistory _processHistoryService;
        private readonly IServiceServiceAssosiate _serviceServiceAssosiate;

        public TestStateFactory(IServiceProcessHistory processHistoryService,
            IServiceServiceAssosiate serviceServiceAssosiate)
        {
            _processHistoryService = processHistoryService;
            _serviceServiceAssosiate = serviceServiceAssosiate;
        }

        public RunState GetState()
        {
            var processMaxTries = Convert.ToInt32(ConfigurationManager.AppSettings["ProcessPaymentMaxTries"]);
            var timesExecuted = _processHistoryService.GetProcessCountForDate(DateTime.Now);
            if (timesExecuted > 0)
            {
                var executedSuccessfully = _processHistoryService.ProcessExecutedSuccessfully(DateTime.Now);
                if (executedSuccessfully)
                {
                    return new FinishedState();
                }
                if (timesExecuted >= processMaxTries)
                {
                    return new FinishedState();
                }
                if (timesExecuted == processMaxTries - 1)
                {
                    return new TestLastRunState(_serviceServiceAssosiate, _processHistoryService);
                }
                return new TestRetryRunState(_serviceServiceAssosiate, _processHistoryService);
            }
            return new TestFirstRunState(_serviceServiceAssosiate, _processHistoryService);
        }

    }
}
