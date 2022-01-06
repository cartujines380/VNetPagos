using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
using VisaNet.Common.Resource.Helpers;
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

namespace VisaNet.Application.Implementations
{
    public class ServiceService : BaseService<Service, ServiceDto>, IServiceService
    {
        private readonly IRepositoryGateway _repositoryGateway;
        private readonly IRepositoryServiceAsociated _repositoryServiceAsociated;
        private readonly IRepositoryPayment _repositoryPayment;
        private readonly IRepositoryApplicationUser _repositoryApplicationUser;
        private readonly IRepositoryBin _repositoryBin;
        private readonly IServiceHighway _serviceHighway;
        private readonly IRepositoryNotification _repositoryNotification;
        private readonly ILoggerService _loggerService;
        private readonly IRepositoryCard _repositoryCard;
        private readonly IServiceFixedNotification _serviceFixedNotification;
        private readonly IServiceEmailMessage _serviceNotificationMessage;
        private readonly IRepositoryBinGroup _repositoryBinGroup;

        private string _folderBlob = ConfigurationManager.AppSettings["AzureServicesImagesUrl"];

        public ServiceService(IRepositoryService repository, IRepositoryGateway repositoryGateway, IRepositoryServiceAsociated repositoryServiceAsociated,
            IRepositoryPayment repositoryPayment, IRepositoryApplicationUser repositoryApplicationUser, IRepositoryBin repositoryBin, IServiceHighway serviceHighway,
            IRepositoryNotification repositoryNotification, ILoggerService loggerService, IRepositoryCard repositoryCard, IServiceFixedNotification serviceFixedNotification,
            IServiceEmailMessage serviceNotificationMessage, IRepositoryBinGroup repositoryBinGroup)
            : base(repository)
        {
            _repositoryGateway = repositoryGateway;
            _repositoryServiceAsociated = repositoryServiceAsociated;
            _repositoryPayment = repositoryPayment;
            _repositoryApplicationUser = repositoryApplicationUser;
            _repositoryBin = repositoryBin;
            _serviceHighway = serviceHighway;
            _repositoryNotification = repositoryNotification;
            _loggerService = loggerService;
            _repositoryCard = repositoryCard;
            _serviceFixedNotification = serviceFixedNotification;
            _serviceNotificationMessage = serviceNotificationMessage;
            _repositoryBinGroup = repositoryBinGroup;
        }

        public override IQueryable<Service> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override ServiceDto Converter(Service entity)
        {
            var serviceDto = new ServiceDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                DescriptionTooltip = entity.DescriptionTooltip,
                ReferenceParamName = entity.ReferenceParamName,
                ReferenceParamName2 = entity.ReferenceParamName2,
                ReferenceParamName3 = entity.ReferenceParamName3,
                ReferenceParamName4 = entity.ReferenceParamName4,
                ReferenceParamName5 = entity.ReferenceParamName5,
                ReferenceParamName6 = entity.ReferenceParamName6,
                ReferenceParamRegex = entity.ReferenceParamRegex,
                ReferenceParamRegex2 = entity.ReferenceParamRegex2,
                ReferenceParamRegex3 = entity.ReferenceParamRegex3,
                ReferenceParamRegex4 = entity.ReferenceParamRegex4,
                ReferenceParamRegex5 = entity.ReferenceParamRegex5,
                ReferenceParamRegex6 = entity.ReferenceParamRegex6,
                Active = entity.Active,
                MerchantId = entity.MerchantId,
                CybersourceAccessKey = entity.CybersourceAccessKey,
                CybersourceProfileId = entity.CybersourceProfileId,
                CybersourceSecretKey = entity.CybersourceSecretKey,
                CybersourceTransactionKey = entity.CybersourceTransactionKey,
                LinkId = entity.LinkId,
                ServiceCategoryId = entity.ServiceCategoryId,
                Tags = entity.Tags,
                ImageName = entity.ImageName,
                ImageUrl = FileStorage.Instance.GetImageUrl(_folderBlob, entity.Id, entity.ImageName),
                ImageTooltipName = entity.ImageTooltipName,
                ImageTooltipUrl = FileStorage.Instance.GetImageTooltipUrl(_folderBlob, entity.Id, entity.ImageTooltipName),
                EnableMultipleBills = entity.EnableMultipleBills,
                Departament = (DepartamentDtoType)(int)entity.Departament,
                ServiceGatewaysDto = new List<ServiceGatewayDto>(),
                ExtractEmail = entity.ExtractEmail,
                CertificateThumbprintExternal = entity.CertificateThumbprintExternal,
                EnableAutomaticPayment = entity.EnableAutomaticPayment,
                EnablePrivatePayment = entity.EnablePrivatePayment,
                EnablePublicPayment = entity.EnablePublicPayment,
                EnablePartialPayment = entity.EnablePartialPayment,
                EnableAssociation = entity.EnableAssociation,
                PostAssociationDesc = entity.PostAssociationDesc,
                TermsAndConditions = entity.TermsAndConditions,
                UrlName = entity.UrlName,
                DiscountType = (DiscountTypeDto)(int)entity.DiscountType,
                ServiceContainerId = entity.ServiceContainerId,
                ExternalUrlAdd = entity.ExternalUrlAdd,
                ExternalUrlRemove = entity.ExternalUrlRemove,
                CertificateThumbprintVisa = entity.CertificateThumbprintVisa,
                Container = entity.Container,
                AllowSelectContentAssociation = entity.AllowSelectContentAssociation,
                AllowSelectContentPayment = entity.AllowSelectContentPayment,
                AskUserForReferences = entity.AskUserForReferences,
                AllowMultipleCards = entity.AllowMultipleCards,
                MaxQuotaAllow = entity.MaxQuotaAllow,
                IntroContent = entity.IntroContent,
                BinGroups = entity.BinGroups != null ? entity.BinGroups.Select(bg => new BinGroupDto
                {
                    Id = bg.Id,
                    Name = bg.Name
                }).ToList() : null,
                InterpreterId = entity.InterpreterId,
                InterpreterAuxParam = entity.InterpreterAuxParam,
                UrlIntegrationVersion = (UrlIntegrationVersionEnumDto)entity.UrlIntegrationVersion,
                InformCardBank = entity.InformCardBank,
                InformCardType = entity.InformCardType,
                InformCardAffiliation = entity.InformCardAffiliation,
                AllowVon = entity.AllowVon,
                AllowWcfPayment = entity.AllowWcfPayment,
                Sort = entity.Sort,
            };

            if (entity.ServiceCategory != null)
            {
                serviceDto.ServiceCategory = new ServiceCategoryDto()
                {
                    Id = entity.ServiceCategory.Id,
                    Name = entity.ServiceCategory.Name
                };
            }

            if (entity.ServiceGateways != null && entity.ServiceGateways.Count > 0)
            {
                foreach (var sg in entity.ServiceGateways)
                {
                    var serviceGatewayDto = new ServiceGatewayDto()
                    {
                        Active = sg.Active,
                        Id = sg.Id,
                        ReferenceId = sg.ReferenceId,
                        GatewayId = sg.GatewayId,
                        ServiceType = sg.ServiceType,
                        SendExtract = sg.SendExtract,
                        AuxiliarData = sg.AuxiliarData,
                        AuxiliarData2 = sg.AuxiliarData2,
                        Gateway = sg.Gateway != null
                                                    ? new GatewayDto()
                                                    {
                                                        Id = sg.GatewayId,
                                                        Name = sg.Gateway.Name,
                                                        Enum = sg.Gateway.Enum
                                                    }
                                                    : null
                    };
                    serviceDto.ServiceGatewaysDto.Add(serviceGatewayDto);
                }
            }
            if (entity.HighwayEnableEmails != null && entity.HighwayEnableEmails.Count > 0)
            {
                serviceDto.HighwayEnableEmails = new List<ServiceEnableEmailDto>();
                foreach (var sg in entity.HighwayEnableEmails)
                {
                    var email = new ServiceEnableEmailDto()
                    {
                        Id = sg.Id,
                        ServiceId = serviceDto.Id,
                        Email = sg.Email
                    };
                    serviceDto.HighwayEnableEmails.Add(email);
                }
            }
            if (entity.ServiceContainer != null)
            {
                serviceDto.ServiceContainerDto = Converter(entity.ServiceContainer);
            }
            return serviceDto;
        }

        public override Service Converter(ServiceDto entity)
        {
            var service = new Service()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                DescriptionTooltip = entity.DescriptionTooltip,
                ReferenceParamName = entity.ReferenceParamName,
                ReferenceParamName2 = entity.ReferenceParamName2,
                ReferenceParamName3 = entity.ReferenceParamName3,
                ReferenceParamName4 = entity.ReferenceParamName4,
                ReferenceParamName5 = entity.ReferenceParamName5,
                ReferenceParamName6 = entity.ReferenceParamName6,
                ReferenceParamRegex = entity.ReferenceParamRegex,
                ReferenceParamRegex2 = entity.ReferenceParamRegex2,
                ReferenceParamRegex3 = entity.ReferenceParamRegex3,
                ReferenceParamRegex4 = entity.ReferenceParamRegex4,
                ReferenceParamRegex5 = entity.ReferenceParamRegex5,
                ReferenceParamRegex6 = entity.ReferenceParamRegex6,
                Active = entity.Active,
                MerchantId = entity.MerchantId,
                CybersourceAccessKey = entity.CybersourceAccessKey,
                CybersourceProfileId = entity.CybersourceProfileId,
                CybersourceSecretKey = entity.CybersourceSecretKey,
                CybersourceTransactionKey = entity.CybersourceTransactionKey,
                LinkId = entity.LinkId,
                ServiceCategoryId = entity.ServiceCategoryId,
                Tags = entity.Tags,
                ImageName = entity.ImageName,
                ImageTooltipName = entity.ImageTooltipName,
                EnableMultipleBills = entity.EnableMultipleBills,
                Departament = (DepartamentType)(int)entity.Departament,
                ExtractEmail = entity.ExtractEmail,
                CertificateThumbprintExternal = entity.CertificateThumbprintExternal,
                EnableAutomaticPayment = entity.EnableAutomaticPayment,
                EnablePrivatePayment = entity.EnablePrivatePayment,
                EnablePublicPayment = entity.EnablePublicPayment,
                EnablePartialPayment = entity.EnablePartialPayment,
                EnableAssociation = entity.EnableAssociation,
                PostAssociationDesc = entity.PostAssociationDesc,
                TermsAndConditions = entity.TermsAndConditions,
                UrlName = entity.UrlName,
                DiscountType = (DiscountType)(int)entity.DiscountType,
                ServiceContainerId = entity.ServiceContainerId,
                ExternalUrlAdd = entity.ExternalUrlAdd,
                ExternalUrlRemove = entity.ExternalUrlRemove,
                CertificateThumbprintVisa = entity.CertificateThumbprintVisa,
                Container = entity.Container,
                AllowSelectContentAssociation = entity.AllowSelectContentAssociation,
                AllowSelectContentPayment = entity.AllowSelectContentPayment,
                AskUserForReferences = entity.AskUserForReferences,
                AllowMultipleCards = entity.AllowMultipleCards,
                MaxQuotaAllow = entity.MaxQuotaAllow,
                IntroContent = entity.IntroContent,
                BinGroups = entity.BinGroups != null ? entity.BinGroups.Select(bg => new BinGroup
                {
                    Id = bg.Id,
                    Name = bg.Name
                }).ToList() : null,
                InterpreterId = entity.InterpreterId == Guid.Empty ? null : entity.InterpreterId,
                InterpreterAuxParam = entity.InterpreterAuxParam,
                UrlIntegrationVersion = (UrlIntegrationVersionEnum)entity.UrlIntegrationVersion,
                InformCardBank = entity.InformCardBank,
                InformCardType = entity.InformCardType,
                InformCardAffiliation = entity.InformCardAffiliation,
                AllowVon = entity.AllowVon,
                AllowWcfPayment = entity.AllowWcfPayment,
                Sort = entity.Sort,
            };

            if (entity.ServiceGatewaysDto != null && entity.ServiceGatewaysDto.Count > 0)
            {
                service.ServiceGateways = new List<ServiceGateway>();
                foreach (var sg in entity.ServiceGatewaysDto)
                {
                    var serviceGateway = new ServiceGateway()
                    {
                        Active = sg.Active,
                        Id = sg.Id,
                        ReferenceId = sg.ReferenceId,
                        GatewayId = sg.GatewayId,
                        ServiceType = sg.ServiceType,
                        SendExtract = sg.SendExtract,
                        AuxiliarData = sg.AuxiliarData,
                        AuxiliarData2 = sg.AuxiliarData2,
                    };
                    if (serviceGateway.Id == Guid.Empty)
                        serviceGateway.GenerateNewIdentity();

                    service.ServiceGateways.Add(serviceGateway);
                }
            }
            if (entity.HighwayEnableEmails != null && entity.HighwayEnableEmails.Count > 0)
            {
                service.HighwayEnableEmails = new List<ServiceEnableEmail>();
                foreach (var sg in entity.HighwayEnableEmails)
                {
                    var email = new ServiceEnableEmail()
                    {
                        Id = sg.Id,
                        Email = sg.Email,
                        ServiceId = service.Id,
                        RouteId = sg.RouteId
                    };
                    if (email.Id == Guid.Empty)
                        email.GenerateNewIdentity();

                    service.HighwayEnableEmails.Add(email);
                }
            }
            return service;
        }

        public IEnumerable<ServiceDto> GetServicesFromMerchand(string idApp, string merchandId, GatewayEnumDto gateway)
        {
            var gatewayId = GetGateway(gateway).Id;
            var query = Repository.AllNoTracking(s => s.MerchantId == merchandId && (s.UrlName == idApp || s.ServiceContainer.UrlName.Equals(idApp))
                //&& s.ServiceGateways.Any(x => x.GatewayId == gatewayId)
                , s => s.HighwayEnableEmails, s => s.ServiceGateways,
                s => s.ServiceContainer, x => x.ServiceCategory);
            var services = query.ToList();
            return services.Select(Converter).ToList();
        }

        public IEnumerable<ServiceDto> GetServices(string idApp, int codCommerce, int codBranch, GatewayEnumDto gateway)
        {
            var codco = codCommerce.ToString();
            var codbr = codBranch.ToString();
            var gatewayId = GetGateway(gateway).Id;
            var query = Repository.AllNoTracking(s => (s.UrlName == idApp || s.ServiceContainer.UrlName.Equals(idApp)) && s.ServiceGateways.Any(x => x.ReferenceId.Equals(codco) && x.ServiceType.Equals(codbr) && x.GatewayId == gatewayId),
                                                    s => s.HighwayEnableEmails, s => s.ServiceGateways, s => s.ServiceContainer, x => x.ServiceCategory);
            var services = query.ToList();
            return services.Select(Converter).ToList();
        }

        public List<GatewayDto> GetGateways()
        {
            var list = _repositoryGateway.All().OrderBy(x => x.Enum);
            return list.Select(gt => new GatewayDto() { Id = gt.Id, Name = gt.Name, Enum = gt.Enum }).ToList();
        }

        public GatewayDto GetGateway(GatewayEnumDto enumDto)
        {
            var item = _repositoryGateway.All().FirstOrDefault(g => g.Enum == (int)enumDto);
            return item == null ? null : new GatewayDto() { Id = item.Id, Name = item.Name, Enum = item.Enum };

        }

        public override ServiceDto Create(ServiceDto entity, bool returnEntity = false)
        {
            if (Repository.AllNoTracking(s => s.Name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase)).Any())
            {
                throw new BusinessException(CodeExceptions.SERVICE_NAME_ALREADY_USED);
            }

            if (!entity.BinGroups.Any())
            {
                throw new BusinessException(CodeExceptions.SERVICE_BINGROUP_REQUIRED);
            }

            if (!entity.Container)
            {
                if (String.IsNullOrEmpty(entity.CybersourceAccessKey))
                {
                    throw new BusinessException(CodeExceptions.CYBERSOURCE_ACCESS_KEY);
                }
                if (String.IsNullOrEmpty(entity.CybersourceSecretKey))
                {
                    throw new BusinessException(CodeExceptions.CYBERSOURCE_SECRET_KEY);
                }
            }

            if (!entity.AskUserForReferences || string.IsNullOrEmpty(entity.ReferenceParamName))
            {
                if (entity.ServiceContainerId.HasValue && entity.ServiceContainerId.Value != Guid.Empty)
                {
                    var serviceContainer = Repository.AllNoTracking(s => s.Id.Equals(entity.ServiceContainerId.Value)).FirstOrDefault();
                    if (serviceContainer != null && (entity.AskUserForReferences || serviceContainer.AskUserForReferences)
                        && string.IsNullOrEmpty(entity.ReferenceParamName) && string.IsNullOrEmpty(serviceContainer.ReferenceParamName))
                    {
                        //El servicio o su contenedor piden referencias y ninguno tiene
                        throw new MissingServiceReferenceParamsException(CodeExceptions.SERVICE_REFERENCES_EMPTY);
                    }
                }
                else if (entity.AskUserForReferences)
                {
                    //El servicio pide referencias y no las tiene, ademas no tiene servicio contenedor
                    throw new MissingServiceReferenceParamsException(CodeExceptions.SERVICE_REFERENCES_EMPTY);
                }
            }

            entity.LinkId = !String.IsNullOrEmpty(entity.LinkId) && entity.LinkId.Contains("http") ? entity.LinkId : "http://" + entity.LinkId;

            if (entity.HighwayEnableEmails != null)
            {
                var emailsToAdd = entity.HighwayEnableEmails.Select(gt => new ServiceEnableEmail
                {
                    Id = Guid.NewGuid(),
                    Email = gt.Email,
                    ServiceId = entity.Id
                }).ToList();

                if (emailsToAdd.Any())
                {
                    var listEmailsUpdated = _serviceHighway.CreateRoutes(emailsToAdd);
                    entity.HighwayEnableEmails = listEmailsUpdated.Select(x =>
                        new ServiceEnableEmailDto
                        {
                            Email = x.Email,
                            Id = x.Id,
                            ServiceId = x.ServiceId,
                            RouteId = x.RouteId
                        }).ToList();
                }
            }

            Repository.ContextTrackChanges = true;
            var dbEntity = this.Converter(entity);

            var binGroupsAdded = entity.BinGroups.ToList();
            dbEntity.BinGroups.Clear();
            foreach (var binGroupDto in binGroupsAdded)
            {
                var dto = binGroupDto;
                dbEntity.BinGroups.Add(_repositoryBinGroup.All(x => x.Id == dto.Id).First());
            }

            Repository.Create(dbEntity);
            Repository.Save();

            Repository.ContextTrackChanges = false;

            return returnEntity ? Converter(dbEntity) : null;
        }

        public IEnumerable<ServiceDto> GetDataForTable(ServiceFilterDto filters)
        {

            var serviceContainerId = Guid.Empty;
            Guid.TryParse(filters.ServiceContainerId, out serviceContainerId);

            var query = Repository.AllNoTracking(null, x => x.ServiceCategory, x => x.ServiceContainer, x => x.ServiceGateways);

            if (filters.ServiceId != Guid.Empty)
            {
                query = query.Where(x => x.Id == filters.ServiceId);
            }

            if (filters.IsContainer != null)
                query = query.Where(x => x.Container == filters.IsContainer);

            if (filters.WithoutServiceInContainer)
                query = query.Where(x => x.ServiceContainerId == null || x.ServiceContainerId == Guid.Empty);

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => sc.Name.ToLower().Contains(filters.GenericSearch.ToLower())
                                          || sc.Description.ToLower().Contains(filters.GenericSearch.ToLower())
                                          || sc.ServiceCategory.Name.ToLower().Contains(filters.GenericSearch.ToLower())
                    );

            if (!string.IsNullOrWhiteSpace(filters.Name))
                query = query.Where(sc => sc.Name.ToLower().Contains(filters.Name.ToLower()));

            if (filters.ServiceCategoryId != Guid.Empty)
                query = query.Where(sc => sc.ServiceCategoryId == filters.ServiceCategoryId);

            if (!string.IsNullOrWhiteSpace(filters.ServiceCategory))
                query = query.Where(sc => sc.ServiceCategory.Name.ToLower().Contains(filters.ServiceCategory.ToLower()));

            if (!string.IsNullOrWhiteSpace(filters.ServiceContainerName))
                query = query.Where(sc => sc.ServiceContainerId != null && sc.ServiceContainerId != Guid.Empty && sc.ServiceContainer.Name.ToLower().Contains(filters.ServiceContainerName.ToLower()));

            if (serviceContainerId != Guid.Empty)
            {
                query = query.Where(sc => sc.ServiceContainerId == serviceContainerId);
            }

            //if (filters.ServiceWithOutContainerAndContainer != null)
            //    query = query.Where(s => s.ServiceContainerId == null || s.ServiceContainerId == Guid.Empty);

            //para mostrar solo los servicios a pagar (los que no tienen la pasarela Apps)
            //if (filters.OnlyToPay)
            //    query = query.Where(x => (x.ServiceContainerId == null || x.ServiceContainerId == Guid.Empty) && x.EnablePrivatePayment && x.EnablePublicPayment);

            ////para mostrar solo los servicios a asociar (los que no tienen servicio contenedor)
            //if (filters.OnlyToAssociate)
            //    query = query.Where(x => x.ServiceContainerId == null || x.ServiceContainerId == Guid.Empty);

            if (filters.EnableAssociation)
                query = query.Where(x => x.EnableAssociation);

            if (filters.EnablePrivatePayment)
                query = query.Where(x => x.EnablePrivatePayment);

            if (filters.EnablePublicPayment)
                query = query.Where(x => x.EnablePublicPayment);

            if (filters.Active)
                query = query.Where(x => x.Active);

            //if (string.IsNullOrEmpty(filters.State) || filters.State == "Activos")
            //    query = query.Where(sc => sc.Active);

            //if (filters.State == "Inactivos")
            //    query = query.Where(sc => !sc.Active);

            if (filters.Gateway > 0)
            {
                query = query.Where(x => x.ServiceGateways.Any(y => y.Gateway.Enum == filters.Gateway && y.Active));
            }

            if (filters.OrderBy.Equals("ServiceCategory"))
            {
                query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.ServiceCategory.Name) : query.OrderByDescending(x => x.ServiceCategory.Name);
            }
            else if (filters.OrderBy.Equals("ServiceContainerDto"))
            {
                query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.ServiceContainer.Name) : query.OrderByDescending(x => x.ServiceContainer.Name);
            }
            else
            {
                query = filters.SortDirection == SortDirection.Asc ? query.OrderByStringProperty(filters.OrderBy) : query.OrderByStringPropertyDescending(filters.OrderBy);
            }

            var services = query.Select(t => new ServiceDto
            {
                Id = t.Id,
                Name = t.Name,
                Active = t.Active,
                Description = t.Description,
                ServiceCategoryName = t.ServiceCategory.Name,
                ServiceCategoryId = t.ServiceCategoryId,
                Sort = t.Sort,
                ReferenceParamName = t.ReferenceParamName,
                ReferenceParamName2 = t.ReferenceParamName2,
                ReferenceParamName3 = t.ReferenceParamName3,
                ReferenceParamName4 = t.ReferenceParamName4,
                ReferenceParamName5 = t.ReferenceParamName5,
                ReferenceParamName6 = t.ReferenceParamName6,

                ServiceGatewaysDto = t.ServiceGateways.Select(g => new ServiceGatewayDto
                {
                    Active = g.Active,
                    GatewayId = g.GatewayId,
                    ReferenceId = g.ReferenceId,
                    ServiceType = g.ServiceType,
                    AuxiliarData = g.AuxiliarData,
                    AuxiliarData2 = g.AuxiliarData2,
                    Gateway = new GatewayDto()
                    {
                        Enum = g.Gateway != null ? g.Gateway.Enum : 0,
                        Name = g.Gateway != null ? g.Gateway.Name : string.Empty
                    }
                }).ToList(),
                Tags = t.Tags,
                PostAssociationDesc = t.PostAssociationDesc,
                TermsAndConditions = t.TermsAndConditions,
                ServiceContainerName = t.ServiceContainerId != null && t.ServiceContainerId != Guid.Empty ? t.ServiceContainer.Name : "",
                //DiscountType = (DiscountType)(int)t.DiscountType,
                Departament = (DepartamentDtoType)(int)t.Departament,
                ImageName = t.ImageName
            }).ToList();

            foreach (var s in services)
            {
                s.ImageUrl = FileStorage.Instance.GetImageUrl(_folderBlob, s.Id, s.ImageName);
            }

            return services;
        }

        public IEnumerable<ServiceDto> GetServicesAutoComplete(string contains)
        {
            if (string.IsNullOrEmpty(contains))
                return new List<ServiceDto>();
            contains = contains.ToLower();

            var data =
                Repository.AllNoTracking(s => s.Name.ToLower().Contains(contains) || s.Tags.ToLower().Contains(contains))
                    .Select(n => new ServiceDto
                    {
                        Id = n.Id,
                        Name = n.Name,
                        Sort = n.Sort,
                    }).ToList();

            return data;
        }

        public override void Edit(ServiceDto entity)
        {
            if (Repository.AllNoTracking(s => s.Name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase)).Any(s => s.Id != entity.Id))
            {
                throw new BusinessException(CodeExceptions.SERVICE_NAME_ALREADY_USED);
            }

            if (!entity.BinGroups.Any())
            {
                throw new BusinessException(CodeExceptions.SERVICE_BINGROUP_REQUIRED);
            }

            if (!entity.AskUserForReferences || string.IsNullOrEmpty(entity.ReferenceParamName))
            {
                if (entity.ServiceContainerId.HasValue && entity.ServiceContainerId.Value != Guid.Empty)
                {
                    var serviceContainer = Repository.AllNoTracking(s => s.Id.Equals(entity.ServiceContainerId.Value)).FirstOrDefault();
                    if (serviceContainer != null && (entity.AskUserForReferences || serviceContainer.AskUserForReferences)
                        && string.IsNullOrEmpty(entity.ReferenceParamName) && string.IsNullOrEmpty(serviceContainer.ReferenceParamName))
                    {
                        //El servicio o su contenedor piden referencias y ninguno tiene
                        throw new MissingServiceReferenceParamsException(CodeExceptions.SERVICE_REFERENCES_EMPTY);
                    }
                }
                else if (entity.AskUserForReferences)
                {
                    //El servicio pide referencias y no las tiene, ademas no tiene servicio contenedor
                    throw new MissingServiceReferenceParamsException(CodeExceptions.SERVICE_REFERENCES_EMPTY);
                }
            }

            if (entity.ServiceContainerId != null && entity.ServiceContainerId != Guid.Empty)
            {
                var allreadyContainer = Repository.AllNoTracking().Any(x => entity.Id == x.ServiceContainerId);
                if (allreadyContainer)
                {
                    throw new BusinessException(CodeExceptions.SERVICE_IS_CONTAINER);
                }
            }

            //Valido que si viene habilitada la pasarela apps, ningun otro servicio (con la pasarela apps habilitada) tenga el mismo merchantId
            if (!string.IsNullOrEmpty(entity.MerchantId) && !entity.Container)
            {
                var gatewayAppsId = _repositoryGateway.AllNoTracking().First(x => x.Enum == (int)GatewayEnum.Apps).Id;
                if (entity.ServiceGatewaysDto.Any(sg => sg.GatewayId == gatewayAppsId && sg.Active) && Repository.All().Any(s => s.Id != entity.Id && s.MerchantId == entity.MerchantId && (s.ServiceGateways.Any(sg => sg.Gateway.Enum == (int)GatewayEnum.Apps))))
                {
                    throw new BusinessException(CodeExceptions.SERVICE_MERCHANTID_DUPLICATED);
                }
            }

            Repository.ContextTrackChanges = true;
            var entity_db = Repository.GetById(entity.Id, s => s.ServiceGateways, s => s.HighwayEnableEmails, s => s.BinGroups);

            entity_db.Name = entity.Name;

            entity_db.MerchantId = entity.MerchantId;
            entity_db.CybersourceProfileId = entity.CybersourceProfileId;
            //en la edicion no se muestra el campo. Si llega null o vacio, es porq no se quiere editar
            entity_db.CybersourceAccessKey = String.IsNullOrEmpty(entity.CybersourceAccessKey)
                ? entity_db.CybersourceAccessKey
                : entity.CybersourceAccessKey;
            entity_db.CybersourceSecretKey = String.IsNullOrEmpty(entity.CybersourceSecretKey)
                ? entity_db.CybersourceSecretKey
                : entity.CybersourceSecretKey;
            entity_db.CybersourceTransactionKey = String.IsNullOrEmpty(entity.CybersourceTransactionKey)
                ? entity_db.CybersourceTransactionKey
                : entity.CybersourceTransactionKey;

            entity_db.Description = entity.Description;
            entity_db.DescriptionTooltip = entity.DescriptionTooltip;
            entity_db.Id = entity.Id;

            entity_db.LinkId = !String.IsNullOrEmpty(entity.LinkId) && entity.LinkId.Contains("http")
                ? entity.LinkId
                : "http://" + entity.LinkId;
            entity_db.ReferenceParamName = entity.ReferenceParamName;
            entity_db.ReferenceParamName2 = entity.ReferenceParamName2;
            entity_db.ReferenceParamName3 = entity.ReferenceParamName3;
            entity_db.ReferenceParamName4 = entity.ReferenceParamName4;
            entity_db.ReferenceParamName5 = entity.ReferenceParamName5;
            entity_db.ReferenceParamName6 = entity.ReferenceParamName6;
            entity_db.ServiceCategoryId = entity.ServiceCategoryId;
            entity_db.Tags = entity.Tags;
            entity_db.PostAssociationDesc = entity.PostAssociationDesc;
            entity_db.TermsAndConditions = entity.TermsAndConditions;
            entity_db.UrlName = entity.UrlName;
            entity_db.ServiceContainerId = entity.ServiceContainerId;

            entity_db.ExtractEmail = entity.ExtractEmail;
            entity_db.CertificateThumbprintExternal = entity.CertificateThumbprintExternal;
            entity_db.EnableMultipleBills = entity.EnableMultipleBills;
            entity_db.EnablePartialPayment = entity.EnablePartialPayment;
            entity_db.EnablePrivatePayment = entity.EnablePrivatePayment;
            entity_db.EnablePublicPayment = entity.EnablePublicPayment;
            entity_db.EnableAssociation = entity.EnableAssociation;

            entity_db.Departament = (DepartamentType)(int)entity.Departament;

            entity_db.EnableAutomaticPayment = entity.EnableAutomaticPayment;

            entity_db.DiscountType = (DiscountType)(int)entity.DiscountType;
            entity_db.ExternalUrlAdd = entity.ExternalUrlAdd;
            entity_db.ExternalUrlRemove = entity.ExternalUrlRemove;
            entity_db.CertificateThumbprintVisa = entity.CertificateThumbprintVisa;

            entity_db.ReferenceParamRegex = entity.ReferenceParamRegex;
            entity_db.ReferenceParamRegex2 = entity.ReferenceParamRegex2;
            entity_db.ReferenceParamRegex3 = entity.ReferenceParamRegex3;
            entity_db.ReferenceParamRegex4 = entity.ReferenceParamRegex4;
            entity_db.ReferenceParamRegex5 = entity.ReferenceParamRegex5;
            entity_db.ReferenceParamRegex6 = entity.ReferenceParamRegex6;

            entity_db.AllowSelectContentAssociation = entity.AllowSelectContentAssociation;
            entity_db.AllowSelectContentPayment = entity.AllowSelectContentPayment;
            entity_db.AskUserForReferences = entity.AskUserForReferences;
            entity_db.AllowMultipleCards = entity.AllowMultipleCards;
            entity_db.MaxQuotaAllow = entity.MaxQuotaAllow;

            entity_db.IntroContent = entity.IntroContent;
            entity_db.InterpreterId = entity.InterpreterId == Guid.Empty ? null : entity.InterpreterId;
            entity_db.InterpreterAuxParam = entity.InterpreterAuxParam;

            entity_db.UrlIntegrationVersion = (UrlIntegrationVersionEnum)entity.UrlIntegrationVersion;
            entity_db.InformCardType = entity.InformCardType;
            entity_db.InformCardBank = entity.InformCardBank;
            entity_db.InformCardAffiliation = entity.InformCardAffiliation;

            entity_db.AllowVon = entity.AllowVon;
            entity_db.AllowWcfPayment = entity.AllowWcfPayment;
            entity_db.Sort = entity.Sort;

            entity_db.ImageName = entity.ImageName;
            entity_db.ImageTooltipName = entity.ImageTooltipName;

            var aux = new List<ServiceGateway>();
            foreach (var dto in entity.ServiceGatewaysDto)
            {
                var sg = entity_db.ServiceGateways.FirstOrDefault(x => x.GatewayId == dto.GatewayId);
                if (dto.Active)
                {
                    if (sg == null)
                    {
                        aux.Add(new ServiceGateway()
                        {
                            Active = dto.Active,
                            GatewayId = dto.GatewayId,
                            ReferenceId = dto.ReferenceId,
                            ServiceType = dto.ServiceType,
                            SendExtract = dto.SendExtract,
                            AuxiliarData = dto.AuxiliarData,
                            AuxiliarData2 = dto.AuxiliarData2,
                            Id = Guid.NewGuid()
                        });
                    }
                    else
                    {
                        sg.Active = dto.Active;
                        sg.ReferenceId = dto.ReferenceId;
                        sg.ServiceType = dto.ServiceType;
                        sg.SendExtract = dto.SendExtract;
                        sg.AuxiliarData = dto.AuxiliarData;
                        sg.AuxiliarData2 = dto.AuxiliarData2;
                        sg.Active = true;
                    }
                }
                else
                {
                    if (sg != null)
                    {
                        sg.Active = false;
                        sg.ReferenceId = dto.ReferenceId;
                        sg.ServiceType = dto.ServiceType;
                        sg.SendExtract = dto.SendExtract;
                        sg.AuxiliarData = dto.AuxiliarData;
                        sg.AuxiliarData2 = dto.AuxiliarData2;
                    }
                }

            }

            foreach (var sg in aux)
            {
                entity_db.ServiceGateways.Add(sg);
            }


            var emailsToAdd = new List<ServiceEnableEmail>();
            var emailsToRemove = new List<ServiceEnableEmail>();

            //si no viene nada, elimino lo que hay en BD
            if (entity.HighwayEnableEmails != null)
            {
                //Los que no vienen hay que eliminarlos
                foreach (var hemail in entity_db.HighwayEnableEmails)
                {
                    var oldemail = entity.HighwayEnableEmails.FirstOrDefault(x => x.Id == hemail.Id);
                    if (oldemail == null)
                        emailsToRemove.Add(entity_db.HighwayEnableEmails.FirstOrDefault(x => x.Id == hemail.Id));
                }

                //Los que vienen, hay que chequear q no fueran modificados
                foreach (var gt in entity.HighwayEnableEmails)
                {
                    var oldemail = entity_db.HighwayEnableEmails.FirstOrDefault(x => x.Id == gt.Id);
                    if (oldemail != null)
                    {
                        if (!oldemail.Email.Equals(gt.Email))
                        {
                            //Modificaron el campo email. Agregar route para uno, eliminar para otro
                            emailsToRemove.Add(oldemail);
                            entity_db.HighwayEnableEmails.Remove(oldemail);
                            var newEmail = new ServiceEnableEmail()
                            {
                                Id = Guid.NewGuid(),
                                Email = gt.Email,
                                ServiceId = entity_db.Id
                            };
                            emailsToAdd.Add(newEmail);
                        }
                    }
                    else
                    {
                        var newEmail = new ServiceEnableEmail()
                        {
                            Id = Guid.NewGuid(),
                            Email = gt.Email,
                            ServiceId = entity_db.Id
                        };
                        emailsToAdd.Add(newEmail);
                    }
                }
            }
            else
            {
                if (entity_db.HighwayEnableEmails != null)
                {
                    emailsToRemove.AddRange(entity_db.HighwayEnableEmails);
                    var listToRemove = entity_db.HighwayEnableEmails.ToList();
                    foreach (var hemail in listToRemove)
                    {
                        Repository.DeleteEntitiesNoRepository(hemail);
                    }
                }
            }


            //LLAMO A HIGHWAY PASANDO LOS MAILS PARA AGREGAR Y ELIMINAR
            if (emailsToAdd.Any())
            {
                var listEmailsUpdated = _serviceHighway.CreateRoutes(emailsToAdd);
                var l = entity_db.HighwayEnableEmails.ToList();
                l.AddRange(listEmailsUpdated);
                entity_db.HighwayEnableEmails = l;
            }
            if (emailsToRemove.Any())
            {
                _serviceHighway.DeleteRoutes(emailsToRemove);
                foreach (var rEmail in emailsToRemove)
                {
                    Repository.DeleteEntitiesNoRepository(rEmail);
                }
            }

            //Si es servicio contenedor, y se deben propagar los cambios de Grupos de Bines a los hijos
            if (entity.Container && entity.PropagateChangesToChildServices)
            {
                var containerBinGroups = entity.BinGroups.Select(x => x.Id).ToList();
                var newBinGroups = entity.BinGroups.Where(p => !entity_db.BinGroups.Any(p2 => p2.Id == p.Id)).Select(x => x.Id).ToList();
                var deletedBinGroups = entity_db.BinGroups.Where(p => !entity.BinGroups.Any(p2 => p2.Id == p.Id)).Select(x => x.Id).ToList();
                UpdateChildsBinGroups(entity_db.Id, containerBinGroups, newBinGroups, deletedBinGroups);
            }

            entity_db.BinGroups.Clear();
            entity.BinGroups.ForEach(b => entity_db.BinGroups.Add(_repositoryBinGroup.All(x => x.Id == b.Id).First()));

            Repository.Edit(entity_db);
            Repository.Save();

            Repository.ContextTrackChanges = false;
        }

        private void UpdateChildsBinGroups(Guid serviceContainerId, List<Guid> containerBinGroups, List<Guid> newBinGroups, List<Guid> deletedBinGroups)
        {
            var services = Repository.All(x => x.ServiceContainerId == serviceContainerId, s => s.BinGroups).ToList();
            try
            {
                foreach (var child in services)
                {
                    //Se obtienen los Ids de todos los grupos de bines finales que deben tener los hijos
                    var binGroupsIds = child.BinGroups.Select(x => x.Id).Where(x => containerBinGroups.Contains(x) && !deletedBinGroups.Contains(x)).ToList();
                    binGroupsIds.AddRange(newBinGroups);

                    //Primero se limpian todos y se agregan los obtenidos
                    child.BinGroups.Clear();
                    binGroupsIds.ForEach(id => child.BinGroups.Add(_repositoryBinGroup.All(x => x.Id == id).First()));
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceService - UpdateChildsBinGroups - Exception.");
                NLogLogger.LogEvent(e);
                throw new Exception("Error al actualizar los grupos de BINs de los servicios hijos.");
            }
        }

        public override void Delete(Guid id)
        {
            var service = Repository.GetById(id, s => s.ServiceGateways, s => s.HighwayEnableEmails, x => x.BinGroups);
            if (_repositoryServiceAsociated.AllNoTracking(sa => sa.ServiceId == id).Any())
                throw new BusinessException(CodeExceptions.SERVICE_ASOCIATED);

            if (_repositoryPayment.AllNoTracking(p => p.ServiceAssosiated.ServiceId == id).Any())
                throw new BusinessException(CodeExceptions.SERVICE_PAYMENTS_ASSOCIATED);

            foreach (var gt in service.ServiceGateways.ToList())
            {
                Repository.DeleteEntitiesNoRepository(gt);
            }

            if (service.HighwayEnableEmails.Any())
            {
                _serviceHighway.DeleteRoutes(service.HighwayEnableEmails.ToList());
                var aux = service.HighwayEnableEmails.ToList();
                foreach (var email in aux)
                {
                    Repository.DeleteEntitiesNoRepository(service.HighwayEnableEmails.First(x => x.Id == email.Id));
                }
            }

            if (service.BinGroups.Any())
            {
                service.BinGroups.Clear();
            }

            base.Delete(id);
        }

        public ICollection<CardDto> GetEnableCards(Guid userId, Guid serviceId)
        {
            var user = _repositoryApplicationUser.GetById(userId, u => u.Cards);
            var result = new Collection<CardDto>();

            foreach (var card in user.Cards.Where(c => c.Active))
            {
                var value = Int32.Parse(card.MaskedNumber.Substring(0, 6));
                var bin = _repositoryBin.AllNoTracking(b => b.Value == value).FirstOrDefault();

                if (bin != null)
                {
                    result.Add(new CardDto()
                    {
                        Active = card.Active,
                        DueDate = card.DueDate,
                        MaskedNumber = card.MaskedNumber,
                        PaymentToken = card.PaymentToken,
                        Description = card.Description,
                        Id = card.Id,
                        State = CalculateState(card, serviceId)
                    });
                }
            }
            return result;
        }

        public ServiceDto GetService(GatewayDto gateway, string serviceName, string serviceType)
        {
            var query = Repository.All(null, s => s.ServiceGateways);
            query = query.Where(s => s.ServiceGateways.Any(x => x.GatewayId == gateway.Id && x.ReferenceId.Equals(serviceName)));
            if (!String.IsNullOrEmpty(serviceType))
            {
                query = query.Where(s => s.ServiceGateways.Any(x => x.ServiceType.Equals(serviceType)));
            }
            return Converter(query.FirstOrDefault());
        }

        public void ChangeStatus(Guid serviceId)
        {
            Repository.ContextTrackChanges = true;
            var entity_db = Repository.GetById(serviceId);

            //si esta activo, lo tengo que desactivar
            if (entity_db.Active)
            {
                var servicesAssociated = _repositoryServiceAsociated.AllNoTracking(sa => sa.ServiceId == serviceId, s => s.NotificationConfig, s => s.AutomaticPayment).ToList();

                //consulto los usuarios que tienen un pago programado activo para este servicio y mando una notificación. 
                var users = _repositoryServiceAsociated.AllNoTracking(sa => sa.ServiceId == serviceId && sa.Active && sa.AutomaticPaymentId != null).Select(sa => sa.RegisteredUser).ToList();

                //deshabilito los servicios asociados
                _repositoryServiceAsociated.ChangeState(servicesAssociated, false, false);

                foreach (var user in users)
                {
                    _serviceNotificationMessage.SendServiceDeletedNotification(entity_db.Name, user);

                    #region AppNotification

                    _repositoryNotification.Create(new Notification
                    {
                        Date = DateTime.Now,
                        Message =
                                                           EnumHelpers.GetName(typeof(EmailType),
                                                               (int)EmailType.ServiceDeletedNotification,
                                                               Common.Resource.Enums.EnumsStrings.ResourceManager),
                        NotificationPrupose = NotificationPrupose.AlertNotification,
                        RegisteredUserId = user.Id,
                        ServiceId = entity_db.Id
                    });

                    #endregion
                }
            }
            else
            {
                //habilito los servicios asociados
                var servicesAssociated = _repositoryServiceAsociated.AllNoTracking(sa => sa.ServiceId == serviceId && !sa.Enabled, s => s.NotificationConfig, s => s.AutomaticPayment).ToList();
                _repositoryServiceAsociated.ChangeState(servicesAssociated, true, true);
            }
            entity_db.Active = !entity_db.Active;

            Repository.Edit(entity_db);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public string GetCertificateName(string codcommerce, string codBranch)
        {
            var dto = Repository.AllNoTracking(x => x.ServiceGateways.Any(y => y.ReferenceId.Equals(codcommerce) && y.ServiceType.Equals(codBranch)),
                x => x.ServiceGateways).FirstOrDefault();
            return dto != null ? dto.CertificateThumbprintExternal : null;
        }
        public string GetCertificateNameIdApp(string idApp)
        {
            var dto = Repository.AllNoTracking(x => x.UrlName.Equals(idApp, StringComparison.InvariantCultureIgnoreCase), x => x.ServiceGateways).FirstOrDefault();
            return dto != null ? dto.CertificateThumbprintExternal : null;
        }

        public ServiceDto GetserviceByUrlName(string serUrlName)
        {
            serUrlName = serUrlName.ToLower();
            var dto = Repository.AllNoTracking(x => x.UrlName.Equals(serUrlName), x => x.ServiceGateways, x => x.ServiceContainer).FirstOrDefault();
            return Converter(dto);
        }
        public IEnumerable<ServiceDto> GetDataForList(Guid serviceId, bool container)
        {
            var query = Repository.AllNoTracking(x => x.Container == container);

            if (serviceId != Guid.Empty)
                query = query.Where(x => x.Id != serviceId);

            query = query.OrderBy(x => x.Name);
            return query.Select(t => new ServiceDto
            {
                Id = t.Id,
                Name = t.Name,
            }).ToList();
        }
        public bool IsFatherOrHim(string idApp, string merchandId, string codcommerce, string codBranch)
        {
            var query = !string.IsNullOrEmpty(merchandId) ? Repository.AllNoTracking(x => x.MerchantId.Equals(merchandId), s => s.ServiceContainer) :
                Repository.AllNoTracking(x => x.ServiceGateways.Any(y => y.ReferenceId.Equals(codcommerce) && y.ServiceType.Equals(codBranch)), x => x.ServiceGateways, s => s.ServiceContainer);

            return query.Any(x => x.ServiceContainer.UrlName.Equals(idApp) || x.UrlName.Equals(idApp));
        }

        public TransactionCommerceResult GetServicesFromFather(string idApp, bool all = true)
        {
            var resultData = new TransactionCommerceResult();
            try
            {
                var father = Repository.AllNoTracking(x => x.UrlName.ToLower().Equals(idApp.ToLower()), x => x.ServiceGateways).FirstOrDefault();
                if (father == null)
                {
                    resultData.OperationResult = 56;
                    var errorLog = string.Format(LogStrings.Log_CommerceNotFound, string.Empty, string.Empty, idApp);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                    return resultData;
                }
                if (!father.Active)
                {
                    resultData.OperationResult = 57;
                    var errorLog = string.Format(LogStrings.Log_CommerceNotFound, string.Empty, string.Empty, idApp);
                    _loggerService.CreateLog(LogType.Error, LogOperationType.WebServicePayment, LogCommunicationType.WebService, errorLog);
                    return resultData;
                }

                var services = AllNoTracking(null, x => x.ServiceContainerId == father.Id, x => x.ServiceGateways);

                if (!all)
                    services = services.Where(x => x.Active);

                if (!services.Any())
                {
                    services = new List<ServiceDto>(){new ServiceDto()
                    {
                        Active = father.Active,
                        MerchantId = father.MerchantId,
                        Name = father.Name,
                        ServiceGatewaysDto = new List<ServiceGatewayDto>(){new ServiceGatewayDto()
                        {
                            ReferenceId = father.ServiceGateways.FirstOrDefault().ReferenceId,
                            ServiceType = father.ServiceGateways.FirstOrDefault().ServiceType,
                        }}
                    }};
                }

                resultData.Commerces = services.ToList();
            }
            catch (Exception exception)
            {
                resultData.OperationResult = 1;
            }
            return resultData;
        }

        public ServiceGatewayDto GetBestGateway(ServiceDto service, IList<ServiceGatewayDto> gatewayDtosList, Guid? cardId = null)
        {
            try
            {
                var serviceGatewaysActiveList = service.ServiceGatewaysDto.Where(x => x.Active).ToList();

                if (!serviceGatewaysActiveList.Any())
                    return null;

                if (serviceGatewaysActiveList.Count(x => x.Active) == 1)
                {
                    var serviceGateway = serviceGatewaysActiveList.First();
                    return serviceGateway;
                }

                if (cardId.HasValue)
                {
                    var cardBin = _repositoryCard.GetById(cardId.Value).MaskedNumber.Substring(0, 6);
                    var binInt = int.Parse(cardBin);
                    var bin = _repositoryBin.AllNoTracking(x => x.Value == binInt, x => x.Gateway).FirstOrDefault();
                    if (bin == null) bin = _repositoryBin.GetDefaultBin();

                    if (gatewayDtosList.Any(x => x.Gateway.Enum == bin.Gateway.Enum))
                    {
                        var serviceGateway = serviceGatewaysActiveList.FirstOrDefault(x => x.Gateway.Enum == bin.Gateway.Enum);
                        if (serviceGateway != null)
                        {
                            return serviceGateway;
                        }
                    }

                }

                foreach (var gatewayDto in gatewayDtosList)
                {
                    var serviceGateway = serviceGatewaysActiveList.FirstOrDefault(x => x.Gateway.Enum == gatewayDto.Gateway.Enum);
                    if (serviceGateway != null)
                    {
                        return serviceGateway;
                    }
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
            }


            return null;
        }

        private ServiceGatewayDto Convert(ServiceGateway serviceGateway)
        {
            return new ServiceGatewayDto()
            {
                Active = true,
                GatewayId = serviceGateway.GatewayId,
                Id = serviceGateway.Id,
                ReferenceId = serviceGateway.ReferenceId,
                ServiceType = serviceGateway.ServiceType,
                AuxiliarData = serviceGateway.AuxiliarData,
                AuxiliarData2 = serviceGateway.AuxiliarData2,
                Gateway = new GatewayDto()
                {
                    Id = serviceGateway.Gateway != null ? serviceGateway.Gateway.Id : Guid.Empty,
                    Name = serviceGateway.Gateway != null ? serviceGateway.Gateway.Name : string.Empty,
                    Enum = serviceGateway.Gateway != null ? serviceGateway.Gateway.Enum : -1
                }
            };
        }

        public void NotifyServiceWithoutActiveGateway(ServiceDto service)
        {
            var detail = string.Format("Se quiso obtener facturas del servicio {0} pero este no tiene pasarela activa. " +
                                           "Esto puede generar distintos errores en el sitio.", service.Name
                                           );
            _serviceFixedNotification.Create(new FixedNotificationDto()
            {
                Category = FixedNotificationCategoryDto.ServiceConfiguration,
                DateTime = DateTime.Now,
                Level = FixedNotificationLevelDto.Error,
                Id = Guid.NewGuid(),
                Description = "Servicio sin Pasarela activa",
                Detail = detail,
            });
        }

        //Metodo para listado de servicios de la APP MOBILE VISANETPAGOS
        public IEnumerable<ServiceDto> GetServicesForApp(ServiceFilterDto filters)
        {
            var query = Repository.AllNoTracking(
                null,
                srv => srv.ServiceCategory,
                srv => srv.ServiceContainer,
                srv => srv.ServiceGateways,
                srv => srv.ServiceGateways.Select(srvGateway => srvGateway.Gateway));

            if (filters.ServiceId != Guid.Empty)
            {
                query = query.Where(s => s.Id == filters.ServiceId);
            }

            if (filters.IsContainer != null)
            {
                query = query.Where(x => x.Container == filters.IsContainer);
            }

            if (filters.WithoutServiceInContainer)
            {
                query = query.Where(x => x.ServiceContainerId == null || x.ServiceContainerId == Guid.Empty);
            }

            if (!string.IsNullOrEmpty(filters.GenericSearch))
            {
                query =
                    query.Where(
                        sc =>
                        sc.Name.ToLower().Contains(filters.GenericSearch.ToLower())
                        || sc.Description.ToLower().Contains(filters.GenericSearch.ToLower())
                        || sc.ServiceCategory.Name.ToLower().Contains(filters.GenericSearch.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                query = query.Where(sc => sc.Name.ToLower().Contains(filters.Name.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(filters.ServiceCategory))
            {
                query = query.Where(sc => sc.ServiceCategory.Name.ToLower().Contains(filters.ServiceCategory.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(filters.ServiceContainerName))
            {
                query =
                    query.Where(
                        sc =>
                        sc.ServiceContainerId != null && sc.ServiceContainerId != Guid.Empty
                        && sc.ServiceContainer.Name.ToLower().Contains(filters.ServiceContainerName.ToLower()));
            }

            if (filters.EnableAssociation)
            {
                query = query.Where(x => x.EnableAssociation);
            }

            if (filters.EnablePrivatePayment)
            {
                query = query.Where(x => x.EnablePrivatePayment);
            }

            if (filters.EnablePublicPayment)
            {
                query = query.Where(x => x.EnablePublicPayment);
            }

            if (filters.Active)
            {
                query = query.Where(x => x.Active);
            }

            if (filters.OrderBy.Equals("ServiceCategory"))
            {
                query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.ServiceCategory.Name) : query.OrderByDescending(x => x.ServiceCategory.Name);
            }
            else if (filters.OrderBy.Equals("ServiceContainerDto"))
            {
                query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.ServiceContainer.Name) : query.OrderByDescending(x => x.ServiceContainer.Name);
            }
            else
            {
                query = filters.SortDirection == SortDirection.Asc ? query.OrderByStringProperty(filters.OrderBy) : query.OrderByStringPropertyDescending(filters.OrderBy);
            }

            var list = query.Select(t => new ServiceDto
            {
                Id = t.Id,
                Name = t.Name,
                Active = t.Active,
                Description = t.Description,
                ServiceCategoryName = t.ServiceCategory.Name,
                ServiceCategoryId = t.ServiceCategoryId,
                ServiceContainerId = t.ServiceContainerId,
                ReferenceParamName = t.ReferenceParamName,
                ReferenceParamName2 = t.ReferenceParamName2,
                ReferenceParamName3 = t.ReferenceParamName3,
                ReferenceParamName4 = t.ReferenceParamName4,
                ReferenceParamName5 = t.ReferenceParamName5,
                ReferenceParamName6 = t.ReferenceParamName6,

                ReferenceParamRegex = t.ReferenceParamRegex,
                ReferenceParamRegex2 = t.ReferenceParamRegex2,
                ReferenceParamRegex3 = t.ReferenceParamRegex3,
                ReferenceParamRegex4 = t.ReferenceParamRegex4,
                ReferenceParamRegex5 = t.ReferenceParamRegex5,
                ReferenceParamRegex6 = t.ReferenceParamRegex6,

                EnableAssociation = t.EnableAssociation,
                AllowMultipleCards = t.AllowMultipleCards,

                ServiceGatewaysDto = t.ServiceGateways.Select(g => new ServiceGatewayDto
                {
                    Active = g.Active,
                    GatewayId = g.GatewayId,
                    ReferenceId = g.ReferenceId,
                    ServiceType = g.ServiceType,
                    AuxiliarData = g.AuxiliarData,
                    AuxiliarData2 = g.AuxiliarData2,
                    Gateway = new GatewayDto()
                    {
                        Enum = g.Gateway != null ? g.Gateway.Enum : 0,
                        Name = g.Gateway != null ? g.Gateway.Name : string.Empty
                    }
                }).ToList(),
                Tags = t.Tags,
                PostAssociationDesc = t.PostAssociationDesc,
                TermsAndConditions = t.TermsAndConditions,
                ServiceContainerName = t.ServiceContainerId != null && t.ServiceContainerId != Guid.Empty ? t.ServiceContainer.Name : "",
                //DiscountType = (DiscountType)(int)t.DiscountType,
                Departament = (DepartamentDtoType)(int)t.Departament,
                ImageName = t.ImageName,
                ImageUrl = FileStorage.Instance.GetImageUrl(_folderBlob, t.Id, t.ImageName, false),
                ImageTooltipName = t.ImageTooltipName,
                ImageTooltipUrl = FileStorage.Instance.GetImageTooltipUrl(_folderBlob, t.Id, t.ImageTooltipName),

                EnableAutomaticPayment = t.EnableAutomaticPayment,
                Sort = t.Sort,
            }).ToList();

            var finalList = new List<ServiceDto>();
            foreach (var serviceDto in list)
            {
                if (serviceDto == null) continue;

                serviceDto.ImageUrl = FileStorage.Instance.GetImageUrl(_folderBlob, serviceDto.Id, serviceDto.ImageName);

                if (serviceDto.EnableAssociation && serviceDto.ServiceGatewaysDto.Any())
                {
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

                // Por defecto todos los servicios son "contenedores" pero no tienen hijos asociados.
                // Por eso la lógica de abajo
                var isContainerService = serviceDto.ServiceContainerId == null
                                         || serviceDto.ServiceContainerId == Guid.Empty;

                if (isContainerService)
                {
                    serviceDto.ServicesInside = new List<ServiceDto>();
                    var sonsList = list.Where(x => x.ServiceContainerId == serviceDto.Id);
                    serviceDto.ServicesInside.AddRange(sonsList);

                    SetImporteValues(serviceDto);
                    serviceDto.AllowGetBills = serviceDto.ServiceGatewaysDto.Any(x => x.Gateway.Enum == (int)GatewayEnumDto.Banred && x.Active
                    || x.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc && x.Active
                    || x.Gateway.Enum == (int)GatewayEnumDto.Carretera && x.Active
                    || x.Gateway.Enum == (int)GatewayEnumDto.Sucive && x.Active
                    || x.Gateway.Enum == (int)GatewayEnumDto.Geocom && x.Active);

                    finalList.Add(serviceDto);
                }
            }

            return finalList;
        }

        public void SetImporteValues(ServiceDto serviceDto)
        {
            serviceDto.AllowInputAmount = serviceDto.ServiceGatewaysDto.Any(x => x.Gateway.Enum == (int)GatewayEnumDto.Importe && x.Active);
            if (serviceDto.AllowInputAmount)
            {
                var gateway = serviceDto.ServiceGatewaysDto.FirstOrDefault(x => x.Gateway.Enum == (int)GatewayEnumDto.Importe && x.Active);
                serviceDto.ImporteInfoDto = new List<ImporteInfoDto>()
                                            {
                                                new ImporteInfoDto()
                                                {
                                                    Currency = CurrencyDto.USD,
                                                    CurrencyDescription = EnumHelpers.GetName(typeof(CurrencyDto), (int)CurrencyDto.USD, EnumsStrings.ResourceManager),
                                                    MinValue = gateway.AuxiliarData,
                                                    MaxValue = gateway.AuxiliarData2
                                                },
                                                new ImporteInfoDto()
                                                {
                                                    Currency = CurrencyDto.UYU,
                                                    CurrencyDescription = EnumHelpers.GetName(typeof(CurrencyDto), (int)CurrencyDto.UYU, EnumsStrings.ResourceManager),
                                                    MinValue = gateway.ReferenceId,
                                                    MaxValue = gateway.ServiceType
                                                }
                                            };
            }
        }

        public bool IsBinAssociatedToService(int binValue, Guid serviceId)
        {
            var isAssociated = ((IRepositoryService)Repository).IsBinAssociatedToService(binValue, serviceId);
            return isAssociated;
        }

        private CardStateDto CalculateState(Card card, Guid serviceId)
        {
            var today = DateTime.Now;

            //Inactivas
            if (!card.Active)
            {
                return CardStateDto.Disabled;
            }

            //Vencidas
            if (!((card.DueDate.Year.CompareTo(today.Year) == 0 && card.DueDate.Month.CompareTo(today.Month) >= 0) || card.DueDate.Year.CompareTo(today.Year) > 0))
            {
                return CardStateDto.Expired;
            }

            //Tarjeta asociable
            if (!IsBinAssociatedToService(int.Parse(card.MaskedNumber.Substring(0, 6)), serviceId))
            {
                return CardStateDto.CanNotBeAssociated;
            }

            return CardStateDto.Active;
        }

        public IEnumerable<ServiceDto> GetServicesLigthWithoutChildens(Guid? selectedServiceId = null, GatewayEnumDto? gatewayEnumDto = null)
        {
            var query = Repository.AllNoTracking();
            if (selectedServiceId == null || selectedServiceId == Guid.Empty)
            {
                query = query.Where(x => x.ServiceContainerId == null || x.ServiceContainerId == Guid.Empty);
            }
            else
            {
                query = query.Where(x => x.ServiceContainerId == selectedServiceId.Value);
            }

            if (gatewayEnumDto != null)
            {
                var gate = (int)gatewayEnumDto.Value;
                query = query.Where(x => x.ServiceGateways.Any(y => y.Active && y.Gateway.Enum == gate));
            }

            query = query.OrderBy(x => x.Name);
            return query.Select(t => new ServiceDto
            {
                Id = t.Id,
                Name = t.Name,
                Container = t.Container,
                Sort = t.Sort,
            }).ToList();
        }

        public IEnumerable<ServiceDto> GetServicesFromContainer(Guid containerId)
        {

            var query = Repository.AllNoTracking(x => x.ServiceContainerId == containerId);
            query = query.OrderBy(x => x.Name);
            return query.Select(t => new ServiceDto
            {
                Id = t.Id,
                Name = t.Name,
                Sort = t.Sort,
            }).ToList();
        }

        public IEnumerable<ReportsUsersVonDto> GetDataForReportsUsersVon(ReportsUserVonFilterDto filters)
        {
            var result = new List<ReportsUsersVonDto>();
            var sqlQuery = GetDataForReportsUsersVonQuery(filters);

            #region Sort
            var orderBy = "";
            switch (filters.OrderBy)
            {
                case "0":
                    orderBy = "ORDER BY AnonymousUserId ";
                    break;
                case "1":
                    orderBy = "ORDER BY CreationDate ";
                    break;
                case "2":
                    orderBy = "ORDER BY Email ";
                    break;
                case "3":
                    orderBy = "ORDER BY Name ";
                    break;
                case "4":
                    orderBy = "ORDER BY Surname ";
                    break;
                case "5":
                    orderBy = "ORDER BY UserExternalId ";
                    break;
                case "6":
                    orderBy = "ORDER BY serviceId ";
                    break;
                case "7":
                    orderBy = "ORDER BY ServiceName ";
                    break;
                case "8":
                    orderBy = "ORDER BY AppId ";
                    break;
                case "9":
                    orderBy = "ORDER BY PaymentsCount ";
                    break;
                case "10":
                    orderBy = "ORDER BY CardsCount ";
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
                    result = context.Database.SqlQuery<ReportsUsersVonDto>(string.Format("WITH aux AS ( SELECT *, ROW_NUMBER() OVER ({1}) AS RowNumber FROM {0} ) SELECT * FROM aux WHERE RowNumber BETWEEN {2} AND {3}", sqlQuery, orderBy, filters.DisplayStart + 1, filters.DisplayStart + (int)filters.DisplayLength)).ToList();
                    return result;
                }
                result = context.Database.SqlQuery<ReportsUsersVonDto>(sqlQuery + orderBy).ToList();
                return result;
            }
        }

        public int GetDataForReportsUsersVonCount(ReportsUserVonFilterDto filters)
        {
            var sqlQuery = "SELECT COUNT(0) FROM ";
            sqlQuery += GetDataForReportsUsersVonQuery(filters);

            using (var context = new AppContext())
            {
                var count = context.Database.SqlQuery<int>(sqlQuery).First();
                return count;
            }
        }

        private static string GetDataForReportsUsersVonQuery(ReportsUserVonFilterDto filters)
        {
            var sqlquery = "(SELECT DISTINCT ";
            //User
            sqlquery += "a.Id AS AnonymousUserId, ";
            sqlquery += "(SELECT MIN(CreationDate)  FROM VonData WHERE AnonymousUserId = a.Id) CreationDate, ";
            sqlquery += "a.Email, ";
            sqlquery += "a.[Name], ";
            sqlquery += "a.Surname, ";
            sqlquery += "v.UserExternalId,";
            //Service
            sqlquery += "s.Id serviceId, ";
            sqlquery += "s.[Name] ServiceName, ";
            sqlquery += "v.AppId AS AppId, ";
            //Counts
            sqlquery += "(SELECT COUNT(0) FROM payments WHERE ServiceId = s.Id AND AnonymousUserId = v.AnonymousUserId AND PaymentPlatform = 5) AS PaymentsCount, ";
            sqlquery += "(SELECT COUNT(0) FROM VonData WHERE AnonymousUserId = a.Id AND AppId = s.UrlName) AS CardsCount ";
            //From Tables
            sqlquery += "FROM VonData v ";
            sqlquery += "INNER JOIN AnonymousUsers a ON a.Id = v.AnonymousUserId ";
            sqlquery += "INNER JOIN Services s ON v.AppId = s.UrlName) TBL ";

            //Filters
            var sqlWhere = "WHERE 1=1 ";

            if (filters.DateFrom == DateTime.MinValue)
                filters.DateFrom = DateTime.Today.AddYears(-20);

            if (filters.DateTo == DateTime.MinValue)
                filters.DateTo = DateTime.Today;
            filters.DateTo = filters.DateTo.AddDays(1);

            if (filters.DateFrom != null && filters.DateTo != null)
            {
                sqlWhere += "AND CreationDate BETWEEN '" + filters.DateFrom.ToString("yyyy-MM-dd") + "' AND '" + filters.DateTo.ToString("yyyy-MM-dd") + "' ";
            }

            if (!string.IsNullOrEmpty(filters.Email))
            {
                sqlWhere += "AND RTRIM(LTRIM(Email))  LIKE '%" + filters.Email.Trim() + "%' ";
            }

            if (!string.IsNullOrEmpty(filters.Service))
            {
                sqlWhere += "AND ServiceId = '" + filters.Service + "' ";
            }

            return sqlquery + sqlWhere;
        }

        public IEnumerable<CardVonDto> GetVonUsersCards(Guid userId, Guid serviceId)
        {
            var sqlQuery = "SELECT v.Id, v.CardName, v.CardMaskedNumber, v.CardDueDate, v.CreationDate  ";
            sqlQuery += "FROM VonData v ";
            sqlQuery += "INNER JOIN AnonymousUsers a ON a.Id = v.AnonymousUserId ";
            sqlQuery += "INNER JOIN Services s ON v.AppId = s.UrlName ";
            sqlQuery += "WHERE a.Id = '" + userId + "' ";
            sqlQuery += "AND s.Id = '" + serviceId + "' ";

            using (var context = new AppContext())
            {
                var result = context.Database.SqlQuery<CardVonDto>(sqlQuery).ToList();
                return result;
            }
        }

    }
}