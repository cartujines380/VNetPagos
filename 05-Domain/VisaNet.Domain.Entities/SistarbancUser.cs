using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("SistarbancUser")]
    [TrackChanges]
    public class SistarbancUser : EntityBase, IAuditable
    {

        public SistarbancUser()
        {
            Active = false;
        }

        [Key]
        [TrackChangesAditionalInfo(Index = 0)]
        public override Guid Id { get; set; }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 UniqueIdentifier { get; set; }

        public bool Active { get; set; }
        
        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
