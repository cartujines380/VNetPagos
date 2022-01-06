using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("Contacts")]
    [TrackChanges]
    public class Contact : EntityBase, IAuditable
    {

        public Contact()
        {
            Taken = false;
        }

        [Key]
        public override Guid Id { get; set; }

        [MaxLength(50)]
        [TrackChangesAditionalInfo(Index = 0)]
        public string Name { get; set; }
        [MaxLength(50)]
        [TrackChangesAditionalInfo(Index = 1)]
        public string Surname { get; set; }
        [MaxLength(50)]
        [TrackChangesAditionalInfo(Index = 2)]
        public string Email { get; set; }
        public QueryType QueryType { get; set; }
        [MaxLength(150)]
        public string Subject { get; set; }
        [MaxLength(500)]
        public string Message { get; set; }
        public DateTime Date { get; set; }

        public bool Taken { get; set; }
        public string Comments { get; set; }

        public Guid? UserTookId { get; set; }
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        public virtual SystemUser UserTook { get; set; }
        
        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
