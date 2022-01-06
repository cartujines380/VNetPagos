using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Administration.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class BinGroupModel
    {
        public Guid Id { get; set; }

        [CustomDisplay("BinGroup_Bins")]
        public ICollection<BinModel> Bins { get; set; }
        public ICollection<BinModel> AddedBins { get; set; }
        public ICollection<BinModel> DeletedBins { get; set; }
        
        public ICollection<Guid> Services { get; set; }
        public ICollection<SelectListItem> ServicesSelectList { get; set; }

        [CustomDisplay("BinGroup_Name")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }
    }
}