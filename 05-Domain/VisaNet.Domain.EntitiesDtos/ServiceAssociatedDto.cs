using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    
    public class ServiceAssociatedDto : IAssociationInfoDto
    {
        public Guid Id { get; set; }

        public bool Active { get; set; }

        public Guid ServiceId { get; set; }
        public ServiceDto ServiceDto { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceNumber { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceNumber2 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceNumber3 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceNumber4 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceNumber5 { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceNumber6 { get; set; }

        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Description { get; set; }

        public Guid? AutomaticPaymentDtoId { get; set; }
        public AutomaticPaymentDto AutomaticPaymentDto { get; set; }

        public Guid UserId { get; set; } //Es el ApplicationUserId
        public ApplicationUserDto RegisteredUserDto { get; set; }

        public Guid NotificationConfigDtoId { get; set; }
        public NotificationConfigDto NotificationConfigDto { get; set; }

        public bool Enabled { get; set; }
        //public bool Deleted { get; set; }

        public Guid DefaultCardId { get; set; }
        public CardDto DefaultCard { get; set; }
        public ICollection<CardDto> CardDtos { get; set; }
        public string OperationId { get; set; }
        public Guid? IdUserExternal { get; set; }

        public bool AllowGetBills { get; set; }
        public bool AllowInputAmount { get; set; }
       
        //Metodos
        public CardDto GetCardFromExternalId(string cardExternalId)
        {
            CardDto card;
            if (this.ServiceDto.AllowMultipleCards)
            {
                card = (this.CardDtos != null) ? this.CardDtos.FirstOrDefault(x => x.ExternalId != null && x.ExternalId == Guid.Parse(cardExternalId)) : null;
            }
            else
            {
                card = this.DefaultCard.ExternalId == Guid.Parse(cardExternalId) ? this.DefaultCard : null;
            }
            return card;
        }

        public PaymentDto FillPaymentServiceAndUserData(PaymentDto paymentDto)
        {
            paymentDto.ServiceAssociatedDto = this;
            paymentDto.ServiceAssociatedId = this.Id;
            paymentDto.RegisteredUserId = this.UserId;
            paymentDto.RegisteredUser = this.RegisteredUserDto;
            return paymentDto;
        }

    }
}