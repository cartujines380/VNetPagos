using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("Subscribers")]
    public class Subscriber : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }
        
        [MaxLength(50)]
        public string Name { get; set; }
        
        [MaxLength(50)]
        public string Surname { get; set; }
        
        [MaxLength(50)]
        public string Email { get; set; }
        
        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
