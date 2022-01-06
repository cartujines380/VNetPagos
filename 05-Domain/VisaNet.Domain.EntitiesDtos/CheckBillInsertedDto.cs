using System;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CheckBillInsertedDto
    {
        public string BillExternalId { get; set; }
        public string SucivePreBillNumber { get; set; }
        public GatewayEnumDto GatewayEnum { get; set; }
        public Guid ServiceId { get; set; }
    }
}
