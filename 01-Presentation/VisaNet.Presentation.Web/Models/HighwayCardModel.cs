using System;

namespace VisaNet.Presentation.Web.Models
{
    public class HighwayCardModel
    {
        public Guid Id { get; set; }
        public string Mask { get; set; }
        public DateTime DueDate { get; set; }
        public bool Active { get; set; }
        public bool AlreadyIn { get; set; }
    }
}