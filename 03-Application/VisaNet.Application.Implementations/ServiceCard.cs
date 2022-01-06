using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.ApiClient;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security;
using VisaNet.Components.CyberSource.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ReportsModel;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Application.Implementations
{
    public class ServiceCard : BaseService<Card, CardDto>, IServiceCard
    {
        private readonly IServiceApplicationUser _serviceApplicationUser;
        private readonly IServiceServiceAssosiate _serviceServiceAssosiate;
        private readonly IRepositoryPayment _repositoryPayment;
        private readonly IRepositoryCard _repositoryCard;
        private readonly IRepositoryService _serviceRepository;
        private readonly ITransactionContext _transactionContext;
        private readonly IRepositoryDebitRequest _repositoryDebitRequest;
        private readonly IRepositoryParameters _repositoryParameters;
        private readonly IServiceNotification _serviceNotification;
        private readonly IServiceEmailMessage _serviceNotificationMessage;
        private readonly ICyberSourceAccess _cyberSourceAccess;

        public ServiceCard(IRepositoryCard repository, IServiceApplicationUser serviceApplicationUser,
            IServiceServiceAssosiate serviceServiceAssosiate, IRepositoryPayment repositoryPayment,
            IRepositoryService serviceRepository, ITransactionContext transactionContext,
            IRepositoryDebitRequest repositoryDebitRequest,
            IServiceNotification serviceNotification, IRepositoryParameters repositoryParameters,
            IServiceEmailMessage serviceNotificationMessage, ICyberSourceAccess cyberSourceAccess)
            : base(repository)
        {
            _repositoryCard = repository;
            _serviceApplicationUser = serviceApplicationUser;
            _serviceServiceAssosiate = serviceServiceAssosiate;
            _repositoryPayment = repositoryPayment;
            _serviceRepository = serviceRepository;
            _transactionContext = transactionContext;
            _repositoryDebitRequest = repositoryDebitRequest;
            _serviceNotificationMessage = serviceNotificationMessage;
            _cyberSourceAccess = cyberSourceAccess;
        }

        public override IQueryable<Card> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override CardDto Converter(Card entity)
        {
            if (entity == null) return null;

            var cardDto = new CardDto
            {
                Id = entity.Id,
                MaskedNumber = entity.MaskedNumber,
                PaymentToken = entity.PaymentToken,
                DueDate = entity.DueDate,
                CybersourceTransactionId = entity.CybersourceTransactionId,
                //ApplicationUser_Id = entity.ApplicationUser_Id,
                Active = entity.Active,
                Name = entity.Name,
                ExternalId = entity.ExternalId,
                Description = entity.Description,
                Deleted = entity.Deleted,
            };
            if (entity.ServicesAssociated != null && entity.ServicesAssociated.Any())
            {
                cardDto.ServicesAssociatedDto = entity.ServicesAssociated.Select(s => new ServiceAssociatedDto()
                {
                    Active = s.Active,
                    Id = s.Id,
                    ServiceId = s.ServiceId,
                    DefaultCardId = s.DefaultCardId
                }).ToList();
            }
            return cardDto;
        }

        public override Card Converter(CardDto entity)
        {
            if (entity == null) return null;

            var card = new Card
            {
                Id = entity.Id,
                MaskedNumber = entity.MaskedNumber,
                PaymentToken = entity.PaymentToken,
                DueDate = entity.DueDate,
                CybersourceTransactionId = entity.CybersourceTransactionId,
                //ApplicationUser_Id = entity.ApplicationUser_Id,
                Active = entity.Active,
                Name = entity.Name,
                ExternalId = entity.ExternalId,
                Description = entity.Description,
                Deleted = entity.Deleted,
            };

            if (entity.ServicesAssociatedDto != null && entity.ServicesAssociatedDto.Any())
            {
                card.ServicesAssociated = entity.ServicesAssociatedDto.Select(s => new ServiceAssociated()
                {
                    Active = s.Active,
                    Id = s.Id,
                    ServiceId = s.ServiceId,
                    DefaultCardId = s.DefaultCardId
                }).ToList();
            }
            return card;
        }

        public IEnumerable<CardDto> GetDataForTable(CardFilterDto filters)
        {
            var user = _serviceApplicationUser.GetById(filters.UserId, s => s.Cards);


            IEnumerable<CardDto> cards = user.CardDtos;
            if (user.CardDtos == null) return new Collection<CardDto>();

            cards = cards.Where(c => !c.Deleted);

            if (!string.IsNullOrEmpty(filters.GenericSearch))
            {
                cards =
                    cards.Where(
                        c =>
                            c.MaskedNumber.ToLower().Contains(filters.GenericSearch.ToLower()));
            }
            if (cards != null && !string.IsNullOrEmpty(filters.MaskedNumber))
            {
                cards = cards.Where(sc => sc.MaskedNumber.Contains(filters.MaskedNumber));
            }
            if (cards != null && !String.IsNullOrEmpty(filters.DueDateMonth))
            {
                cards = cards.Where(sc => sc.DueDate.Month == Int32.Parse(filters.DueDateMonth));
            }
            if (cards != null && !String.IsNullOrEmpty(filters.DueDateYear))
            {
                cards = cards.Where(sc => sc.DueDate.Year == Int32.Parse(filters.DueDateYear));
            }
            if (filters.Active.HasValue)
            {
                cards = cards.Where(sc => sc.Active == filters.Active);
            }

            if (filters.DisplayLength == null)
                return cards.Skip(filters.DisplayStart).ToList();

            return cards.Skip(filters.DisplayStart)
                        .Take(filters.DisplayLength.Value)
                        .ToList();
        }

        public CardDto GetByToken(string token)
        {
            var card = Repository.AllNoTracking(c => c.PaymentToken.Equals(token)).First();
            return Converter(card);
        }

        public void ActivateCard(CardOperationDto cardOperationDto)
        {
            var user = UserIsOwnerOfCard(cardOperationDto);
            ChangeState(cardOperationDto.CardId, true, cardOperationDto.UserId);
        }

        public void DesactivateCard(CardOperationDto cardOperationDto)
        {
            UserIsOwnerOfCard(cardOperationDto);

            //Controlo que no sea la tarjeta por defecto de algún ServiceAssociated
            var isDefaultForServiceAssociated =
                _serviceServiceAssosiate.AllNoTracking(null, s => s.DefaultCardId == cardOperationDto.CardId && s.Active, s => s.Cards, s => s.Service).Any();

            if (isDefaultForServiceAssociated)
            {
                var card = GetById(cardOperationDto.CardId, x => x.ServicesAssociated);
                if (card.ServicesAssociatedDto != null && card.ServicesAssociatedDto.Any())
                {
                    foreach (var sa in card.ServicesAssociatedDto)
                    {
                        if (sa.DefaultCardId == cardOperationDto.CardId)
                        {
                            var s = _serviceServiceAssosiate.GetById(sa.Id, x => x.Cards, x => x.Service);
                            // no debo dejar que elimine si no cambio el default
                            if (s.CardDtos.Count > 1)
                            {
                                //cambiar default
                                var other = s.CardDtos.FirstOrDefault(x => x.Id != cardOperationDto.CardId);
                                _serviceServiceAssosiate.AddCardToService(s.Id, other.Id, cardOperationDto.CardId, cardOperationDto.UserId, string.Empty);
                            }
                            else
                            {
                                throw new BusinessException(CodeExceptions.SERVICE_CARD_IS_USED_MIGRATE_SERVICES);
                            }
                        }
                    }
                }
                throw new BusinessException(CodeExceptions.SERVICE_CARD_IS_DEFAULT_MIGRATE_SERVICES);
            }

            //Controlo que no sea la tarjeta por defecto de algún DebitRequest
            var isDefaultForDebit =
                _repositoryDebitRequest.AllNoTracking(x =>
                    x.CardId == cardOperationDto.CardId &&
                    (x.State == DebitRequestState.Accepted
                     || x.State == DebitRequestState.Synchronized
                     || x.State == DebitRequestState.ManualSynchronization
                     || x.State == DebitRequestState.Pending
                     || x.State == DebitRequestState.PendingCancellation
                     || x.State == DebitRequestState.Synchronized
                    )).Any();

            if (isDefaultForDebit)
            {
                throw new BusinessException(CodeExceptions.SERVICE_CARD_IS_USED_MIGRATE_DEBITS);
            }

            ChangeState(cardOperationDto.CardId, false, cardOperationDto.UserId);
        }

        public void EliminateCard(CardOperationDto cardOperationDto, bool notifymail = true)
        {
            var card = Repository.GetById(cardOperationDto.CardId);
            var user = _serviceApplicationUser.GetById(cardOperationDto.UserId, s => s.Cards);

            var hasPayments = false;
            var hasServiceAssociated = false;
            var hasDebitRequest = false;
            var hasOtherActiveCard = false;

            //Controlo si tiene algún Payment que lo referencie
            hasPayments = _repositoryPayment.AllNoTracking(s => s.CardId == cardOperationDto.CardId).Any();

            if (!hasPayments)
            {
                //Controlo si tiene algún ServiceAssociated que lo referencie
                hasServiceAssociated = _serviceServiceAssosiate.AllNoTracking(null, x => x.DefaultCardId == cardOperationDto.CardId).Any();

                if (!hasServiceAssociated)
                {
                    //Controlo si tiene algún DebitRequest que lo referencie
                    hasDebitRequest = _repositoryDebitRequest.AllNoTracking(x => x.CardId == cardOperationDto.CardId).Any();
                }
            }

            //elimino de cybersouce
            var delete = new DeleteCardDto
            {
                UserId = cardOperationDto.UserId,
                RequestId = card.Id.ToString(),
                Token = card.PaymentToken,
                IdTransaccion = card.CybersourceTransactionId.ToString()
            };

            

            var csOperationData = _cyberSourceAccess.DeleteCard(delete);

            if (csOperationData.DeleteData.DeleteResponseCode == (int)CybersourceMsg.Accepted)
            {
                Repository.ContextTrackChanges = true;

                if (hasPayments || hasServiceAssociated || hasDebitRequest)
                {
                    //Borrado lógico
                    card.Deleted = true;
                    card.DeletedFromCs = true;
                }
                else
                {
                    try
                    {
                        Repository.Delete(card);
                    }
                    catch (Exception ex)
                    {
                        this.CardDeletedFromCS(card.Id, true);                          
                    }
                }
                Repository.Save();

                //verifico si hay otra tarjeta activa
                hasOtherActiveCard = user.CardDtos.Any(c => !c.Deleted && c.Id != cardOperationDto.CardId && c.Active);
                //enviar mail de motificacion 
                if (notifymail && !hasServiceAssociated && !hasOtherActiveCard)
                {
                    SendUserNotificationMail(cardOperationDto, card, user);
                }

                Repository.ContextTrackChanges = false;
            }
            else
                throw new Exception(PresentationAdminStrings.Reports_Cards_DeleteCs_Fail);

            
        }

        public void SendUserNotificationMail(CardOperationDto cardOperationDto, Card card, ApplicationUserDto user)
        {
            _serviceNotificationMessage.SendCustomerEliminateCard(

                new DeleteCardRequestDto
                {
                    ApplicationUserDto = new ApplicationUserDto
                    {
                        Id = cardOperationDto.UserId,
                        Email = user.Email
                    }
                     ,
                    Type = "High",
                    Status = "OK",
                    References = new Dictionary<string, string>(),
                    ProductName = "",
                    ServiceName = "",
                    MaskedNumber = card.MaskedNumber,
                    CardDto = new CardDto
                    {
                        Id = cardOperationDto.CardId,
                        Name = card.Name,
                        MaskedNumber = card.MaskedNumber,
                        DueDate = card.DueDate,
                        Description = card.Description
                    }
                });
        }

        public void MigrateServices(Guid userId, Guid oldCardId, Guid newCardId)
        {
            var cardOperationDto = new CardOperationDto
            {
                CardId = oldCardId,
                UserId = userId
            };
            UserIsOwnerOfCard(cardOperationDto);

            var newCard = Repository.GetById(newCardId);
            var servicesAssociated = _serviceServiceAssosiate.AllNoTracking(null, x => x.DefaultCardId == oldCardId, x => x.Service);

            //Esto para los servicios que tiene tarjeta por default
            foreach (var service in servicesAssociated)
            {
                //Solo si el servicio permite asociar el BIN
                if (_serviceRepository.IsBinAssociatedToService(newCard.BIN, service.ServiceId))
                {
                    _serviceServiceAssosiate.AddCardToService(service.Id, newCardId, oldCardId, userId, string.Empty);
                }
            }

            //servicios apps
            var services = _serviceServiceAssosiate.All(null, x => x.Cards.Any(y => y.Id == oldCardId), x => x.Cards, x => x.Service);
            foreach (var sa in services)
            {
                var delete = false;
                if (!sa.CardDtos.Any(x => x.Id == newCardId))
                {
                    //Solo si el servicio permite asociar el BIN
                    if (_serviceRepository.IsBinAssociatedToService(newCard.BIN, sa.ServiceId))
                    {
                        delete = _serviceServiceAssosiate.AddCardToService(sa.Id, newCardId, oldCardId, userId, string.Empty);
                    }
                }
                else
                {
                    delete = true;
                }
                if (delete)
                    _serviceServiceAssosiate.DeleteCardFromService(sa.Id, oldCardId, userId);
            }
        }

        private void ChangeState(Guid id, bool state, Guid userId)
        {
            var result = _serviceServiceAssosiate.DeleteCardFromService(Guid.Empty, id, userId);

            if (!result)
                return;

            Repository.ContextTrackChanges = true;
            var card = Repository.GetById(id, x => x.ServicesAssociated);
            card.Active = state;

            Repository.Edit(card);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public CardDto GenerateExternalId(Guid id)
        {
            var card = _repositoryCard.GenerateExternalId(id);
            return card != null ? Converter(card) : null;
        }

        private ApplicationUserDto UserIsOwnerOfCard(CardOperationDto cardOperationDto)
        {
            var user = _serviceApplicationUser.AllNoTracking(null,
               x => x.Id == cardOperationDto.UserId && x.Cards.Any(y => y.Id == cardOperationDto.CardId), x => x.Cards).FirstOrDefault();

            if (user == null)
                throw new BusinessException(CodeExceptions.USER_CARD_NOT_MATCH);

            return user;
        }

        public CardMigrationTestDto TestMigration(Guid oldCardId, Guid newCardId, Guid applicationUserId)
        {
            var servicesAssociated = _serviceServiceAssosiate.AllNoTracking(null, x => x.DefaultCardId == oldCardId && x.Active, x => x.Service);
            var newCard = Repository.GetById(newCardId);

            return new CardMigrationTestDto
            {
                FailedServices = servicesAssociated.Where(associatedService => !_serviceRepository.IsBinAssociatedToService(newCard.BIN, associatedService.ServiceId)).ToList(),
                SuccessfulServices = servicesAssociated.Where(associatedService => _serviceRepository.IsBinAssociatedToService(newCard.BIN, associatedService.ServiceId)).ToList()
            };
        }

        public IEnumerable<ServiceAssociatedDto> GetAssociatedServices(Guid cardId)
        {
            var servicesAssociated = _serviceServiceAssosiate.AllNoTracking(null, x => x.DefaultCardId == cardId && x.Active, x => x.Service);
            return servicesAssociated;
        }

        public IEnumerable<ReportCardsViewDto> ReportsCardsData(ReportsCardsFilterDto filters)
        {
            const string query = "SELECT * FROM [dbo].[ReportCardsView] ";
            //string query = "SELECT rep.*, ";
            //query += "LastPaymentDate = (SELECT MAX(p.date) FROM [dbo].[Payments] p WHERE CardId = rep.CardId ),";
            //query += "NumServicesAsociated = (SELECT COUNT(1) FROM [dbo].[ServicesAssociated] WHERE [DefaultCardId] = rep.CardId) ";
            //query += "FROM [dbo].[ReportCardsView] rep ";
            var where = "WHERE 1=1 ";

            if (!string.IsNullOrEmpty(filters.ClientEmail))
                where += "AND (Email LIKE '%" + filters.ClientEmail + "%') ";

            if (!string.IsNullOrEmpty(filters.ClientName))
                where += "AND (Name LIKE '%" + filters.ClientName + "%') ";

            if (!string.IsNullOrEmpty(filters.ClientSurname))
                where += "AND (Surname LIKE '%" + filters.ClientSurname + "%') ";

            if (!string.IsNullOrEmpty(filters.CardMaskedNumber))
                where += "AND (CardMaskedNumber LIKE '%" + filters.CardMaskedNumber + "%') ";

            if (!string.IsNullOrEmpty(filters.CardBin))
                where += "AND (BinValue LIKE '" + filters.CardBin + "%') ";

            if (filters.CardState == 1)
                where += "AND (CardActive = '1') ";

            if (filters.CardState == 2)
                where += "AND (CardActive = '0') ";

            if (filters.CardType != 0)
                where += "AND (CardType = '" + filters.CardType + "') ";

            #region Sort
            var orderBy = "";
            switch (filters.OrderBy)
            {
                case "0":
                    orderBy = "ORDER BY Email ";
                    break;
                case "1":
                    orderBy = "ORDER BY Name ";
                    break;
                case "2":
                    orderBy = "ORDER BY Surname ";
                    break;
                case "3":
                    orderBy = "ORDER BY CardMaskedNumber ";
                    break;
                case "4":
                    orderBy = "ORDER BY CardDueDate ";
                    break;
                case "5":
                    orderBy = "ORDER BY BinValue ";
                    break;
                case "6":
                    orderBy = "ORDER BY CardType ";
                    break;
                case "7":
                    orderBy = "ORDER BY CardActive ";
                    break;
                case "8":
                    orderBy = "ORDER BY CardDeleted ";
                    break;
                default:
                    orderBy = "ORDER BY Email ";
                    break;
            }

            if (filters.SortDirection == SortDirection.Desc)
                orderBy += "DESC";
            #endregion

            using (var context = new AppContext())
            {
                if (filters.DisplayLength != null)
                {
                    //try
                    //{

                    //}
                    //catch (Exception e)
                    //{

                    //    throw;
                    //}

                    var result = context.Database.SqlQuery<ReportCardsViewDto>(string.Format("WITH aux AS ( SELECT *, ROW_NUMBER() OVER ({1}) AS RowNumber FROM [dbo].[ReportCardsView] {0} ) SELECT * FROM aux WHERE RowNumber BETWEEN {2} AND {3}", where, orderBy, filters.DisplayStart + 1, filters.DisplayStart + (int)filters.DisplayLength)).ToList();
                    return result;

                }
                return context.Database.SqlQuery<ReportCardsViewDto>(query + where + orderBy).ToList();
            }
        }

        public int ReportsCardsDataCount(ReportsCardsFilterDto filters)
        {
            var query = "SELECT COUNT(0) FROM [dbo].[ReportCardsView] WHERE 1=1 ";

            if (!string.IsNullOrEmpty(filters.ClientEmail))
                query += "AND (Email LIKE '%" + filters.ClientEmail + "%') ";

            if (!string.IsNullOrEmpty(filters.ClientName))
                query += "AND (Name LIKE '%" + filters.ClientName + "%') ";

            if (!string.IsNullOrEmpty(filters.ClientSurname))
                query += "AND (Surname LIKE '%" + filters.ClientSurname + "%') ";

            if (!string.IsNullOrEmpty(filters.CardMaskedNumber))
                query += "AND (CardMaskedNumber LIKE '%" + filters.CardMaskedNumber + "%') ";

            if (!string.IsNullOrEmpty(filters.CardBin))
                query += "AND (BinValue LIKE '" + filters.CardBin + "%') ";

            if (filters.CardState == 1)
                query += "AND (CardActive = '1') ";

            if (filters.CardState == 2)
                query += "AND (CardActive = '0') ";

            if (filters.CardType != 0)
                query += "AND (CardType = '" + filters.CardType + "') ";

            using (var context = new AppContext())
            {
                return context.Database.SqlQuery<int>(query).First();
            }
        }

        public string GetQuotasForBinAndService(int cardBin, Guid serviceId)
        {
            var quotas = LIFApiClient<string>.GetQuotasForBinAndService(Mapper, cardBin, serviceId, _transactionContext);
            return quotas;
        }

        public string GetQuotasForBin(int cardBin)
        {
            var quotas = LIFApiClient<string>.GetQuotasForBin(Mapper, cardBin, _transactionContext);
            return quotas;
        }

        private string Mapper(string json)
        {
            var content = JsonConvert.DeserializeObject<dynamic>(json);
            return content;
        }

        private void DeleteCards(string sqlQuery, bool notifyMail)
        {

            using (var context = new AppContext())
            {
                var result = context.Database.SqlQuery<CardDeleteCSDto>(sqlQuery).ToList();

                foreach (var card in result)
                {
                    EliminateCard(new CardOperationDto()
                    {
                        CardId = card.Id,
                        UserId = card.ApplicationUser_Id != null ? card.ApplicationUser_Id.Value : card.AnonymousUserId.Value
                    }, notifyMail);

                }

            }
        }

        public void EditCardDescription(Guid id, string description)
        {
            var card = _repositoryCard.GetById(id);

            if (card != null)
            {
                card.Description = description;
                _repositoryCard.Edit(card);
                _repositoryCard.Save();
            }
        }

        public void CardDeletedFromCS(Guid id, bool del)
        {
            var card = _repositoryCard.GetById(id);

            if (card != null)
            {
                card.DeletedFromCs = del;
                _repositoryCard.Edit(card);
                _repositoryCard.Save();
            }
        }

        public void DeleteOldCardsToken()
        {
            //cards inactivas
            var sqlQuery = "dbo.StoreProcedure_GetInactiveCards";

            DeleteCards(sqlQuery, false);

            //cards sin payments y vonData sin payments
            sqlQuery = "dbo.StoreProcedure_GetCardsWithoutPaymentsLastYear";
            DeleteCards(sqlQuery, true);
        }
    }
}