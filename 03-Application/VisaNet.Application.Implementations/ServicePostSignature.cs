using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Utilities.DigitalSignature;

namespace VisaNet.Application.Implementations
{
    public abstract class ServicePostSignature : IServicePostSignature
    {
        public abstract IDictionary<string, string> GetFieldsForNewUserSignature(WebhookNewAssociationDto entity);
        public abstract IDictionary<string, string> GetFieldsForNewCardSignature(WebhookNewAssociationDto entity);
        public abstract IDictionary<string, string> GetFieldsForNewPaymentSignature(WebhookNewAssociationDto entity);
        public abstract IDictionary<string, string> GetFieldsForUserDownSignature(WebhookDownDto entity);
        public abstract IDictionary<string, string> GetFieldsForCardDownSignature(WebhookDownDto entity);

        public string GetSignature(IDictionary<string, string> fieldsToSign, string certificateThumbprintVisa)
        {
            var fieldsToSignArray = fieldsToSign.Values.ToArray();
            var signature = DigitalSignature.GenerateSignature(fieldsToSignArray, certificateThumbprintVisa);
            return signature;
        }

    }
}