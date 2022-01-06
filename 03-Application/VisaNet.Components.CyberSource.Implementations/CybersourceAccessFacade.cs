using System.Collections.Generic;
using VisaNet.Components.CyberSource.Interfaces;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Utilities.Cybersource;

namespace CyberSource
{
    public class CybersourceAccessFacade : ICybersourceAccessFacade
    {
        private readonly ICyberSourceAccess _cybersoureAccess;

        public CybersourceAccessFacade(ICyberSourceAccess cybersoureAccess)
        {
            _cybersoureAccess = cybersoureAccess;
        }

        public IDictionary<string, string> GenerateKeys(IGenerateToken token)
        {
            // PAGO
            if (token.GetType() == typeof(KeysInfoForPaymentAnonymousUser))
            {
                //Pago Anonimo
                return _cybersoureAccess.LoadKeysForAnonymousUserPayment(token);
            }
            if (token.GetType() == typeof(KeysInfoForPaymentRegisteredUser))
            {
                //Pago Usuario Registrado
                return _cybersoureAccess.LoadKeysForRegisteredUserPayment(token);
            }
            if (token.GetType() == typeof(KeysInfoForPaymentRecurrentUser))
            {
                //Pago Usuario Recurrente
                return _cybersoureAccess.LoadKeysForRecurrentUserPayment(token);
            }
            if (token.GetType() == typeof(KeysInfoForPaymentNewUser))
            {
                //Pago con posterior creacion de Usuario Recurrente
                return _cybersoureAccess.LoadKeysForNewUserPayment(token);
            }


            // TOKENIZACION
            if (token.GetType() == typeof(KeysInfoForTokenRegisteredUser) &&
                (token.RedirectTo == (RedirectEnums.AppAdmission).ToString("D") ||
                token.RedirectTo == (RedirectEnums.VisaNetOnTokenizationRegistered).ToString("D")))
            {
                //Tokenizacion Usuario Apps
                return _cybersoureAccess.LoadKeysForRegisteredUserTokenApps(token);
            }
            if (token.GetType() == typeof(KeysInfoForTokenRecurrentUser))
            {
                //Tokenizacion Usuario Recurrente VisaNetOn
                return _cybersoureAccess.LoadKeysForRecurrentUserToken(token);
            }
            if (token.GetType() == typeof(KeysInfoForTokenNewUser) &&
                (token.RedirectTo == (RedirectEnums.AppAdmission).ToString("D")))
            {
                //Tokenizacion con posterior creacion de Usuario Registrado
                return _cybersoureAccess.LoadKeysForNewUserTokenApps(token);
            }
            if (token.GetType() == typeof(KeysInfoForTokenNewUser) &&
                token.RedirectTo == (RedirectEnums.VisaNetOnTokenizationNewUser).ToString("D"))
            {
                //Tokenizacion con posterior creacion de Usuario Recurrente VisaNetOn
                return _cybersoureAccess.LoadKeysForNewRecurrentUser(token);
            }
            if (token.GetType() == typeof(KeysInfoForTokenRegisteredUser))
            {
                //Tokenizacion Usuario Registrado
                return _cybersoureAccess.LoadKeysForToken(token);
            }

            //DEBITO
            if (token.GetType() == typeof(KeysInfoForTokenDebitNewUser))
            {
                //Debito tokenizacion Usuario nuevo
                return _cybersoureAccess.LoadKeysForTokenDebitNewUser(token);
            }
            if (token.GetType() == typeof(KeysInfoForTokenDebitRegisteredUser))
            {
                //Debito tokenizacion usuario registrado
                return _cybersoureAccess.LoadKeysForTokenDebitRegisteredUser(token);
            }

            return null;
        }

    }
}