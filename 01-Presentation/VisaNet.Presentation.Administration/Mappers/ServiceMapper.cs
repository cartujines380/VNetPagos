using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Domain.EntitiesDtos.Enums;
namespace VisaNet.Presentation.Administration.Mappers
{
    public static class ServiceMapper
    {
        public static ServiceModel ToModel(this ServiceDto entity)
        {
            var dto = new ServiceModel
            {
                Id = entity.Id,
                Name = entity.Name,
                MerchantId = entity.MerchantId,
                Description = entity.Description,
                CybersourceAccessKey = entity.CybersourceAccessKey,
                CybersourceProfileId = entity.CybersourceProfileId,
                CybersourceSecretKey = entity.CybersourceSecretKey,
                Image = entity.ImageName,
                DeleteImage = entity.ImageDeleted,
                ImagePath = entity.ImageUrl,
                InformCardBank = entity.InformCardBank,
                InformCardType = entity.InformCardType,
                InformAffiliationCard = entity.InformCardAffiliation,
                InterpreterId = entity.InterpreterId != null ? entity.InterpreterId.ToString() : null,
                ServiceContainerId = entity.ServiceContainerId != null ? entity.ServiceContainerId.ToString() : null,
                UrlName = entity.UrlName,
                Sort = entity.Sort,
                Tags = entity.Tags,
                ServiceCategoryName = entity.ServiceCategoryName,
                EnableAssociation = entity.EnableAssociation,
                EnableAutomaticPayment = entity.EnableAutomaticPayment,
                EnableMultipleBills = entity.EnableMultipleBills,
                EnablePartialPayment = entity.EnablePartialPayment,
                EnablePrivatePayment = entity.EnablePrivatePayment,
                EnablePublicPayment = entity.EnablePublicPayment,
                ExternalUrlAdd = entity.ExternalUrlAdd,
                ExternalUrlRemove = entity.ExternalUrlRemove,
                LinkId = entity.LinkId,
                TermsAndConditions = entity.TermsAndConditions,
                ServiceCategoryId = entity.ServiceCategoryId.ToString(),
                ExtractEmail = entity.ExtractEmail,
                Active = entity.Active,
                AllowMultipleCards = entity.AllowMultipleCards,
                AllowVon = entity.AllowVon,
                AllowWcfPayment = entity.AllowWcfPayment,
                AskUserForReferences = entity.AskUserForReferences,
                CertificateThumbprintExternal = entity.CertificateThumbprintExternal,
                CertificateThumbprintVisa = entity.CertificateThumbprintVisa,
                Container = entity.Container,
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
                ContentIntro = entity.IntroContent
            };

            return dto;
        }
    }
}