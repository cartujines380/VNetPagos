using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.AzureUpload;
using VisaNet.Common.Exceptions;
using VisaNet.Common.FrameworkExtensions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Logging.Services;
using VisaNet.Common.Resource.Enums;
using VisaNet.Components.CyberSource.Interfaces;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;
using ProductDto = VisaNet.Domain.EntitiesDtos.ProductDto;

namespace VisaNet.Application.Implementations
{
    public class ServiceDebitRequest : BaseService<DebitRequest, DebitRequestDto>, IServiceDebitRequest
    {
        private readonly IServiceCustomerSite _serviceCustomerSite;
        private readonly IServiceBin _serviceBin;
        private readonly IServiceAnalyzeCsCall _serviceAnalyzeCsCall;
        private readonly IServiceApplicationUser _serviceApplicationUser;
        private readonly IServiceCard _serviceCard;
        private readonly ILoggerService _loggerService;
        private readonly ICyberSourceAccess _cyberSourceAccess;
        private readonly IServiceNotification _serviceNotification;
        private readonly IServiceParameters _serviceParameters;
        private readonly IServiceEmailMessage _serviceEmailMessage;

        private string _folderBlob = ConfigurationManager.AppSettings["AzureCommercesImagesUrl"];

        public ServiceDebitRequest(
            IRepositoryDebitRequest repository,
            IServiceCustomerSite serviceCustomerSite,
            IServiceBin serviceBin,
            IServiceAnalyzeCsCall serviceAnalyzeCsCall,
            IServiceApplicationUser serviceApplicationUser,
            IServiceCard serviceCard,
            ILoggerService loggerService,
            ICyberSourceAccess cyberSourceAccess,
            IServiceNotification serviceNotification,
            IServiceParameters serviceParameters, IServiceEmailMessage serviceEmailMessage)
            : base(repository)
        {
            _serviceCustomerSite = serviceCustomerSite;
            _serviceBin = serviceBin;
            _serviceAnalyzeCsCall = serviceAnalyzeCsCall;
            _serviceApplicationUser = serviceApplicationUser;
            _serviceCard = serviceCard;
            _loggerService = loggerService;
            _cyberSourceAccess = cyberSourceAccess;
            _serviceNotification = serviceNotification;
            _serviceParameters = serviceParameters;
            _serviceEmailMessage = serviceEmailMessage;
        }

        #region Public methods

        public IEnumerable<DebitAssociatedDto> GetAssociatedDebits(Guid cardId)
        {

            System.Linq.Expressions.Expression<Func<DebitRequest, bool>> exp = x => x.CardId == cardId && (x.State == DebitRequestState.Accepted || x.State == DebitRequestState.Synchronized || x.State == DebitRequestState.ManualSynchronization || x.State == DebitRequestState.Pending || x.State == DebitRequestState.PendingCancellation);

            var debitsAssociated = this.AllNoTracking(null, exp)
            .Select(s => new DebitAssociatedDto
            {
                ReferenceNumber = s.ReferenceNumber,
                State = s.State.ToString(),
                CommerceName = _serviceCustomerSite.GetCommercesDebit(new List<int> { s.DebitProductId }).First().Name
            });

            return debitsAssociated;
        }

        public IEnumerable<DebitRequestDto> GetByUserId(Guid userId)
        {
            var result = Repository.AllNoTracking().Where(x => x.UserId == userId).ToList();

            return result.Select(x => Converter(x));
        }

        public IEnumerable<DebitRequestSyncDto> GetDebitToSync()
        {
            //return new List<DebitRequestSyncDto>()
            //{
            //    new DebitRequestSyncDto
            //    {
            //        Id = Guid.NewGuid(),
            //        CardMonth = 4,
            //        CardYear = 2020,
            //        CardNumber = "5323844100061704",
            //        MerchantGroupId = 23,
            //        MerchantId = 553,
            //        MerchantProductId = 504,                    
            //        Type = DebitRequestTypeDto.High,
            //        User = new UserSyncDto
            //        {
            //            Address = "sin nombre",
            //            Email = "j@j.com",
            //            FullName = "jorge a",
            //            IdentityNumber = "3679938-6",
            //            PhoneNumber = "421212121"
            //        },
            //        References = new List<DebitRequestReferenceDto>()
            //        {
            //            new DebitRequestReferenceDto
            //            {
            //                Id = Guid.NewGuid(),
            //                Index = 0,
            //                ProductPropertyId = 971,
            //                Value = "12474110"
            //            }
            //        }
            //    },
            //    new DebitRequestSyncDto
            //    {
            //        Id = Guid.NewGuid(),
            //        CardMonth = 4,
            //        CardYear = 2020,
            //        CardNumber = "5323844100061704",
            //        MerchantGroupId = 23,
            //        MerchantId = 553,
            //        MerchantProductId = 504,
            //        Type = DebitRequestTypeDto.High,
            //        User = new UserSyncDto
            //        {
            //            Address = "sin nombre",
            //            Email = "j@j.com",
            //            FullName = "jorge a",
            //            IdentityNumber = "3679938-6",
            //            PhoneNumber = "421212121"
            //        },
            //        References = new List<DebitRequestReferenceDto>()
            //        {
            //            new DebitRequestReferenceDto
            //            {
            //                Id = Guid.NewGuid(),
            //                Index = 0,
            //                ProductPropertyId = 971,
            //                Value = "12474110"
            //            }
            //        }
            //    },
            //};

            var debitRequests = Repository.AllNoTracking()
                .Include(x => x.User)
                .Include(x => x.Card)
                .Include(x => x.References)
                .Where(x => x.State == DebitRequestState.Pending)
                .ToList();

            var debitToSync = new List<DebitRequestSyncDto>();

            if (debitRequests.Any())
            {
                var productIds = debitRequests.Select(x => x.DebitProductId).Distinct().ToList();
                var merchantCommerces = _serviceCustomerSite.GetCommercesDebit(productIds);
                var param = _serviceParameters.GetParametersForCard();

                debitRequests.ToList().ForEach(debit =>
                {
                    var commerce = merchantCommerces
                        .Where(y => y.ProductosListDto.Any(z => z.DebitProductid == debit.DebitProductId)
                            && y.DebitmerchantId.HasValue
                            && y.DebitMerchantGroupId.HasValue)
                        .Select(x => new { merchantId = x.DebitmerchantId.Value, merchantGroupId = x.DebitMerchantGroupId.Value })
                        .First();

                    var cybersourceDto = new CybersourceGetCardNameDto
                    {
                        MerchantId = param.MerchantId,
                        MerchantReferenceCode = debit.Card.CybersourceTransactionId,
                        Token = debit.Card.PaymentToken,
                        TransactionKey = param.CybersourceTransactionKey
                    };

                    var dto = CreateDebitRequestDto(debit, commerce.merchantId, commerce.merchantGroupId, cybersourceDto);

                    if (dto != null)
                    {
                        debitToSync.Add(dto);
                    }
                });
            }
            return debitToSync;
        }

        public override IQueryable<DebitRequest> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public List<DebitRequestTableDto> GetDataForFromList(DebitRequestsFilterDto filters)
        {
            var query = Repository.AllNoTracking()
                .Include(x => x.Card)
                .Include(x => x.User)
                .Include(x => x.References)
                .Where(x => x.State != DebitRequestState.Cancelled && x.Type == DebitRequestType.High);

            if (filters.UserId != default(Guid))
            {
                query = query.Where(x => x.UserId == filters.UserId);
            }
            if (filters.DateFrom != null && filters.DateFrom != default(DateTime))
            {
                query = query.Where(x => x.CreationDate >= filters.DateFrom.Value);
            }

            if (filters.DateTo != null && filters.DateTo != default(DateTime))
            {
                filters.DateTo = filters.DateTo.Value.Date.AddDays(1);
                query = query.Where(x => x.CreationDate < filters.DateTo.Value);
            }
            if (filters.DebitState > 0)
            {
                if (filters.DebitState == DebitRequestStateDto.Pending)
                {
                    query = query.Where(x =>
                        (int)x.State == (int)DebitRequestStateDto.Pending
                        || (int)x.State == (int)DebitRequestStateDto.ManualSynchronization
                        || (int)x.State == (int)DebitRequestStateDto.Synchronized);
                }
                else
                {
                    query = query.Where(x => (int)x.State == (int)filters.DebitState);
                }
            }
            if (filters.DebitType > 0)
            {
                query = query.Where(x => (int)x.Type == (int)filters.DebitType);
            }
            if (filters.CardId != default(Guid))
            {
                query = query.Where(x => x.CardId == filters.CardId);
            }
            if (!string.IsNullOrEmpty(filters.Service))
            {
                var productIds = _serviceCustomerSite.GetCommercesDebit(search: filters.Service)
                    .SelectMany(x => x.ProductosListDto)
                    .Distinct()
                    .Select(x => x.DebitProductid);

                if (productIds.Any())
                {
                    query = query.Where(x => productIds.Contains(x.DebitProductId));
                }
                else
                {
                    query = Enumerable.Empty<DebitRequest>().AsQueryable();
                }
            }

            query = query.OrderByDescending(x => x.CreationDate);

            if (filters.DisplayLength.HasValue)
            {
                query = query.Skip(filters.DisplayStart * filters.DisplayLength.Value);
                query = query.Take(filters.DisplayLength.Value);
            }
            else
            {
                query = query.Skip(filters.DisplayStart);
            }

            var debitList = new List<DebitRequestTableDto>();

            if (query.Any())
            {
                var productIds = query.Select(x => x.DebitProductId).ToList();

                var merchantCommerces = _serviceCustomerSite.GetCommercesDebit(productIds);

                query.ToList().ForEach(debit =>
                {
                    var merchantCommerce = merchantCommerces
                        .Where(y => y.ProductosListDto.Any(z => z.DebitProductid == debit.DebitProductId))
                        .Select(y => new { commerceName = y.Name, commerceId = y.Id, product = y.ProductosListDto.First(z => z.DebitProductid == debit.DebitProductId), imageName = y.ImageName })
                        .First();

                    debitList.Add(new DebitRequestTableDto
                    {
                        Id = debit.Id,
                        CardId = debit.CardId,
                        CardNumber = debit.Card.MaskedNumber,
                        CardDescription = debit.Card.Description,
                        CreationDate = debit.CreationDate,
                        MerchantProductName = merchantCommerce.product.Description,
                        MerchantName = merchantCommerce.commerceName,
                        MerchantId = merchantCommerce.commerceId,
                        ReferenceNumber = debit.ReferenceNumber,
                        References = ConverterReferenceTable(debit.References, merchantCommerce.product.ProductPropertyList),
                        State = (DebitRequestStateDto)debit.State,
                        Type = (DebitRequestTypeDto)debit.Type,
                        UserId = debit.UserId,
                        DebitImageUrl = FileStorage.Instance.GetImageUrl(_folderBlob, merchantCommerce.commerceId, merchantCommerce.imageName),
                    });
                });
            }

            return debitList;
        }

        public bool ValidateCardType(int binValue)
        {
            var bin = _serviceBin.Find(binValue);
            var listCardTypes = ConfigurationManager.AppSettings["CardTypesForDebit"];
            var listCards = listCardTypes.Split(',');

            return listCards.Any(x => x.Equals(bin.CardType.ToString()));
        }

        public bool CancelDebitRequest(Guid id)
        {
            NLogLogger.LogEvent(NLogType.Info, string.Format("Inicio de cancelación de débito: {0}", id), OperationType.DebitRequestCancelDebit, Repository.GetDataLog());
            bool result = false;
            try
            {
                var request = Repository.GetById(id, x => x.References);

                if (request.State == DebitRequestState.Accepted || request.State == DebitRequestState.Synchronized)
                {
                    var newRequest = new DebitRequest
                    {
                        CardId = request.CardId,
                        DebitProductId = request.DebitProductId,
                        UserId = request.UserId,
                        State = DebitRequestState.Pending,
                        Type = DebitRequestType.Low,
                        References = request.References
                            .Select(x =>
                                new DebitRequestReference
                                {
                                    Id = Guid.NewGuid(),
                                    Index = x.Index,
                                    ProductPropertyId = x.ProductPropertyId,
                                    Value = x.Value
                                })
                            .ToList()
                    };
                    Repository.Create(newRequest);

                    request.AssociatedDebitRequestId = newRequest.Id;
                    request.State = DebitRequestState.PendingCancellation;
                    Repository.Edit(request);

                    DebitCreationNotification(Converter(newRequest));
                }
                else
                {
                    request.State = DebitRequestState.Cancelled;
                    Repository.Edit(request);
                    DebitCancelNotification(Converter(request));
                }

                Repository.Save();
                result = true;
                NLogLogger.LogEvent(NLogType.Info, string.Format("Finalizo de cancelación de débito: {0}", id), OperationType.DebitRequestCancelDebit, Repository.GetDataLog());
            }
            catch (Exception e)
            {
                NLogLogger.LogDebitEvent(e, OperationType.DebitRequestCancelDebit);
            }

            return result;
        }

        public CybersourceCreateDebitWithNewCardDto ProccesDataFromCybersource(IDictionary<string, string> csDictionary)
        {
            var result = new CybersourceCreateDebitWithNewCardDto() { };
            var isNewUser = false;
            var userCreated = false;
            var cardCreated = false;
            ApplicationUserDto user = null;
            CybersourceTransactionsDataDto processedData = null;

            try
            {
                processedData = _serviceAnalyzeCsCall.ProcessCybersourceOperation(csDictionary);
                result.TokenizationData = processedData.TokenizationData;

                if (processedData.TokenizationData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                {
                    //si existe user ID no necesito crear usuario
                    if (processedData.DebitRequestDto.UserId == Guid.Empty)
                    {
                        //HAY QUE CREAR EL USUARIO
                        _serviceApplicationUser.Create(new ApplicationUserCreateEditDto()
                        {
                            Name = processedData.DebitRequestDto.ApplicationUserDto.Name,
                            Surname = processedData.DebitRequestDto.ApplicationUserDto.Surname,
                            Email = processedData.DebitRequestDto.ApplicationUserDto.Email,
                            Address = processedData.DebitRequestDto.ApplicationUserDto.Address,
                            IdentityNumber = processedData.DebitRequestDto.ApplicationUserDto.IdentityNumber,
                            Password = processedData.DebitRequestDto.ApplicationUserDto.Password,
                            PasswordAlreadyHashed = true,
                            PhoneNumber = processedData.DebitRequestDto.ApplicationUserDto.PhoneNumber,
                            MobileNumber = processedData.DebitRequestDto.ApplicationUserDto.MobileNumber,
                            CallCenterKey = processedData.DebitRequestDto.ApplicationUserDto.CallCenterKey
                        });

                        user =
                            _serviceApplicationUser.GetUserByUserName(
                                processedData.DebitRequestDto.ApplicationUserDto.Email);
                        isNewUser = true;
                        userCreated = true;
                        result.UserCreated = true;
                        //SE CREO EL USUARIO EN ESTE PUNTO
                    }
                    else
                    {
                        user = _serviceApplicationUser.GetById(processedData.DebitRequestDto.UserId);
                        userCreated = true;
                    }

                    processedData.DebitRequestDto.UserId = user.Id;

                    var strLog = string.Format(LogStrings.Debit_Init, processedData.DebitRequestDto.ApplicationUserDto.Email,
                        processedData.CyberSourceMerchantDefinedData.CommerceAndProduct);

                    //LogCybersourceData(processedData.PaymentDto, processedData, strLog, LogOperationType.Webhooks);

                    //CREO LA TARJETA
                    var newCard = _serviceApplicationUser.AddCard(processedData.DebitRequestDto.CardDto, user.Id);
                    cardCreated = true;

                    processedData.DebitRequestDto.CardId = newCard.Id;

                    //SE CREO LA TARJETA PARA EL USUARIO
                    LogCybersourceData(user, processedData, "Se persistieron los datos de la nueva tarjeta para adhesión a débito", LogOperationType.DebitPaymentBatch);

                    //CREO LA SOLICITUD DE ALTA CON LA TARJETA NUEVA
                    var commerces = _serviceCustomerSite.GetCommercesDebit();
                    var commerce =
                        commerces.FirstOrDefault(x => x.Id == processedData.CyberSourceMerchantDefinedData.CommerceId);
                    result.CommerceId = commerce.Id;

                    var product =
                        commerce.ProductosListDto.FirstOrDefault(
                            x => x.Id == processedData.CyberSourceMerchantDefinedData.ProductId);
                    result.ProductId = product.Id;

                    for (int i = 0; i < product.ProductPropertyList.Count; i++)
                    {
                        var dto = new DebitRequestReferenceDto()
                        {
                            Index = product.ProductPropertyList.ElementAt(i).InputSequence,
                            ProductPropertyId = product.ProductPropertyList.ElementAt(i).DebitProductPropertyId.Value,
                            Id = Guid.NewGuid(),
                        };
                        switch (i)
                        {
                            case 0:
                                dto.Value = processedData.CyberSourceMerchantDefinedData.ReferenceNumber1;
                                break;
                            case 1:
                                dto.Value = processedData.CyberSourceMerchantDefinedData.ReferenceNumber2;
                                break;
                            case 2:
                                dto.Value = processedData.CyberSourceMerchantDefinedData.ReferenceNumber3;
                                break;
                            case 3:
                                dto.Value = processedData.CyberSourceMerchantDefinedData.ReferenceNumber4;
                                break;
                            case 4:
                                dto.Value = processedData.CyberSourceMerchantDefinedData.ReferenceNumber5;
                                break;
                            case 5:
                                dto.Value = processedData.CyberSourceMerchantDefinedData.ReferenceNumber6;
                                break;
                        }
                        processedData.DebitRequestDto.References.Add(dto);
                    }
                    processedData.DebitRequestDto.DebitProductId = product.DebitProductid.Value;

                    var finalDebit = Create(processedData.DebitRequestDto, true);

                    result.DebitRequestDto = finalDebit;
                    result.DebitRequestDto.ApplicationUserDto = user;

                    return result;
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogDebitEvent(e, OperationType.DebitRequestProcessDataCybersource);
                result.ExceptionCapture = e;
            }
            result.DebitRequestDto = null;
            if (userCreated)
            {
                var email = user != null ? user.Email : string.Empty;
                if (cardCreated)
                {
                    result.InternalErrorCode = isNewUser ? 15 : 17;
                    result.InternalErrorDesc =
                        isNewUser ?
                        string.Format("Ha ocurrido un error. Se creo el usuario {0} y tambien se creo la tarjeta pero no se pudo terminar con el proceso de asociación. Por favor intente nuevamente y si el error persiste comuníquese con el call center", email)
                        : string.Format("Ha ocurrido un error. Para el usuario {0} se creo la tarjeta pero no se pudo terminar con el proceso de asociación. Por favor intente nuevamente y si el error persiste comuníquese con el call center", email);
                }
                else
                {
                    result.InternalErrorCode = 14;
                    result.InternalErrorDesc = string.Format(
                            "Ha ocurrido un error. Se creo el usuario {0} pero no se pudo terminar con el proceso de asociación. Por favor intente nuevamente y si el error persiste comuníquese con el call center",
                            email);
                }
            }
            else
            {
                result.InternalErrorCode = processedData.TokenizationData.PaymentResponseCode;
                result.InternalErrorDesc = processedData.TokenizationData.PaymentResponseMsg;
            }
            return result;
        }

        public override DebitRequestDto Create(DebitRequestDto entity, bool returnEntity = false)
        {
            entity.State = DebitRequestStateDto.Pending;
            var result = base.Create(entity, returnEntity);
            DebitCreationNotification(result);
            return returnEntity ? GetById(result.Id, x => x.Card) : null;
        }

        public override void Edit(DebitRequestDto entity)
        {
            throw new NotImplementedException();
        }

        public List<DebitRequestDto> GetDebitSuscriptionList(DebitRequestsFilterDto filters)
        {
            var query = GetDataQueryable(filters);

            query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.CreationDate) : query.OrderByDescending(x => x.CreationDate);

            query = query.Skip(filters.DisplayStart);
            if (filters.DisplayLength.HasValue)
                query = query.Take(filters.DisplayLength.Value);

            var debitList = new List<DebitRequestDto>();

            if (query.Any())
            {
                var productIds = query.Select(x => x.DebitProductId).ToList();

                var merchantCommerces = _serviceCustomerSite.GetCommercesDebit(productIds);

                query.ToList().ForEach(debit =>
                {
                    var merchantCommerce = merchantCommerces
                        .Where(y => y.ProductosListDto.Any(z => z.DebitProductid == debit.DebitProductId))
                        .Select(y => new { serviceId = y.ServiceId, commerceName = y.Name, commerceId = y.Id, product = y.ProductosListDto.First(z => z.DebitProductid == debit.DebitProductId) })
                        .First();

                    debitList.Add(new DebitRequestDto
                    {
                        Id = debit.Id,
                        CreationDate = debit.CreationDate,
                        State = (DebitRequestStateDto)debit.State,
                        Type = (DebitRequestTypeDto)debit.Type,
                        UserId = debit.UserId,
                        ApplicationUserDto = debit.User != null ? new ApplicationUserDto()
                        {
                            Id = debit.User.Id,
                            Email = debit.User.Email,
                            Name = debit.User.Name,
                            Surname = debit.User.Surname,
                            MobileNumber = !string.IsNullOrEmpty(debit.User.MobileNumber) ? debit.User.MobileNumber : debit.User.PhoneNumber,
                        } : null,
                        CardId = debit.CardId,
                        CardDto = debit.Card != null ? new CardDto()
                        {
                            MaskedNumber = debit.Card.MaskedNumber,
                            Description = debit.Card.Description
                        } : null,
                        DebitProductId = debit.DebitProductId,
                        CommerceDto = new CommerceDto()
                        {
                            Name = merchantCommerce.commerceName,
                            Id = merchantCommerce.commerceId,
                            ServiceId = merchantCommerce.serviceId,
                            ProductosListDto = new List<ProductDto>()
                            {
                                new ProductDto()
                                {
                                    Id = merchantCommerce.product.Id,
                                    Description = merchantCommerce.product.Description,
                                    ProductPropertyList = merchantCommerce.product.ProductPropertyList.Select(x => new VisaNet.Domain.EntitiesDtos.ProductPropertyDto()
                                    {
                                        Id = x.Id,
                                        Name = x.Name,
                                    }).ToList()
                                }
                            }
                        },
                        ReferenceNumber = debit.ReferenceNumber,

                    });
                });
            }

            return debitList;
        }

        public int GetDebitSuscriptionListCount(DebitRequestsFilterDto filters)
        {
            var query = GetDataQueryable(filters);
            return query.Count();
        }

        public void SetRequestSynchronizated(Guid id, int referenceId)
        {
            try
            {
                var request = Repository.GetById(id, x => x.References);
                request.DebitRequestEventId = referenceId;
                request.State = DebitRequestState.Synchronized;
                Repository.Edit(request);
                Repository.Save();
            }
            catch (Exception e)
            {
                NLogLogger.LogDebitEvent(e, OperationType.DebitRequestSetSynchronizated);
            }
        }

        public void SetRequestErrorSynchronization(Guid id, string errorMessage)
        {
            try
            {
                var request = Repository.GetById(id, x => x.References);
                var limitOfAttemps = int.Parse(ConfigurationManager.AppSettings["limitOfAttemps"]);

                request.SynchronizationLimitOfAttemps = request.SynchronizationLimitOfAttemps.HasValue
                    ? request.SynchronizationLimitOfAttemps.Value + 1
                    : 1;

                if (request.SynchronizationLimitOfAttemps.HasValue && request.SynchronizationLimitOfAttemps.Value > limitOfAttemps)
                {
                    request.State = DebitRequestState.ManualSynchronization;
                }

                request.SynchronizationDate = DateTime.Today;
                request.SynchronizationMessage = errorMessage;

                Repository.Edit(request);
                Repository.Save();
            }
            catch (Exception e)
            {
                NLogLogger.LogDebitEvent(e, OperationType.DebitRequestSetErrorSynchronization);
            }
        }

        public void SendDebitSuscriptionNotification(Guid debitRequestId)
        {

            var dto = Repository.GetById(debitRequestId, x => x.Card, x => x.User, x => x.References);
            var commerces = _serviceCustomerSite.GetCommercesDebit(new List<int>() { dto.DebitProductId });
            var commerce = commerces.FirstOrDefault();
            var product = commerce.ProductosListDto.FirstOrDefault(x => x.DebitProductid == dto.DebitProductId);

            var emailNotification = new DebitRequestEmailDto()
            {
                ApplicationUserId = dto.UserId,
                Status = EnumsStrings.ResourceManager.GetString(string.Format("{0}_{1}", typeof(DebitRequestStateDto).Name, Enum.GetName(typeof(DebitRequestStateDto), dto.State))),
                Type = EnumsStrings.ResourceManager.GetString(string.Format("{0}_{1}", typeof(DebitRequestTypeDto).Name, Enum.GetName(typeof(DebitRequestTypeDto), dto.Type))),
                MaskedNumber = dto.Card.MaskedNumber,
                Email = dto.User.Email,
                ServiceName = commerce.Name,
                ProductName = product.Description,
            };

            var references = new Dictionary<string, string>();
            foreach (var prop in product.ProductPropertyList.OrderBy(x => x.InputSequence))
            {
                var input = dto.References.FirstOrDefault(x => x.ProductPropertyId == prop.DebitProductPropertyId);
                if (!string.IsNullOrEmpty(input.Value))
                {
                    references.Add(prop.Name, input.Value);
                }
            }

            emailNotification.References = references;

            _serviceEmailMessage.SendDebitSuscriptionNotification(emailNotification);
        }

        public IEnumerable<DebitRequestExcelDto> ExcelExportManualSynchronization()
        {
            var debitRequests = Repository.AllNoTracking()
                .Include(x => x.User)
                .Include(x => x.Card)
                .Include(x => x.References)
                .Where(x => x.State == DebitRequestState.ManualSynchronization)
                .ToList();

            var debits = new List<DebitRequestExcelDto>();

            if (debitRequests.Any())
            {
                var productIds = debitRequests.Select(x => x.DebitProductId).Distinct();
                var merchantCommerces = _serviceCustomerSite.GetCommercesDebit(productIds.ToList());
                var cardTokens = debitRequests.Select(x => x.Card.PaymentToken);
                var param = _serviceParameters.GetParametersForCard();

                debitRequests.ToList().ForEach(debit =>
                {
                    var commerce = merchantCommerces
                        .Where(y => y.ProductosListDto.Any(z => z.DebitProductid == debit.DebitProductId)
                            && y.DebitmerchantId.HasValue
                            && y.DebitMerchantGroupId.HasValue)
                        .Select(x =>
                            new
                            {
                                name = x.Name,
                                merchantGroupName = x.DebitMerchantGroupName,
                                productName = x.ProductosListDto.First(p => p.DebitProductid == debit.DebitProductId).Description,
                                productReferences = x.ProductosListDto.First(p => p.DebitProductid == debit.DebitProductId).ProductPropertyList
                            })
                        .First();

                    var references = ConverterReferenceTable(debit.References, commerce.productReferences);

                    var dto = CreateDebitRequestExcelDto(debit, commerce, references, param.MerchantId);

                    if (dto != null)
                    {
                        debits.Add(dto);
                    }
                });
            }
            return debits;
        }

        #region Converters

        public override DebitRequest Converter(DebitRequestDto entity)
        {
            if (entity == null) return null;

            var debitRequest = new DebitRequest
            {
                Id = entity.Id,
                DebitProductId = entity.DebitProductId,
                Type = (DebitRequestType)entity.Type,
                State = (DebitRequestState)entity.State,
                UserId = entity.UserId,
                CardId = entity.CardId,
            };

            if (entity.References != null && entity.References.Any())
            {
                debitRequest.References = entity.References
                    .Select(x =>
                        new DebitRequestReference
                        {
                            Id = x.Id,
                            Index = x.Index,
                            ProductPropertyId = x.ProductPropertyId,
                            Value = x.Value
                        })
                    .ToList();
            }

            return debitRequest;
        }

        public override DebitRequestDto Converter(DebitRequest entity)
        {
            if (entity == null) return null;

            var debitRequestDto = new DebitRequestDto
            {
                Id = entity.Id,
                DebitProductId = entity.DebitProductId,
                Type = (DebitRequestTypeDto)entity.Type,
                State = (DebitRequestStateDto)entity.State,
                UserId = entity.UserId,
                CardId = entity.CardId,
                CreationDate = entity.CreationDate,
                ReferenceNumber = entity.ReferenceNumber,
                CardDto = entity.Card == null ? null : new CardDto()
                {
                    MaskedNumber = entity.Card.MaskedNumber,
                    Description = entity.Card.Description,
                    DueDate = entity.Card.DueDate,
                    Active = entity.Card.Active,
                    Name = entity.Card.Name,
                },
                ApplicationUserDto = entity.User == null ? null : new ApplicationUserDto()
                {
                    Email = entity.User.Email,
                    Name = entity.User.Name,
                    Surname = entity.User.Surname,
                    MobileNumber = entity.User.MobileNumber
                }
            };

            if (entity.References != null && entity.References.Any())
            {
                debitRequestDto.References = entity.References
                    .Select(x =>
                        new DebitRequestReferenceDto
                        {
                            Id = x.Id,
                            Index = x.Index,
                            ProductPropertyId = x.ProductPropertyId,
                            Value = x.Value
                        })
                    .ToList();
            }

            return debitRequestDto;
        }

        public DebitRequestReferenceDto ConverterReference(DebitRequestReference entity)
        {
            return new DebitRequestReferenceDto
            {
                Id = entity.Id,
                Index = entity.Index,
                ProductPropertyId = entity.ProductPropertyId,
                Value = entity.Value
            };
        }

        #endregion Converters

        #endregion Public methods

        #region Private methods
        private ApplicationUserDto UserIsOwnerOfCard(CardOperationDto cardOperationDto)
        {
            var user = _serviceApplicationUser.AllNoTracking(null,
               x => x.Id == cardOperationDto.UserId && x.Cards.Any(y => y.Id == cardOperationDto.CardId), x => x.Cards).FirstOrDefault();

            if (user == null)
                throw new BusinessException(CodeExceptions.USER_CARD_NOT_MATCH);

            return user;
        }

        private IList<DebitRequestReferenceTableDto> ConverterReferenceTable(ICollection<DebitRequestReference> references, ICollection<CustomerSite.EntitiesDtos.Debit.ProductPropertyDto> properties)
        {
            return references
                .Where(reference => properties.Any(x => x.DebitProductPropertyId == reference.ProductPropertyId) && !string.IsNullOrEmpty(reference.Value))
                .Select(reference =>
                    new DebitRequestReferenceTableDto
                    {
                        Name = properties.First(x => x.DebitProductPropertyId == reference.ProductPropertyId).Name,
                        Value = reference.Value
                    })
                .ToList();
        }

        private void LogCybersourceData(ApplicationUserDto user, CybersourceTransactionsDataDto csTransactionsDataDto, string msg, LogOperationType logOperationType)
        {
            var cyberSourceData = csTransactionsDataDto.CyberSourceData;
            var verifyByVisaData = csTransactionsDataDto.VerifyByVisaData;

            _loggerService.CreateLog(LogType.Info, logOperationType, LogCommunicationType.VisaNet,
                user.Id, msg, msg, null,
                new CyberSourceLogDataDto
                {
                    AuthAmount = cyberSourceData.AuthAmount,
                    AuthAvsCode = cyberSourceData.AuthAvsCode,
                    AuthCode = cyberSourceData.AuthCode,
                    AuthResponse = cyberSourceData.AuthResponse,
                    AuthTime = cyberSourceData.AuthTime,
                    AuthTransRefNo = cyberSourceData.AuthTransRefNo,
                    BillTransRefNo = cyberSourceData.BillTransRefNo,
                    Decision = cyberSourceData.Decision,
                    Message = cyberSourceData.Message,
                    PaymentToken = cyberSourceData.PaymentToken,
                    ReasonCode = cyberSourceData.ReasonCode,
                    ReqAmount = cyberSourceData.ReqAmount,
                    ReqCardExpiryDate =
                        cyberSourceData.ReqCardExpiryDate,
                    ReqCardNumber = cyberSourceData.ReqCardNumber,
                    ReqCardType = cyberSourceData.ReqCardType,
                    ReqCurrency = cyberSourceData.ReqCurrency,
                    ReqPaymentMethod = cyberSourceData.ReqPaymentMethod,
                    ReqProfileId = cyberSourceData.ReqProfileId,
                    ReqReferenceNumber = cyberSourceData.ReqReferenceNumber,
                    ReqTransactionType = cyberSourceData.ReqTransactionType,
                    ReqTransactionUuid = cyberSourceData.ReqTransactionUuid,
                    TransactionId = cyberSourceData.TransactionId,
                    TransactionType = TransactionType.CardToken,
                    PaymentPlatform = PaymentPlatform.VisaNet
                },
                new CyberSourceVerifyByVisaDataDto
                {
                    PayerAuthenticationXid = verifyByVisaData.PayerAuthenticationXid,
                    PayerAuthenticationProofXml = verifyByVisaData.PayerAuthenticationProofXml,
                    PayerAuthenticationCavv = verifyByVisaData.PayerAuthenticationCavv,
                    PayerAuthenticationEci = verifyByVisaData.PayerAuthenticationEci,
                },
                null
            );
        }

        private void DebitCreationNotification(DebitRequestDto entity)
        {
            DebitNotification(entity, "Se creo una nueva solicitud de {0} de debito para el servicio {1} - {2}. ");
        }

        private void DebitCancelNotification(DebitRequestDto entity)
        {
            DebitNotification(entity, "Se canceló la solicitud de {0} de debito para el servicio {1} - {2}. ");
        }

        private void DebitNotification(DebitRequestDto entity, string textFormat)
        {
            if (entity == null)
                return;

            var commerce = _serviceCustomerSite.GetCommercesDebit(new List<int>() { entity.DebitProductId }).FirstOrDefault();
            var product = commerce.ProductosListDto.FirstOrDefault(x => x.DebitProductid == entity.DebitProductId);
            var type = Common.Resource.Enums.EnumsStrings.ResourceManager.GetString(string.Format("{0}_{1}", typeof(DebitRequestTypeDto).Name, Enum.GetName(typeof(DebitRequestTypeDto), entity.Type)));

            var strProps = string.Empty;
            foreach (var prodProp in product.ProductPropertyList.OrderBy(x => x.InputSequence))
            {
                var refe = entity.References.FirstOrDefault(x => x.ProductPropertyId == prodProp.DebitProductPropertyId);
                if (refe != null)
                {
                    if (!string.IsNullOrEmpty(strProps))
                    {
                        strProps = strProps + ", ";
                    }
                    strProps = strProps + prodProp.Name + ": " + refe.Value;
                }
            }

            _serviceNotification.Create(new NotificationDto()
            {
                Date = DateTime.Now,
                NotificationPrupose = NotificationPruposeDto.SueccessNotification,
                RegisteredUserId = entity.UserId,
                Message = string.Format(textFormat, type, commerce.Name, product.Description)
            });
        }

        private DebitRequestSyncDto CreateDebitRequestDto(DebitRequest debit, int merchantId, int merchantGroupId, CybersourceGetCardNameDto cybersourceDto)
        {
            try
            {
                var cardNumber = _cyberSourceAccess.GetCardNumberByToken(cybersourceDto);
                return new DebitRequestSyncDto
                {
                    Id = debit.Id,
                    User = new UserSyncDto
                    {
                        FullName = debit.User.FullName,
                        IdentityNumber = debit.User.IdentityNumber,
                        Address = debit.User.Address,
                        PhoneNumber = debit.User.PhoneNumber,
                        Email = debit.User.Email
                    },
                    MerchantProductId = debit.DebitProductId,
                    MerchantId = merchantId,
                    MerchantGroupId = merchantGroupId,
                    Type = (DebitRequestTypeDto)debit.Type,
                    CardMonth = debit.Card.DueDate.Month,
                    CardYear = debit.Card.DueDate.Year,
                    CardNumber = cardNumber,
                    References = debit.References == null
                            ? new List<DebitRequestReferenceDto>()
                            : debit.References.Select(x => ConverterReference(x)).ToList(),

                };
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                return null;
            }
        }

        private DebitRequestExcelDto CreateDebitRequestExcelDto(DebitRequest debit, dynamic commerce, IList<DebitRequestReferenceTableDto> references, string merchantId)
        {
            try
            {
                return new DebitRequestExcelDto
                {
                    CreationDate = debit.CreationDate,
                    SynchronizationDate = debit.SynchronizationDate ?? DateTime.Now,
                    Merchant = commerce.name,
                    MerchantGroup = commerce.merchantGroupName,
                    MerchantProduct = commerce.productName,
                    UserFullName = debit.User.FullName,
                    UserIdentityNumber = debit.User.IdentityNumber,
                    UserAddress = debit.User.Address,
                    UserPhoneNumber = debit.User.PhoneNumber,
                    UserEmail = debit.User.Email,
                    Type = EnumsStrings.ResourceManager.GetString(string.Format("{0}_{1}", typeof(DebitRequestTypeDto).Name, Enum.GetName(typeof(DebitRequestTypeDto), debit.Type))),
                    CardMonth = debit.Card.DueDate.Month,
                    CardYear = debit.Card.DueDate.Year,
                    References = references == null
                            ? string.Empty
                            : string.Join(", ", references.Select(r => string.Format("{0}: {1}", r.Name, r.Value))),

                    MerchantId = merchantId,
                    MerchantReferenceCode = debit.Card.CybersourceTransactionId,
                    Token = debit.Card.PaymentToken
                };
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                return null;
            }
        }

        private IQueryable<DebitRequest> GetDataQueryable(DebitRequestsFilterDto filters)
        {
            var query = Repository.AllNoTracking()
                .Include(x => x.Card)
                .Include(x => x.User)
                .Include(x => x.References)
                .Where(x => x.State != DebitRequestState.Cancelled);

            if (filters.UserId != default(Guid))
            {
                query = query.Where(x => x.UserId == filters.UserId);
            }
            if (filters.DateFrom != null && filters.DateFrom != default(DateTime))
            {
                query = query.Where(x => x.CreationDate >= filters.DateFrom.Value);
            }

            if (filters.DateTo != null && filters.DateTo != default(DateTime))
            {
                filters.DateTo = filters.DateTo.Value.Date.AddDays(1);
                query = query.Where(x => x.CreationDate < filters.DateTo.Value);
            }
            if (filters.DebitState > 0)
            {
                query = query.Where(x => (int)x.State == (int)filters.DebitState);
            }
            if (filters.DebitType > 0)
            {
                query = query.Where(x => (int)x.Type == (int)filters.DebitType);
            }
            if (filters.CardId != default(Guid))
            {
                query = query.Where(x => x.CardId == filters.CardId);
            }
            if (!string.IsNullOrEmpty(filters.Email))
            {
                var emailLower = filters.Email.ToLower();
                query = query.Where(x => x.User != null && x.User.Email.ToLower().Contains(emailLower));
            }

            if (!string.IsNullOrEmpty(filters.Service))
            {
                var productIds = _serviceCustomerSite.GetCommercesDebit(search: filters.Service)
                    .SelectMany(x => x.ProductosListDto)
                    .Distinct()
                    .Select(x => x.DebitProductid);

                if (productIds.Any())
                {
                    query = query.Where(x => productIds.Contains(x.DebitProductId));
                }
                else
                {
                    query = Enumerable.Empty<DebitRequest>().AsQueryable();
                }
            }

            return query;
        }

        #endregion Private methods
    }
}
