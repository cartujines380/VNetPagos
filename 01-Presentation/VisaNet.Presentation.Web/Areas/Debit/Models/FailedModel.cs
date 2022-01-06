using System.Collections.Generic;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Areas.Debit.Models
{
    public class FailedModel
    {
        public string CurrentStep { get; set; }
        public string CurrentStepMin { get; set; }
        
        //TIPO DE PAGINA Y PASO
        public IDictionary<DebitsStepsEnum, int> Setps { get; set; }

        public CommerceModel CommerceModel { get; set; }
        public IEnumerable<CommerceModel> Commerces { get; set; }
        public ApplicationUserModel ApplicationUserModel { get; set; }
        public CardModel CardModel { get; set; }

        public string FailedMsg { get; set; }
    }
}