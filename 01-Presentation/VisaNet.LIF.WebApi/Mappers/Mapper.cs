using System;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Lif.Domain.EntitesDtos;
using VisaNet.LIF.WebApi.Models;

namespace VisaNet.LIF.WebApi.Mappers
{
    public static class Mapper
    {
        const int DecimalDigits = 2;

        public static BillDto ToDomainObject(this BillModel bill)
        {
            return new BillDto
            {
                Amount = Math.Round(bill.Amount / 100.0, DecimalDigits),
                TaxedAmount = Math.Round(bill.TaxedAmount / 100.0, DecimalDigits),
                IsFinalConsumer = bill.IsFinalConsumer,
                Currency = bill.Currency,
                LawId = bill.LawId
            };
        }        
    }
}