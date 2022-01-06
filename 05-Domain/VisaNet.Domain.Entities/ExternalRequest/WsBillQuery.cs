using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities.ExternalRequest
{
    [Table("WsBillQueries")]
    public class WsBillQuery : EntityBase, IAuditable
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
        public string RefClient { get; set; }
        public string RefClient2 { get; set; }
        public string RefClient3 { get; set; }
        public string RefClient4 { get; set; }
        public string RefClient5 { get; set; }
        public string RefClient6 { get; set; }
        public string BillNumber { get; set; }
        
        public DateTime Date{ get; set; }
        public int Codresult { get; set; }
        
        public string CreationUser { get; set; }
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }

        public string WcfVersion { get; set; }
    }
    
}
