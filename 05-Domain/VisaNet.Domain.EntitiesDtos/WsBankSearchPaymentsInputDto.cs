using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class WsBankSearchPaymentsInputDto
    {
        public string TransactionId { get; set; }
        public string ServiceId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
