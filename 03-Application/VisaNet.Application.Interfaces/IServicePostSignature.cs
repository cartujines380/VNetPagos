using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;

namespace VisaNet.Application.Interfaces
{
    public interface IServicePostSignature
    {
        IDictionary<string, string> GetFieldsForNewUserSignature(WebhookNewAssociationDto entity);
        IDictionary<string, string> GetFieldsForNewCardSignature(WebhookNewAssociationDto entity);
        IDictionary<string, string> GetFieldsForNewPaymentSignature(WebhookNewAssociationDto entity);
        IDictionary<string, string> GetFieldsForUserDownSignature(WebhookDownDto entity);
        IDictionary<string, string> GetFieldsForCardDownSignature(WebhookDownDto entity);
        string GetSignature(IDictionary<string, string> fieldsToSign, string certificateThumbprintVisa);
    }
}