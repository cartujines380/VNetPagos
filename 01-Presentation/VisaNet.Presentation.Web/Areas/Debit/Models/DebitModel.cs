using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Web.CustomAttributes;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Areas.Debit.Models
{
    public class DebitModel
    {
        public CommerceModel CommerceModel { get; set; }
        public IEnumerable<CommerceModel> Commerces { get; set; }

        public string CurrentStep { get; set; }
        public string CurrentStepMin { get; set; }

        public ApplicationUserModel ApplicationUserModel { get; set; }

        public CardModel CardModel { get; set; }

        //TIPO DE PAGINA Y PASO
        public IDictionary<DebitsStepsEnum, int> Setps { get; set; }
    }

    public class CardModel
    {
        public Guid Id { get; set; }

        [CustomDisplay("CardDto_Name")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }

        [CustomDisplay("CardDto_MaskedNumber")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(25, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Number { get; set; }

        [CustomDisplay("CardDto_SecurityCode")]
        [StringLength(5, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string SecurityCode { get; set; }

        public string PaymentToken { get; set; }

        [CustomDisplay("CardDto_DueDate")]
        public string DueDate { get; set; }
        
        public bool Active { get; set; }

        public string Description { get; set; }
    }
}