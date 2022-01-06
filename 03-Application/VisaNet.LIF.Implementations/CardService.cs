using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.LIF.Interfaces;
using BinDto = VisaNet.Lif.Domain.EntitesDtos.BinDto;

namespace VisaNet.LIF.Implementations
{
    public class CardService : ICardService
    {
        private readonly IServiceBin _binService;

        public CardService(IServiceBin binService)
        {
            _binService = binService;
        }

        public BinDto GetCardInfo(string bin, bool includeIssuingCompany)
        {
            int value;
            try
            {
                value = int.Parse(bin);
            }
            catch (FormatException)
            {
                throw new BusinessException(CodeExceptions.BIN_VALUE_NOT_RECOGNIZED);
            }
            var model = _binService.Find(value);
            char cardType;
            bool national;
            switch (model.CardType)
            {
                case CardTypeDto.InternationalDebit:
                    cardType = 'D';
                    national = false;
                    break;
                case CardTypeDto.Debit:
                    cardType = 'D';
                    national = true;
                    break;
                case CardTypeDto.Credit:
                    cardType = 'C';
                    national = true;
                    break;
                case CardTypeDto.InternationalCredit:
                    cardType = 'C';
                    national = false;
                    break;
                case CardTypeDto.NationalPrepaid:
                    cardType = 'P';
                    national = true;
                    break;
                case CardTypeDto.InternationalPrepaid:
                    cardType = 'P';
                    national = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("CardType");
            }
            return new BinDto
            {
                CardType = cardType,
                National = national,
                IssuingCompany = (model.BankDto == null || !includeIssuingCompany) ? "" : model.BankDto.Name,
                Installments = model.BankDto != null ? model.BankDto.QuotesPermited != null ? model.BankDto.QuotesPermited.Split(',').Select(int.Parse).ToArray() : new[] { 1 } : new[] { 1 }
            };
        }

        public ICollection<BinDto> GetNationalBins(bool includeIssuingCompany)
        {
            var binsDtos = _binService.AllNoTracking(x => new VisaNet.Domain.EntitiesDtos.BinDto()
            {
                Value = x.Value,
                CardType = (CardTypeDto)x.CardType,
                BankDto = x.Bank != null ? new BankDto { Name = x.Bank.Name, QuotesPermited = x.Bank.QuotesPermited } : null
            }, x => x.CardType == CardType.Credit || x.CardType == CardType.Debit || x.CardType == CardType.NationalPrepaid);

            var bins = binsDtos.Select(x => new BinDto()
            {
                Value = x.Value.ToString().PadLeft(6, '0'),
                CardType = (x.CardType == CardTypeDto.Credit) ? 'C' : (x.CardType == CardTypeDto.Debit) ? 'D' : 'P',
                IssuingCompany = (x.BankDto == null || !includeIssuingCompany) ? "" : x.BankDto.Name,
                National = true,
                Installments = x.BankDto != null ? x.BankDto.QuotesPermited != null ? x.BankDto.QuotesPermited.Split(',').Select(int.Parse).ToArray() : new[] { 1 } : new[] { 1 }
            });

            return bins.ToList();
        }
    }
}
