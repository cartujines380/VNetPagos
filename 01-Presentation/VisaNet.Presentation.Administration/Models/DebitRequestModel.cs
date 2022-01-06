using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class DebitRequestModel
    {
        
        public CommerceModel CommerceModel { get; set; }
        
        public Guid Id { get; set; }
        public DebitRequestTypeModel Type { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUserModel ApplicationUserModel { get; set; }
        public Guid CardId { get; set; }
        public int DebitProductId { get; set; }
        [CustomDisplay("PaymentDto_ReferenceNumber")] 
        public long ReferenceNumber { get; set; }
        public DebitRequestStateModel State { get; set; }
        public virtual ICollection<DebitRequestReferenceModel> References { get; set; }

        public CardModel CardModel{ get; set; }
        public DateTime CreationDate { get; set; }

    }

    public class DebitRequestReferenceModel
    {
        public Guid Id { get; set; }
        public int Index { get; set; }
        public int ProductPropertyId { get; set; }
        public string Value { get; set; }
    }

    public enum DebitRequestStateModel
    {
        Pending = 1,
        Synchronized = 2,
        Accepted = 3,
        Rejected = 4,
        Canceled = 5
    }

    public enum DebitRequestTypeModel
    {
        High = 1,
        Low = 2
    }
}