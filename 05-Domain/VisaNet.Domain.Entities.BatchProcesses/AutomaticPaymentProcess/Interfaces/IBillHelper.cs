using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces
{
    public interface IBillHelper
    {
        ICollection<BillDto> ObtainBillsForService(ServiceAssociatedDto serviceAssociatedDto);
        ICollection<BillDto> FilterBills(ICollection<BillDto> bills, ServiceAssociatedDto serviceAssociatedDto,
            ref Dictionary<Guid, PaymentResultTypeDto> billsDictionary);
    }
}
