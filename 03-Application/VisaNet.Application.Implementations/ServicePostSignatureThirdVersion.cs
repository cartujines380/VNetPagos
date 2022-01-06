using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;

namespace VisaNet.Application.Implementations
{
    public class ServicePostSignatureThirdVersion : ServicePostSignature
    {
        public override IDictionary<string, string> GetFieldsForNewUserSignature(WebhookNewAssociationDto entity)
        {
            var fieldsToSign = new Dictionary<string, string>() 
            {
                {"Email", entity.UserData.Email},
                {"Nombre", entity.UserData.Name},
                {"Apellido", entity.UserData.Surname},
                {"Direccion", entity.UserData.Address},
                {"Telefono", entity.UserData.PhoneNumber},
                {"Movil", entity.UserData.MobileNumber},
                {"CI", entity.UserData.IdentityNumber},
                {"IdUsuario", entity.IdUser},
                {"IdTarjeta", entity.IdCard},
                {"VencTarjeta", entity.CardDueDate},
                {"SufijoTarjeta", entity.CardMask},
                {"RefCliente1", entity.RefCliente1},
                {"RefCliente2", entity.RefCliente2},
                {"RefCliente3", entity.RefCliente3},
                {"RefCliente4", entity.RefCliente4},
                {"RefCliente5", entity.RefCliente5},
                {"RefCliente6", entity.RefCliente6},
                {"IdOperacionApp", entity.IdOperationApp},
                {"IdOperacion", entity.IdOperation},
                {"EnvioAsociacion", entity.IsAssociation.ToString()},
                {"EnvioPago", entity.IsPayment.ToString()},
                {"EmisorTarjeta", entity.CardBank},
                {"TipoTarjeta", entity.CardType != null ? ((int) entity.CardType).ToString() : null},
            };
            return fieldsToSign;
        }

        public override IDictionary<string, string> GetFieldsForNewCardSignature(WebhookNewAssociationDto entity)
        {
            var fieldsToSign = new Dictionary<string, string>()
            {
                {"IdUsuario", entity.IdUser},
                {"IdTarjeta", entity.IdCard},
                {"VencTarjeta", entity.CardDueDate},
                {"SufijoTarjeta", entity.CardMask},
                {"IdOperacionApp", entity.IdOperationApp},
                {"IdOperacion", entity.IdOperation},
                {"EnvioAsociacion", entity.IsAssociation.ToString()},
                {"EnvioPago", entity.IsPayment.ToString()},
                {"EmisorTarjeta", entity.CardBank},
                {"TipoTarjeta", entity.CardType != null ? ((int) entity.CardType).ToString() : null},
            };
            return fieldsToSign;
        }

        public override IDictionary<string, string> GetFieldsForNewPaymentSignature(WebhookNewAssociationDto entity)
        {
            var fieldsToSign = new Dictionary<string, string>()
            {
                {"Email", entity.UserData.Email},
                {"Nombre", entity.UserData.Name},
                {"Apellido", entity.UserData.Surname},
                {"Direccion", entity.UserData.Address},
                {"Telefono", entity.UserData.PhoneNumber},
                {"Movil", entity.UserData.MobileNumber},
                {"CI", entity.UserData.IdentityNumber},
                {"IdUsuario", entity.IdUser},
                {"IdTarjeta", entity.IdCard},
                {"VencTarjeta", entity.CardDueDate},
                {"SufijoTarjeta", entity.CardMask},
                {"RefCliente1", entity.RefCliente1},
                {"RefCliente2", entity.RefCliente2},
                {"RefCliente3", entity.RefCliente3},
                {"RefCliente4", entity.RefCliente4},
                {"RefCliente5", entity.RefCliente5},
                {"RefCliente6", entity.RefCliente6},
                {"IdOperacionApp", entity.IdOperationApp},
                {"IdOperacion", entity.IdOperation},
                {"EnvioAsociacion", entity.IsAssociation.ToString()},
                {"EnvioPago", entity.IsPayment.ToString()},
                {"NroTransaccion", entity.TransactionNumber},
                {"FacturaImporteDescuento", entity.DiscountAmount.ToString("0.##").Replace(",", "").Replace(".", "")},
                {"EmisorTarjeta", entity.CardBank},
                {"TipoTarjeta", entity.CardType != null ? ((int) entity.CardType).ToString() : null},
            };
            return fieldsToSign;
        }

        public override IDictionary<string, string> GetFieldsForUserDownSignature(WebhookDownDto entity)
        {
            var fieldsToSign = new Dictionary<string, string>()
            {
                {"IdUsuario", entity.IdUser},
                {"IdOperacion", entity.IdOperation},
            };
            return fieldsToSign;
        }

        public override IDictionary<string, string> GetFieldsForCardDownSignature(WebhookDownDto entity)
        {
            var fieldsToSign = new Dictionary<string, string>()
            {
                {"IdUsuario", entity.IdUser},
                {"IdTarjeta", entity.IdCard},
                {"IdOperacion", entity.IdOperation},
            };
            return fieldsToSign;
        }

    }
}