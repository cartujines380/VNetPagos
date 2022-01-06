using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities.ExternalRequest
{
    [Table("WsBillPaymentOnlines")]
    public class WsBillPaymentOnline : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }
        [Index("IX_IdApp_IdOperation", 1, IsUnique = true)]
        [MaxLength(100)]
        public string IdApp { get; set; }
        [Index("IX_IdApp_IdOperation", 2, IsUnique = true)]
        [MaxLength(100)]
        public string IdOperation { get; set; }

        public int CodCommerce { get; set; }
        public int CodBrunch { get; set; }
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
        public ICollection<AuxiliarField> AuxiliarFields { get; set; }

        public int Codresult { get; set; }

        public Guid? PaymentId { get; set; }
        public virtual Payment Payment { get; set; }

        public string CreationUser { get; set; }
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }

        public CustomerShippingAddres CustomerShippingAddres { get; set; }
        public string DeviceFingerprint { get; set; }
        public string CustomerIp { get; set; }
        public string CustomerPhone { get; set; }

        public string WcfVersion{ get; set; }
    }

    [ComplexType]
    public class CustomerShippingAddres
    {
        public string Street { get; set; } 
        public string DoorNumber { get; set; }
        public string Complement  { get; set; }
        public string Corner { get; set; }
        public string Neighborhood { get; set; }
        public string PostalCode { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
    
    [Table("WsBillPaymentOnlineAuxiliarFields")]
    public class AuxiliarField
    {
        public Guid Id { get; set; }
        public string IdValue { get; set; }
        public string Value { get; set; }
        public Guid WsBillPaymentOnlineId { get; set; }
        public virtual WsBillPaymentOnline WsBillPaymentOnline { get; set; }
    }

}
