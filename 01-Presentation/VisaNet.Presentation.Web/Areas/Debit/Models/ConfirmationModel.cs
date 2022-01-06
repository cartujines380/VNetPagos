using System.Collections.Generic;
using VisaNet.Presentation.Web.CustomAttributes;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Areas.Debit.Models
{
    public class ConfirmationModel
    {

        public string CommerceName { get; set; }
        
        [CustomDisplay("CommerceModel_ProductSelected")]
        public string ProductName { get; set; }

        public ICollection<ProductPropertyModel> ProductPropertyModelList { get; set; }
        
        public string CurrentStep { get; set; }
        public string CurrentStepMin { get; set; }

        public ApplicationUserModel ApplicationUserModel { get; set; }

        [CustomDisplay("CardDto_MaskedNumber")]
        public string Number { get; set; }

        //TIPO DE PAGINA Y PASO
        public IDictionary<DebitsStepsEnum, int> Setps { get; set; }
    }
}