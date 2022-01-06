using System;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.CustomerSite.EntitiesDtos
{
    public class CustomerSiteGenerateAccessTokenDto
    {
        public string Email { get; set; }
        public string IdentityNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }

        public string BillExternalId { get; set; }
        public double BillAmount { get; set; }
        public double BillTaxedAmount { get; set; }
        public string BillCurrency { get; set; }
        public bool BillFinalConsumer { get; set; }
        public DateTime BillGenerationDate { get; set; }
        public DateTime BillExpirationDate { get; set; }

        public bool BillQuota { get; set; }
        public string BillDescription { get; set; }

        public string IdOperation { get; set; }
        public string UrlCallback { get; set; }
        public Guid ServiceId { get; set; }
        public string EnableRememberUser { get; set; }
        public string SendEmail { get; set; }

        public ServiceDto ServiceDto { get; set; }

        public string AccessTokenBaseUrl { get; set; }
        public string GeneratedUrl { get; set; }
    }
}