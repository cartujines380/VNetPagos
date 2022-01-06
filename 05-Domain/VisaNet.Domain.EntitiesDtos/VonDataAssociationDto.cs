using System;
using System.Collections.Generic;
using System.Linq;

namespace VisaNet.Domain.EntitiesDtos
{
    public class VonDataAssociationDto : IAssociationInfoDto
    {
        public Guid Id { get; set; }

        public Guid ServiceId { get; set; }
        public ServiceDto ServiceDto { get; set; }

        public Guid UserId { get; set; } //Es el AnonymousUserId
        public AnonymousUserDto AnonymousUserDto { get; set; }
        public Guid? IdUserExternal { get; set; } //UserExternalId de la tabla VonData

        public Guid DefaultCardId { get; set; } //CardExternalId de la tabla VonData ?? sino deberia ser Guid.Empty
        public CardDto DefaultCard { get; set; }
        public ICollection<CardDto> CardDtos { get; set; }

        public bool Active { get; set; }
        public bool Enabled { get; set; }
        public string ReferenceNumber { get; set; }
        public string ReferenceNumber2 { get; set; }
        public string ReferenceNumber3 { get; set; }
        public string ReferenceNumber4 { get; set; }
        public string ReferenceNumber5 { get; set; }
        public string ReferenceNumber6 { get; set; }
        public string Description { get; set; }
        public string OperationId { get; set; }
        public bool AllowGetBills { get; set; }
        public bool AllowInputAmount { get; set; }

        //Las siguientes propiedades deberian ser nulas
        public Guid? AutomaticPaymentDtoId { get; set; }
        public AutomaticPaymentDto AutomaticPaymentDto { get; set; }
        public ApplicationUserDto RegisteredUserDto { get; set; }
        public Guid NotificationConfigDtoId { get; set; }
        public NotificationConfigDto NotificationConfigDto { get; set; }

        public VonDataAssociationDto()
        {
            Active = true;
            Enabled = true;
            AllowGetBills = true;
            AllowInputAmount = true;
        }


        //Metodos
        public CardDto GetCardFromExternalId(string cardExternalId)
        {
            CardDto card;
            if (this.DefaultCard.ExternalId == Guid.Parse(cardExternalId))
            {
                card = this.DefaultCard;
            }
            else
            {
                card = (this.CardDtos != null) ? this.CardDtos.FirstOrDefault(x => x.ExternalId != null && x.ExternalId == Guid.Parse(cardExternalId)) : null;
            }
            return card;
        }

        public PaymentDto FillPaymentServiceAndUserData(PaymentDto paymentDto)
        {
            paymentDto.AnonymousUserId = this.UserId;
            paymentDto.AnonymousUser = this.AnonymousUserDto;
            return paymentDto;
        }

    }
}