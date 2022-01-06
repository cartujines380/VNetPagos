using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ConciliationSistarbancDto
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string IdTransaccionSTB { get; set; }

        public Int64 VisaTransactionId { get; set; }

        public Int64 SistarbancUserId { get; set; }

        public string BillExternalId { get; set; }

        public string Currency { get; set; }

        public double Amount { get; set; }

        //importe con descuento aplicado
        public double AmountTaxed { get; set; }
    }
}
