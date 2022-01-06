using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisaNet.Common.Security.Entities
{
    [Table("Functionalities")]
    public class Functionality
    {
        public Functionality()
        {
            Actions = new Collection<Action>();
            FunctionalitiesMembers = new Collection<Functionality>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int Order { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string IconClass { get; set; }

        public virtual ICollection<Action> Actions { get; set; }

        public int? FunctionalityGroupId { get; set; }
        [ForeignKey("FunctionalityGroupId")]
        public virtual FunctionalityGroup FunctionalityGroup { get; set; }


        public int? MemberOfFunctionalityId { get; set; }
        [ForeignKey("MemberOfFunctionalityId")]
        public Functionality MemberOfFunctionality { get; set; }

        public virtual ICollection<Functionality> FunctionalitiesMembers { get; set; }
    }
}
