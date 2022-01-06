using System.Linq;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Mappers
{
    public static class BillMapper
    {
        public static BillDto ToDto(this BillModel entity)
        {
            return new BillDto
            {
                Id = entity.Id,
                ExpirationDate = entity.ExpirationDate,
                Amount = entity.Amount,
                Currency = entity.Currency,
                Description = entity.Description,
                BillExternalId = entity.BillExternalId,
                FinalConsumer = entity.FinalConsumer,
                Discount = entity.Discount,
                TaxedAmount = entity.TaxedAmount,
                Payable = entity.Payable,
                Gateway = entity.Gateway,
                Line = entity.Line,
                GeneratedDate = entity.GeneratedDate,
                DashboardDescription = entity.DashboardDescription
            };
        }

        public static BillModel ToModel(this BillDto entity)
        {
            var amount = entity.Bills != null && entity.Bills.Any() ? entity.Bills.Sum(x => x.Amount) : 0;
            var model = new BillModel
            {
                Id = entity.Id,
                ExpirationDate = entity.ExpirationDate,
                Amount = entity.Amount + amount,
                Currency = entity.Currency,
                Description = entity.Description,
                BillExternalId = entity.BillExternalId,
                FinalConsumer = entity.FinalConsumer,
                Discount = entity.Discount,
                TaxedAmount = entity.TaxedAmount,
                Payable = entity.Payable,
                Gateway = entity.Gateway,
                Line = entity.Line,
                GeneratedDate = entity.GeneratedDate,
                HasAnnualPatent = entity.HasAnnualPatent,
                DashboardDescription = entity.DashboardDescription
            };
            if (entity.Bills != null && entity.Bills.Any())
            {
                model.Bills = entity.Bills.Select(x => new BillModel()
                {
                    Amount = x.Amount,
                    Id = x.Id,
                    Currency = x.Currency,
                    ExpirationDate = x.ExpirationDate
                }).ToList();
            }
            return model;
        }

    }
}