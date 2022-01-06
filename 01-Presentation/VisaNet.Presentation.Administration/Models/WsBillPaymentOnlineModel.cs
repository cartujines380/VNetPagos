using System;

namespace VisaNet.Presentation.Administration.Models
{
    public class WsBillPaymentOnlineModel
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }
        public string IdApp { get; set; }
        public string IdOperation { get; set; }
        public string CodCommerce { get; set; }
        public string CodBranch { get; set; }
        public string IdMerchant { get; set; }

        public string IdUser { get; set; }
        public string IdCard { get; set; }

        public string BillNumber { get; set; }
        public string Description { get; set; }
        public DateTime DateBill { get; set; }
        public string Currency { get; set; }
        public string AmountTotal { get; set; }
        public string AmountTaxed { get; set; }
        public string Indi { get; set; }
        public string ConsFinal { get; set; }
        public string Quota { get; set; }

        public int Codresult { get; set; }

        public Guid? PaymentId { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentTransactionNumber { get; set; }

        public CustomerShippingAddressModel CustomerShippingAddress { get; set; }

        public string DeviceFingerprint { get; set; }
        public string CustomerIp { get; set; }
        public string CustomerPhone { get; set; }
        public string WcfVersion { get; set; }
    }
}

public class CustomerShippingAddressModel
{
    public string Street { get; set; }
    public string DoorNumber { get; set; }
    public string Complement { get; set; }
    public string Corner { get; set; }
    public string Neighborhood { get; set; }
    public string PostalCode { get; set; }
    public string Longitude { get; set; }
    public string Latitude { get; set; }
    public string Phone { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}