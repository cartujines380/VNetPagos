using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("Faqs")]
    public class Faq : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }
        
        public int Order { get; set; }
        
        [MaxLength(250)]
        public string Question { get; set; }

        [MaxLength(1024)]
        public string Answer { get; set; }
        
        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
