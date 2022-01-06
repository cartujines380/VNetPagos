using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Utilities.Exportation.ExtensionMethods;
using VisaNet.WebService.VisaWCF.EntitiesDto;

namespace VisaNet.WebService.VisaWCF.Mappers
{
    public static class ServiceMapper
    {
        public static VisaNetServices ToVisaNetServiceResult(this ServiceDto serviceDto)
        {
            var service = new VisaNetServices
            {
                Active = serviceDto.Active,
                ServiceType = serviceDto.ServiceCategoryName,
                ServiceName = serviceDto.Name,
                ServiceReferenceName = serviceDto.ReferenceParamName,
                ServiceReferenceName2 = serviceDto.ReferenceParamName2,
                ServiceReferenceName3 = serviceDto.ReferenceParamName3,
                ServiceReferenceName4 = serviceDto.ReferenceParamName4,
                ServiceReferenceName5 = serviceDto.ReferenceParamName5,
                ServiceReferenceName6 = serviceDto.ReferenceParamName6,
                //CreditCard = serviceDto.CreditCard,
                //CreditCardInternational = serviceDto.CreditCardInternational,
                //DebitCard = serviceDto.DebitCard,
                //DebitCardInternational = serviceDto.DebitCardInternational,
                //CybersourceTransactionKey = serviceDto.CybersourceTransactionKey,
                MerchantId = serviceDto.MerchantId,
                Gateways = new List<string>(),
                MultipleBillsAllowed = serviceDto.EnableMultipleBills,
                ServiceId = serviceDto.Id.ToString()
            };
            foreach (var gat in serviceDto.ServiceGatewaysDto.Where(gat => gat.Active))
            {
                service.Gateways.Add(((GatewayEnumDto)gat.Gateway.Enum).ToString());
            }
            return service;
        }
    }
}