using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities.ExternalRequest
{
    [Table("WsCommerceQueries")]
    public class WsCommerceQuery : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        [Index("IX_IdApp_IdOperation", 1, IsUnique = true)]
        [MaxLength(100)]
        public string IdApp { get; set; }
        [Index("IX_IdApp_IdOperation", 2, IsUnique = true)]
        [MaxLength(100)]
        public string IdOperation { get; set; }

        public int Codresult { get; set; }

        public string CreationUser { get; set; }
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }

        public string WcfVersion { get; set; }
    }

}
