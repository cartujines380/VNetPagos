using System;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Web.Models
{
    public class FaqModel
    {
        public Guid Id { get; set; }

        [CustomDisplay("FaqDto_Order")]
        public int Order { get; set; }

        [CustomDisplay("FaqDto_Question")]
        public string Question { get; set; }

        [CustomDisplay("FaqDto_Answer")]
        public string Answer { get; set; }
    }
}
