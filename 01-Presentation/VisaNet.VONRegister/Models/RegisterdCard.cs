using System;

namespace VisaNet.VONRegister.Models
{
    public class RegisterdCard
    {
        public Guid Id { get; set; }
        public string MaskedNumber { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool Active { get; set; }
        public bool AlreadyIn { get; set; }
        public bool Expired { get; set; }
    }
}