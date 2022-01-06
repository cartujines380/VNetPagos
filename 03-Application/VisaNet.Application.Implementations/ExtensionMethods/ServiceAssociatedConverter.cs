using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using VisaNet.Application.Implementations.ExtensionMethods.Filters;
using VisaNet.Common.AzureUpload;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Application.Implementations.ExtensionMethods
{
    public static class ServiceAssociatedConverter
    {
        private static readonly string FolderBlob = ConfigurationManager.AppSettings["AzureServicesImagesUrl"];

        public static ServiceAssociatedDto Convert(this ServiceAssociated entity, ServiceAssociatedIncludeFilters filter)
        {
            var dto = new ServiceAssociatedDto
            {
                Id = entity.Id,

                //Refs
                ServiceId = entity.ServiceId,
                AutomaticPaymentDtoId = entity.AutomaticPaymentId,
                DefaultCardId = entity.DefaultCardId,
                UserId = entity.RegisteredUserId,
                NotificationConfigDtoId = entity.NotificationConfigId,

                //Data
                Active = entity.Active,
                Enabled = entity.Enabled,
                Description = entity.Description,
                ReferenceNumber = entity.ReferenceNumber,
                ReferenceNumber2 = entity.ReferenceNumber2,
                ReferenceNumber3 = entity.ReferenceNumber3,
                ReferenceNumber4 = entity.ReferenceNumber4,
                ReferenceNumber5 = entity.ReferenceNumber5,
                ReferenceNumber6 = entity.ReferenceNumber6,
                IdUserExternal = entity.IdUserExternal
            };

            if (filter.IncludeServiceInfo && entity.Service != null)
            {
                //Se agrega la info del Service
                dto.ServiceDto = FillServiceData(entity.Service, filter.IncludeServiceGatewaysInfo, filter.IncludeServiceCategoryInfo);

                if (filter.IncludeServiceContainerInfo && entity.Service.ServiceContainer != null)
                {
                    dto.ServiceDto.ServiceContainerDto = FillServiceData(entity.Service.ServiceContainer, filter.IncludeServiceGatewaysInfo, filter.IncludeServiceCategoryInfo);
                }
            }

            if (filter.IncludeAutomaticPaymentInfo && entity.AutomaticPayment != null)
            {
                //Se agrega la info del AutomaticPayment
                dto.AutomaticPaymentDto = FillAutomaticPaymentData(entity.AutomaticPayment);
            }

            if (filter.IncludeDefaultCardInfo && entity.DefaultCard != null)
            {
                //Se agrega la info del DefaultCard
                dto.DefaultCard = FillCardData(entity.DefaultCard);
            }

            if (filter.IncludeApplicationUserInfo && entity.RegisteredUser != null)
            {
                //Se agrega la info del ApplicationUser
                dto.RegisteredUserDto = FillApplicationUserData(entity.RegisteredUser);
            }

            if (filter.IncludeNotificationConfigInfo && entity.NotificationConfig != null)
            {
                //Se agrega la info del NotificationConfig
                dto.NotificationConfigDto = FillNotificationConfigData(entity.NotificationConfig);
            }

            if (filter.IncludeCardListInfo && entity.Cards != null && entity.Cards.Any())
            {
                //Se agrega la info de las Cards
                dto.CardDtos = new List<CardDto>();
                foreach (var card in entity.Cards)
                {
                    dto.CardDtos.Add(FillCardData(card));
                }
            }

            return dto;
        }

        private static ServiceDto FillServiceData(Service entity, bool includeGateways, bool includeCategory)
        {
            var dto = new ServiceDto
            {
                Id = entity.Id,

                //Refs
                ServiceContainerId = entity.ServiceContainerId,
                ServiceCategoryId = entity.ServiceCategoryId,

                //Data
                Name = entity.Name,
                UrlName = entity.UrlName,
                MerchantId = entity.MerchantId,
                Description = entity.Description,
                Active = entity.Active,
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
                Departament = (DepartamentDtoType)(int)entity.Departament,
                DiscountType = (DiscountTypeDto)(int)entity.DiscountType,
                MaxQuotaAllow = entity.MaxQuotaAllow,
                Sort = entity.Sort,
                DescriptionTooltip = entity.DescriptionTooltip,
                ExtractEmail = entity.ExtractEmail,
                InterpreterAuxParam = entity.InterpreterAuxParam,
                IntroContent = entity.IntroContent,
                LinkId = entity.LinkId,
                PostAssociationDesc = entity.PostAssociationDesc,
                ServiceCategoryName = entity.ServiceCategory != null ? entity.ServiceCategory.Name : null,
                ServiceContainerName = entity.ServiceContainer != null ? entity.ServiceContainer.Name : null,
                Tags = entity.Tags,
                TermsAndConditions = entity.TermsAndConditions,
                CybersourceTransactionKey = entity.CybersourceTransactionKey,
                CybersourceAccessKey = entity.CybersourceAccessKey,
                CybersourceSecretKey = entity.CybersourceSecretKey,
                CybersourceProfileId = entity.CybersourceProfileId,

                //Images
                ImageName = entity.ImageName,
                ImageUrl = FileStorage.Instance.GetImageUrl(FolderBlob, entity.Id, entity.ImageName),
                ImageTooltipName = entity.ImageTooltipName,
                ImageTooltipUrl = FileStorage.Instance.GetImageTooltipUrl(FolderBlob, entity.Id, entity.ImageTooltipName),
                ServiceContainerImageName = entity.ServiceContainer != null ? entity.ServiceContainer.ImageName : null,
                ServiceContainerImageUrl = entity.ServiceContainer != null ? FileStorage.Instance.GetImageUrl(FolderBlob, entity.ServiceContainer.Id, entity.ServiceContainer.ImageName) : null,

                //Integration
                ExternalUrlAdd = entity.ExternalUrlAdd,
                ExternalUrlRemove = entity.ExternalUrlRemove,
                CertificateThumbprintVisa = entity.CertificateThumbprintVisa,
                CertificateThumbprintExternal = entity.CertificateThumbprintExternal,
                UrlIntegrationVersion = (UrlIntegrationVersionEnumDto)(int)entity.UrlIntegrationVersion,
                InformCardAffiliation = entity.InformCardAffiliation,
                InformCardBank = entity.InformCardBank,
                InformCardType = entity.InformCardType,

                //Booleans
                Container = entity.Container,
                EnableAutomaticPayment = entity.EnableAutomaticPayment,
                EnablePrivatePayment = entity.EnablePrivatePayment,
                EnablePublicPayment = entity.EnablePublicPayment,
                EnableAssociation = entity.EnableAssociation,
                EnableMultipleBills = entity.EnableMultipleBills,
                EnablePartialPayment = entity.EnablePartialPayment,
                AskUserForReferences = entity.AskUserForReferences,
                AllowSelectContentAssociation = entity.AllowSelectContentAssociation,
                AllowSelectContentPayment = entity.AllowSelectContentPayment,
                AllowMultipleCards = entity.AllowMultipleCards,
                AllowVon = entity.AllowVon,
                AllowWcfPayment = entity.AllowWcfPayment
            };

            if (includeGateways && entity.ServiceGateways != null && entity.ServiceGateways.Any())
            {
                //Se agrega la info de las Gateways
                dto.ServiceGatewaysDto = entity.ServiceGateways.Select(g => new ServiceGatewayDto
                {
                    Id = g.Id,

                    Active = g.Active,
                    ReferenceId = g.ReferenceId,
                    AuxiliarData = g.AuxiliarData,
                    AuxiliarData2 = g.AuxiliarData2,
                    ServiceType = g.ServiceType,
                    GatewayId = g.GatewayId,
                    SendExtract = g.SendExtract,
                    Gateway = new GatewayDto
                    {
                        Id = g.Gateway.Id,
                        Name = g.Gateway.Name,
                        Enum = g.Gateway.Enum
                    }
                }).ToList();
            }

            if (includeCategory && entity.ServiceCategory != null)
            {
                //Se agrega la info del ServiceCategory
                dto.ServiceCategory = new ServiceCategoryDto
                {
                    Id = entity.ServiceCategory.Id,
                    Name = entity.ServiceCategory.Name
                };
            }

            return dto;
        }

        private static AutomaticPaymentDto FillAutomaticPaymentData(AutomaticPayment entity)
        {
            var dto = new AutomaticPaymentDto
            {
                Id = entity.Id,

                //Data
                DaysBeforeDueDate = entity.DaysBeforeDueDate,
                Maximum = entity.Maximum,
                Quotas = entity.Quotas,
                UnlimitedQuotas = entity.UnlimitedQuotas,
                QuotasDone = entity.QuotasDone,
                SuciveAnnualPatent = entity.SuciveAnnualPatent,
                UnlimitedAmount = entity.UnlimitedAmount,
                LastRunDate = entity.LastRunDate,
                LastRunResult = (PaymentResultTypeDto?)entity.LastRunResult,
                LastSuccessfulPaymentDate = entity.LastSuccessfulPaymentDate,
                LastSuccessfulPaymentIteration = entity.LastSuccessfulPaymentIteration,
                LastErrorDate = entity.LastErrorDate,
                LastErrorResult = (PaymentResultTypeDto?)entity.LastErrorResult
            };

            return dto;
        }

        private static CardDto FillCardData(Card entity)
        {
            var dto = new CardDto
            {
                Id = entity.Id,

                //Data
                MaskedNumber = entity.MaskedNumber,
                DueDate = entity.DueDate,
                PaymentToken = entity.PaymentToken,
                ExternalId = entity.ExternalId,
                Name = entity.Name,
                CybersourceTransactionId = entity.CybersourceTransactionId,
                Active = entity.Active,
                Deleted = entity.Deleted,
                Description = entity.Description
            };

            return dto;
        }

        private static ApplicationUserDto FillApplicationUserData(ApplicationUser entity)
        {
            var dto = new ApplicationUserDto
            {
                Id = entity.Id,

                //Refs
                MembreshipIdentifier = entity.MembershipIdentifier,
                SistarbancBrouUserId = entity.SistarbancBrouUserId,
                SistarbancUserId = entity.SistarbancUserId,

                //Data
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email,
                Address = entity.Address,
                IdentityNumber = entity.IdentityNumber,
                PhoneNumber = entity.PhoneNumber,
                MobileNumber = entity.MobileNumber,
                CallCenterKey = entity.CallCenterKey,
                CyberSourceIdentifier = entity.CyberSourceIdentifier,
                Platform = (PlatformDto)entity.Platform,
                CreationDate = entity.CreationDate
            };

            if (entity.MembershipIdentifierObj != null)
            {
                dto.MembershipIdentifierObj = new MembershipUserDto
                {
                    Id = entity.MembershipIdentifierObj.Id,

                    //Data
                    Active = entity.MembershipIdentifierObj.Active,
                    LastAttemptToLogIn = entity.MembershipIdentifierObj.LastAttemptToLogIn,
                    FailLogInCount = entity.MembershipIdentifierObj.FailLogInCount,
                    LastResetPassword = entity.MembershipIdentifierObj.LastResetPassword,
                    Blocked = entity.MembershipIdentifierObj.Blocked,
                    ConfirmationToken = entity.MembershipIdentifierObj.ConfirmationToken,
                    PasswordHasBeenChangedFromBO = entity.MembershipIdentifierObj.PasswordHasBeenChangedFromBO,
                    ResetPasswordToken = entity.MembershipIdentifierObj.ResetPasswordToken
                };
            }

            return dto;
        }

        private static NotificationConfigDto FillNotificationConfigData(NotificationConfig entity)
        {
            var dto = new NotificationConfigDto
            {
                Id = entity.Id,

                //Refs
                ServiceAsociatedDtoId = entity.ServiceAsociatedId,

                //Data
                DaysBeforeDueDate = entity.DaysBeforeDueDate,
                BeforeDueDateConfigDto = entity.BeforeDueDateConfig != null
                    ? new DaysBeforeDueDateConfigDto
                    {
                        Email = entity.BeforeDueDateConfig.Email,
                        Sms = entity.BeforeDueDateConfig.Sms,
                        Web = entity.BeforeDueDateConfig.Web,
                        Label = PresentationWebStrings.Service_Step2_BillExpiration
                    }
                    : null,
                ExpiredBillDto = entity.ExpiredBill != null
                    ? new ExpiredBillDto
                    {
                        Email = entity.ExpiredBill.Email,
                        Sms = entity.ExpiredBill.Sms,
                        Web = entity.ExpiredBill.Web,
                        Label = PresentationWebStrings.Service_Step2_BillExpired
                    }
                    : null,
                NewBillDto = entity.NewBill != null
                    ? new NewBillDto
                    {
                        Email = entity.NewBill.Email,
                        Sms = entity.NewBill.Sms,
                        Web = entity.NewBill.Web,
                        Label = PresentationWebStrings.Service_Step2_BillMade
                    }
                    : null,
                FailedAutomaticPaymentDto = entity.FailedAutomaticPayment != null
                    ? new FailedAutomaticPaymentDto
                    {
                        Email = entity.FailedAutomaticPayment.Email,
                        Sms = entity.FailedAutomaticPayment.Sms,
                        Web = entity.FailedAutomaticPayment.Web,
                        Label = PresentationWebStrings.Service_Step2_FailedAutomaticPayment
                    }
                    : null,
                SuccessPaymentDto = entity.SuccessPayment != null
                    ? new SuccessPaymentDto
                    {
                        Email = entity.SuccessPayment.Email,
                        Sms = entity.SuccessPayment.Sms,
                        Web = entity.SuccessPayment.Web,
                        Label = PresentationWebStrings.Service_Step2_PaymentMade
                    }
                    : null
            };

            return dto;
        }

    }
}