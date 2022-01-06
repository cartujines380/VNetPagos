using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces
{
    public interface IPaymentHandler
    {
        PaymentResultTypeDto Pay(BillDto bill, ServiceAssociatedDto serviceAssociatedDto);
    }
}
