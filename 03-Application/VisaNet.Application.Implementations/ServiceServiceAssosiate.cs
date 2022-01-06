using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Net;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Implementations.ExtensionMethods;
using VisaNet.Application.Implementations.ExtensionMethods.Filters;
using VisaNet.Application.Interfaces;
using VisaNet.Common.AzureUpload;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Logging.Services;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.ReportsModel;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;
using SortDirection = VisaNet.Domain.EntitiesDtos.TableFilters.SortDirection;

namespace VisaNet.Application.Implementations
{
    public class ServiceServiceAssosiate : BaseService<ServiceAssociated, ServiceAssociatedDto>, IServiceServiceAssosiate
    {
        private readonly ILoggerService _loggerService;
        private readonly IRepositoryPayment _repositoryPayment;
        private readonly IRepositoryApplicationUser _repositoryApplicationUser;
        private readonly IRepositoryCard _repositoryCard;
        private readonly IRepositoryBin _repositoryBin;
        private readonly IServiceAnalyzeCsCall _serviceAnalyzeCsCall;
        private readonly IRepositoryServiceAsociated _repositoryServiceAsociated;
        private readonly IServiceService _serviceService;
        private readonly IServiceApplicationUser _serviceApplicationUser;
        private readonly IServiceWebhookRegistration _serviceWebhookRegistration;
        private readonly IServiceProcessHistory _serviceProcessHistory;
        private readonly IServiceExternalNotification _serviceExternalNotification;

        private string _folderBlob = ConfigurationManager.AppSettings["AzureServicesImagesUrl"];

        public ServiceServiceAssosiate(IRepositoryServiceAsociated repository, ILoggerService loggerService,
            IRepositoryPayment repositoryPayment, IRepositoryApplicationUser repositoryApplicationUser,
            IRepositoryCard repositoryCard, IRepositoryBin repositoryBin, IServiceAnalyzeCsCall serviceAnalyzeCsCall,
            IServiceService serviceService, IServiceApplicationUser serviceApplicationUser,
            IServiceWebhookRegistration serviceWebhookRegistration, IServiceProcessHistory serviceProcessHistory,
            IServiceExternalNotification serviceExternalNotification)
            : base(repository)
        {
            _repositoryServiceAsociated = repository;
            _loggerService = loggerService;
            _repositoryPayment = repositoryPayment;
            _repositoryApplicationUser = repositoryApplicationUser;
            _repositoryCard = repositoryCard;
            _repositoryBin = repositoryBin;
            _serviceAnalyzeCsCall = serviceAnalyzeCsCall;
            _serviceService = serviceService;
            _serviceApplicationUser = serviceApplicationUser;
            _serviceWebhookRegistration = serviceWebhookRegistration;
            _serviceProcessHistory = serviceProcessHistory;
            _serviceExternalNotification = serviceExternalNotification;
        }

        public override IQueryable<ServiceAssociated> GetDataForTable()
        {
            throw new NotImplementedException();
        }

        public override ServiceAssociatedDto Converter(ServiceAssociated entity)
        {
            var saDto = new ServiceAssociatedDto()
            {
                Active = entity.Active,
                Id = entity.Id,
                AutomaticPaymentDtoId = entity.AutomaticPaymentId,
                Description = entity.Description,
                ServiceId = entity.ServiceId,
                UserId = entity.RegisteredUserId,
                NotificationConfigDtoId = entity.NotificationConfigId,
                ReferenceNumber = entity.ReferenceNumber,
                ReferenceNumber2 = entity.ReferenceNumber2,
                ReferenceNumber3 = entity.ReferenceNumber3,
                ReferenceNumber4 = entity.ReferenceNumber4,
                ReferenceNumber5 = entity.ReferenceNumber5,
                ReferenceNumber6 = entity.ReferenceNumber6,
                Enabled = entity.Enabled,
                DefaultCardId = entity.DefaultCardId,
                IdUserExternal = entity.IdUserExternal,
            };
            if (entity.DefaultCard != null)
            {
                saDto.DefaultCard = new CardDto()
                {
                    MaskedNumber = entity.DefaultCard.MaskedNumber,
                    Id = entity.DefaultCard.Id,
                    DueDate = entity.DefaultCard.DueDate,
                    PaymentToken = entity.DefaultCard.PaymentToken,
                    ExternalId = entity.DefaultCard.ExternalId,
                    Description = entity.DefaultCard.Description
                };
            }
            if (entity.Service != null)
            {
                saDto.ServiceDto = new ServiceDto()
                {
                    ReferenceParamName = entity.Service.ReferenceParamName,
                    ReferenceParamName2 = entity.Service.ReferenceParamName2,
                    ReferenceParamName3 = entity.Service.ReferenceParamName3,
                    ReferenceParamName4 = entity.Service.ReferenceParamName4,
                    ReferenceParamName5 = entity.Service.ReferenceParamName5,
                    ReferenceParamName6 = entity.Service.ReferenceParamName6,
                    Id = entity.ServiceId,
                    Name = entity.Service.Name,
                    //CreditCard = entity.Service.CreditCard,
                    //CreditCardInternational = entity.Service.CreditCardInternational,
                    //DebitCard = entity.Service.DebitCard,
                    //DebitCardInternational = entity.Service.DebitCardInternational,
                    UrlName = entity.Service.UrlName,

                    EnableAutomaticPayment = entity.Service.EnableAutomaticPayment,
                    EnablePrivatePayment = entity.Service.EnablePrivatePayment,
                    EnablePublicPayment = entity.Service.EnablePublicPayment,
                    Container = entity.Service.Container,
                    ExternalUrlAdd = entity.Service.ExternalUrlAdd,
                    ExternalUrlRemove = entity.Service.ExternalUrlRemove,
                    CertificateThumbprintVisa = entity.Service.CertificateThumbprintVisa,
                    CertificateThumbprintExternal = entity.Service.CertificateThumbprintExternal,

                    AskUserForReferences = entity.Service.AskUserForReferences,
                    AllowSelectContentAssociation = entity.Service.AllowSelectContentAssociation,
                    AllowSelectContentPayment = entity.Service.AllowSelectContentPayment,
                    AllowMultipleCards = entity.Service.AllowMultipleCards,
                };

                if (entity.Service.ServiceContainer != null)
                {
                    saDto.ServiceDto.ServiceContainerId = entity.Service.ServiceContainer.Id;
                    saDto.ServiceDto.ServiceContainerDto = new ServiceDto
                    {
                        Id = entity.Service.ServiceContainer.Id,
                        ReferenceParamName = entity.Service.ServiceContainer.ReferenceParamName,
                        ReferenceParamName2 = entity.Service.ServiceContainer.ReferenceParamName2,
                        ReferenceParamName3 = entity.Service.ServiceContainer.ReferenceParamName3,
                        ReferenceParamName4 = entity.Service.ServiceContainer.ReferenceParamName4,
                        ReferenceParamName5 = entity.Service.ServiceContainer.ReferenceParamName5,
                        ReferenceParamName6 = entity.Service.ServiceContainer.ReferenceParamName6,
                        Name = entity.Service.ServiceContainer.Name,
                        UrlName = entity.Service.ServiceContainer.UrlName,
                        EnableAutomaticPayment = entity.Service.ServiceContainer.EnableAutomaticPayment,
                        EnablePrivatePayment = entity.Service.ServiceContainer.EnablePrivatePayment,
                        EnablePublicPayment = entity.Service.ServiceContainer.EnablePublicPayment,
                        Container = entity.Service.ServiceContainer.Container,
                        ExternalUrlAdd = entity.Service.ServiceContainer.ExternalUrlAdd,
                        ExternalUrlRemove = entity.Service.ServiceContainer.ExternalUrlRemove,
                        CertificateThumbprintVisa = entity.Service.ServiceContainer.CertificateThumbprintVisa,
                        CertificateThumbprintExternal = entity.Service.ServiceContainer.CertificateThumbprintExternal,
                        AskUserForReferences = entity.Service.ServiceContainer.AskUserForReferences,
                        AllowSelectContentAssociation = entity.Service.ServiceContainer.AllowSelectContentAssociation,
                        AllowSelectContentPayment = entity.Service.ServiceContainer.AllowSelectContentPayment,
                        AllowMultipleCards = entity.Service.ServiceContainer.AllowMultipleCards,
                    };
                }

            }
            if (entity.Service != null && entity.Service.ServiceGateways != null && entity.Service.ServiceGateways.Any())
            {
                saDto.ServiceDto.ServiceGatewaysDto = entity.Service.ServiceGateways.Select(s => new ServiceGatewayDto()
                {
                    Active = s.Active,
                    GatewayId = s.GatewayId,
                    Gateway = new GatewayDto()
                    {
                        Enum = s.Gateway != null ? s.Gateway.Enum : 0,
                        Name = s.Gateway != null ? s.Gateway.Name : string.Empty,
                    }
                }).ToList();
            }
            if (entity.NotificationConfig != null)
            {
                var notiDto = new NotificationConfigDto()
                {
                    Id = entity.NotificationConfigId,
                    DaysBeforeDueDate = entity.NotificationConfig.DaysBeforeDueDate,
                    BeforeDueDateConfigDto = entity.NotificationConfig.BeforeDueDateConfig != null ? new DaysBeforeDueDateConfigDto()
                    {
                        Email = entity.NotificationConfig.BeforeDueDateConfig.Email,
                        Sms = entity.NotificationConfig.BeforeDueDateConfig.Sms,
                        Web = entity.NotificationConfig.BeforeDueDateConfig.Web,
                        Label = PresentationWebStrings.Service_Step2_BillExpiration
                    } : null,
                    ExpiredBillDto = new ExpiredBillDto()
                    {
                        Email = entity.NotificationConfig.ExpiredBill.Email,
                        Sms = entity.NotificationConfig.ExpiredBill.Sms,
                        Web = entity.NotificationConfig.ExpiredBill.Web,
                        Label = PresentationWebStrings.Service_Step2_BillExpired
                    },
                    NewBillDto = new NewBillDto()
                    {
                        Email = entity.NotificationConfig.NewBill.Email,
                        Sms = entity.NotificationConfig.NewBill.Sms,
                        Web = entity.NotificationConfig.NewBill.Web,
                        Label = PresentationWebStrings.Service_Step2_BillMade
                    },
                    FailedAutomaticPaymentDto = new FailedAutomaticPaymentDto()
                    {
                        Email =
                                                                      entity.NotificationConfig.FailedAutomaticPayment
                                                                      .Email,
                        Sms =
                                                                      entity.NotificationConfig.FailedAutomaticPayment
                                                                      .Sms,
                        Web =
                                                                      entity.NotificationConfig.FailedAutomaticPayment
                                                                      .Web,
                        Label = PresentationWebStrings.Service_Step2_FailedAutomaticPayment
                    },
                    SuccessPaymentDto = new SuccessPaymentDto()
                    {
                        Email = entity.NotificationConfig.SuccessPayment.Email,
                        Sms = entity.NotificationConfig.SuccessPayment.Sms,
                        Web = entity.NotificationConfig.SuccessPayment.Web,
                        Label = PresentationWebStrings.Service_Step2_PaymentMade
                    }
                };
                saDto.NotificationConfigDto = notiDto;
            }

            if (entity.AutomaticPayment != null)
            {
                saDto.AutomaticPaymentDto = new AutomaticPaymentDto()
                {
                    Id = entity.AutomaticPayment.Id,
                    DaysBeforeDueDate = entity.AutomaticPayment.DaysBeforeDueDate,
                    Maximum = entity.AutomaticPayment.Maximum,
                    Quotas = entity.AutomaticPayment.Quotas,
                    UnlimitedQuotas = entity.AutomaticPayment.UnlimitedQuotas,
                    QuotasDone = entity.AutomaticPayment.QuotasDone,
                    SuciveAnnualPatent = entity.AutomaticPayment.SuciveAnnualPatent,
                    UnlimitedAmount = entity.AutomaticPayment.UnlimitedAmount,
                    LastRunDate = entity.AutomaticPayment.LastRunDate,
                    LastRunResult = (PaymentResultTypeDto?)entity.AutomaticPayment.LastRunResult,
                    LastSuccessfulPaymentDate = entity.AutomaticPayment.LastSuccessfulPaymentDate,
                    LastSuccessfulPaymentIteration = entity.AutomaticPayment.LastSuccessfulPaymentIteration,
                    LastErrorDate = entity.AutomaticPayment.LastErrorDate,
                    LastErrorResult = (PaymentResultTypeDto?)entity.AutomaticPayment.LastErrorResult
                };
            }

            if (entity.RegisteredUser != null)
            {
                saDto.RegisteredUserDto = new ApplicationUserDto
                {
                    Id = entity.RegisteredUser.Id,
                    Name = entity.RegisteredUser.Name,
                    Surname = entity.RegisteredUser.Surname,
                    Email = entity.RegisteredUser.Email,
                    Address = entity.RegisteredUser.Address,
                    IdentityNumber = entity.RegisteredUser.IdentityNumber,
                    PhoneNumber = entity.RegisteredUser.PhoneNumber,
                    MobileNumber = entity.RegisteredUser.MobileNumber,
                    CallCenterKey = entity.RegisteredUser.CallCenterKey,
                    MembreshipIdentifier = entity.RegisteredUser.MembershipIdentifier,
                };
                if (entity.RegisteredUser.MembershipIdentifierObj != null)
                {
                    saDto.RegisteredUserDto.MembershipIdentifierObj = new MembershipUserDto()
                    {
                        Blocked = entity.RegisteredUser.MembershipIdentifierObj.Blocked,
                        Active = entity.RegisteredUser.MembershipIdentifierObj.Active
                    };
                }
            }

            if (entity.Cards != null && entity.Cards.Any())
            {
                saDto.CardDtos = entity.Cards.Select(s => new CardDto()
                {
                    Active = s.Active,
                    MaskedNumber = s.MaskedNumber,
                    ExternalId = s.ExternalId,
                    Id = s.Id,
                    PaymentToken = s.PaymentToken,
                    Description = s.Description
                }).ToList();
            }
            return saDto;
        }

        public override ServiceAssociated Converter(ServiceAssociatedDto entity)
        {
            var sa = new ServiceAssociated()
            {
                Active = entity.Active,
                Id = entity.Id,
                AutomaticPaymentId = entity.AutomaticPaymentDtoId,
                Description = entity.Description,
                ServiceId = entity.ServiceId,
                RegisteredUserId = entity.UserId,
                NotificationConfigId = entity.NotificationConfigDtoId,
                ReferenceNumber = entity.ReferenceNumber,
                ReferenceNumber2 = entity.ReferenceNumber2,
                ReferenceNumber3 = entity.ReferenceNumber3,
                ReferenceNumber4 = entity.ReferenceNumber4,
                ReferenceNumber5 = entity.ReferenceNumber5,
                ReferenceNumber6 = entity.ReferenceNumber6,
                Enabled = entity.Enabled,
                DefaultCardId = entity.DefaultCardId,
                IdUserExternal = entity.IdUserExternal,
            };

            if (entity.NotificationConfigDto != null)
            {
                var noti = new NotificationConfig()
                {
                    Id = entity.NotificationConfigDtoId,
                    DaysBeforeDueDate = entity.NotificationConfigDto.DaysBeforeDueDate,
                    BeforeDueDateConfig = new DaysBeforeDueDateConfig()
                    {
                        Email =
                            entity.NotificationConfigDto.BeforeDueDateConfigDto.Email,
                        Sms = entity.NotificationConfigDto.BeforeDueDateConfigDto.Sms,
                        Web = entity.NotificationConfigDto.BeforeDueDateConfigDto.Web
                    },
                    ExpiredBill = new ExpiredBill()
                    {
                        Email = entity.NotificationConfigDto.ExpiredBillDto.Email,
                        Sms = entity.NotificationConfigDto.ExpiredBillDto.Sms,
                        Web = entity.NotificationConfigDto.ExpiredBillDto.Web
                    },
                    NewBill = new NewBill()
                    {
                        Email = entity.NotificationConfigDto.NewBillDto.Email,
                        Sms = entity.NotificationConfigDto.NewBillDto.Sms,
                        Web = entity.NotificationConfigDto.NewBillDto.Web
                    },
                    FailedAutomaticPayment = new FailedAutomaticPayment()
                    {
                        Email =
                            entity.NotificationConfigDto.FailedAutomaticPaymentDto
                            .Email,
                        Sms =
                            entity.NotificationConfigDto.FailedAutomaticPaymentDto
                            .Sms,
                        Web =
                            entity.NotificationConfigDto.FailedAutomaticPaymentDto
                            .Web
                    },
                    SuccessPayment = new SuccessPayment()
                    {
                        Email = entity.NotificationConfigDto.SuccessPaymentDto.Email,
                        Sms = entity.NotificationConfigDto.SuccessPaymentDto.Sms,
                        Web = entity.NotificationConfigDto.SuccessPaymentDto.Web
                    }
                };
                if (noti.Id == Guid.Empty)
                    noti.GenerateNewIdentity();

                sa.NotificationConfigId = noti.Id;
                sa.NotificationConfig = noti;
            }
            if (sa.Id == Guid.Empty)
                sa.GenerateNewIdentity();

            if (sa.NotificationConfig != null)
                sa.NotificationConfig.ServiceAsociatedId = sa.Id;

            if (entity.CardDtos != null && entity.CardDtos.Any())
            {
                sa.Cards = entity.CardDtos.Select(s => new Card()
                {
                    Active = s.Active,
                    MaskedNumber = s.MaskedNumber,
                    ExternalId = s.ExternalId,
                    Id = s.Id,
                    PaymentToken = s.PaymentToken,
                    Name = s.Name,
                    Description = s.Description,
                    CybersourceTransactionId = s.CybersourceTransactionId
                }).ToList();
            }
            return sa;
        }

        public IEnumerable<ServiceAssociatedDto> GetByServiceId(Guid id)
        {
            return GetDataForTable(new ServiceFilterAssosiateDto { ServiceAssociatedId = id });
        }

        public IEnumerable<ServiceAssociatedDto> GetDataForTable(ServiceFilterAssosiateDto filters)
        {
            var query = Repository.AllNoTracking(null,
                s => s.Service,
                s => s.Service.ServiceContainer,
                s => s.Service.ServiceCategory,
                s => s.Service.ServiceGateways,
                s => s.Service.ServiceGateways.Select(x => x.Gateway),
                s => s.AutomaticPayment,
                s => s.DefaultCard,
                s => s.Cards,
                s => s.NotificationConfig);

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => sc.Service.Name.ToLower().Contains(filters.GenericSearch.ToLower()));

            if (!string.IsNullOrEmpty(filters.Service))
                query = query.Where(sc => sc.Service.Name.Contains(filters.Service) ||
                                          sc.Service.Tags.Contains(filters.Service) ||
                                          sc.Description.Contains(filters.Service));

            if (!string.IsNullOrEmpty(filters.ReferenceNumber))
                query = query.Where(sc => sc.ReferenceNumber.Contains(filters.ReferenceNumber) ||
                                          sc.ReferenceNumber2.Contains(filters.ReferenceNumber) ||
                                          sc.ReferenceNumber3.Contains(filters.ReferenceNumber) ||
                                          sc.ReferenceNumber4.Contains(filters.ReferenceNumber) ||
                                          sc.ReferenceNumber5.Contains(filters.ReferenceNumber) ||
                                          sc.ReferenceNumber6.Contains(filters.ReferenceNumber));

            if (filters.ServiceAssociatedId != default(Guid))
                query = query.Where(s => s.Id == filters.ServiceAssociatedId);

            if (filters.WithAutomaticPaymentsInt == 1)
                query = query.Where(x => x.AutomaticPaymentId != Guid.Empty && x.AutomaticPaymentId != null);

            if (filters.WithAutomaticPaymentsInt == 2)
                query = query.Where(x => x.AutomaticPaymentId == Guid.Empty || x.AutomaticPaymentId == null);

            if (!filters.IncludeDeleted)
            {
                query = query.Where(sc => sc.Active);
            }

            query = query.Where(sc => sc.RegisteredUserId == filters.UserId).OrderBy(sc => sc.Service.Name);

            query = query.Skip(filters.DisplayStart);

            if (filters.DisplayLength.HasValue)
                query = query.Take(filters.DisplayLength.Value);

            var includeFilters = new ServiceAssociatedIncludeFilters
            {
                IncludeApplicationUserInfo = true,
                IncludeAutomaticPaymentInfo = true,
                IncludeCardListInfo = true,
                IncludeDefaultCardInfo = true,
                IncludeNotificationConfigInfo = true,
                IncludeServiceInfo = true,
                IncludeServiceGatewaysInfo = true,
                IncludeServiceCategoryInfo = true,
                IncludeServiceContainerInfo = false
            };
            var queryList = query.ToList();
            var associatedServicesList = queryList.Select(t => t.Convert(includeFilters)).ToList();

            //Carga de notificaciones del servicio 
            //TODO: Esta todo hardcoded en true, lo tomé de los demás lugares en que se carga esto en el DTO y está igual. Se debería calcular en función del servicio y no siempre ser true.
            for (int i = 0; i < associatedServicesList.Count; i++)
            {
                var associatedService = associatedServicesList[i];
                var serviceDto = associatedService.ServiceDto;

                var gateways = serviceDto.ServiceGatewaysDto.Any(x => (x.Gateway.Enum == (int)GatewayEnumDto.Banred && x.Active)
                        || (x.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc && x.Active) || (x.Gateway.Enum == (int)GatewayEnumDto.Carretera && x.Active)
                        || (x.Gateway.Enum == (int)GatewayEnumDto.Geocom && x.Active) || (x.Gateway.Enum == (int)GatewayEnumDto.Sucive && x.Active));

                serviceDto.NotificationConfigDto = new NotificationConfigDto()
                {
                    DaysBeforeDueDate = 5,
                    SuccessPaymentDto = new SuccessPaymentDto()
                    {
                        Email = true,
                        Label = PresentationWebStrings.Service_Step2_PaymentMade
                    },
                    BeforeDueDateConfigDto = gateways ? new DaysBeforeDueDateConfigDto()
                    {
                        Email = true,
                        Label = PresentationWebStrings.Service_Step2_BillExpiration
                    } : null,
                    ExpiredBillDto = gateways ? new ExpiredBillDto()
                    {
                        Email = true,
                        Label = PresentationWebStrings.Service_Step2_BillExpired
                    } : null,
                    FailedAutomaticPaymentDto = gateways ? new FailedAutomaticPaymentDto()
                    {
                        Email = true,
                        Label = PresentationWebStrings.Service_Step2_FailedAutomaticPayment
                    } : null,
                    NewBillDto = gateways ? new NewBillDto()
                    {
                        Email = true,
                        Label = PresentationWebStrings.Service_Step2_BillMade
                    } : null,
                };
            }

            return associatedServicesList;
        }

        public IEnumerable<ServiceAssociatedDto> ReportsServicesAssociatedData(ReportsServicesAssociatedFilterDto filters)
        {
            var query = Repository.AllNoTracking(null, s => s.Service, s => s.Service.ServiceCategory, s => s.DefaultCard, s => s.RegisteredUser);

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => sc.RegisteredUser.Email.ToLower().Contains(filters.GenericSearch.ToLower()) ||
                                          sc.RegisteredUser.Name.ToLower().Contains(filters.GenericSearch.ToLower()) ||
                                          sc.RegisteredUser.Surname.ToLower().Contains(filters.GenericSearch.ToLower()) ||
                                          sc.Service.Name.ToLower().Contains(filters.GenericSearch.ToLower()) ||
                                          sc.Service.ServiceCategory.Name.ToLower().Contains(filters.GenericSearch.ToLower()));

            if (!string.IsNullOrEmpty(filters.ClientEmail))
                query = query.Where(sc => sc.RegisteredUser.Email.ToLower().Contains(filters.ClientEmail.ToLower()));

            if (!string.IsNullOrEmpty(filters.ClientName))
                query = query.Where(sc => sc.RegisteredUser.Name.ToLower().Contains(filters.ClientName.ToLower()));

            if (!string.IsNullOrEmpty(filters.ClientSurname))
                query = query.Where(sc => sc.RegisteredUser.Surname.ToLower().Contains(filters.ClientSurname.ToLower()));

            //if (filters.ServiceId != default(Guid))
            //    query = query.Where(s => s.ServiceId == filters.ServiceId);

            //if (filters.ServiceCategoryId != default(Guid))
            //    query = query.Where(s => s.Service.ServiceCategoryId == filters.ServiceCategoryId);


            return query.Select(t => new ServiceAssociatedDto
            {
                RegisteredUserDto = new ApplicationUserDto
                {
                    Email = t.RegisteredUser != null ? t.RegisteredUser.Email : "",
                    Name = t.RegisteredUser != null ? t.RegisteredUser.Name : "",
                    Surname = t.RegisteredUser != null ? t.RegisteredUser.Surname : "",
                    IdentityNumber = t.RegisteredUser != null ? t.RegisteredUser.IdentityNumber : "",
                    MobileNumber = t.RegisteredUser != null ? t.RegisteredUser.MobileNumber : "",
                    PhoneNumber = t.RegisteredUser != null ? t.RegisteredUser.PhoneNumber : "",
                    Address = t.RegisteredUser != null ? t.RegisteredUser.Address : ""
                },
                ServiceDto = new ServiceDto()
                {
                    Name = t.Service.Name,
                    ServiceCategory = new ServiceCategoryDto
                    {
                        Name = t.Service.ServiceCategory != null ? t.Service.ServiceCategory.Name : ""
                    },
                    EnableAutomaticPayment = t.Service.EnableAutomaticPayment,
                    EnablePublicPayment = t.Service.EnablePublicPayment,
                    EnablePrivatePayment = t.Service.EnablePrivatePayment,
                    EnableMultipleBills = t.Service.EnableMultipleBills,
                    Container = t.Service.Container,
                },
                ReferenceNumber = t.ReferenceNumber,
                Description = t.Description,
                DefaultCardId = t.DefaultCardId,
                IdUserExternal = t.IdUserExternal,
                DefaultCard = new CardDto
                {
                    MaskedNumber = t.DefaultCard != null ? t.DefaultCard.MaskedNumber : "",
                    DueDate = t.DefaultCard != null ? t.DefaultCard.DueDate : default(DateTime)
                },
                AutomaticPaymentDtoId = t.AutomaticPaymentId,
                Enabled = t.Enabled,
            }).ToList();

        }

        public IEnumerable<ServiceAssociatedViewDto> ReportsServicesAssociatedDataFromDbView(ReportsServicesAssociatedFilterDto filters)
        {
            var query = "SELECT * FROM dbo.ReportServiceAssociatedView ";
            var where = "WHERE 1=1 ";

            if (!string.IsNullOrEmpty(filters.CreationDateFromString))
            {
                var from = DateTime.Parse(filters.CreationDateFromString).ToString("MM/dd/yyyy");
                where += "AND (CreationDate >= '" + from + "') ";
            }

            if (!string.IsNullOrEmpty(filters.CreationDateToString))
            {
                var to = DateTime.Parse(filters.CreationDateToString, new CultureInfo("es-UY")).AddDays(1).ToString("MM/dd/yyyy");
                where += "AND (CreationDate < '" + to + "') ";
            }

            if (!string.IsNullOrEmpty(filters.ClientEmail))
                where += "AND (ClientEmail LIKE '%" + filters.ClientEmail + "%') ";

            if (!string.IsNullOrEmpty(filters.ClientName))
                where += "AND (ClientName LIKE '%" + filters.ClientName + "%') ";

            if (!string.IsNullOrEmpty(filters.ClientSurname))
                where += "AND (ClientSurname LIKE '%" + filters.ClientSurname + "%') ";

            if (!string.IsNullOrEmpty(filters.ServiceNameAndDesc))
                where += "AND (ServiceNameAndDesc LIKE '%" + filters.ServiceNameAndDesc + "%') ";

            if (filters.ServiceCategoryId != default(Guid))
                where += "AND (ServiceCategoryId = '" + filters.ServiceCategoryId + "') ";

            if (filters.Enabled == 1)
                where += "AND (Enabled = '" + 1 + "') ";
            else if (filters.Enabled == 2)
                where += "AND (Enabled = '" + 0 + "') ";

            if (filters.HasAutomaticPayment == 1)
                where += "AND (AutomaticPaymentId IS NOT NULL) ";
            else if (filters.HasAutomaticPayment == 2)
                where += "AND (AutomaticPaymentId IS NULL) ";

            #region Sort

            var orderBy = "";
            switch (filters.OrderBy)
            {
                case "0":
                    orderBy = "ORDER BY ClientEmail ";
                    break;
                case "1":
                    orderBy = "ORDER BY ClientName ";
                    break;
                case "2":
                    orderBy = "ORDER BY ClientSurname ";
                    break;
                case "3":
                    orderBy = "ORDER BY ServiceNameAndDesc ";
                    break;
                case "4":
                    orderBy = "ORDER BY ServiceCategory ";
                    break;
                case "5":
                    orderBy = "ORDER BY ReferenceNumber ";
                    break;
                case "6":
                    orderBy = "ORDER BY Enabled ";
                    break;
                case "7":
                    orderBy = "ORDER BY Active ";
                    break;
                case "8":
                    orderBy = "ORDER BY AutomaticPaymentId ";
                    break;
                case "9":
                    orderBy = "ORDER BY DefaultCardMask ";
                    break;
                case "10":
                    orderBy = "ORDER BY PaymentsCount ";
                    break;
                case "11":
                    orderBy = "ORDER BY CreationDate ";
                    break;
                case "12":
                    orderBy = "ORDER BY LastModificationDate ";
                    break;
                default:
                    orderBy = "ORDER BY ClientEmail ";
                    break;
            }

            if (filters.SortDirection == SortDirection.Desc)
                orderBy += "DESC";
            #endregion

            using (var context = new AppContext())
            {
                if (filters.DisplayLength != null)
                {
                    return context.Database.SqlQuery<ServiceAssociatedViewDto>(string.Format("WITH aux AS ( SELECT *, ROW_NUMBER() OVER ({1}) AS RowNumber FROM [dbo].[ReportServiceAssociatedView] {0} ) SELECT * FROM aux WHERE RowNumber BETWEEN {2} AND {3}", where, orderBy, filters.DisplayStart + 1, filters.DisplayStart + (int)filters.DisplayLength)).ToList();
                }
                return context.Database.SqlQuery<ServiceAssociatedViewDto>(query + where + orderBy).ToList();
            }
        }

        public int ReportsServicesAssociatedDataCount(ReportsServicesAssociatedFilterDto filters)
        {
            var query = "SELECT COUNT(0) FROM dbo.ReportServiceAssociatedView WHERE 1=1 ";
            //return co
            if (!string.IsNullOrEmpty(filters.CreationDateFromString))
            {
                var from = DateTime.Parse(filters.CreationDateFromString).ToString("MM/dd/yyyy");
                query += "AND (CreationDate >= '" + from + "') ";
            }

            if (!string.IsNullOrEmpty(filters.CreationDateToString))
            {
                var to = DateTime.Parse(filters.CreationDateToString, new CultureInfo("es-UY")).AddDays(1).ToString("MM/dd/yyyy");
                query += "AND (CreationDate < '" + to + "') ";
            }

            if (!string.IsNullOrEmpty(filters.ClientEmail))
                query += "AND (ClientEmail LIKE '%" + filters.ClientEmail + "%') ";

            if (!string.IsNullOrEmpty(filters.ClientName))
                query += "AND (ClientName LIKE '%" + filters.ClientName + "%') ";

            if (!string.IsNullOrEmpty(filters.ClientSurname))
                query += "AND (ClientSurname LIKE '%" + filters.ClientSurname + "%') ";

            if (!string.IsNullOrEmpty(filters.ServiceNameAndDesc))
                query += "AND (ServiceNameAndDesc LIKE '%" + filters.ServiceNameAndDesc + "%') ";

            if (filters.ServiceCategoryId != default(Guid))
                query += "AND (ServiceCategoryId = '" + filters.ServiceCategoryId + "') ";

            if (filters.Enabled == 1)
                query += "AND (Enabled = '" + 1 + "') ";
            else if (filters.Enabled == 2)
                query += "AND (Enabled = '" + 0 + "') ";

            if (filters.HasAutomaticPayment == 1)
                query += "AND (AutomaticPaymentId IS NOT NULL) ";
            else if (filters.HasAutomaticPayment == 2)
                query += "AND (AutomaticPaymentId IS NULL) ";

            using (var context = new AppContext())
            {
                return context.Database.SqlQuery<int>(query).First();
            }
        }

        public void AddAutomaticPayment(AutomaticPaymentDto dto)
        {

            if (!dto.UnlimitedAmount && dto.Maximum < 1)
            {
                throw new BusinessException(CodeExceptions.AUTOMATIC_PAYMENT_AMOUNT_LESS_THAN_ONE);
            }

            if (dto.DaysBeforeDueDate < 1 || dto.DaysBeforeDueDate > 5)
            {
                throw new BusinessException(CodeExceptions.AUTOMATIC_PAYMENT_DAYS_WRONG);
            }

            Repository.ContextTrackChanges = true;
            var entity = Repository.GetById(dto.ServiceAssosiateId, s => s.AutomaticPayment);

            if (entity.AutomaticPaymentId == null)
            {
                entity.AutomaticPayment = new AutomaticPayment();
                entity.AutomaticPayment.GenerateNewIdentity();
            }
            entity.AutomaticPayment.DaysBeforeDueDate = dto.DaysBeforeDueDate;
            entity.AutomaticPayment.Maximum = dto.Maximum;
            entity.AutomaticPayment.Quotas = dto.Quotas;
            entity.AutomaticPayment.UnlimitedQuotas = dto.UnlimitedQuotas;

            entity.AutomaticPayment.SuciveAnnualPatent = dto.SuciveAnnualPatent;

            entity.AutomaticPayment.UnlimitedAmount = dto.UnlimitedAmount;


            Repository.Edit(entity);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public void AutomaticPaymentAddQuotasDone(AutomaticPaymentDto dto)
        {
            Repository.ContextTrackChanges = true;
            var entity = Repository.GetById(dto.ServiceAssosiateId, s => s.AutomaticPayment, s => s.Service);
            if (entity.AutomaticPaymentId != null)
            {
                entity.AutomaticPayment.QuotasDone = dto.QuotasDone;
                Repository.Edit(entity);
                Repository.Save();

                var service = entity.Service.Name;
                if (string.IsNullOrEmpty(entity.Description) == false)
                    service = string.Format("{0} - {1}", service, entity.Description);

                _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.VisaNet,
                    string.Format(LogStrings.Payment_Automatic, service));
            }
            Repository.ContextTrackChanges = false;
        }

        public override ServiceAssociatedDto Create(ServiceAssociatedDto entity, bool returnEntity = false)
        {
            return CreateServiceAssociated(entity);
        }

        public ServiceAssociatedDto CreateWithoutExternalNotification(ServiceAssociatedDto entity)
        {
            const bool notifyExternal = false;
            return CreateServiceAssociated(entity, notifyExternal);
        }

        private ServiceAssociatedDto CreateServiceAssociated(ServiceAssociatedDto entity, bool? notifyExternal = null)
        {
            entity.Active = true;
            //si no tiene idusuario externo lo creo
            if (!entity.IdUserExternal.HasValue)
            {
                entity.IdUserExternal = Guid.NewGuid();
            }

            entity.Active = true;
            entity.Enabled = true;

            var dto = base.Create(entity, true);

            //en este metodo envio la info a agentes externos si es neceasrio
            var result = SetUpServiceCards(dto.Id, entity.DefaultCardId, true, entity.OperationId, notifyExternal);
            //DefaultCard(dto.RegisteredUserDtoId,dto.Id,entity.DefaultCardId);

            if (result)
            {
                dto = GetById(dto.Id, s => s.DefaultCard, s => s.Service);
                _loggerService.CreateLog(LogType.Info, LogOperationType.ServiceAssociated, LogCommunicationType.VisaNet, entity.UserId, string.Format(LogStrings.ServiceAssosiated_Create,
                entity.ServiceDto != null ? entity.ServiceDto.Name : "",
                entity.RegisteredUserDto != null ? entity.RegisteredUserDto.Email : ""));
                return dto;
            }
            DeleteService(dto.Id, false);
            return null;
        }

        public override void Edit(ServiceAssociatedDto entity)
        {
            Repository.ContextTrackChanges = true;
            var entityDb = Repository.GetById(entity.Id, s => s.NotificationConfig, s => s.AutomaticPayment);

            //Bussines logic validations
            var user = _repositoryApplicationUser.GetById(entity.UserId, x => x.Cards);

            //The card associated to the service must be a user's card
            if (entity.DefaultCardId != Guid.Empty && user.Cards.All(c => c.Id != entity.DefaultCardId))
                throw new BusinessException(CodeExceptions.INVALID_CARD_ASOCIATION);

            var isPaymentDoneWithServiceAssosiated = _repositoryPayment.AllNoTracking().Any(p => p.ServiceAssosiatedId == entity.Id);


            //Para cambiar el estado hay un metodo (ChangeState).
            entityDb.Active = true;
            entityDb.Description = entity.Description;

            //si no existen pagos realizados entonces se pueden modificar los nros de referencia.
            //en otro caso no.
            if (isPaymentDoneWithServiceAssosiated == false)
            {
                entityDb.ReferenceNumber = entity.ReferenceNumber;
                entityDb.ReferenceNumber2 = entity.ReferenceNumber2;
                entityDb.ReferenceNumber3 = entity.ReferenceNumber3;
                entityDb.ReferenceNumber4 = entity.ReferenceNumber4;
                entityDb.ReferenceNumber5 = entity.ReferenceNumber5;
                entityDb.ReferenceNumber6 = entity.ReferenceNumber6;
            }


            entityDb.NotificationConfig.DaysBeforeDueDate = entity.NotificationConfigDto.DaysBeforeDueDate;
            entityDb.NotificationConfig.BeforeDueDateConfig.Email = entity.NotificationConfigDto.BeforeDueDateConfigDto.Email;
            entityDb.NotificationConfig.BeforeDueDateConfig.Sms = entity.NotificationConfigDto.BeforeDueDateConfigDto.Sms;
            entityDb.NotificationConfig.BeforeDueDateConfig.Web = entity.NotificationConfigDto.BeforeDueDateConfigDto.Web;

            entityDb.NotificationConfig.ExpiredBill.Email = entity.NotificationConfigDto.ExpiredBillDto.Email;
            entityDb.NotificationConfig.ExpiredBill.Sms = entity.NotificationConfigDto.ExpiredBillDto.Sms;
            entityDb.NotificationConfig.ExpiredBill.Web = entity.NotificationConfigDto.ExpiredBillDto.Web;

            entityDb.NotificationConfig.NewBill.Email = entity.NotificationConfigDto.NewBillDto.Email;
            entityDb.NotificationConfig.NewBill.Sms = entity.NotificationConfigDto.NewBillDto.Sms;
            entityDb.NotificationConfig.NewBill.Web = entity.NotificationConfigDto.NewBillDto.Web;

            entityDb.NotificationConfig.FailedAutomaticPayment.Email = entity.NotificationConfigDto.FailedAutomaticPaymentDto.Email;
            entityDb.NotificationConfig.FailedAutomaticPayment.Sms = entity.NotificationConfigDto.FailedAutomaticPaymentDto.Sms;
            entityDb.NotificationConfig.FailedAutomaticPayment.Web = entity.NotificationConfigDto.FailedAutomaticPaymentDto.Web;

            entityDb.NotificationConfig.SuccessPayment.Email = entity.NotificationConfigDto.SuccessPaymentDto.Email;
            entityDb.NotificationConfig.SuccessPayment.Sms = entity.NotificationConfigDto.SuccessPaymentDto.Sms;
            entityDb.NotificationConfig.SuccessPayment.Web = entity.NotificationConfigDto.SuccessPaymentDto.Web;

            entityDb.Enabled = entity.Enabled;

            //Visanet App edita el servicio modificando la tarjeta por defecto, la web no
            if (entity.DefaultCardId != Guid.Empty && entity.DefaultCardId != entityDb.DefaultCardId)
            {
                entityDb.DefaultCardId = entity.DefaultCardId;
            }

            //Visanet App edita el servicio modificando el pago automático, la web no
            if (entity.AutomaticPaymentDto != null)
            {
                this.AddAutomaticPayment(entity.AutomaticPaymentDto);
            }

            /*
            if (entityDb.AutomaticPayment == null)
            {
                entityDb.AutomaticPayment = new AutomaticPayment();
                entityDb.AutomaticPayment.GenerateNewIdentity();
            }

            entityDb.AutomaticPayment.Maximum = entity.AutomaticPaymentDto.Maximum;
            entityDb.AutomaticPayment.Quotas = entity.AutomaticPaymentDto.Quotas;
            entityDb.AutomaticPayment.DaysBeforeDueDate = entity.AutomaticPaymentDto.DaysBeforeDueDate;
            */

            Repository.Edit(entityDb);
            Repository.Save();

            Repository.ContextTrackChanges = false;
        }

        public void EditDescription(ServiceAssociatedDto entity)
        {
            Repository.ContextTrackChanges = true;
            var entityDb = Repository.GetById(entity.Id, s => s.NotificationConfig, s => s.AutomaticPayment);

            entityDb.Description = entity.Description;

            Repository.Edit(entityDb);
            Repository.Save();

            Repository.ContextTrackChanges = false;
        }

        public void ChangeState(Object[] data)
        {
            _repositoryServiceAsociated.ChangeState(data);
        }

        public IEnumerable<ServiceAssociatedDto> GetServicesForAutomaticPayment(ServiceFilterAssosiateDto filters)
        {
            var query = Repository.AllNoTracking(null,
                s => s.Service,
                s => s.Service.ServiceGateways,
                s => s.Service.ServiceGateways.Select(x => x.Gateway),
                s => s.AutomaticPayment,
                s => s.DefaultCard,
                s => s.Service.ServiceContainer);

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => sc.Service.Name.ToLower().Contains(filters.GenericSearch.ToLower()));

            if (!string.IsNullOrEmpty(filters.Service))
                query = query.Where(sc => sc.Service.Name.Equals(filters.Service, StringComparison.InvariantCultureIgnoreCase));

            query = query.Where(sc => sc.RegisteredUserId == filters.UserId && sc.Active).OrderBy(sc => sc.Service.Name);

            //if (filters.SortDirection == SortDirection.Asc)
            //    query = query.OrderByStringProperty(filters.OrderBy);
            //else
            //    query = query.OrderByStringPropertyDescending(filters.OrderBy);

            query = query.Skip(filters.DisplayStart);

            if (filters.DisplayLength.HasValue)
                query = query.Take(filters.DisplayLength.Value);

            var includeFilters = new ServiceAssociatedIncludeFilters
            {
                IncludeApplicationUserInfo = false,
                IncludeAutomaticPaymentInfo = true,
                IncludeCardListInfo = false,
                IncludeDefaultCardInfo = true,
                IncludeNotificationConfigInfo = false,
                IncludeServiceInfo = true,
                IncludeServiceGatewaysInfo = true,
                IncludeServiceCategoryInfo = true,
                IncludeServiceContainerInfo = true
            };

            var result = query.ToList().Select(t => t.Convert(includeFilters)).ToList();

            return result;
        }

        public IEnumerable<ServiceAssociatedDto> GetServicesActiveAutomaticPayment(ServiceFilterAssosiateDto filters)
        {
            var query = Repository.AllNoTracking(null,
                s => s.Service,
                s => s.Service.ServiceGateways,
                s => s.Service.ServiceGateways.Select(x => x.Gateway),
                s => s.AutomaticPayment,
                s => s.DefaultCard,
                s => s.Service.ServiceContainer);

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => sc.Service.Name.ToLower().Contains(filters.GenericSearch.ToLower()));

            if (!string.IsNullOrEmpty(filters.Service))
                query = query.Where(sc => sc.Service.Name.Equals(filters.Service, StringComparison.InvariantCultureIgnoreCase));

            query = query.Where(sc => sc.RegisteredUserId == filters.UserId && sc.Active && sc.AutomaticPaymentId != null).OrderBy(sc => sc.Service.Name);

            query = query.Skip(filters.DisplayStart);

            if (filters.DisplayLength.HasValue)
                query = query.Take(filters.DisplayLength.Value);

            var includeFilters = new ServiceAssociatedIncludeFilters
            {
                IncludeApplicationUserInfo = false,
                IncludeAutomaticPaymentInfo = true,
                IncludeCardListInfo = false,
                IncludeDefaultCardInfo = true,
                IncludeNotificationConfigInfo = false,
                IncludeServiceInfo = true,
                IncludeServiceGatewaysInfo = true,
                IncludeServiceCategoryInfo = true,
                IncludeServiceContainerInfo = true
            };

            var result = query.ToList().Select(t => t.Convert(includeFilters)).ToList();

            return result;
        }

        public bool HasAsosiatedService(Guid userId)
        {
            return Repository.AllNoTracking(p => p.RegisteredUserId == userId).Any();
        }

        public bool HasAutomaticPaymentCreated(Guid userId)
        {
            var has = Repository.AllNoTracking(p => p.RegisteredUserId == userId && p.AutomaticPaymentId != null).Any();
            if (!has)
                return false;

            var ids = Repository.AllNoTracking(p => p.RegisteredUserId == userId && p.AutomaticPaymentId != null).Select(p => p.AutomaticPaymentId);
            var servicesId = Repository.AllNoTracking(p => p.RegisteredUserId == userId && p.Active && ids.Contains(p.AutomaticPaymentId)).Any();

            return servicesId;
        }

        public void DeleteAutomaticPayment(Guid serviceAssosiatedId)
        {
            Repository.ContextTrackChanges = true;

            var entityDb = Repository.GetById(serviceAssosiatedId, s => s.AutomaticPayment);
            Repository.DeleteEntitiesNoRepository(entityDb.AutomaticPayment);
            Repository.Save();

            Repository.ContextTrackChanges = false;
        }

        public void UpdateAutomaticPaymentRunsData(Guid processHistoryId, Dictionary<Guid, PaymentResultTypeDto> results)
        {
            var processHistory = _serviceProcessHistory.GetById(processHistoryId);
            var date = DateTime.Now;

            Repository.ContextTrackChanges = true;
            foreach (var automaticPaymentResult in results)
            {
                var entityDb = Repository.GetById(automaticPaymentResult.Key, s => s.AutomaticPayment);

                if (entityDb.AutomaticPayment != null)
                {
                    entityDb.AutomaticPayment.LastRunDate = date;
                    entityDb.AutomaticPayment.LastRunResult = (PaymentResultType?)automaticPaymentResult.Value;

                    if (automaticPaymentResult.Value == PaymentResultTypeDto.Success)
                    {
                        entityDb.AutomaticPayment.LastSuccessfulPaymentDate = date;
                        entityDb.AutomaticPayment.LastSuccessfulPaymentIteration = processHistory.Count;
                    }
                    else if (automaticPaymentResult.Value != PaymentResultTypeDto.NoBills)
                    {
                        entityDb.AutomaticPayment.LastErrorDate = date;
                        entityDb.AutomaticPayment.LastErrorResult = (PaymentResultType?)automaticPaymentResult.Value;
                    }

                    Repository.Edit(entityDb);
                }
            }
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public bool DeleteService(Guid serviceAssociatedId, bool notify = true)
        {
            var resultOk = false;
            Repository.ContextTrackChanges = true;

            var entityDb = Repository.GetById(serviceAssociatedId, s => s.Service, s => s.AutomaticPayment, s => s.Cards);
            Repository.DeleteEntitiesNoRepository(entityDb.AutomaticPayment);

            var result = !notify || NotifyExternalSourceDeleteAssociation(entityDb);

            var cardsIDs = new List<Guid>();

            //notificar externo
            if (result)
            {
                bool hasPayments =
                    _repositoryPayment.AllNoTracking().Any(p => p.ServiceAssosiatedId == serviceAssociatedId);
                if (hasPayments)
                {
                    entityDb.Active = false;
                    //entityDb.Enabled = false;
                    if (entityDb.Cards.Any())
                        cardsIDs.AddRange(entityDb.Cards.Select(x => x.Id));
                    Repository.Edit(entityDb);
                }
                else
                {
                    Repository.Delete(serviceAssociatedId);
                }
                Repository.Save();
                Repository.ContextTrackChanges = false;
                resultOk = true;
            }
            else
            {
                Repository.ContextTrackChanges = false;
                resultOk = false;
                throw new BusinessException(CodeExceptions.SERVICE_NOT_DELETED);
            }

            //de esta forma elimino el listado de tarjetas.
            var deletecardFromList = true;
            if (cardsIDs.Any())
            {
                foreach (var card in cardsIDs)
                {
                    if (!DeleteCardFromService(entityDb.Id, card, entityDb.RegisteredUserId, notify))
                    {
                        deletecardFromList = false;
                    }
                }
            }
            //si no se elimina el listado de tarjetas, no vuelvo atras.?
            return resultOk;
        }

        //SI EL SERVICIO NO PIDE REFERENCIAS, SOLO HAY UN SERVICIO POR USUARIO CREADO
        public Guid IsServiceAssosiatedToUser(Guid userId, Guid serviceId, string ref1, string ref2, string ref3, string ref4, string ref5, string ref6)
        {
            //se edito para que devuelva uno si esta Activo
            if (userId != Guid.Empty && serviceId != Guid.Empty)
            {
                var query = Repository.AllNoTracking();
                query = query.Where(s => s.RegisteredUserId == userId && s.ServiceId == serviceId);

                if (!string.IsNullOrEmpty(ref1))
                    query = query.Where(s => s.ReferenceNumber.Equals(ref1));
                if (!string.IsNullOrEmpty(ref2))
                    query = query.Where(s => s.ReferenceNumber2.Equals(ref2));
                if (!string.IsNullOrEmpty(ref3))
                    query = query.Where(s => s.ReferenceNumber3.Equals(ref3));
                if (!string.IsNullOrEmpty(ref4))
                    query = query.Where(s => s.ReferenceNumber4.Equals(ref4));
                if (!string.IsNullOrEmpty(ref5))
                    query = query.Where(s => s.ReferenceNumber5.Equals(ref5));
                if (!string.IsNullOrEmpty(ref6))
                    query = query.Where(s => s.ReferenceNumber6.Equals(ref6));

                query = query.Where(s => s.Active).OrderBy(sc => sc.Service.Name);

                return query.Select(s => s.Id).FirstOrDefault();
            }

            return Guid.Empty;
        }
        public ServiceAssociatedDto ServiceAssosiatedToUser(Guid userId, Guid serviceId, string ref1, string ref2, string ref3, string ref4, string ref5, string ref6)
        {
            var query = Repository.AllNoTracking(null,
                s => s.Service,
                s => s.Service.ServiceGateways,
                s => s.Service.ServiceGateways.Select(x => x.Gateway),
                s => s.AutomaticPayment,
                s => s.DefaultCard,
                s => s.Service.ServiceContainer);

            if (userId != Guid.Empty)
            {
                query = query.Where(s => s.RegisteredUserId == userId);
            }
            if (serviceId != Guid.Empty)
            {
                query = query.Where(s => s.ServiceId == serviceId);
            }

            if (!string.IsNullOrEmpty(ref1))
                query = query.Where(s => s.ReferenceNumber.Equals(ref1));
            if (!string.IsNullOrEmpty(ref2))
                query = query.Where(s => s.ReferenceNumber2.Equals(ref2));
            if (!string.IsNullOrEmpty(ref3))
                query = query.Where(s => s.ReferenceNumber3.Equals(ref3));
            if (!string.IsNullOrEmpty(ref4))
                query = query.Where(s => s.ReferenceNumber4.Equals(ref4));
            if (!string.IsNullOrEmpty(ref5))
                query = query.Where(s => s.ReferenceNumber5.Equals(ref5));
            if (!string.IsNullOrEmpty(ref6))
                query = query.Where(s => s.ReferenceNumber6.Equals(ref6));

            query = query.OrderBy(sc => sc.Service.Name);

            var includeFilters = new ServiceAssociatedIncludeFilters
            {
                IncludeApplicationUserInfo = false,
                IncludeAutomaticPaymentInfo = true,
                IncludeCardListInfo = true,
                IncludeDefaultCardInfo = true,
                IncludeNotificationConfigInfo = false,
                IncludeServiceInfo = true,
                IncludeServiceGatewaysInfo = true,
                IncludeServiceCategoryInfo = true,
                IncludeServiceContainerInfo = true
            };

            var result = query.ToList().Select(t => t.Convert(includeFilters)).FirstOrDefault();

            return result;
        }

        public List<ServiceAssociatedDto> GetServicesForBills(Guid userId)
        {
            var query = Repository.AllNoTracking(null,
                s => s.Service,
                s => s.Service.ServiceGateways,
                s => s.Service.ServiceGateways.Select(x => x.Gateway),
                s => s.AutomaticPayment,
                s => s.DefaultCard,
                s => s.Service.ServiceContainer);

            query = query.Where(sc => sc.RegisteredUserId == userId).OrderBy(sc => sc.Service.Name);

            var includeFilters = new ServiceAssociatedIncludeFilters
            {
                IncludeApplicationUserInfo = false,
                IncludeAutomaticPaymentInfo = true,
                IncludeCardListInfo = false,
                IncludeDefaultCardInfo = true,
                IncludeNotificationConfigInfo = false,
                IncludeServiceInfo = true,
                IncludeServiceGatewaysInfo = true,
                IncludeServiceCategoryInfo = true,
                IncludeServiceContainerInfo = true
            };

            var result = query.ToList().Select(t => t.Convert(includeFilters)).ToList();

            return result;
        }

        public IEnumerable<ServiceAssociatedDto> GetServicesActiveAutomaticPaymentOrNotification()
        {
            var query = QueryForAutomaticPaymentOrNotification();

            //Traigo los que tienen pago programado configurado o tienen notificaciones activas
            query = query.Where(sc =>
                (sc.AutomaticPayment != null) ||
                (
                    sc.NotificationConfig.BeforeDueDateConfig.Email ||
                    sc.NotificationConfig.ExpiredBill.Email ||
                    sc.NotificationConfig.NewBill.Email
                )).OrderBy(sc => sc.Service.Name);

            var includeFilters = new ServiceAssociatedIncludeFilters
            {
                IncludeApplicationUserInfo = true,
                IncludeAutomaticPaymentInfo = true,
                IncludeCardListInfo = false,
                IncludeDefaultCardInfo = true,
                IncludeNotificationConfigInfo = true,
                IncludeServiceInfo = true,
                IncludeServiceGatewaysInfo = true,
                IncludeServiceCategoryInfo = true,
                IncludeServiceContainerInfo = true
            };

            var result = query.ToList().Select(t => t.Convert(includeFilters)).ToList();

            return result;
        }

        public IEnumerable<ServiceAssociatedDto> GetServicesActiveAutomaticPayment()
        {
            var query = QueryForAutomaticPaymentOrNotification();

            //Traigo los que tienen pago programado configurado
            query = query.Where(sc => sc.AutomaticPayment != null).OrderBy(sc => sc.Service.Name);

            var includeFilters = new ServiceAssociatedIncludeFilters
            {
                IncludeApplicationUserInfo = true,
                IncludeAutomaticPaymentInfo = true,
                IncludeCardListInfo = false,
                IncludeDefaultCardInfo = true,
                IncludeNotificationConfigInfo = true,
                IncludeServiceInfo = true,
                IncludeServiceGatewaysInfo = true,
                IncludeServiceCategoryInfo = true,
                IncludeServiceContainerInfo = true
            };

            var result = query.ToList().Select(t => t.Convert(includeFilters)).ToList();

            return result;
        }

        public IEnumerable<ServiceAssociatedDto> GetServicesActiveNotification()
        {
            var query = QueryForAutomaticPaymentOrNotification();

            //Traigo los que tienen notificaciones activas y no pago programado
            query = query.Where(sc =>
                (sc.AutomaticPayment == null) &&
                (
                    sc.NotificationConfig.BeforeDueDateConfig.Email ||
                    sc.NotificationConfig.ExpiredBill.Email ||
                    sc.NotificationConfig.NewBill.Email
                )).OrderBy(sc => sc.Service.Name);

            var includeFilters = new ServiceAssociatedIncludeFilters
            {
                IncludeApplicationUserInfo = true,
                IncludeAutomaticPaymentInfo = true,
                IncludeCardListInfo = false,
                IncludeDefaultCardInfo = true,
                IncludeNotificationConfigInfo = true,
                IncludeServiceInfo = true,
                IncludeServiceGatewaysInfo = true,
                IncludeServiceCategoryInfo = true,
                IncludeServiceContainerInfo = true
            };

            var result = query.ToList().Select(t => t.Convert(includeFilters)).ToList();

            return result;
        }

        private IQueryable<ServiceAssociated> QueryForAutomaticPaymentOrNotification()
        {
            var query = Repository.AllNoTracking(null,
                s => s.Service,
                s => s.Service.ServiceCategory,
                s => s.Service.ServiceGateways,
                s => s.Service.ServiceGateways.Select(x => x.Gateway),
                s => s.AutomaticPayment,
                s => s.DefaultCard,
                s => s.RegisteredUser,
                s => s.RegisteredUser.MembershipIdentifierObj,
                s => s.NotificationConfig,
                s => s.Service.ServiceContainer);

            //No traigo los servicios de usuarios bloqueados
            query = query.Where(x => !x.RegisteredUser.MembershipIdentifierObj.Blocked);

            //Traigo los que tienen servicio asociado activo y servicio activo
            query = query.Where(sc => sc.Active && sc.Service.Active);

            //Traigo los servicios que tienen pasarela activa sin ser Apps o Importe
            query = query.Where(sc => sc.Service.ServiceGateways.Any(g => g.Active &&
                    (g.Gateway.Enum == (int)GatewayEnumDto.Sucive ||
                    g.Gateway.Enum == (int)GatewayEnumDto.Banred ||
                    g.Gateway.Enum == (int)GatewayEnumDto.Carretera ||
                    g.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc ||
                    g.Gateway.Enum == (int)GatewayEnumDto.Geocom))
                );

            //Controlo si hay servicios momentaneamente fuera de servicio
            var servicesOffline = ServicesMomentarilyDisabledIds();
            if (servicesOffline != null && servicesOffline.Any())
            {
                query = query.Where(sc => !servicesOffline.Contains(sc.Service.UrlName) || (
                    sc.Service.ServiceContainer != null && !servicesOffline.Contains(sc.Service.ServiceContainer.UrlName)));
            }

            //Para poder filtrar solamente los servicios de un usuario
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["FilterAutomaticPaymentEmail"]))
            {
                var email = ConfigurationManager.AppSettings["FilterAutomaticPaymentEmail"];
                query = query.Where(sc => sc.RegisteredUser.Email == email);
            }

            //Para poder filtrar por servicio asociado
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["FilterAutomaticPaymentServiceAssociatedId"]))
            {
                var serviceAssociatedId = Guid.Parse(ConfigurationManager.AppSettings["FilterAutomaticPaymentServiceAssociatedId"]);
                query = query.Where(sc => sc.Id == serviceAssociatedId);
            }

            //Para poder filtrar solamente un servicio
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["FilterAutomaticPaymentServiceId"]))
            {
                var serviceId = Guid.Parse(ConfigurationManager.AppSettings["FilterAutomaticPaymentServiceId"]);
                query = query.Where(sc => sc.ServiceId == serviceId);
            }

            return query;
        }

        private bool DefaultCard(Guid userId, Guid serviceAssociatedId, Guid cardId, string operationId)
        {

            var user = _serviceApplicationUser.AllNoTracking(null, u => u.Cards.Any(c => c.Id == cardId) && u.Id == userId, x => x.Cards).FirstOrDefault();
            var serviceAssociated = Repository.GetById(serviceAssociatedId, x => x.Cards, x => x.Service, x => x.RegisteredUser, x => x.DefaultCard, x => x.NotificationConfig);

            if (user == null)
                return false;

            var result = true;

            if (!string.IsNullOrEmpty(serviceAssociated.Service.ExternalUrlAdd))
            {
                var cardDto = user.CardDtos.FirstOrDefault(x => x.Id == cardId);
                var card = new Card()
                {
                    Active = cardDto.Active,
                    Name = cardDto.Name,
                    CybersourceTransactionId = cardDto.CybersourceTransactionId,
                    Deleted = cardDto.Deleted,
                    DueDate = cardDto.DueDate,
                    ExternalId = cardDto.ExternalId,
                    Id = cardDto.Id,
                    MaskedNumber = cardDto.MaskedNumber,
                    PaymentToken = cardDto.PaymentToken,
                    Description = cardDto.Description
                };
                result = _serviceExternalNotification.NotifyExternalSourceNewCard(serviceAssociated, card, operationId);
            }

            if (result)
            {
                var defaultOldCard = serviceAssociated.DefaultCard;

                if (!string.IsNullOrEmpty(serviceAssociated.Service.ExternalUrlRemove) && defaultOldCard.ExternalId.HasValue)
                {
                    //TODO: falta obtener resultado de la baja
                    NotifyExternalSourceDeleteCard(serviceAssociated, defaultOldCard.ExternalId.Value);
                }

                Repository.ContextTrackChanges = true;
                var serEdited = Repository.GetById(serviceAssociatedId, x => x.Service, x => x.RegisteredUser, x => x.DefaultCard);
                var card = _repositoryCard.GetById(cardId);

                serEdited.DefaultCardId = cardId;
                serEdited.Cards.Clear();
                serEdited.Cards.Add(card);
                Repository.Edit(serEdited);
                Repository.Save();
                Repository.ContextTrackChanges = false;
            }
            return result;
        }

        public IQueryable<WebServiceApplicationClientDto> AssosiatedServiceClientUpdate(WebServiceClientInputDto dto)
        {
            var result = _repositoryServiceAsociated.AssosiatedServiceClientUpdate(new WebServiceClientInput()
            {
                CodBranch = dto.CodBranch,
                CodCommerce = dto.CodCommerce,
                FechaDesde = dto.FechaDesde,
                IdApp = dto.IdApp,
                RefCliente1 = dto.RefCliente1,
                RefCliente2 = dto.RefCliente2,
                RefCliente3 = dto.RefCliente3,
                RefCliente4 = dto.RefCliente4,
                RefCliente5 = dto.RefCliente5,
                RefCliente6 = dto.RefCliente6,
            });
            return result.Select(x => new WebServiceApplicationClientDto()
            {
                Apellido = x.Apellido,
                Nombre = x.Nombre,
                Documento = x.Documento,
                Email = x.Email,
                RefCliente1 = x.RefCliente1,
                RefCliente2 = x.RefCliente2,
                RefCliente3 = x.RefCliente3,
                RefCliente4 = x.RefCliente4,
                RefCliente5 = x.RefCliente5,
                RefCliente6 = x.RefCliente6,
                Estado = x.Estado,
                FchModificacion = x.FchModificacion,
                Telefono = x.Telefono,
                UserId = x.UserId
            });
        }

        public IList<ServiceAssociatedDto> GetServicesDebit(string refCliente, Guid serviceId)
        {
            var query = Repository.AllNoTracking(null, s => s.Service, s => s.DefaultCard, s => s.RegisteredUser);

            query = query.Where(x => serviceId == x.ServiceId && refCliente.Equals(x.ReferenceNumber)).OrderBy(sc => sc.Service.Name);

            return query.Select(t => new ServiceAssociatedDto
            {
                Id = t.Id,
                Description = t.Description,
                ReferenceNumber = t.ReferenceNumber,
                Active = t.Active,
                ReferenceNumber3 = t.ReferenceNumber3,
                ReferenceNumber2 = t.ReferenceNumber2,
                ReferenceNumber4 = t.ReferenceNumber4,
                ReferenceNumber5 = t.ReferenceNumber5,
                ReferenceNumber6 = t.ReferenceNumber6,
                ServiceId = t.ServiceId,
                ServiceDto = new ServiceDto()
                {
                    Name = t.Service.Name,
                    Id = t.ServiceId,
                    ServiceCategory = new ServiceCategoryDto { Id = t.Service.ServiceCategory.Id, Name = t.Service.ServiceCategory.Name },
                    ReferenceParamName = t.Service.ReferenceParamName,
                    ReferenceParamName2 = t.Service.ReferenceParamName2,
                    ReferenceParamName3 = t.Service.ReferenceParamName3,
                    ReferenceParamName4 = t.Service.ReferenceParamName4,
                    ReferenceParamName5 = t.Service.ReferenceParamName5,
                    ReferenceParamName6 = t.Service.ReferenceParamName6,

                    EnableAutomaticPayment = t.Service.EnableAutomaticPayment,
                    EnablePrivatePayment = t.Service.EnablePrivatePayment,
                    EnablePublicPayment = t.Service.EnablePublicPayment,
                    Container = t.Service.Container,

                    Departament = (DepartamentDtoType)(int)t.Service.Departament,
                    MerchantId = t.Service.MerchantId,
                    CybersourceAccessKey = t.Service.CybersourceAccessKey,
                    CybersourceProfileId = t.Service.CybersourceProfileId,
                    CybersourceSecretKey = t.Service.CybersourceSecretKey,
                    CybersourceTransactionKey = t.Service.CybersourceTransactionKey,
                },
                DefaultCardId = t.DefaultCardId,
                DefaultCard = new CardDto
                {
                    Active = t.DefaultCard.Active,
                    DueDate = t.DefaultCard.DueDate,
                    Id = t.DefaultCard.Id,
                    MaskedNumber = t.DefaultCard.MaskedNumber,
                    PaymentToken = t.DefaultCard.PaymentToken,
                    Name = t.DefaultCard.Name,
                    Description = t.DefaultCard.Description
                },
                Enabled = t.Enabled,
                UserId = t.RegisteredUserId,
                RegisteredUserDto = new ApplicationUserDto()
                {
                    Name = t.RegisteredUser.Name,
                    Address = t.RegisteredUser.Address,
                    Email = t.RegisteredUser.Email,
                    MobileNumber = t.RegisteredUser.MobileNumber,
                    PhoneNumber = t.RegisteredUser.PhoneNumber,
                    Surname = t.RegisteredUser.Surname,
                    IdentityNumber = t.RegisteredUser.IdentityNumber,

                },
            }).ToList();
        }

        public ServiceAssociatedDto CreateOrUpdateDeleted(ServiceAssociatedDto entityDto, bool? notifyExternal = null)
        {
            var serviceDeleted = Repository.All(s =>
                s.RegisteredUserId == entityDto.UserId &&
                s.ServiceId == entityDto.ServiceId &&
                s.ReferenceNumber.Equals(entityDto.ReferenceNumber) &&
                s.ReferenceNumber2.Equals(entityDto.ReferenceNumber2) &&
                s.ReferenceNumber5.Equals(entityDto.ReferenceNumber3) &&
                s.ReferenceNumber4.Equals(entityDto.ReferenceNumber4) &&
                s.ReferenceNumber3.Equals(entityDto.ReferenceNumber5) &&
                s.ReferenceNumber6.Equals(entityDto.ReferenceNumber6) &&
                !s.Active, s => s.Service, s => s.Service.ServiceGateways,
                s => s.NotificationConfig, s => s.AutomaticPayment).FirstOrDefault();

            if (serviceDeleted == null)
            {
                NLogLogger.LogEvent(NLogType.Info, "serv aso CREO");

                if (notifyExternal == null || notifyExternal.Value == true)
                {
                    var dto = Create(entityDto, true);
                    return dto;
                }
                else
                {
                    //Por ejemplo el caso que se esta realizando un pago de usuario que desea ser recurrente
                    var dto = CreateWithoutExternalNotification(entityDto);
                    return dto;
                }
            }
            else
            {
                Repository.ContextTrackChanges = true;
                var entityDb = serviceDeleted;
                var isPaymentDoneWithServiceAssosiated = _repositoryPayment.AllNoTracking().Any(p => p.ServiceAssosiatedId == serviceDeleted.Id);

                entityDb.Active = true;
                //entityDb.Enabled = true;
                entityDb.Description = entityDto.Description;

                //si no existen pagos realizados entonces se pueden modificar los nros de referencia.
                //en otro caso no.
                if (isPaymentDoneWithServiceAssosiated == false)
                {
                    entityDb.ReferenceNumber = entityDto.ReferenceNumber;
                    entityDb.ReferenceNumber2 = entityDto.ReferenceNumber2;
                    entityDb.ReferenceNumber3 = entityDto.ReferenceNumber3;
                    entityDb.ReferenceNumber4 = entityDto.ReferenceNumber4;
                    entityDb.ReferenceNumber5 = entityDto.ReferenceNumber5;
                    entityDb.ReferenceNumber6 = entityDto.ReferenceNumber6;
                }

                entityDb.NotificationConfig.DaysBeforeDueDate = entityDto.NotificationConfigDto.DaysBeforeDueDate;
                entityDb.NotificationConfig.BeforeDueDateConfig.Email = entityDto.NotificationConfigDto.BeforeDueDateConfigDto.Email;
                entityDb.NotificationConfig.BeforeDueDateConfig.Sms = entityDto.NotificationConfigDto.BeforeDueDateConfigDto.Sms;
                entityDb.NotificationConfig.BeforeDueDateConfig.Web = entityDto.NotificationConfigDto.BeforeDueDateConfigDto.Web;

                entityDb.NotificationConfig.ExpiredBill.Email = entityDto.NotificationConfigDto.ExpiredBillDto.Email;
                entityDb.NotificationConfig.ExpiredBill.Sms = entityDto.NotificationConfigDto.ExpiredBillDto.Sms;
                entityDb.NotificationConfig.ExpiredBill.Web = entityDto.NotificationConfigDto.ExpiredBillDto.Web;

                entityDb.NotificationConfig.NewBill.Email = entityDto.NotificationConfigDto.NewBillDto.Email;
                entityDb.NotificationConfig.NewBill.Sms = entityDto.NotificationConfigDto.NewBillDto.Sms;
                entityDb.NotificationConfig.NewBill.Web = entityDto.NotificationConfigDto.NewBillDto.Web;

                entityDb.NotificationConfig.FailedAutomaticPayment.Email = entityDto.NotificationConfigDto.FailedAutomaticPaymentDto.Email;
                entityDb.NotificationConfig.FailedAutomaticPayment.Sms = entityDto.NotificationConfigDto.FailedAutomaticPaymentDto.Sms;
                entityDb.NotificationConfig.FailedAutomaticPayment.Web = entityDto.NotificationConfigDto.FailedAutomaticPaymentDto.Web;

                entityDb.NotificationConfig.SuccessPayment.Email = entityDto.NotificationConfigDto.SuccessPaymentDto.Email;
                entityDb.NotificationConfig.SuccessPayment.Sms = entityDto.NotificationConfigDto.SuccessPaymentDto.Sms;
                entityDb.NotificationConfig.SuccessPayment.Web = entityDto.NotificationConfigDto.SuccessPaymentDto.Web;

                entityDb.DefaultCardId = entityDto.DefaultCardId;

                //entityDb.Enabled = entity.Enabled;
                Repository.Edit(entityDb);
                Repository.Save();

                NLogLogger.LogEvent(NLogType.Info, "serv aso UPDATE");

                Repository.ContextTrackChanges = false;

                _loggerService.CreateLog(LogType.Info, LogOperationType.ServiceAssociated, LogCommunicationType.VisaNet, string.Format(LogStrings.ServiceAssosiated_UpdateAndActivate, entityDto.ServiceDto != null ? entityDto.ServiceDto.Name : "", entityDto.RegisteredUserDto != null ? entityDto.RegisteredUserDto.Email : ""));

                var result = SetUpServiceCards(entityDb.Id, entityDb.DefaultCardId, true, entityDto.OperationId);
                //DefaultCard(entityDb.RegisteredUserId, entityDb.Id, entityDb.DefaultCardId);
                if (result)
                {
                    var dto = GetById(serviceDeleted.Id, s => s.Service, s => s.DefaultCard);
                    entityDto.ServiceDto = dto.ServiceDto;
                    entityDto.Id = serviceDeleted.Id;
                    entityDto.DefaultCard = dto.DefaultCard;
                    return entityDto;
                }

                DeleteService(serviceDeleted.Id, false);
                return null;
            }
        }

        private bool SetUpServiceCards(Guid serviceAssociatedId, Guid cardId, bool newUser, string operationId, bool? notifyExternal = null)
        {
            var serviceAssociated = Repository.GetById(serviceAssociatedId, x => x.Cards, x => x.Service, x => x.RegisteredUser);
            return SetUpServiceCards(serviceAssociated, cardId, newUser, operationId, notifyExternal);
        }
        private bool SetUpServiceCards(ServiceAssociated serviceAssociated, Guid cardId, bool newUser, string operationId, bool? notifyExternal = null)
        {
            Repository.ContextTrackChanges = true;
            var card = _repositoryCard.GetById(cardId);

            //Si no exite esta tarjeta en el listado del servicio, la agrego
            if (serviceAssociated.Cards.All(x => x.Id != cardId))
            {
                serviceAssociated.Cards.Add(card);
            }
            Repository.Edit(serviceAssociated);
            Repository.Save();
            Repository.ContextTrackChanges = false;

            if (notifyExternal != null && notifyExternal.Value == false)
            {
                //Se indica que no se debe notificar por ejemplo cuando es un pago de usuario recurrente
                return true;
            }
            if (!string.IsNullOrEmpty(serviceAssociated.Service.ExternalUrlAdd))
            {
                if (newUser)
                {
                    return NotifyExternalSourceAssociation(serviceAssociated, card, operationId);
                }
                return NotifyExternalSourceCard(serviceAssociated, card, operationId);
            }
            return true;
        }

        public bool DeleteCardFromService(Guid serviceId, Guid cardId, Guid userId, bool notifyExternalSource = true)
        {
            var result = true;
            ServiceAssociated sa = null;
            Card card = null;

            var userDto = _serviceApplicationUser.GetById(userId, x => x.Cards);
            if (userDto.CardDtos.All(x => x.Id != cardId))
            {
                throw new BusinessException(CodeExceptions.USER_CARD_NOT_MATCH);
            }

            if (serviceId == Guid.Empty)
            {
                //elimino de todos los servicios asociados a esa tarjeta
                var list = Repository.All(x => x.Cards.Any(y => y.Id == cardId), x => x.Cards, x => x.Service).ToList();
                Repository.ContextTrackChanges = true;
                foreach (var serviceAssociated in list)
                {
                    var aux = serviceAssociated.Cards.First(c => c.Id == cardId);
                    var r = !aux.ExternalId.HasValue || NotifyExternalSourceDeleteCard(serviceAssociated, aux.ExternalId.Value);
                    if (r)
                    {
                        serviceAssociated.Cards.Remove(aux);
                        Repository.Edit(serviceAssociated);
                    }
                    //Q PASA SI ELIMINO DE 1 SERVICIO Y FALLA EN EL 2DO SERVICIO?
                }

                Repository.Save();
                Repository.ContextTrackChanges = false;
                return true;
            }
            else
            {
                //solo el servicio seleccionado
                sa = Repository.GetById(serviceId, x => x.Cards, x => x.Service);
                if (sa.RegisteredUserId != userId)
                {
                    throw new BusinessException(CodeExceptions.APPLICATION_USER_SERVICEASSOCIATED);
                }

                //SI EL SERVICIO ASOCIADO SOLO TIENE UNA TARJETA NO SE PUEDE ELIMINAR. EN ESTE CASO EL SERVICIO DEBERA ELIMINAR EL SERVICIO ASOCIADO.
                //SI EL SERVICIO NO ESTA ACTIVO, SE PUEDEN ELIMINAR LAS TARJETAS DEL LISTADO.
                if (sa.Active && sa.Cards.Count == 1)
                {
                    throw new ServiceAssociatedWithoutCardException(CodeExceptions.CARD_DELETE_ERROR_SERVICEASSOCIATED_WITHOUT_CARD);
                }

                var aux = sa.Cards.FirstOrDefault(c => c.Id == cardId);
                if (aux == null)
                {
                    Repository.ContextTrackChanges = false;
                    return true;
                }

                //TODO ESTO NO FUNCIONA CON SERVICIOS QUE TIENE SOLO 1 TARJETA. SI ELIMINO LA DEFAULT TENGO Q ELIMINAR LA ASOCIACION. HAY Q PERMITIR Q EL SERV ASOCIADO NO TENGA TARJETA POR DEFAULT 
                sa.Cards.Remove(aux);
                Repository.Edit(sa);

                if (notifyExternalSource)
                {
                    result = aux.ExternalId == null || Guid.Empty == aux.ExternalId || NotifyExternalSourceDeleteCard(sa, aux.ExternalId.Value);
                }

                if (result)
                {
                    try
                    {
                        Repository.ContextTrackChanges = true;
                        sa.Cards.Remove(card);
                        Repository.Edit(sa);
                        Repository.Save();
                        Repository.ContextTrackChanges = false;
                        return true;
                    }
                    catch (Exception exception)
                    {
                        NLogLogger.LogEvent(NLogType.Info, string.Format("DeleteCardFromService - Error al intentar eliminar una tarjeta. Id Operación {0}", string.Empty));
                        NLogLogger.LogEvent(exception);

                        //AGREGO LA TARJETA EN EL COMERCIO
                        var addResult = NotifyExternalSourceCard(sa, card, string.Empty);
                        if (!addResult)
                        {
                            NLogLogger.LogEvent(NLogType.Info, string.Format("AddCardToService - Se elimino una tarjeta en un comercio y no se pudo agregar luego de una excepcion. Id Operación {0}", string.Empty));
                        }
                        Repository.ContextTrackChanges = false;
                        return false;
                    }
                }
            }
            return false;
        }

        public bool AddCardToService(Guid serviceId, Guid cardId, Guid oldCardId, Guid userId, string operationId)
        {
            var sa = Repository.GetById(serviceId, x => x.Cards, x => x.Service, x => x.RegisteredUser);
            var card = _repositoryCard.GetById(cardId);
            var userDto = _serviceApplicationUser.GetById(userId, x => x.Cards);

            if (card == null)
            {
                return false;
            }

            if (sa.RegisteredUserId != userId)
            {
                throw new BusinessException(CodeExceptions.APPLICATION_USER_SERVICEASSOCIATED);
            }
            if (!userDto.CardDtos.Any(x => x.Id == cardId))
            {
                throw new BusinessException(CodeExceptions.USER_CARD_NOT_MATCH);
            }

            var maskedNumber = int.Parse(card.MaskedNumber.Substring(0, 6));
            var bin = _repositoryBin.AllNoTracking(x => x.Value == maskedNumber).FirstOrDefault();

            if (bin == null)
            {
                bin = _repositoryBin.GetDefaultBin();
            }

            if (!bin.Active)
            {
                throw new BusinessException(CodeExceptions.BIN_NOTACTIVE);
            }

            //TODO LIF REVISAR ESTO. DONDE CONTROLO QUE NO SE AGREGUE UNA TARJETA QUE NO TIENE PERMITIDO EL SERVICIO
            //if (((bin.CardType == CardType.Debit && !sa.Service.DebitCard) ||
            //    (bin.CardType == CardType.Credit && !sa.Service.CreditCard) ||
            //    (bin.CardType == CardType.InternationalDebit && !sa.Service.DebitCardInternational) ||
            //    (bin.CardType == CardType.InternationalCredit && !sa.Service.CreditCardInternational)))

            if (!_serviceService.IsBinAssociatedToService(bin.Value, sa.ServiceId))
            {
                throw new BusinessException(CodeExceptions.BIN_NOTVALID_FOR_SERVICE);
            }

            if (!sa.Service.AllowMultipleCards)
            {
                var resultDefaultCard = DefaultCard(sa.RegisteredUserId, sa.Id, cardId, operationId);
                return resultDefaultCard;
            }

            if (!card.ExternalId.HasValue)
                card.ExternalId = Guid.NewGuid();

            var result = NotifyExternalSourceCard(sa, card, operationId);

            if (result)
            {
                try
                {
                    Repository.ContextTrackChanges = true;
                    ////En servicios de multiple tarjeta, si lo hago es migrar, la vieja no es mas default
                    //if (oldCardId != Guid.Empty && sa.DefaultCardId == oldCardId)
                    //{
                    //    sa.DefaultCardId = card.Id;
                    //}

                    //LA ULTIMA TARJETA SELECCIONADA ES LA NUEVA POR DEFECTO
                    sa.DefaultCardId = card.Id;


                    //Si no exite esta tarjeta en el listado del servicio, la agrego
                    if (!sa.Cards.Any(x => x.Id == cardId))
                    {
                        sa.Cards.Add(card);
                    }

                    Repository.Edit(sa);
                    Repository.Save();
                    Repository.ContextTrackChanges = false;
                    return true;
                }
                catch (Exception exception)
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("AddCardToService - Error al intentar guardar un servicio asociado. Id Operación {0}", operationId));
                    NLogLogger.LogEvent(exception);

                    //CANCELO LA TARJETA EN EL ENTE
                    var cancelationResult = NotifyExternalSourceDeleteCard(sa, card.ExternalId.Value);
                    if (!cancelationResult)
                    {
                        NLogLogger.LogEvent(NLogType.Info, string.Format("AddCardToService - Se dio de alta una tarjeta en un comercio y no se pudo eliminar luego de una excepcion. Id Operación {0}", operationId));
                    }
                    Repository.ContextTrackChanges = false;
                    return false;
                }
            }
            return false;
        }

        public IEnumerable<ServiceAssociatedDto> GetServiceAssociatedLigth(ServiceFilterAssosiateDto filters)
        {
            var query = Repository.AllNoTracking(null, s => s.Service, s => s.AutomaticPayment, s => s.DefaultCard);

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => sc.Service.Name.ToLower().Contains(filters.GenericSearch.ToLower()));

            if (!string.IsNullOrEmpty(filters.Service))
                query = query.Where(sc => sc.Service.Name.Contains(filters.Service) ||
                                          sc.Service.Tags.Contains(filters.Service) ||
                                          sc.Description.Contains(filters.Service)/* ||
                                          sc.ReferenceNumber.Contains(filters.Service) ||
                                          sc.ReferenceNumber2.Contains(filters.Service) ||
                                          sc.ReferenceNumber3.Contains(filters.Service) ||
                                          sc.ReferenceNumber4.Contains(filters.Service) ||
                                          sc.ReferenceNumber5.Contains(filters.Service) ||
                                          sc.ReferenceNumber6.Contains(filters.Service)*/);

            if (!string.IsNullOrEmpty(filters.ReferenceNumber))
                query = query.Where(sc => sc.ReferenceNumber.Contains(filters.ReferenceNumber) ||
                                          sc.ReferenceNumber2.Contains(filters.ReferenceNumber) ||
                                          sc.ReferenceNumber3.Contains(filters.ReferenceNumber) ||
                                          sc.ReferenceNumber4.Contains(filters.ReferenceNumber) ||
                                          sc.ReferenceNumber5.Contains(filters.ReferenceNumber) ||
                                          sc.ReferenceNumber6.Contains(filters.ReferenceNumber));

            if (filters.ServiceAssociatedId != default(Guid))
                query = query.Where(s => s.Id == filters.ServiceAssociatedId);

            if (filters.WithAutomaticPaymentsInt == 1)
                query = query.Where(x => x.AutomaticPaymentId != Guid.Empty && x.AutomaticPaymentId != null);

            if (filters.WithAutomaticPaymentsInt == 2)
                query = query.Where(x => x.AutomaticPaymentId == Guid.Empty || x.AutomaticPaymentId == null);

            if (!filters.IncludeDeleted)
            {
                query = query.Where(sc => sc.Active);
            }

            query = query.Where(sc => sc.RegisteredUserId == filters.UserId).OrderBy(sc => sc.Service.Name);

            query = query.Skip(filters.DisplayStart);

            if (filters.DisplayLength.HasValue)
                query = query.Take(filters.DisplayLength.Value);

            var list = query.Select(t => new ServiceAssociatedDto
            {
                Id = t.Id,
                Description = t.Description,
                ReferenceNumber = t.ReferenceNumber,
                Active = t.Active,
                ReferenceNumber3 = t.ReferenceNumber3,
                ReferenceNumber2 = t.ReferenceNumber2,
                ReferenceNumber4 = t.ReferenceNumber4,
                ReferenceNumber5 = t.ReferenceNumber5,
                ReferenceNumber6 = t.ReferenceNumber6,
                ServiceId = t.ServiceId,
                DefaultCardId = t.DefaultCardId,
                DefaultCard = t.DefaultCard != null ? new CardDto
                {
                    MaskedNumber = t.DefaultCard.MaskedNumber,
                    DueDate = t.DefaultCard.DueDate,
                    Active = t.DefaultCard.Active,
                    Id = t.DefaultCard.Id,
                    Description = t.DefaultCard.Description
                } : new CardDto
                {
                    MaskedNumber = "",
                    DueDate = default(DateTime),
                    Active = false,
                    Id = default(Guid)
                },
                ServiceDto = new ServiceDto()
                {
                    Name = t.Service.Name,
                    Id = t.ServiceId,
                    ReferenceParamName = t.Service.ReferenceParamName,
                    ReferenceParamName2 = t.Service.ReferenceParamName2,
                    ReferenceParamName3 = t.Service.ReferenceParamName3,
                    ReferenceParamName4 = t.Service.ReferenceParamName4,
                    ReferenceParamName5 = t.Service.ReferenceParamName5,
                    ReferenceParamName6 = t.Service.ReferenceParamName6,
                    Departament = (DepartamentDtoType)(int)t.Service.Departament,

                    EnableAutomaticPayment = t.Service.EnableAutomaticPayment,
                    EnablePrivatePayment = t.Service.EnablePrivatePayment,
                    EnablePublicPayment = t.Service.EnablePublicPayment,
                    EnableMultipleBills = t.Service.EnableMultipleBills,
                    Container = t.Service.Container,
                    ImageName = t.Service.ImageName,
                    ServiceContainerImageName = t.Service.ServiceContainer != null ? t.Service.ServiceContainer.ImageName : string.Empty,
                    ServiceGatewaysDto = t.Service.ServiceGateways.Select(g => new ServiceGatewayDto()
                    {
                        Active = g.Active,
                        ReferenceId = g.ReferenceId,
                        Id = g.Id,
                        ServiceType = g.ServiceType,
                        Gateway = new GatewayDto()
                        {
                            Id = g.Gateway.Id,
                            Name = g.Gateway.Name,
                            Enum = g.Gateway.Enum
                        }
                    }).ToList(),
                },
                AutomaticPaymentDtoId = t.AutomaticPaymentId,
                AutomaticPaymentDto = t.AutomaticPayment != null ? new AutomaticPaymentDto
                {
                    DaysBeforeDueDate = t.AutomaticPayment.DaysBeforeDueDate,
                    SuciveAnnualPatent = t.AutomaticPayment.SuciveAnnualPatent
                } : new AutomaticPaymentDto
                {
                    DaysBeforeDueDate = 0,
                    SuciveAnnualPatent = false
                },
                Enabled = t.Enabled,
            }).ToList();

            foreach (var serviceAssociatedDto in list)
            {
                serviceAssociatedDto.ServiceDto.ImageUrl = FileStorage.Instance.GetImageUrl(_folderBlob, serviceAssociatedDto.ServiceDto.Id, serviceAssociatedDto.ServiceDto.ImageName);
                if (serviceAssociatedDto.ServiceDto.ServiceContainerId.HasValue)
                {
                    serviceAssociatedDto.ServiceDto.ServiceContainerImageUrl = FileStorage.Instance.GetImageUrl(_folderBlob, serviceAssociatedDto.ServiceDto.ServiceContainerId.Value, serviceAssociatedDto.ServiceDto.ServiceContainerImageName);
                }
            }

            return list;
        }

        public IEnumerable<AutomaticPaymentsViewDto> ReportsAutomaticPaymentsDataFromDbView(ReportsAutomaticPaymentsFilterDto filters)
        {
            var query = "SELECT * FROM dbo.ReportAutomaticPaymentView ";
            var where = "WHERE 1=1 ";

            if (!string.IsNullOrEmpty(filters.CreationDateFromString))
            {
                var from = DateTime.Parse(filters.CreationDateFromString).ToString("MM/dd/yyyy");
                where += "AND (CreationDate >= '" + from + "') ";
            }

            if (!string.IsNullOrEmpty(filters.CreationDateToString))
            {
                var to = DateTime.Parse(filters.CreationDateToString, new CultureInfo("es-UY")).AddDays(1).ToString("MM/dd/yyyy");
                where += "AND (CreationDate < '" + to + "') ";
            }

            if (!string.IsNullOrEmpty(filters.ClientEmail))
                where += "AND (ClientEmail LIKE '%" + filters.ClientEmail + "%') ";

            if (!string.IsNullOrEmpty(filters.ServiceNameAndDesc))
                where += "AND (ServiceNameAndDesc LIKE '%" + filters.ServiceNameAndDesc + "%') ";

            //para GET por reporte de servicios asociados
            if (filters.ServiceAssociatedId != default(Guid))
                where += "AND (ServiceAssociatedId = '" + filters.ServiceAssociatedId + "') ";

            #region Sort

            var orderBy = "";
            switch (filters.OrderBy)
            {
                case "0":
                    orderBy = "ORDER BY ClientEmail ";
                    break;
                case "1":
                    orderBy = "ORDER BY ServiceNameAndDesc ";
                    break;
                case "2":
                    orderBy = "ORDER BY Maximum ";
                    break;
                case "3":
                    orderBy = "ORDER BY DaysBeforeDueDate ";
                    break;
                case "4":
                    orderBy = "ORDER BY Quotas ";
                    break;
                case "5":
                    orderBy = "ORDER BY SuciveAnnualPatent ";
                    break;
                case "6":
                    orderBy = "ORDER BY PaymentsCount ";
                    break;
                case "7":
                    orderBy = "ORDER BY PaymentsAmountPesos ";
                    break;
                case "8":
                    orderBy = "ORDER BY PaymentsAmountDollars ";
                    break;
                case "9":
                    orderBy = "ORDER BY CreationDate ";
                    break;
                default:
                    orderBy = "ORDER BY ClientEmail ";
                    break;
            }

            if (filters.SortDirection == SortDirection.Desc)
                orderBy += "DESC";
            #endregion

            using (var context = new AppContext())
            {
                if (filters.DisplayLength != null)
                {
                    return context.Database.SqlQuery<AutomaticPaymentsViewDto>(string.Format("WITH aux AS ( SELECT *, ROW_NUMBER() OVER ({1}) AS RowNumber FROM [dbo].[ReportAutomaticPaymentView] {0} ) SELECT * FROM aux WHERE RowNumber BETWEEN {2} AND {3}", where, orderBy, filters.DisplayStart + 1, filters.DisplayStart + (int)filters.DisplayLength)).ToList();
                }
                return context.Database.SqlQuery<AutomaticPaymentsViewDto>(query + where + orderBy).ToList();
            }
        }

        public int ReportsAutomaticPaymentsDataCount(ReportsAutomaticPaymentsFilterDto filters)
        {
            var query = "SELECT COUNT(0) FROM dbo.ReportAutomaticPaymentView WHERE 1=1 ";

            if (!string.IsNullOrEmpty(filters.CreationDateFromString))
            {
                var from = DateTime.Parse(filters.CreationDateFromString).ToString("MM/dd/yyyy");
                query += "AND (CreationDate >= '" + from + "') ";
            }

            if (!string.IsNullOrEmpty(filters.CreationDateToString))
            {
                var to = DateTime.Parse(filters.CreationDateToString, new CultureInfo("es-UY")).AddDays(1).ToString("MM/dd/yyyy");
                query += "AND (CreationDate < '" + to + "') ";
            }

            if (!string.IsNullOrEmpty(filters.ClientEmail))
                query += "AND (ClientEmail LIKE '%" + filters.ClientEmail + "%') ";

            if (!string.IsNullOrEmpty(filters.ServiceNameAndDesc))
                query += "AND (ServiceNameAndDesc LIKE '%" + filters.ServiceNameAndDesc + "%') ";

            //para GET por reporte de servicios asociados
            if (filters.ServiceAssociatedId != default(Guid))
                query += "AND (ServiceAssociatedId = '" + filters.ServiceAssociatedId + "') ";

            using (var context = new AppContext())
            {
                return context.Database.SqlQuery<int>(query).First();
            }
        }

        //SI LLEGA ACA ES PORQUE SE CREO UNA TARJETA NUEVA EN CS
        public CybersourceCreateServiceAssociatedDto ProccesDataFromCybersource(IDictionary<string, string> csDictionary)
        {
            var result = new CybersourceCreateServiceAssociatedDto() { CybersourceCreateCardDto = new CybersourceCreateCardDto() };
            var cardCreated = false;
            try
            {
                var processedData = _serviceAnalyzeCsCall.ProcessCybersourceOperation(csDictionary);
                result.CybersourceCreateCardDto.TokenizationData = processedData.TokenizationData;

                if (processedData.TokenizationData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                {

                    var user = _serviceApplicationUser.GetById(processedData.PaymentDto.ServiceAssociatedDto.UserId);
                    processedData.PaymentDto.ServiceAssociatedDto.RegisteredUserDto = new ApplicationUserDto()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Surname = user.Surname,
                        Address = user.Address,
                        CallCenterKey = user.CallCenterKey,
                        Email = user.Email,
                        IdentityNumber = user.IdentityNumber,
                        MembreshipIdentifier = user.MembreshipIdentifier,
                        MobileNumber = user.MobileNumber,
                        PhoneNumber = user.PhoneNumber,
                    };

                    var service = _serviceService.GetById(processedData.PaymentDto.ServiceAssociatedDto.ServiceId);
                    processedData.PaymentDto.ServiceAssociatedDto.ServiceDto = service;
                    var strLog = string.Format(LogStrings.Service_init, processedData.PaymentDto.ServiceAssociatedDto.RegisteredUserDto.Email,
                    processedData.PaymentDto.ServiceAssociatedDto.RegisteredUserDto.Name,
                    processedData.PaymentDto.ServiceAssociatedDto.RegisteredUserDto.Surname,
                    processedData.PaymentDto.ServiceAssociatedDto.ServiceDto != null ? processedData.PaymentDto.ServiceAssociatedDto.ServiceDto.Name : string.Empty);
                    LogCybersourceData(processedData.PaymentDto, processedData, strLog, LogOperationType.Webhooks);

                    //PRIMERO CREO LA TARJETA
                    var card = new CardDto()
                    {
                        Active = true,
                        CybersourceTransactionId =
                                       processedData.PaymentDto.ServiceAssociatedDto.DefaultCard
                                       .CybersourceTransactionId,
                        DueDate = processedData.PaymentDto.ServiceAssociatedDto.DefaultCard.DueDate,
                        ExternalId = processedData.PaymentDto.ServiceAssociatedDto.DefaultCard.ExternalId,
                        MaskedNumber = processedData.PaymentDto.ServiceAssociatedDto.DefaultCard.MaskedNumber,
                        Name = processedData.PaymentDto.ServiceAssociatedDto.DefaultCard.Name,
                        PaymentToken = processedData.PaymentDto.ServiceAssociatedDto.DefaultCard.PaymentToken,

                    };
                    var newCard = _serviceApplicationUser.AddCard(card, processedData.PaymentDto.ServiceAssociatedDto.UserId);
                    cardCreated = true;
                    processedData.PaymentDto.ServiceAssociatedDto.DefaultCardId = newCard.Id;
                    processedData.PaymentDto.ServiceAssociatedDto.DefaultCard = null;

                    var creationresult = CreateOrUpdateDeleted(processedData.PaymentDto.ServiceAssociatedDto);
                    result.ServiceAssociatedDto = creationresult;
                    result.CybersourceCreateCardDto.TokenizationData = processedData.TokenizationData;

                    //SE REALIZO LA ASOCIACION
                    if (creationresult != null)
                    {
                        NotifyExternalSourceAssociation(creationresult, newCard,
                            processedData.CyberSourceMerchantDefinedData.OperationId);
                    }
                    return result;
                }
            }
            catch (BusinessException ex)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceServiceAssosiate - ProccesDataFromCybersource - BusinessException");
                NLogLogger.LogEvent(ex);
                result.ExceptionCapture = ex;
            }
            catch (ProviderWithoutConectionException ex)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceServiceAssosiate - ProccesDataFromCybersource - ProviderWithoutConectionException");
                NLogLogger.LogEvent(ex);
                result.ExceptionCapture = ex;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceServiceAssosiate - ProccesDataFromCybersource - Exception");
                NLogLogger.LogEvent(e);
                result.ExceptionCapture = e;
            }
            result.ServiceAssociatedDto = null;
            if (cardCreated)
            {
                result.AssociationInternalErrorCode = 15;
                result.AssociationInternalErrorDesc = string.Format(
                        "Ha ocurrido un error. Para el usuario {0} se creo la tarjeta pero no se pudo terminar con el proceso de asociación. Por favor intente nuevamente y si el error persiste comuníquese con el call center",
                        result.ServiceAssociatedDto.RegisteredUserDto.Email);
            }
            return result;
        }

        //SI LLEGA ACA ES PORQUE SE CREO UNA ASOCIACIÓN POR VONREGISTER
        //ESTE METODO YA NOTIFICA A EXTERNAL SORUCES
        public CybersourceCreateAppAssociationDto ProccesDataFromCybersourceForApps(IDictionary<string, string> csDictionary)
        {
            var result = new CybersourceCreateAppAssociationDto
            {
                CybersourceCreateServiceAssociatedDto = new CybersourceCreateServiceAssociatedDto
                {
                    CybersourceCreateCardDto = new CybersourceCreateCardDto()
                }
            };

            var isNewUser = false;
            var userCreated = false;
            var cardCreated = false;
            ApplicationUserDto user = null;
            CybersourceTransactionsDataDto processedData = null;
            try
            {
                processedData = _serviceAnalyzeCsCall.ProcessCybersourceOperation(csDictionary);
                result.CybersourceCreateServiceAssociatedDto.CybersourceCreateCardDto.TokenizationData = processedData.TokenizationData;
                result.CybersourceCreateServiceAssociatedDto.CyberSourceMerchantDefinedData = processedData.CyberSourceMerchantDefinedData;

                var service = _serviceService.GetById(processedData.CyberSourceMerchantDefinedData.ServiceId, x => x.ServiceContainer);
                result.WebhookRegistrationDto = _serviceWebhookRegistration.GetByIdOperation(processedData.CyberSourceMerchantDefinedData.OperationId, service.UrlName);

                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("ProccesDataFromCybersourceForApps - idApp: {0}, Operación: {1}", result.WebhookRegistrationDto.IdApp, result.WebhookRegistrationDto.IdOperation));

                //SI NO TERMINO OK, NO CREO EL USUARIO
                if (processedData.TokenizationData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                {
                    processedData.PaymentDto.ServiceAssociatedDto.ServiceDto = service;

                    var tempStr = string.Format("El usuario quiere asociar el servicio {0}, operación {1}", service.Name, processedData.CyberSourceMerchantDefinedData.OperationId);

                    if (processedData.PaymentDto.ServiceAssociatedDto.UserId != Guid.Empty)
                    {
                        processedData.PaymentDto.ServiceAssociatedDto.RegisteredUserDto =
                            _serviceApplicationUser.GetById(
                                processedData.PaymentDto.ServiceAssociatedDto.UserId);
                        _loggerService.CreateLog(LogType.Info, LogOperationType.Webhooks, LogCommunicationType.VisaNet,
                        processedData.PaymentDto.ServiceAssociatedDto.RegisteredUserDto.Id, tempStr, tempStr);
                        NLogLogger.LogAppsEvent(NLogType.Info, string.Format("ProccesDataFromCybersourceForApps - Cargo Usuario - idApp: {0}, Operación: {1}, Usuario {2}", result.WebhookRegistrationDto.IdApp,
                        result.WebhookRegistrationDto.IdOperation, result.WebhookRegistrationDto.UserData.Email));
                        userCreated = true;
                        isNewUser = false;
                        user = processedData.PaymentDto.ServiceAssociatedDto.RegisteredUserDto;
                    }
                    else
                    {
                        user = processedData.PaymentDto.ServiceAssociatedDto.RegisteredUserDto;
                        _serviceApplicationUser.Create(new ApplicationUserCreateEditDto()
                        {
                            Name = user.Name,
                            Surname = user.Surname,
                            Email = user.Email,
                            Address = user.Address,
                            IdentityNumber = user.IdentityNumber,
                            Password = user.Password,
                            PasswordAlreadyHashed = true,
                            PhoneNumber = user.PhoneNumber,
                            MobileNumber = user.MobileNumber,
                            CallCenterKey = user.CallCenterKey
                        });

                        processedData.PaymentDto.ServiceAssociatedDto.RegisteredUserDto =
                            _serviceApplicationUser.GetUserByUserName(processedData.PaymentDto.ServiceAssociatedDto.RegisteredUserDto.Email);
                        processedData.PaymentDto.ServiceAssociatedDto.UserId =
                            processedData.PaymentDto.ServiceAssociatedDto.RegisteredUserDto.Id;
                        isNewUser = true;
                        _loggerService.CreateLog(LogType.Info, LogOperationType.Webhooks, LogCommunicationType.VisaNet,
                        processedData.PaymentDto.ServiceAssociatedDto.RegisteredUserDto.Id, tempStr, tempStr);
                        NLogLogger.LogAppsEvent(NLogType.Info, string.Format("ProccesDataFromCybersourceForApps - Creo Usuario - idApp: {0}, Operación: {1}, Usuario {2}", result.WebhookRegistrationDto.IdApp,
                        result.WebhookRegistrationDto.IdOperation, user.Email));
                        userCreated = true;
                        //SE CREO EL USUARIO EN ESTE PUNTO
                    }

                    var strLog = string.Format(LogStrings.Service_Apps_init, processedData.PaymentDto.ServiceAssociatedDto.RegisteredUserDto.Email,
                        service.UrlName, processedData.CyberSourceMerchantDefinedData.OperationId);
                    LogCybersourceData(processedData.PaymentDto, processedData, strLog, LogOperationType.Webhooks);
                    result.CybersourceCreateServiceAssociatedDto.CybersourceCreateCardDto.TokenizationData = processedData.TokenizationData;

                    //PRIMERO CREO LA TARJETA
                    var card = new CardDto()
                    {
                        Active = true,
                        CybersourceTransactionId =
                            processedData.PaymentDto.ServiceAssociatedDto.DefaultCard.CybersourceTransactionId,
                        DueDate = processedData.PaymentDto.ServiceAssociatedDto.DefaultCard.DueDate,
                        ExternalId = processedData.PaymentDto.ServiceAssociatedDto.DefaultCard.ExternalId,
                        MaskedNumber = processedData.PaymentDto.ServiceAssociatedDto.DefaultCard.MaskedNumber,
                        Name = processedData.PaymentDto.ServiceAssociatedDto.DefaultCard.Name,
                        PaymentToken = processedData.PaymentDto.ServiceAssociatedDto.DefaultCard.PaymentToken,
                    };
                    var newCard = _serviceApplicationUser.AddCard(card,
                        processedData.PaymentDto.ServiceAssociatedDto.RegisteredUserDto.Id);

                    NLogLogger.LogAppsEvent(NLogType.Info,
                        string.Format(
                            "ProccesDataFromCybersourceForApps - Creo Tarjeta - idApp: {0}, Operación: {1}, Tarjeta: {2}",
                            result.WebhookRegistrationDto.IdApp,
                            result.WebhookRegistrationDto.IdOperation, card.MaskedNumber));
                    cardCreated = true;
                    //SE CREO LA TARJETA EN ESTE PUNTO

                    processedData.PaymentDto.ServiceAssociatedDto.DefaultCardId = newCard.Id;
                    processedData.PaymentDto.ServiceAssociatedDto.DefaultCard = null;

                    ServiceAssociatedDto serviceAssociatedDto = null;
                    if (!isNewUser)
                    {
                        var refs = new string[6];
                        if (service.AskUserForReferences)
                        {
                            refs[0] = processedData.PaymentDto.ServiceAssociatedDto.ReferenceNumber;
                            refs[1] = processedData.PaymentDto.ServiceAssociatedDto.ReferenceNumber2;
                            refs[2] = processedData.PaymentDto.ServiceAssociatedDto.ReferenceNumber3;
                            refs[3] = processedData.PaymentDto.ServiceAssociatedDto.ReferenceNumber4;
                            refs[4] = processedData.PaymentDto.ServiceAssociatedDto.ReferenceNumber5;
                            refs[5] = processedData.PaymentDto.ServiceAssociatedDto.ReferenceNumber6;
                        }
                        serviceAssociatedDto =
                            ServiceAssosiatedToUser(processedData.PaymentDto.ServiceAssociatedDto.UserId,
                                service.Id, refs[0], refs[1], refs[2], refs[3], refs[4], refs[5]);
                    }

                    ServiceAssociatedDto finalServiceAssociatedDto = null;
                    //si el usuario ya tiene el servicio asociado o activo solamente seteo la tarjeta nueva 
                    if (serviceAssociatedDto != null && serviceAssociatedDto.Active)
                    {
                        var resultFromAdd = AddCardToService(serviceAssociatedDto.Id,
                            processedData.PaymentDto.ServiceAssociatedDto.DefaultCardId, Guid.Empty, user.Id,
                            processedData.CyberSourceMerchantDefinedData.OperationId);

                        if (resultFromAdd)
                        {
                            finalServiceAssociatedDto = GetById(serviceAssociatedDto.Id, x => x.Service,
                                x => x.NotificationConfig, x => x.Cards, x => x.DefaultCard, x => x.RegisteredUser);
                            NLogLogger.LogAppsEvent(NLogType.Info,
                                string.Format(
                                    "ProccesDataFromCybersourceForApps - Agrego tarjeta al servicio - idApp: {0}, Operación: {1}",
                                    result.WebhookRegistrationDto.IdApp,
                                    result.WebhookRegistrationDto.IdOperation));
                        }
                    }
                    else
                    {
                        finalServiceAssociatedDto = CreateOrUpdateDeleted(processedData.PaymentDto.ServiceAssociatedDto);
                        if (finalServiceAssociatedDto != null)
                        {
                            NLogLogger.LogAppsEvent(NLogType.Info,
                                string.Format(
                                    "ProccesDataFromCybersourceForApps - Creo servicio y agrgeo tarjeta - idApp: {0}, Operación: {1}",
                                    result.WebhookRegistrationDto.IdApp,
                                    result.WebhookRegistrationDto.IdOperation));
                        }
                        else
                        {
                            NLogLogger.LogAppsEvent(NLogType.Info,
                                string.Format(
                                    "ProccesDataFromCybersourceForApps - No se genero el servicio - idApp: {0}, Operación: {1}",
                                    result.WebhookRegistrationDto.IdApp,
                                    result.WebhookRegistrationDto.IdOperation));
                        }

                    }

                    result.CybersourceCreateServiceAssociatedDto.ServiceAssociatedDto = finalServiceAssociatedDto;
                    result.CybersourceCreateServiceAssociatedDto.CybersourceCreateCardDto.TokenizationData =
                        processedData.TokenizationData;

                    if (result.CybersourceCreateServiceAssociatedDto.ServiceAssociatedDto != null)
                    {
                        var strend = string.Format(LogStrings.Service_Apps_end_ok, processedData.CyberSourceMerchantDefinedData.OperationId);
                        _loggerService.CreateLog(LogType.Info, LogOperationType.Webhooks, LogCommunicationType.VisaNet,
                            processedData.PaymentDto.ServiceAssociatedDto.RegisteredUserDto.Id, strend, strend);
                        return result;
                    }
                }
                else
                {
                    NLogLogger.LogAppsEvent(NLogType.Info, string.Format("ProccesDataFromCybersourceForApps - Cybersource devolvio distinto de Ok - idApp: {0}, Operación: {1}, Codigo CS {2}",
                        result.WebhookRegistrationDto.IdApp, result.WebhookRegistrationDto.IdOperation, processedData.TokenizationData.PaymentResponseCode));
                }
            }
            catch (BusinessException ex)
            {
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("ServiceServiceAssosiate - ProccesDataFromCybersource - BusinessException. Operación {0}",
                    result.WebhookRegistrationDto != null ? result.WebhookRegistrationDto.IdOperation : string.Empty));
                NLogLogger.LogAppsEvent(ex);
                result.CybersourceCreateServiceAssociatedDto.ExceptionCapture = ex;
            }
            catch (ProviderWithoutConectionException ex)
            {
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("ServiceServiceAssosiate - ProccesDataFromCybersource - ProviderWithoutConectionException. Operación {0}",
                    result.WebhookRegistrationDto != null ? result.WebhookRegistrationDto.IdOperation : string.Empty));
                NLogLogger.LogAppsEvent(ex);
                result.CybersourceCreateServiceAssociatedDto.ExceptionCapture = ex;
            }
            catch (Exception e)
            {
                NLogLogger.LogAppsEvent(NLogType.Info, string.Format("ServiceServiceAssosiate - ProccesDataFromCybersource - Exception. Operación {0}",
                    result.WebhookRegistrationDto != null ? result.WebhookRegistrationDto.IdOperation : string.Empty));
                NLogLogger.LogAppsEvent(e);
                result.CybersourceCreateServiceAssociatedDto.ExceptionCapture = e;
            }
            result.CybersourceCreateServiceAssociatedDto.ServiceAssociatedDto = null;

            if (userCreated)
            {
                var email = user != null ? user.Email : string.Empty;
                if (cardCreated)
                {
                    result.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorCode = isNewUser ? 15 : 17;
                    result.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorDesc =
                        isNewUser ?
                        string.Format("Ha ocurrido un error. Se creo el usuario {0} y tambien se creo la tarjeta pero no se pudo terminar con el proceso de asociación. Por favor intente nuevamente y si el error persiste comuníquese con el call center", email)
                        : string.Format("Ha ocurrido un error. Para el usuario {0} se creo la tarjeta pero no se pudo terminar con el proceso de asociación. Por favor intente nuevamente y si el error persiste comuníquese con el call center", email);
                }
                else
                {
                    result.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorCode = 14;
                    result.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorDesc = string.Format(
                            "Ha ocurrido un error. Se creo el usuario {0} pero no se pudo terminar con el proceso de asociación. Por favor intente nuevamente y si el error persiste comuníquese con el call center",
                            email);
                }
            }
            else
            {
                result.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorCode = processedData.TokenizationData.PaymentResponseCode;
                result.CybersourceCreateServiceAssociatedDto.AssociationInternalErrorDesc = processedData.TokenizationData.PaymentResponseMsg;
            }
            return result;
        }

        //ASOCIACION DE SERVICIO TARJETA YA CREADA DESDE WEB
        public ServiceAssociatedDto AssociateServiceToUserFromCardCreated(ServiceAssociatedDto dto)
        {
            var user = _serviceApplicationUser.GetById(dto.UserId, x => x.Cards);
            var service = _serviceService.GetById(dto.ServiceId);

            var strLog = string.Format(LogStrings.Service_init, user.Email, user.Name, user.Surname, service.Name);
            _loggerService.CreateLog(LogType.Info, LogOperationType.ServiceAssociated, LogCommunicationType.VisaNet, user.Id, strLog);

            // chequeo que la tarjeta sea la del usuario
            var cardSelected = user.CardDtos.First(x => x.Id == dto.DefaultCardId);
            if (cardSelected == null)
            {
                return null;
            }
            if (cardSelected.ExternalId == null || cardSelected.ExternalId == Guid.Empty)
            {
                _repositoryCard.GenerateExternalId(dto.DefaultCardId);
            }

            var refs = new string[6];
            if (service.AskUserForReferences)
            {
                refs[0] = dto.ReferenceNumber;
                refs[1] = dto.ReferenceNumber2;
                refs[2] = dto.ReferenceNumber3;
                refs[3] = dto.ReferenceNumber4;
                refs[4] = dto.ReferenceNumber5;
                refs[5] = dto.ReferenceNumber6;
            }

            var serviceAssociatedDto = ServiceAssosiatedToUser(dto.UserId, dto.ServiceId, dto.ReferenceNumber, dto.ReferenceNumber2, dto.ReferenceNumber3,
                dto.ReferenceNumber4, dto.ReferenceNumber5, dto.ReferenceNumber6);

            //si el usuario ya tiene el servicio asociado solamente seteo la tarjeta nueva
            if (serviceAssociatedDto != null && serviceAssociatedDto.Active)
            {
                var resultFromAdd = AddCardToService(serviceAssociatedDto.Id, dto.DefaultCardId, Guid.Empty, user.Id, dto.OperationId);
                if (resultFromAdd)
                {
                    NLogLogger.LogAppsEvent(NLogType.Info,
                        string.Format(
                            "ProccesDataFromCybersourceForApps - Agrego tarjeta al servicio - idApp: {0}, Operación: {1}",
                            service.UrlName,
                            dto.OperationId));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                dto.Enabled = true;
                dto.Active = true;

                #region Notificaciones
                if (dto.NotificationConfigDto == null)
                {
                    var noticonf = new NotificationConfigDto()
                    {
                        DaysBeforeDueDate = 5,
                        BeforeDueDateConfigDto = new DaysBeforeDueDateConfigDto()
                        {
                            Email = false,
                            Sms = false,
                            Web = false
                        },
                        ExpiredBillDto = new ExpiredBillDto()
                        {
                            Email = false,
                            Sms = false,
                            Web = false
                        },
                        NewBillDto = new NewBillDto()
                        {
                            Email = false,
                            Sms = false,
                            Web = false
                        },
                        FailedAutomaticPaymentDto = new FailedAutomaticPaymentDto()
                        {
                            Email = false,
                            Sms = false,
                            Web = false
                        },
                        SuccessPaymentDto = new SuccessPaymentDto()
                        {
                            Email = true,
                            Sms = false,
                            Web = false
                        }
                    };
                    dto.NotificationConfigDto = noticonf;
                }
                #endregion

                serviceAssociatedDto = CreateOrUpdateDeleted(dto);
                NLogLogger.LogAppsEvent(NLogType.Info, "AppAdmissionController - Associate - Servicio asociado a usuario " + user.Email + ", Operacion " + dto.OperationId);
            }

            return serviceAssociatedDto;
        }

        public void LogCybersourceData(PaymentDto payment, CybersourceTransactionsDataDto csTransactionsDataDto, string msg, LogOperationType logOperationType)
        {
            Guid tempGuid;
            Guid? tempGuid2 = null;
            tempGuid2 = Guid.TryParse(csTransactionsDataDto.CyberSourceMerchantDefinedData.TemporaryTransactionIdentifier, out tempGuid) ? tempGuid : tempGuid2;

            var cyberSourceData = csTransactionsDataDto.CyberSourceData;
            var verifyByVisaData = csTransactionsDataDto.VerifyByVisaData;

            if (payment.RegisteredUser != null)
            {
                _loggerService.CreateLog(LogType.Info, logOperationType, LogCommunicationType.VisaNet,
                    payment.RegisteredUser.Id, msg, msg, null,
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
                    tempGuid2
                    );
            }
        }

        //Metodo para listado de servicios asociados de la APP MOBILE VISANETPAGOS
        public IList<ServiceAssociatedDto> GetDataForFrontList(ServiceFilterAssosiateDto filters)
        {
            var list = GetDataForTable(filters).ToList();

            foreach (var serviceAssociatedDto in list)
            {
                serviceAssociatedDto.CardDtos = serviceAssociatedDto.ServiceDto.AllowMultipleCards ?
                            serviceAssociatedDto.CardDtos != null ? serviceAssociatedDto.CardDtos.Select(c => new CardDto()
                            {
                                MaskedNumber = c.MaskedNumber,
                                Id = c.Id,
                                Description = c.Description
                            }).ToList() :
                                                                        new List<CardDto>() :
                                                                        new List<CardDto>() {new CardDto()
                                                                                                    {
                                                                                                        MaskedNumber = serviceAssociatedDto.DefaultCard.MaskedNumber,
                                                                                                        Id = serviceAssociatedDto.DefaultCardId,
                                                                                                        Description = serviceAssociatedDto.DefaultCard.Description
                                                                                                    }};
                var allowGetBills = serviceAssociatedDto.ServiceDto.ServiceGatewaysDto.Any(x => x.Gateway.Enum == (int)GatewayEnumDto.Banred && x.Active
                    || x.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc && x.Active
                    || x.Gateway.Enum == (int)GatewayEnumDto.Carretera && x.Active
                    || x.Gateway.Enum == (int)GatewayEnumDto.Sucive && x.Active
                    || x.Gateway.Enum == (int)GatewayEnumDto.Geocom && x.Active);

                //serviceAssociatedDto.AllowGetBills = allowGetBills;
                serviceAssociatedDto.ServiceDto.AllowGetBills = allowGetBills;

                var allowInputAmount = serviceAssociatedDto.ServiceDto.ServiceGatewaysDto.Any(x => x.Gateway.Enum == (int)GatewayEnumDto.Importe && x.Active);
                //serviceAssociatedDto.AllowInputAmount = allowInputAmount;
                serviceAssociatedDto.ServiceDto.AllowInputAmount = allowInputAmount;

                var gateways = serviceAssociatedDto.ServiceDto.ServiceGatewaysDto.Any(x => (x.Gateway.Enum == (int)GatewayEnumDto.Banred && x.Active)
                        || (x.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc && x.Active) || (x.Gateway.Enum == (int)GatewayEnumDto.Carretera && x.Active)
                        || (x.Gateway.Enum == (int)GatewayEnumDto.Geocom && x.Active) || (x.Gateway.Enum == (int)GatewayEnumDto.Sucive && x.Active));

                serviceAssociatedDto.NotificationConfigDto.BeforeDueDateConfigDto = gateways ? serviceAssociatedDto.NotificationConfigDto.BeforeDueDateConfigDto : null;
                serviceAssociatedDto.NotificationConfigDto.ExpiredBillDto = gateways ? serviceAssociatedDto.NotificationConfigDto.ExpiredBillDto : null;
                serviceAssociatedDto.NotificationConfigDto.FailedAutomaticPaymentDto = gateways ? serviceAssociatedDto.NotificationConfigDto.FailedAutomaticPaymentDto : null;
                serviceAssociatedDto.NotificationConfigDto.NewBillDto = gateways ? serviceAssociatedDto.NotificationConfigDto.NewBillDto : null;

                _serviceService.SetImporteValues(serviceAssociatedDto.ServiceDto);

            }
            return list;
        }

        private IList<string> ServicesMomentarilyDisabledIds()
        {
            try
            {
                var servicesOfflineList = new List<string>();
                var fileName = ConfigurationManager.AppSettings["ServiceIdMomentarilyDisabled"];
                using (var r = new StreamReader(fileName))
                {
                    string json = r.ReadToEnd();
                    var services = JsonConvert.DeserializeObject<IEnumerable<ServiceOfflineDto>>(json);
                    var today = DateTime.Now;

                    var servicesOffline = services.Where(x => today >= x.DateFrom && today <= x.DateTo).ToList();

                    foreach (var serviceOfflineDto in servicesOffline)
                    {
                        servicesOfflineList.AddRange(serviceOfflineDto.IdApps);
                    }
                    return servicesOfflineList;
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "Error en metodo ServiceServiceAssociate - ServicesMomentarilyDisabled");
                NLogLogger.LogEvent(e);
                return null;
            }
        }

        public ServiceAssociatedDto GetServiceAssociatedDtoFromIdUserExternal(string idUserExternal, string idApp)
        {
            var userId = Guid.Parse(idUserExternal);

            var query = AllNoTracking(null, x => x.IdUserExternal == userId,
                    x => x.RegisteredUser,
                    x => x.RegisteredUser.MembershipIdentifierObj,
                    x => x.Service,
                    x => x.Service.ServiceContainer,
                    x => x.DefaultCard,
                    x => x.Cards
                );

            query = query.Where(x => x.ServiceDto.UrlName.Equals(idApp) ||
                (x.ServiceDto.ServiceContainerDto != null && x.ServiceDto.ServiceContainerDto.UrlName.Equals(idApp)));

            var serviceAssociated = query.FirstOrDefault();
            return serviceAssociated;
        }

        private bool NotifyExternalSourceAssociation(ServiceAssociated sa, Card card, string idOperation)
        {
            var notificationOk = _serviceExternalNotification.NotifyExternalSourceNewAssociation(sa, card, idOperation);
            return true;
        }

        private bool NotifyExternalSourceAssociation(ServiceAssociatedDto saDto, CardDto cardDto, string idOperation)
        {
            var sa = Converter(saDto);
            var card = sa.Cards.FirstOrDefault(x => x.PaymentToken == cardDto.PaymentToken);
            if (card == null && sa.DefaultCard.PaymentToken == cardDto.PaymentToken)
            {
                card = sa.DefaultCard;
                var notificationOk = NotifyExternalSourceAssociation(sa, card, idOperation);
                return true;
            }
            return false;
        }

        private bool NotifyExternalSourceCard(ServiceAssociated sa, Card card, string idOperation)
        {
            var notificationOk = _serviceExternalNotification.NotifyExternalSourceNewCard(sa, card, idOperation);
            return true;
        }

        private bool NotifyExternalSourceDeleteCard(ServiceAssociated sa, Guid externalCardId)
        {
            var notificationOk = _serviceExternalNotification.NotifyExternalSourceDownCard(sa, externalCardId);
            return true;
        }

        private bool NotifyExternalSourceDeleteAssociation(ServiceAssociated sa)
        {
            var notificationOk = _serviceExternalNotification.NotifyExternalSourceDownService(sa);
            return true;
        }

    }
}