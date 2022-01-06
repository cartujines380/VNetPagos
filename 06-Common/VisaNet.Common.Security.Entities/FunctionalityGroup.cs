using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisaNet.Common.Security.Entities
{
    [Table("FunctionalitiesGroups")]
    public class FunctionalityGroup
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int Order { get; set; }
        
        [MaxLength(50)]
        public string Name { get; set; }
        
        [MaxLength(50)]
        public string IconClass { get; set; }

        public virtual ICollection<Functionality> Functionalities { get; set; }
    }
}
