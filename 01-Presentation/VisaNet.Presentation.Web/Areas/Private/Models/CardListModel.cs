using System;
using VisaNet.Presentation.Web.CustomAttributes;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class CardListModel
    {
        public Guid Id { get; set; }
        [CustomDisplay("Card")]
        public string Mask { get; set; }
        [CustomDisplay("Card_DueDate")]
        public string DueDate { get; set; }

        public bool Active { get; set; }
        public bool Default { get; set; }

        public Guid ServiceId { get; set; }
        public string CardImage { get; set; }
        public bool Expired { get; set; }

        public bool MultipleCards{ get; set; }
        public bool Used { get; set; }
        public string Description { get; set; }
    }
}