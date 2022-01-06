using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisaNet.Domain.EntitiesDtos.ExternalRequest
{
    public class WsBillPaymentOnlineDto
    {
        public Guid Id { get; set; }
        public string IdApp { get; set; }
        public string IdOperation { get; set; }
        public int CodCommerce { get; set; }
        public int CodBranch { get; set; }
        public string IdMerchant { get; set; }
        public string IdUser { get; set; }
        public string IdCard { get; set; }
        public string BillNumber { get; set; }
        public string Description { get; set; }
        public DateTime DateBill { get; set; }
        public string Currency { get; set; }
        public double AmountTotal { get; set; }
        public int Indi { get; set; }
        public double AmountTaxed { get; set; }
        public bool ConsFinal { get; set; }
        public int Quota { get; set; }
        public ICollection<AuxiliarFieldDto> AuxiliarFields { get; set; }

        public int Codresult { get; set; }

        public Guid? PaymentId { get; set; }
        public virtual PaymentDto PaymentDto { get; set; }

        public DateTime CreationDate { get; set; }

        public CustomerShippingAddresDto CustomerShippingAddresDto { get; set; }
        public string DeviceFingerprint { get; set; }
        public string CustomerIp { get; set; }
        public string CustomerPhone { get; set; }
        public string WcfVersion { get; set; }
    }

    [ComplexType]
    public class CustomerShippingAddresDto
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
    

    public class AuxiliarFieldDto
    {
        public Guid Id { get; set; }
        public string IdValue { get; set; }
        public string Value { get; set; }
        public Guid WsBillPaymentOnlineId { get; set; }
        public virtual WsBillPaymentOnlineDto WsBillPaymentOnlineDto { get; set; }
    }

}
