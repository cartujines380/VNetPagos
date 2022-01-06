using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class RoleModel
    {
        public RoleModel()
        {
            ActionsIds = new List<int>();
        }
        public Guid Id { get; set; }
        
        [CustomDisplay("RoleModel_Name")]
        [Required]
        public string Name { get; set; }

        public IList<int> ActionsIds { get; set; }
    }
}
