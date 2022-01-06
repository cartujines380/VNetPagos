using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class DebitRequestDto
    {
        public Guid Id { get; set; }
        public DebitRequestTypeDto Type { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUserDto ApplicationUserDto { get; set; }
        public Guid CardId { get; set; }
        public int DebitProductId { get; set; }
        public DebitRequestStateDto State { get; set; }
        public virtual ICollection<DebitRequestReferenceDto> References { get; set; }

        public CardDto CardDto { get; set; }
        public CommerceDto CommerceDto { get; set; }

        public long ReferenceNumber { get; set; } 
         
        public DateTime CreationDate { get; set; }
    }

    public class DeleteCardRequestDto
    {
        public string ProductName { get; set; }
        public string ServiceName { get; set; }
        public string MaskedNumber { get; set; }
        public string Status { get; set; }
        public Dictionary<string, string> References { get; set; }
        public string Type { get; set; }
        public ApplicationUserDto ApplicationUserDto { get; set; }
        public CardDto CardDto { get; set; }

    }
    public class DebitRequestReferenceDto
    {
        public Guid Id { get; set; }
        public int Index { get; set; }
        public int ProductPropertyId { get; set; }
        public string Value { get; set; }
    }

    public class CommerceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ServiceId { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<ProductDto> ProductosListDto { get; set; }
    }

    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public ICollection<ProductPropertyDto> ProductPropertyList { get; set; }
    }

    public class ProductPropertyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }

        public int MaxSize { get; set; }
        public int InputSequence { get; set; }
        public bool Requiered { get; set; }

        public int? DebitProductPropertyId { get; set; }
        public int? DebitProductId { get; set; }
    }

}
