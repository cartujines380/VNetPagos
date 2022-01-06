using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.WebPages;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceWebhookNewAssociation : BaseService<WebhookNewAssociation, WebhookNewAssociationDto>, IServiceWebhookNewAssociation
    {
        public ServiceWebhookNewAssociation(IRepositoryWebhookNewAssociation repository)
            : base(repository)
        {
        }

        public override IQueryable<WebhookNewAssociation> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override WebhookNewAssociationDto Converter(WebhookNewAssociation entity)
        {
            if (entity == null) return null;

            var model = new WebhookNewAssociationDto
            {
                Id = entity.Id,
                IdApp = entity.IdApp,
                IdOperation = entity.IdOperation,
                IdOperationApp = entity.IdOperationApp,
                IdCard = entity.IdCard,
                IdUser = entity.IdUser,
                CardMask = entity.CardMask,
                CardDueDate = entity.CardDueDate,
                RefCliente1 = entity.RefCliente1,
                RefCliente2 = entity.RefCliente2,
                RefCliente3 = entity.RefCliente3,
                RefCliente4 = entity.RefCliente4,
                RefCliente5 = entity.RefCliente5,
                RefCliente6 = entity.RefCliente6,
                CardBank = entity.CardBank,
                CardBankCode = entity.CardBankCode,
                CardAffiliation = entity.CardAffiliation,
                CardAffiliationCode = entity.CardAffiliationCode,
                CardType = entity.CardType != null ? (CardTypeDto)((int)entity.CardType) : (CardTypeDto?)null,
                TransactionNumber = entity.TransactionNumber,
                DiscountAmount = entity.DiscountAmount,
                IsAssociation = entity.IsAssociation,
                IsPayment = entity.IsPayment,
                HttpResponseCode = entity.HttpResponseCode,
                CreationDate = entity.CreationDate,
            };

            if (entity.UserData != null)
            {
                model.UserData = new UserDataInputDto()
                {
                    Address = entity.UserData.Address,
                    PhoneNumber = entity.UserData.PhoneNumber,
                    MobileNumber = entity.UserData.MobileNumber,
                    IdentityNumber = entity.UserData.IdentityNumber,
                    Email = entity.UserData.Email,
                    Name = entity.UserData.Name,
                    Surname = entity.UserData.Surname
                };
            }
            return model;
        }

        public override WebhookNewAssociation Converter(WebhookNewAssociationDto entity)
        {
            if (entity == null) return null;

            var model = new WebhookNewAssociation
            {
                Id = entity.Id,
                IdApp = entity.IdApp,
                IdOperation = entity.IdOperation,
                IdOperationApp = entity.IdOperationApp,
                IdCard = entity.IdCard,
                IdUser = entity.IdUser,
                CardMask = entity.CardMask,
                CardDueDate = entity.CardDueDate,
                RefCliente1 = entity.RefCliente1,
                RefCliente2 = entity.RefCliente2,
                RefCliente3 = entity.RefCliente3,
                RefCliente4 = entity.RefCliente4,
                RefCliente5 = entity.RefCliente5,
                RefCliente6 = entity.RefCliente6,
                CardBank = entity.CardBank,
                CardBankCode = entity.CardBankCode,
                CardAffiliation = entity.CardAffiliation,
                CardAffiliationCode = entity.CardAffiliationCode,
                CardType = entity.CardType != null ? (CardType)((int)entity.CardType) : (CardType?)null,
                TransactionNumber = entity.TransactionNumber,
                DiscountAmount = entity.DiscountAmount,
                IsAssociation = entity.IsAssociation,
                IsPayment = entity.IsPayment,
                HttpResponseCode = entity.HttpResponseCode
            };

            model.UserData = new UserDataInput
            {
                Address = entity.UserData == null ? null : entity.UserData.Address,
                PhoneNumber = entity.UserData == null ? null : entity.UserData.PhoneNumber,
                MobileNumber = entity.UserData == null ? null : entity.UserData.MobileNumber,
                IdentityNumber = entity.UserData == null ? null : entity.UserData.IdentityNumber,
                Email = entity.UserData == null ? null : entity.UserData.Email,
                Name = entity.UserData == null ? null : entity.UserData.Name,
                Surname = entity.UserData == null ? null : entity.UserData.Surname
            };
            return model;
        }

        public ICollection<WebhookNewAssociationDto> GetWebhookNewAssociationsForTable(ReportsIntegrationFilterDto filterDto)
        {
            var query = GetWebhookNewAssociationsForTableQuery(filterDto);

            //ordeno, skip y take
            query = filterDto.SortDirection == SortDirection.Desc ? query.OrderByDescending(x => x.CreationDate) : query.OrderBy(x => x.CreationDate);
            query = query.Skip(filterDto.DisplayStart);
            if (filterDto.DisplayLength.HasValue)
                query = query.Take(filterDto.DisplayLength.Value);

            var result = query.Select(Converter).ToList();

            return result;
        }

        public int GetWebhookNewAssociationsForTableCount(ReportsIntegrationFilterDto filterDto)
        {
            var query = GetWebhookNewAssociationsForTableQuery(filterDto);

            return query.Select(t => new WebhookNewAssociationDto
            {
                Id = t.Id
            }).Count();
        }

        private IQueryable<WebhookNewAssociation> GetWebhookNewAssociationsForTableQuery(ReportsIntegrationFilterDto filterDto)
        {
            var query = Repository.AllNoTracking();

            DateTime from = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filterDto.DateFromString))
            {
                from = DateTime.Parse(filterDto.DateFromString, new CultureInfo("es-UY"));
            }

            DateTime to = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filterDto.DateToString))
            {
                to = DateTime.Parse(filterDto.DateToString, new CultureInfo("es-UY"));
            }

            if (!from.Equals(DateTime.MinValue))
            {
                query = query.Where(p => p.CreationDate.CompareTo(from) >= 0);
            }

            if (!to.Equals(DateTime.MinValue))
            {
                var dateTo = to.AddDays(1);
                query = query.Where(p => p.CreationDate.CompareTo(dateTo) <= 0);
            }

            if (!filterDto.IdOperation.IsEmpty())
                query = query.Where(x => x.IdOperationApp.StartsWith(filterDto.IdOperation));

            if (!filterDto.IdApp.IsEmpty())
                query = query.Where(x => x.IdApp.Contains(filterDto.IdApp));

            if (!filterDto.TransactionNumber.IsEmpty())
                query = query.Where(x => x.TransactionNumber.Contains(filterDto.TransactionNumber));

            return query;
        }

        public override void Edit(WebhookNewAssociationDto entity)
        {
            Repository.ContextTrackChanges = true;
            var entityDb = Repository.GetById(entity.Id);
            entityDb.HttpResponseCode = entity.HttpResponseCode;
            Repository.Edit(entityDb);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public bool AlreadyNotifiedExternalCard(string idApp, string idUserExternal, string idCardExternal)
        {
            var registers = All(null, x =>
                    x.IdApp == idApp
                    && x.IdUser == idUserExternal
                    && x.IdCard == idCardExternal
                    && x.HttpResponseCode == "200"
                ).ToList();

            var alreadyNotified = registers.Any();
            return alreadyNotified;
        }

        public IList<WebhookNewAssociationDto> GetUrlTransacctionPosts(WsUrlTransactionQueryDto dto)
        {
            var datefrom = dto.QueryDate.Date;
            var datoTo = dto.QueryDate.AddDays(1).Date;
            var query = Repository.AllNoTracking(x => x.IdApp.Equals(dto.IdApp) &&
                (x.CreationDate > datefrom && x.CreationDate < datoTo));

            return query.Select(entity => new WebhookNewAssociationDto
            {
                Id = entity.Id,
                IdApp = entity.IdApp,
                IdOperation = entity.IdOperation,
                IdOperationApp = entity.IdOperationApp,
                IdCard = entity.IdCard,
                IdUser = entity.IdUser,
                CardMask = entity.CardMask,
                CardDueDate = entity.CardDueDate,
                RefCliente1 = entity.RefCliente1,
                RefCliente2 = entity.RefCliente2,
                RefCliente3 = entity.RefCliente3,
                RefCliente4 = entity.RefCliente4,
                RefCliente5 = entity.RefCliente5,
                RefCliente6 = entity.RefCliente6,
                CardBank = entity.CardBank,
                CardBankCode = entity.CardBankCode,
                CardAffiliation = entity.CardAffiliation,
                CardAffiliationCode = entity.CardAffiliationCode,
                CardType = entity.CardType != null ? (CardTypeDto)((int)entity.CardType) : (CardTypeDto?)null,
                TransactionNumber = entity.TransactionNumber,
                DiscountAmount = entity.DiscountAmount,
                IsAssociation = entity.IsAssociation,
                IsPayment = entity.IsPayment,
                HttpResponseCode = entity.HttpResponseCode,
                CreationDate = entity.CreationDate,
                UserData = new UserDataInputDto()
                {
                    Email = entity.UserData.Email
                }
            }).ToList();
        }

    }
}