using System;
using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{
    public interface IAssociationInfoDto
    {
        Guid Id { get; set; }
        bool Active { get; set; }

        Guid ServiceId { get; set; }
        ServiceDto ServiceDto { get; set; }

        string ReferenceNumber { get; set; }
        string ReferenceNumber2 { get; set; }
        string ReferenceNumber3 { get; set; }
        string ReferenceNumber4 { get; set; }
        string ReferenceNumber5 { get; set; }
        string ReferenceNumber6 { get; set; }

        string Description { get; set; }

        Guid? AutomaticPaymentDtoId { get; set; }
        AutomaticPaymentDto AutomaticPaymentDto { get; set; }

        Guid UserId { get; set; }
        ApplicationUserDto RegisteredUserDto { get; set; }

        Guid NotificationConfigDtoId { get; set; }
        NotificationConfigDto NotificationConfigDto { get; set; }

        bool Enabled { get; set; }

        Guid DefaultCardId { get; set; }
        CardDto DefaultCard { get; set; }
        ICollection<CardDto> CardDtos { get; set; }
        string OperationId { get; set; }
        Guid? IdUserExternal { get; set; }

        bool AllowGetBills { get; set; }
        bool AllowInputAmount { get; set; }


        //Metodos
        CardDto GetCardFromExternalId(string cardExternalId);
        PaymentDto FillPaymentServiceAndUserData(PaymentDto paymentDto);
    }
}