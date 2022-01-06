using System.ComponentModel.DataAnnotations.Schema;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CyberSourceOperationData
    {
        public CsResponseData PaymentData { get; set; }
        public CsResponseData VoidData { get; set; }
        public CsResponseData RefundData { get; set; }
        public CsResponseData ReversalData { get; set; }
        public CsResponseDeleteData DeleteData { get; set; }
    }

    [ComplexType]
    public class CsResponseData
    {
        public int PaymentResponseCode { get; set; }
        public string PaymentRequestId { get; set; }
        public string PaymentResponseMsg { get; set; }
    }

    [ComplexType]
    public class CsResponseDeleteData
    {
        public int DeleteResponseCode { get; set; }
        public string DeleteRequestId { get; set; }
        public string DeleteResponseMsg { get; set; }
    }

}