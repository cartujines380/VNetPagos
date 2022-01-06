using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ServiceDto
    {
        public ServiceDto()
        {
            ServiceGatewaysDto = new Collection<ServiceGatewayDto>();
        }

        public Guid Id { get; set; }
        public Guid ServiceCategoryId { get; set; }
        public ServiceCategoryDto ServiceCategory { get; set; }
        public string ServiceCategoryName { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Description { get; set; }
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string DescriptionTooltip { get; set; }
        public string LinkId { get; set; }
        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Tags { get; set; }

        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
        public bool ImageDeleted { get; set; }

        public string ImageTooltipName { get; set; }
        public string ImageTooltipUrl { get; set; }
        public bool ImageTooltipDeleted { get; set; }

        public DepartamentDtoType Departament { get; set; }

        public bool Container { get; set; }
        public bool AllowSelectContentAssociation { get; set; }
        public bool AllowSelectContentPayment { get; set; }
        public bool AskUserForReferences { get; set; }

        public bool Active { get; set; }
        public ICollection<ServiceGatewayDto> ServiceGatewaysDto { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string MerchantId { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CybersourceProfileId { get; set; }
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CybersourceAccessKey { get; set; }
        [StringLength(500, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CybersourceSecretKey { get; set; }
        [StringLength(500, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CybersourceTransactionKey { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceParamName { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceParamName2 { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceParamName3 { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceParamName4 { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceParamName5 { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceParamName6 { get; set; }

        //public bool DebitCard { get; set; }
        //public bool CreditCard { get; set; }
        //public bool DebitCardInternational { get; set; }
        //public bool CreditCardInternational { get; set; }

        public bool EnableMultipleBills { get; set; }
        public bool EnableAutomaticPayment { get; set; }
        public bool EnablePublicPayment { get; set; }
        public bool EnablePrivatePayment { get; set; }
        public bool EnablePartialPayment { get; set; }
        public bool EnableAssociation { get; set; }
        public bool AllowMultipleCards { get; set; }
        public int MaxQuotaAllow { get; set; }

        public string CertificateThumbprintExternal { get; set; }

        public List<ServiceEnableEmailDto> HighwayEnableEmails { get; set; }
        public string ExtractEmail { get; set; }
        public string PostAssociationDesc { get; set; }
        public string TermsAndConditions { get; set; }
        public string UrlName { get; set; }

        public Guid? ServiceContainerId { get; set; }
        public ServiceDto ServiceContainerDto { get; set; }
        public string ServiceContainerName { get; set; }

        public DiscountTypeDto DiscountType { get; set; }

        public string ExternalUrlAdd { get; set; }
        public string ExternalUrlRemove { get; set; }
        public string CertificateThumbprintVisa { get; set; }
        public string ReferenceParamRegex { get; set; }
        public string ReferenceParamRegex2 { get; set; }
        public string ReferenceParamRegex3 { get; set; }
        public string ReferenceParamRegex4 { get; set; }
        public string ReferenceParamRegex5 { get; set; }
        public string ReferenceParamRegex6 { get; set; }

        public string ServiceContainerImageName { get; set; }
        public string ServiceContainerImageUrl { get; set; }

        public string IntroContent { get; set; }

        public List<ServiceDto> ServicesInside { get; set; }
        public NotificationConfigDto NotificationConfigDto { get; set; }

        public string[] LoadReferences(IDictionary<string, string> references)
        {
            var result = new string[6];
            var props = typeof(ServiceDto).GetProperties().Where(p => p.Name.StartsWith("ReferenceParamName"));

            foreach (var propertyInfo in props)
            {
                object propValue = null;
                if (this.AskUserForReferences && string.IsNullOrEmpty(this.ReferenceParamName)
                    && this.ServiceContainerDto != null && !string.IsNullOrEmpty(this.ServiceContainerDto.ReferenceParamName))
                {
                    //Tomo referencias del Servicio Contenedor
                    propValue = propertyInfo.GetValue(this.ServiceContainerDto, null);
                }
                else
                {
                    //Tomo referencias de este Servicio
                    propValue = propertyInfo.GetValue(this, null);
                }

                if (propValue != null && references.ContainsKey(propValue.ToString()))
                {
                    char referenceParamLastChar = propertyInfo.Name[propertyInfo.Name.Length - 1];
                    if (char.IsNumber(referenceParamLastChar))
                    {
                        var tmp = char.GetNumericValue(referenceParamLastChar);
                        result[int.Parse(tmp.ToString()) - 1] = references[propValue.ToString()];
                    }
                    else
                    {
                        result[0] = references[propValue.ToString()];
                    }
                }
            }
            return result;
        }

        public List<BinGroupDto> BinGroups { get; set; }

        public Guid? InterpreterId { get; set; }
        public InterpreterDto InterpreterDto { get; set; }
        public string InterpreterAuxParam { get; set; }

        public bool AllowGetBills { get; set; }
        public bool AllowInputAmount { get; set; }

        public List<ImporteInfoDto> ImporteInfoDto { get; set; }

        public bool InformCardBank { get; set; }
        public bool InformCardType { get; set; }
        public bool InformCardAffiliation { get; set; }
        public UrlIntegrationVersionEnumDto UrlIntegrationVersion { get; set; }
        public bool AllowVon { get; set; }
        public bool AllowWcfPayment { get; set; }

        public int Sort { get; set; }

        public bool PropagateChangesToChildServices { get; set; }
    }

    public class ImporteInfoDto
    {
        public CurrencyDto Currency { get; set; }
        public string CurrencyDescription { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
    }

}
