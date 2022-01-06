using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.WebPages;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceWebhookRegistration : BaseService<WebhookRegistration, WebhookRegistrationDto>, IServiceWebhookRegistration
    {
        private readonly IRepositoryService _repositoryService;

        public ServiceWebhookRegistration(IRepositoryWebhookRegistration repository, IRepositoryService repositoryService)
            : base(repository)
        {
            _repositoryService = repositoryService;
        }

        public override IQueryable<WebhookRegistration> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override WebhookRegistrationDto Converter(WebhookRegistration entity)
        {
            var dto = new WebhookRegistrationDto()
            {
                EnableEmailChange = entity.EnableEmailChange,
                IdOperation = entity.IdOperation,
                UrlCallback = entity.UrlCallback,
                IdApp = entity.IdApp,
                CreationDate = entity.CreationDate,
                IdUsuario = entity.IdUsuario,
                Id = entity.Id,
                MerchantId = entity.MerchantId,
                Action = (WebhookRegistrationActionEnumDto)entity.Action,
                ReferenceNumber = entity.ReferenceNumber,
                ReferenceNumber2 = entity.ReferenceNumber2,
                ReferenceNumber3 = entity.ReferenceNumber3,
                ReferenceNumber4 = entity.ReferenceNumber4,
                ReferenceNumber5 = entity.ReferenceNumber5,
                ReferenceNumber6 = entity.ReferenceNumber6,
                PaymentId = entity.PaymentId,
                SendEmail = entity.SendEmail,
                EnableRememberUser = entity.EnableRememberUser,
            };
            if (entity.Payment != null)
            {
                dto.PaymentDto = new PaymentDto()
                {
                    Id = entity.PaymentId.Value,
                    TransactionNumber = entity.Payment.TransactionNumber,
                    Date = entity.Payment.Date
                };
            }
            if (entity.UserData != null)
            {
                dto.UserData = new UserDataInputDto()
                {
                    Address = entity.UserData.Address,
                    PhoneNumber = entity.UserData.PhoneNumber,
                    MobileNumber = entity.UserData.MobileNumber,
                    IdentityNumber = entity.UserData.IdentityNumber,
                    Email = entity.UserData.Email,
                    Name = entity.UserData.Name,
                    Surname = entity.UserData.Surname,
                };
            }
            if (entity.Bill != null)
            {
                dto.Bill = new BillDataInputDto()
                {
                    Amount = entity.Bill.Amount,
                    Currency = entity.Bill.Currency,
                    GenerationDate = entity.Bill.GenerationDate,
                    Description = entity.Bill.Description,
                    FinalConsumer = entity.Bill.FinalConsumer,
                    ExternalId = entity.Bill.ExternalId,
                    Quota = entity.Bill.Quota,
                    TaxedAmount = entity.Bill.TaxedAmount,
                    ExpirationDate = entity.Bill.ExpirationDate,
                };
            }
            else
            {
                dto.Bill = new BillDataInputDto();
            }

            if (entity.BillLines != null && entity.BillLines.Any())
            {
                dto.BillLines = entity.BillLines.Select(x => new WebhookRegistrationLineDto()
                {
                    Amount = x.Amount,
                    Concept = x.Concept,
                    Order = x.Order
                }).ToList();
            }
            if (entity.WebhookAccessToken != null)
            {
                dto.WebhookAccessTokenDto = new WebhookAccessTokenDto()
                {
                    AccessToken = entity.WebhookAccessToken.AccessToken,
                    CreationDate = entity.WebhookAccessToken.CreationDate,
                    StateDto = (WebhookAccessStateDto)(int)entity.WebhookAccessToken.State,
                    Id = entity.WebhookAccessToken.Id,
                };
            }

            return dto;
        }

        public override WebhookRegistration Converter(WebhookRegistrationDto entity)
        {
            var model = new WebhookRegistration()
            {
                EnableEmailChange = entity.EnableEmailChange,
                IdOperation = entity.IdOperation,
                UrlCallback = entity.UrlCallback,
                IdApp = entity.IdApp,
                CreationDate = entity.CreationDate,
                IdUsuario = entity.IdUsuario,
                Id = entity.Id,
                MerchantId = entity.MerchantId,
                Action = (WebhookRegistrationActionEnum)entity.Action,
                ReferenceNumber = entity.ReferenceNumber,
                ReferenceNumber2 = entity.ReferenceNumber2,
                ReferenceNumber3 = entity.ReferenceNumber3,
                ReferenceNumber4 = entity.ReferenceNumber4,
                ReferenceNumber5 = entity.ReferenceNumber5,
                ReferenceNumber6 = entity.ReferenceNumber6,
                PaymentId = entity.PaymentId,
                SendEmail = entity.SendEmail.HasValue ? entity.SendEmail.Value : true,
                EnableRememberUser = entity.EnableRememberUser,
            };
            if (entity.UserData != null)
            {
                model.UserData = new UserDataInput()
                {
                    Address = entity.UserData.Address,
                    PhoneNumber = entity.UserData.PhoneNumber,
                    MobileNumber = entity.UserData.MobileNumber,
                    IdentityNumber = entity.UserData.IdentityNumber,
                    Email = entity.UserData.Email,
                    Name = entity.UserData.Name,
                    Surname = entity.UserData.Surname,
                };
            }
            else
            {
                model.UserData = new UserDataInput();
            }
            if (entity.Bill != null)
            {
                model.Bill = new BillDataInput()
                {
                    Amount = entity.Bill.Amount,
                    Currency = entity.Bill.Currency,
                    GenerationDate = entity.Bill.GenerationDate,
                    Description = entity.Bill.Description,
                    FinalConsumer = entity.Bill.FinalConsumer,
                    ExternalId = entity.Bill.ExternalId,
                    Quota = entity.Bill.Quota,
                    TaxedAmount = entity.Bill.TaxedAmount,
                    ExpirationDate = entity.Bill.ExpirationDate,
                };
            }
            else
            {
                model.Bill = new BillDataInput();
            }

            if (entity.BillLines != null && entity.BillLines.Any())
            {
                model.BillLines = entity.BillLines.Select(x => new WebhookRegistrationLine()
                {
                    Amount = x.Amount,
                    Concept = x.Concept,
                    Order = x.Order,
                    Id = Guid.NewGuid()
                }).ToList();
            }
            if (entity.WebhookAccessTokenDto != null)
            {
                model.WebhookAccessToken = new WebhookAccessToken()
                {
                    AccessToken = entity.WebhookAccessTokenDto.AccessToken,
                    CreationDate = entity.WebhookAccessTokenDto.CreationDate,
                };
            }
            return model;
        }

        public override WebhookRegistrationDto Create(WebhookRegistrationDto entity, bool returnEntity = false)
        {
            Repository.ContextTrackChanges = true;
            var efEntity = Converter(entity);

            try
            {
                efEntity.GenerateNewIdentity();
                Repository.Create(efEntity);
                Repository.Save();
            }
            catch (DbUpdateException exception)
            {
                Repository.ContextTrackChanges = false;
                NLogLogger.LogEvent(exception);
                if (exception.Message.Contains("IX_IdApp_IdOperation") ||
                    (exception.InnerException != null && exception.InnerException.Message.Contains("IX_IdApp_IdOperation")) ||
                    (exception.InnerException.InnerException != null && exception.InnerException.InnerException.Message.Contains("IX_IdApp_IdOperation")))
                {
                    //IDOPERACION REPETIDO
                    throw new BusinessException(CodeExceptions.OPERATION_ID_REPETED);
                }
                //TODO subo bussinesexception ?
            }

            Repository.ContextTrackChanges = false;
            return returnEntity ? GetById(efEntity.Id) : null;
        }

        public WebhookRegistrationDto GetByIdOperation(string idOperation, string idapp)
        {
            idapp = idapp.ToUpper();

            var query = Repository.AllNoTracking();
            query = query.Where(x => x.IdOperation == idOperation && x.IdApp.ToUpper().Equals(idapp));

            var entity = query.FirstOrDefault();
            if (entity != null)
            {
                return Converter(entity);
            }
            return null;
        }

        public WebhookRegistrationDto GetByIdOperation(string idOperation, Guid serviceId)
        {
            var service = _repositoryService.GetById(serviceId, x => x.ServiceContainer);

            var idapp = service.ServiceContainer != null && !string.IsNullOrEmpty(service.ServiceContainer.UrlName) ?
                service.ServiceContainer.UrlName :
                service.UrlName;

            idapp = idapp.ToUpper();

            var query = Repository.AllNoTracking(null, x => x.WebhookAccessToken);
            query = query.Where(x => x.IdOperation == idOperation && x.IdApp.ToUpper().Equals(idapp));

            var entity = query.FirstOrDefault();

            if (entity == null && service.ServiceContainer != null)
            {
                //Pudo haber sido porque el hijo y padre tienen IdApp. Ahora voy a buscar con el hijo (no debería suceder esto pero es un control adicional)
                idapp = service.UrlName.ToUpper();
                query = Repository.AllNoTracking(null, x => x.WebhookAccessToken);
                query = query.Where(x => x.IdOperation == idOperation && x.IdApp.ToUpper().Equals(idapp));
                entity = query.FirstOrDefault();
            }

            if (entity != null)
            {
                return Converter(entity);
            }
            return null;
        }

        public ICollection<WebhookRegistrationDto> GetWebhookRegistrationsForTable(ReportsIntegrationFilterDto filterDto)
        {
            var query = Repository.AllNoTracking(null, x => x.Payment);
            var result = new Collection<WebhookRegistrationDto>();

            var from = DateTime.MinValue;
            if (!string.IsNullOrEmpty(filterDto.DateFromString))
            {
                from = DateTime.Parse(filterDto.DateFromString, new CultureInfo("es-UY"));
            }

            var to = DateTime.MinValue;
            if (!string.IsNullOrEmpty(filterDto.DateToString))
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
                query = query.Where(x => x.IdOperation.StartsWith(filterDto.IdOperation));

            if (!filterDto.IdApp.IsEmpty())
                query = query.Where(x => x.IdApp.Contains(filterDto.IdApp));

            if (!filterDto.TransactionNumber.IsEmpty())
                query = query.Where(x => x.PaymentId != Guid.Empty && x.Payment.TransactionNumber.Equals(filterDto.TransactionNumber, StringComparison.OrdinalIgnoreCase));

            //ordeno, skip y take
            query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.CreationDate) : query.OrderByDescending(x => x.CreationDate);
            query = query.Skip(filterDto.DisplayStart);
            if (filterDto.DisplayLength.HasValue)
                query = query.Take(filterDto.DisplayLength.Value);

            var list = query.ToList();
            foreach (var webhookRegistration in list)
            {
                result.Add(Converter(webhookRegistration));
            }

            return result;
        }

        public int GetWebhookRegistrationsForTableCount(ReportsIntegrationFilterDto filterDto)
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
                query = query.Where(x => x.IdOperation.StartsWith(filterDto.IdOperation));

            if (!filterDto.IdApp.IsEmpty())
                query = query.Where(x => x.IdApp.Contains(filterDto.IdApp));

            return query.Select(t => new WebhookRegistrationDto
            {
                Id = t.Id
            }).Count();
        }

        public bool IsOperationIdRepited(string idOperation, string idApp)
        {
            var isOperationIdRepited =
               Repository.AllNoTracking(x => x.IdOperation.Equals(idOperation, StringComparison.InvariantCultureIgnoreCase) &&
                   x.IdApp.Equals(idApp, StringComparison.InvariantCultureIgnoreCase))
                   .Any();

            return isOperationIdRepited;
        }

        public WebhookAccessTokenDto GenerateAccessToken(WebhookRegistrationDto entity)
        {
            var token = string.Empty;
            var idApp = entity.IdApp;
            var idOperation = entity.IdOperation;
            var now = DateTime.Now.ToString("yyyyMMddhhmmssfffff");
            var newguid = Guid.NewGuid();

            var temp = idOperation + idApp + now + newguid;
            var hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(temp));
            token = Convert.ToBase64String(hash);

            var accesTokenDto = new WebhookAccessTokenDto()
            {
                AccessToken = token,
                Id = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
            };

            Repository.ContextTrackChanges = true;
            var efEntity = Repository.GetById(entity.Id);
            efEntity.WebhookAccessToken = new WebhookAccessToken()
            {
                AccessToken = accesTokenDto.AccessToken,
                CreationDate = accesTokenDto.CreationDate,
                Id = accesTokenDto.Id,
                State = WebhookAccessState.New
            };
            Repository.Edit(efEntity);
            Repository.Save();
            Repository.ContextTrackChanges = false;

            return accesTokenDto;
        }

        public void ResetAccessToken(Guid webhookRegistrationId)
        {
            Repository.ContextTrackChanges = true;

            var efEntity = Repository.GetById(webhookRegistrationId, x => x.WebhookAccessToken);
            efEntity.WebhookAccessToken.CreationDate = DateTime.UtcNow;
            Repository.Edit(efEntity);
            Repository.Save();

            Repository.ContextTrackChanges = false;
        }

        public WebhookAccessTokenDto RegenerateToken(Guid webhookRegistrationId)
        {
            Repository.ContextTrackChanges = true;
            var efEntity = Repository.GetById(webhookRegistrationId, x => x.WebhookAccessToken);

            var token = string.Empty;
            var idApp = efEntity.IdApp;
            var idOperation = efEntity.IdOperation;
            var now = DateTime.Now.ToString("yyyyMMddhhmmssfffff");
            var newguid = Guid.NewGuid();

            var temp = idOperation + idApp + now + newguid;
            var hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(temp));
            token = Convert.ToBase64String(hash);

            efEntity.WebhookAccessToken.CreationDate = DateTime.UtcNow;
            efEntity.WebhookAccessToken.AccessToken = token;

            Repository.Edit(efEntity);
            Repository.Save();
            Repository.ContextTrackChanges = false;

            return Converter(efEntity).WebhookAccessTokenDto;
        }

        public bool ValidateAccessToken(WebhookAccessTokenDto dto)
        {
            var query = Repository.AllNoTracking(null, x => x.WebhookAccessToken);
            var webhookRegistration = query.FirstOrDefault(x => x.WebhookAccessToken.AccessToken.Equals(dto.AccessToken));

            if (webhookRegistration == null)
                throw new IdNotFoundException(CodeExceptions.ID_NOT_FOUND);

            //Tiempo de vida
            ValidateAccessTokenTime(webhookRegistration);

            //Estado
            ValidateAccessTokenState(webhookRegistration.WebhookAccessToken);

            //Fecha de vencimiento de la factura
            ValidateBillExpirationDate(webhookRegistration.Bill);

            return true;
        }

        private void ValidateAccessTokenTime(WebhookRegistration webhookRegistration)
        {
            var service = _repositoryService.GetService(webhookRegistration.MerchantId, webhookRegistration.IdApp);
            var pagoLinkInt = (int)GatewayEnum.PagoLink;
            var sGateway = service.ServiceGateways.FirstOrDefault(x => x.Active && x.Gateway.Enum == pagoLinkInt);
            var seconds = 0;
            if (sGateway != null)
            {
                if (!string.IsNullOrEmpty(sGateway.ReferenceId))
                {
                    int.TryParse(sGateway.ReferenceId, out seconds);
                }
                else
                {
                    NLogLogger.LogEvent(NLogType.Info, "ServiceWebhookRegistration - ValidateAccessToken - " +
                        "La pasarela PagoLink del servicio " + service.Name + " tiene ReferenceId vacío. Se debe especificar el tiempo de vida del token en ese campo.");
                }
            }

            var now = DateTime.UtcNow;
            var lifetime = seconds > 0 ? seconds : int.Parse(ConfigurationManager.AppSettings["AccessTokenLifeTimeSec"]);

            if (webhookRegistration.WebhookAccessToken.CreationDate.AddSeconds(lifetime).CompareTo(now) < 0)
                throw new AccessTokenExpired(CodeExceptions.WEBBHOOKREGISTRATION_ACCESSTOKEN_EXPIRED);
        }

        private void ValidateAccessTokenState(WebhookAccessToken accessToken)
        {
            if (accessToken.State == WebhookAccessState.Paid)
            {
                throw new BillAlreadyPaid(CodeExceptions.BILL_ALREADY_PAID);
            }

            if (accessToken.State == WebhookAccessState.Expired)
            {
                throw new AccessTokenExpired(CodeExceptions.ACCESS_TOKEN_EXPIRED);
            }

            if (accessToken.State == WebhookAccessState.Cancelled)
            {
                throw new AccessTokenInvalidState(CodeExceptions.ACCESS_TOKEN_INVALID_STATE);
            }
        }

        private void ValidateBillExpirationDate(BillDataInput bill)
        {
            if (bill != null && !string.IsNullOrEmpty(bill.ExpirationDate))
            {
                var year = int.Parse(bill.ExpirationDate.Substring(0, 4));
                var month = int.Parse(bill.ExpirationDate.Substring(4, 2));
                var day = int.Parse(bill.ExpirationDate.Substring(6, 2));
                var expirationDate = new DateTime(year, month, day);

                if (expirationDate.CompareTo(DateTime.Today) < 0)
                {
                    throw new BillExpired(CodeExceptions.BILL_EXPIRED);
                }
            }
        }

        public bool IsTokenActive(AccessTokenFilterDto dto)
        {
            var query = Repository.AllNoTracking(null, x => x.WebhookAccessToken);

            if (dto.AccessTokenId != Guid.Empty)
            {
                query = query.Where(x => x.WebhookAccessTokenId == dto.AccessTokenId);
            }
            else if (dto.WebhookRegistrationId != Guid.Empty)
            {
                query = query.Where(x => x.Id == dto.WebhookRegistrationId);
            }
            else
            {
                query = query.Where(x => x.WebhookAccessToken != null && x.WebhookAccessToken.AccessToken.Equals(dto.Token));
            }

            var webhook = query.FirstOrDefault();

            return webhook != null && (webhook.WebhookAccessToken.State == WebhookAccessState.New
                                       || webhook.WebhookAccessToken.State == WebhookAccessState.Renewed
                                       || webhook.WebhookAccessToken.State == WebhookAccessState.Send);
        }

        public WebhookRegistrationDto GetByAccessToken(WebhookAccessTokenDto dto)
        {
            var query = Repository.AllNoTracking(null, x => x.WebhookAccessToken, x => x.BillLines);
            var webhookRegistration = query.FirstOrDefault(x => x.WebhookAccessToken.AccessToken.Equals(dto.AccessToken));
            if (webhookRegistration == null) return null;

            var webhookRegistrationDto = Converter(webhookRegistration);
            return webhookRegistrationDto;
        }

        public void UpdatewithPaymentId(string idApp, string idOperation, Guid paymentId)
        {
            Repository.ContextTrackChanges = true;
            try
            {
                var registration = Repository.All(x => x.IdApp.Equals(idApp, StringComparison.OrdinalIgnoreCase)
                    && x.IdOperation.Equals(idOperation, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if (registration != null)
                {
                    registration.PaymentId = paymentId;
                    Repository.Edit(registration);
                    Repository.Save();
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "VisaNetOn - ServiceWebhookRegistration - UpdatewithPaymentId - Excepcion");
                NLogLogger.LogEvent(exception);
            }
            Repository.ContextTrackChanges = false;
        }

        public bool CancelAccessToken(Guid accessTokenId)
        {
            try
            {
                Repository.ContextTrackChanges = true;
                var webhook = Repository.All(x => x.WebhookAccessTokenId == accessTokenId, x => x.WebhookAccessToken).FirstOrDefault();
                webhook.WebhookAccessToken.State = WebhookAccessState.Cancelled;
                Repository.Edit(webhook);
                Repository.Save();
            }
            catch (Exception exception)
            {
                Repository.ContextTrackChanges = false;
                return false;
            }

            Repository.ContextTrackChanges = false;
            return true;
        }

        public bool UpdateStatusAccessToken(Guid webhookId, WebhookAccessState status)
        {
            try
            {
                Repository.ContextTrackChanges = true;

                var webhook = Repository.GetById(webhookId, x => x.WebhookAccessToken);
                webhook.WebhookAccessToken.State = status;

                Repository.Edit(webhook);
                Repository.Save();
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
                Repository.ContextTrackChanges = false;
                return false;
            }

            Repository.ContextTrackChanges = false;
            return true;
        }

    }
}