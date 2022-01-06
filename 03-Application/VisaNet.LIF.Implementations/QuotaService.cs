using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.LIF.Interfaces;

namespace VisaNet.LIF.Implementations
{
    public class QuotaService : IQuotaService
    {
        private readonly IServiceBin _binService;
        private readonly IServiceService _serviceService;

        public QuotaService(IServiceBin binService, IServiceService serviceService)
        {
            _binService = binService;
            _serviceService = serviceService;
        }

        public string GetQuotasForBin(int cardBin)
        {
            return GetQuotas(cardBin);
        }

        public string GetQuotasForBinAndService(int cardBin, Guid serviceId)
        {
            var service = _serviceService.GetById(serviceId);
            var quotas = GetQuotas(cardBin, service.MaxQuotaAllow);
            return quotas;
        }

        private string GetQuotas(int cardBin, int? maxQuotasInService = null)
        {
            var quotas = "1";

            //Se controla los tipos de tarjeta que aceptan quotas (definidos en web.config de API-LIF)
            var strCardTypesForQuotas = ConfigurationManager.AppSettings["CardTypesForQuotas"];
            var cardTypesForQuotas = new List<CardTypeDto>();
            if (string.IsNullOrEmpty(strCardTypesForQuotas))
            {
                cardTypesForQuotas.Add(CardTypeDto.Credit);
            }
            else
            {
                var split = strCardTypesForQuotas.Split(',');
                foreach (var s in split)
                {
                    CardTypeDto value;
                    if (Enum.TryParse(s, true, out value))
                    {
                        cardTypesForQuotas.Add(value);
                    }
                }
            }

            var bin = _binService.Find(cardBin);

            //Se controla que el tipo de tarjeta y el banco emisor permitan cuotas
            if (bin != null && cardTypesForQuotas.Contains(bin.CardType) && bin.BankDto != null)
            {
                if (!string.IsNullOrEmpty(bin.BankDto.QuotesPermited) && bin.BankDto.QuotesPermited != "1")
                {
                    //devolver el maximo de cuotas entre el banco y el servicio
                    if (!maxQuotasInService.HasValue)
                    {
                        quotas = bin.BankDto.QuotesPermited;
                    }
                    else
                    {
                        var bankQuotas = bin.BankDto.QuotesPermited.Split(',').Select(int.Parse).ToList();
                        var newList = bankQuotas.Where(bankQuota => bankQuota <= maxQuotasInService.Value);
                        quotas = string.Join(",", newList);
                    }
                }
            }
            return quotas;
        }

    }
}