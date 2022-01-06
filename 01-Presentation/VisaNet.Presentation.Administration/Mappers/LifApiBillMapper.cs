using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class LifApiBillMapper
    {
        public static LifApiBillModel ToModel(this LifApiBillDto entity)
        {
            var model = new LifApiBillModel
            {
                Id = entity.Id,
                AppId = entity.AppId,
                OperationId = entity.OperationId,
                Currency = entity.Currency,
                Amount = entity.Amount,
                TaxedAmount = entity.TaxedAmount,
                IsFinalConsumer = entity.IsFinalConsumer,
                LawId = entity.LawId,
                BinValue = entity.BinValue,
                CardType = entity.CardType,
                IssuingCompany = entity.IssuingCompany,
                DiscountAmount = entity.DiscountAmount,
                AmountToCyberSource = entity.AmountToCyberSource,
                CreationDate = entity.CreationDate,
                Error = entity.Error
            };
            return model;
        }
    }
}