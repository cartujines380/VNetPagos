using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Common.Security.Entities.Security;


namespace VisaNet.Common.Security.Entities
{
    [Table("Actions")]
    public class Action
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string MvcController { get; set; }
        [MaxLength(50)]
        public string MvcAction { get; set; }

        public bool IsDefaultAction { get; set; }
        public int ActionType { get; set; }

        public int FunctionalityId { get; set; }
        [ForeignKey("FunctionalityId")]
        public virtual Functionality Functionality { get; set; }

        public int? ActionRequiredId { get; set; }
        [ForeignKey("ActionRequiredId")]
        public virtual Action ActionRequired { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
