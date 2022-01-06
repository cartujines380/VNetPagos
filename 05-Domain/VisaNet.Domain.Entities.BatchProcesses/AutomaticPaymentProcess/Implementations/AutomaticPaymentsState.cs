using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations
{
    public class AutomaticPaymentsState : FirstRunState
    {
        public AutomaticPaymentsState(IServiceServiceAssosiate serviceServiceAssosiate, IServiceProcessHistory processHistoryService)
            : base(serviceServiceAssosiate, processHistoryService)
        {
        }

        public override List<ServiceAssociatedDto> GetServices()
        {
            return _serviceServiceAssosiate.GetServicesActiveAutomaticPayment().ToList();
        }

    }
}