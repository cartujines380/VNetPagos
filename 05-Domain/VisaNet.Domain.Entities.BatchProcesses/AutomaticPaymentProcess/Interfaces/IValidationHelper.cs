using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces
{
    public interface IValidationHelper
    {
        PaymentResultTypeDto ValidateService(ServiceAssociatedDto serviceAssociatedDto);

    }
}
