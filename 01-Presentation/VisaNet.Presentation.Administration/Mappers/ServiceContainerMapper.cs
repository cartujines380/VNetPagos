using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class ServiceContainerMapper
    {
        public static ServiceContainerModel ToModel(this ServiceDto dto, ICollection<GatewayDto> gateways)
        {
            var model = new ServiceContainerModel()
            {
                Name = dto.Name,
                Description = dto.Description,
                Active = dto.Active,
                LinkId = dto.LinkId,
                Tags = dto.Tags,
                ServiceCategoryId = dto.ServiceCategoryId.ToString(),
                CertificateThumbprintExternal = dto.CertificateThumbprintExternal,
                PostAssociationDesc = dto.PostAssociationDesc,
                TermsAndConditions = dto.TermsAndConditions,
                UrlName = !String.IsNullOrEmpty(dto.UrlName) ? dto.UrlName.ToLower() : "",
                ExternalUrlAdd = dto.ExternalUrlAdd,
                ExternalUrlRemove = dto.ExternalUrlRemove,
                CertificateThumbprintVisa = dto.CertificateThumbprintVisa,
                Container = true,
                AllowSelectContentAssociation = dto.AllowSelectContentAssociation,
                AllowSelectContentPayment = dto.AllowSelectContentPayment,
                MaxQuotaAllow = dto.MaxQuotaAllow,
                EnablePrivatePayment = dto.EnablePrivatePayment,
                EnablePublicPayment = dto.EnablePublicPayment,
                EnableAssociation = dto.EnableAssociation,
                ReferenceParamName = dto.ReferenceParamName,
                ReferenceParamName2 = dto.ReferenceParamName2,
                ReferenceParamName3 = dto.ReferenceParamName3,
                ReferenceParamName4 = dto.ReferenceParamName4,
                ReferenceParamName5 = dto.ReferenceParamName5,
                ReferenceParamName6 = dto.ReferenceParamName6,
                ReferenceParamRegex = dto.ReferenceParamRegex,
                ReferenceParamRegex2 = dto.ReferenceParamRegex2,
                ReferenceParamRegex3 = dto.ReferenceParamRegex3,
                ReferenceParamRegex4 = dto.ReferenceParamRegex4,
                ReferenceParamRegex5 = dto.ReferenceParamRegex5,
                ReferenceParamRegex6 = dto.ReferenceParamRegex6,
                HasReferences = !string.IsNullOrEmpty(dto.ReferenceParamName),
                ServiceGateways = new Collection<ServiceGatewayModel>(),
                AskUserForReferences = dto.AskUserForReferences,
                AllowMultipleCards = dto.AllowMultipleCards,
                ContentIntro = string.IsNullOrEmpty(dto.IntroContent) ? string.Empty : dto.IntroContent.Replace(Environment.NewLine, string.Empty),
                BinGroups = dto.BinGroups.Select(bg => bg.Id).ToList(),
                UrlIntegrationVersion = (int)dto.UrlIntegrationVersion,
                AllowVon = dto.AllowVon,
                AllowWcfPayment = dto.AllowWcfPayment,
                InformCardBank = dto.InformCardBank,
                InformCardType = dto.InformCardType,
                Sort = dto.Sort,
                Image = dto.ImageName,
                ImagePath = dto.ImageUrl,
                InformAffiliationCard = dto.InformCardAffiliation,
            };

            //Gateways
            if (dto.ServiceGatewaysDto != null && dto.ServiceGatewaysDto.Count > 0)
            {
                foreach (var gt in gateways)
                {
                    var gActive = dto.ServiceGatewaysDto.FirstOrDefault(x => x.GatewayId == gt.Id);
                    model.ServiceGateways.Add(new ServiceGatewayModel()
                    {
                        Id = gActive != null ? gActive.Id : new Guid(),
                        GatewayName = gt.Name,
                        Active = gActive != null && gActive.Active,
                        SendExtract = gActive != null && gActive.SendExtract,
                        GatewayId = gt.Id,
                        ReferenceId = gActive != null ? gActive.ReferenceId : "",
                        ServiceType = gActive != null ? gActive.ServiceType : "",
                        GatewayEnum = gt.Enum,
                        AuxiliarData = gActive != null ? gActive.AuxiliarData : "",
                        AuxiliarData2 = gActive != null ? gActive.AuxiliarData2 : "",
                    });
                }

            }
            else
            {
                foreach (var gt in gateways)
                {
                    model.ServiceGateways.Add(new ServiceGatewayModel()
                    {
                        Active = false,
                        GatewayId = gt.Id,
                        GatewayName = gt.Name,
                        GatewayEnum = gt.Enum,
                    });
                }
            }

            return model;
        }

        public static ServiceDto ToDto(this ServiceContainerModel model)
        {
            var dto = new ServiceDto()
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Active = model.Active,
                LinkId = model.LinkId,
                Tags = model.Tags,
                ImageDeleted = model.DeleteImage,
                CertificateThumbprintExternal = model.CertificateThumbprintExternal,
                PostAssociationDesc = model.PostAssociationDesc,
                TermsAndConditions = model.TermsAndConditions,
                UrlName = !String.IsNullOrEmpty(model.UrlName) ? model.UrlName.ToLower() : "",
                ExternalUrlAdd = model.ExternalUrlAdd,
                ExternalUrlRemove = model.ExternalUrlRemove,
                CertificateThumbprintVisa = model.CertificateThumbprintVisa,
                Container = true,
                ServiceCategoryId = Guid.Parse(model.ServiceCategoryId),
                AllowSelectContentAssociation = model.AllowSelectContentAssociation,
                AllowSelectContentPayment = model.AllowSelectContentPayment,
                MaxQuotaAllow = model.MaxQuotaAllow,
                EnablePrivatePayment = model.EnablePrivatePayment,
                EnablePublicPayment = model.EnablePublicPayment,
                EnableAssociation = model.EnableAssociation,
                ReferenceParamName = model.ReferenceParamName,
                ReferenceParamName2 = model.ReferenceParamName2,
                ReferenceParamName3 = model.ReferenceParamName3,
                ReferenceParamName4 = model.ReferenceParamName4,
                ReferenceParamName5 = model.ReferenceParamName5,
                ReferenceParamName6 = model.ReferenceParamName6,
                ReferenceParamRegex = model.ReferenceParamRegex,
                ReferenceParamRegex2 = model.ReferenceParamRegex2,
                ReferenceParamRegex3 = model.ReferenceParamRegex3,
                ReferenceParamRegex4 = model.ReferenceParamRegex4,
                ReferenceParamRegex5 = model.ReferenceParamRegex5,
                ReferenceParamRegex6 = model.ReferenceParamRegex6,
                AskUserForReferences = model.AskUserForReferences,
                AllowMultipleCards = model.AllowMultipleCards,
                IntroContent = !string.IsNullOrEmpty(model.ContentIntro) ? model.ContentIntro.Replace("\"", "'") : string.Empty,
                BinGroups = model.BinGroups.Select(bg => new BinGroupDto { Id = bg }).ToList(),
                UrlIntegrationVersion = (UrlIntegrationVersionEnumDto)model.UrlIntegrationVersion,
                AllowVon = model.AllowVon,
                AllowWcfPayment = model.AllowWcfPayment,
                InformCardBank = model.InformCardBank,
                InformCardType = model.InformCardType,
                InformCardAffiliation = model.InformAffiliationCard,
                Sort = model.Sort,
                ImageName = model.Image,
                PropagateChangesToChildServices = model.PropagateChangesToChildServices
            };

            //Gateways
            if (model.ServiceGateways != null && model.ServiceGateways.Count > 0)
            {
                dto.ServiceGatewaysDto = new List<ServiceGatewayDto>();
                foreach (var gt in model.ServiceGateways)
                {
                    dto.ServiceGatewaysDto.Add(new ServiceGatewayDto()
                    {
                        Active = gt.Active,
                        GatewayId = gt.GatewayId,
                        ReferenceId = gt.ReferenceId,
                        Id = gt.Id,
                        ServiceType = gt.ServiceType,
                        SendExtract = gt.SendExtract,
                        AuxiliarData = gt.AuxiliarData,
                        AuxiliarData2 = gt.AuxiliarData2
                    });
                }
            }

            return dto;
        }

    }
}