using System;

namespace VisaNet.Domain.EntitiesDtos.ReportsModel
{
    public class ServiceAssociatedViewDto
    {
        public Guid ServiceAssociatedId { get; set; }
        public Guid ServiceId { get; set; }
        public bool Active { get; set; }
        public string ServiceNameAndDesc { get; set; }
        public string ReferenceNumber { get; set; }
        public string ReferenceNumber2 { get; set; }
        public string ReferenceNumber3 { get; set; }
        public string ReferenceNumber4 { get; set; }
        public string ReferenceNumber5 { get; set; }
        public string ReferenceNumber6 { get; set; }
        public Guid? AutomaticPaymentId { get; set; }
        public Guid RegisteredUserId { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        public Guid DefaultCardId { get; set; }
        public string ServiceCategory { get; set; }
        public string ClientEmail { get; set; }
        public string ClientName { get; set; }
        public string ClientSurname { get; set; }
        public string DefaultCardMask { get; set; }
        public DateTime DueDate { get; set; }
        public bool CardActive { get; set; }
        public bool CardDeleted { get; set; }
        public int PaymentsCount { get; set; }

    }
}