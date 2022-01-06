using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class DebitRequestExcelDto
    {
        public DateTime CreationDate { get; set; }
        public DateTime SynchronizationDate { get; set; }
        public string UserFullName { get; set; }
        public string UserIdentityNumber { get; set; }
        public string UserAddress { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserEmail { get; set; }
        public string MerchantGroup { get; set; }
        public string Merchant { get; set; }
        public string MerchantProduct { get; set; }
        public string Type { get; set; }
        public string CardNumber { get; set; }
        public int CardMonth { get; set; }
        public int CardYear { get; set; }        
        public string References { get; set; }

        #region Cybersource Data
        public string Token { get; set; }
        public string MerchantId { get; set; }
        public string MerchantReferenceCode { get; set; }
        #endregion
    }
}
