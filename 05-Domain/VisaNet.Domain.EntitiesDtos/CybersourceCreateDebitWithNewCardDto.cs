using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CybersourceCreateDebitWithNewCardDto
    {
        public CsResponseData TokenizationData { get; set; }
        public DebitRequestDto DebitRequestDto{ get; set; }
        public Exception ExceptionCapture { get; set; }

        public int InternalErrorCode { get; set; }
        public string InternalErrorDesc { get; set; }

        public bool UserCreated { get; set; }
        public Guid CommerceId { get; set; }
        public Guid ProductId { get; set; }
    }
}

