using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CyberSourceExtraDataDto
    {
        //Monto debitado en tarjeta
        public double CybersourceAmount { get; set; }
        //factuas con monto de descuento actualizado
        public BillDto BillDto { get; set; }
        public DiscountDto DiscountDto { get; set; }
        public string CallcenterUser { get; set; }
        public string OperationId { get; set; }
        public int BinNumber { get; set; }
        public Guid ServiceId { get; set; }
    }
}
