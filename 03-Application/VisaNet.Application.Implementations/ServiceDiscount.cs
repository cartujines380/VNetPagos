using System;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceDiscount : BaseService<Discount, DiscountDto>, IServiceDiscount
    {
        public ServiceDiscount(IRepositoryDiscount repository)
            : base(repository)
        {
        }

        public override IQueryable<Discount> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override DiscountDto Converter(Discount entity)
        {
            if (entity == null) return null;

            return new DiscountDto
            {
                Id = entity.Id,
                CardType = (CardTypeDto)(int)entity.CardType,
                From = entity.From,
                To = entity.To,
                Fixed = entity.Fixed,
                Additional = entity.Additional,
                MaximumAmount = entity.MaximumAmount,
                DiscountType = (DiscountTypeDto)(int)entity.DiscountType,
                DiscountLabel = (DiscountLabelTypeDto)(int)entity.DiscountLabel
            };
        }

        public override Discount Converter(DiscountDto entity)
        {
            if (entity == null) return null;

            return new Discount
            {
                Id = entity.Id,
                CardType = (CardType)(int)entity.CardType,
                From = entity.From.Date,
                To = entity.To.Date,
                Fixed = entity.Fixed,
                Additional = entity.Additional,
                MaximumAmount = entity.MaximumAmount,
                DiscountType = (DiscountType)(int)entity.DiscountType,
                DiscountLabel = (DiscountLabelType)(int)entity.DiscountLabel
            };
        }

        public DiscountDto GetDiscount(DateTime date, CardTypeDto cardType, DiscountTypeDto discountType)
        {
            if (discountType != DiscountTypeDto.NoDiscount)
            {
                date = date.Date;
                var discount =
                    AllNoTracking(null, d =>
                        d.From <= date
                        && d.To >= date
                        && (int)d.CardType == (int)cardType
                        && (int)d.DiscountType == (int)discountType
                        ).FirstOrDefault();

                return discount;
            }
            else
            {
                var discount = new DiscountDto
                {
                    Additional = 0,
                    Fixed = 0,
                    CardType = cardType,
                    DiscountType = DiscountTypeDto.NoDiscount,
                    From = DateTime.Now,
                    To = DateTime.Now.AddYears(1),
                    MaximumAmount = 0,
                };
                return discount;
            }
        }

        public override void Edit(DiscountDto entity)
        {
            Repository.ContextTrackChanges = true;
            var entity_db = Repository.GetById(entity.Id);

            entity_db.CardType = (CardType)(int)entity.CardType;
            entity_db.From = entity.From.Date;
            entity_db.To = entity.To.Date;
            entity_db.Fixed = entity.Fixed;
            entity_db.Additional = entity.Additional;
            entity_db.MaximumAmount = entity.MaximumAmount;
            entity_db.DiscountType = (DiscountType)(int)entity.DiscountType;
            entity_db.DiscountLabel = (DiscountLabelType)(int)entity.DiscountLabel;

            Repository.Edit(entity_db);
            Repository.Save();

            Repository.ContextTrackChanges = false;
        }

    }
}