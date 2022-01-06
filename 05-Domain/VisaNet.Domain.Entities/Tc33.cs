using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("Tc33")]
    [TrackChanges]
    public class Tc33: EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }
        [TrackChangesAditionalInfo(Index = 0)]
        public string InputFileName { get; set; }
        public string OutputFileName { get; set; }
        public Tc33State State { get; set; }
        
        public string Errors { get; set; }
        public virtual ICollection<Tc33Transaction> Transactions { get; set; }

        //858 = U$ , 840 = USD
        public int TransactionPesosCount { get; set; }
        public int TransactionDollarCount { get; set; }
        public double TransactionPesosAmount { get; set; }
        public double TransactionDollarAmount { get; set; }
        
        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }

}
