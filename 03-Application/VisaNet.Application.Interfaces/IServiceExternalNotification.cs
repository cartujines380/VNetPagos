using System;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceExternalNotification
    {
        bool NotifyExternalSourceNewAssociation(ServiceAssociated entity, Card card, string operationId);
        bool NotifyExternalSourceNewAssociation(PaymentDto paymentDto, string idUserExternal, string idCardExternal);
        bool NotifyExternalSourceNewCard(ServiceAssociated entity, Card card, string operationId);
        bool NotifyExternalSourceNewCard(PaymentDto paymentDto, string idUserExternal, string idCardExternal);
        bool NotifyExternalSourceNewPayment(NotifyPaymentDto notify);
        bool NotifyExternalSourceNewPayment(PaymentDto paymentDto, string idUserExternal, string idCardExternal, bool withAssociation);
        bool NotifyExternalSourceNewPaymentAnonymous(PaymentDto paymentDto);
        bool NotifyExternalSourceDownService(ServiceAssociated entity);
        bool NotifyExternalSourceDownCard(ServiceAssociated entity, Guid externalCardId);
        bool AlreadyNotifiedExternalCard(string idApp, string idUserExternal, string idCardExternal);
    }
}