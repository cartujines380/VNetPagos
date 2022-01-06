using System;
using System.Collections.Generic;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class NotifyPaymentDto
    {
        public string[] References { get; set; }
        public Guid GatewayId { get; set; }
        public ICollection<ServiceGatewayDto> PossibleGateways { get; set; }
        public ICollection<BillDto> Bills { get; set; }
        public string TransactionNumber { get; set; }
        public GatewayEnumDto GatewayEnum { get; set; }
        public string ServiceGatewayReferenceId { get; set; }
        public string ServiceType { get; set; }
        public string Bin { get; set; }

        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool UserRegistred { get; set; }
        public int ServiceDepartament { get; set; }

        public AutomaticPaymentDto AutomaticPaymentDto { get; set; }

        public LogUserType UserType { get; set; }
        public string CybersourceTransactionNumber { get; set; }

        //Nuevo para VisaNetOn (Apps)
        public PaymentDto PaymentDto { get; set; }

    }
}