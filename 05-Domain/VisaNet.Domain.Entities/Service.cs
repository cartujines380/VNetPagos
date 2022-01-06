using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("Services")]
    [TrackChanges]
    public class Service : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public Guid ServiceCategoryId { get; set; }
        public virtual ServiceCategory ServiceCategory { get; set; }

        [TrackChangesAditionalInfo(Index = 0)]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        [MaxLength(250)]
        public string DescriptionTooltip { get; set; }
        public string LinkId { get; set; }
        [MaxLength(250)]
        public string Tags { get; set; }

        [MaxLength(250)]
        public string ImageName { get; set; }
        [MaxLength(250)]
        public string ImageTooltipName { get; set; }

        //TODO: Las imagenes como refereencias se van a eliminar en un proximo release
        public Guid? ImageId { get; set; }
        public virtual Image Image { get; set; }
        public Guid? ImageTooltipId { get; set; }
        public virtual Image ImageTooltip { get; set; }

        public bool Active { get; set; }
        public virtual ICollection<ServiceGateway> ServiceGateways { get; set; }

        //Esto se agrega para sucive. 
        public DepartamentType Departament { get; set; }
        [MaxLength(100)]
        public string MerchantId { get; set; }
        [MaxLength(100)]
        public string CybersourceProfileId { get; set; }
        [MaxLength(100)]
        public string CybersourceAccessKey { get; set; }
        [MaxLength(500)]
        public string CybersourceSecretKey { get; set; }
        [MaxLength(500)]
        public string CybersourceTransactionKey { get; set; }

        [MaxLength(50)]
        public string ReferenceParamName { get; set; }
        [MaxLength(50)]
        public string ReferenceParamName2 { get; set; }
        [MaxLength(50)]
        public string ReferenceParamName3 { get; set; }
        [MaxLength(50)]
        public string ReferenceParamName4 { get; set; }
        [MaxLength(50)]
        public string ReferenceParamName5 { get; set; }
        [MaxLength(50)]
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
        public bool Container { get; set; }
        public bool AllowSelectContentAssociation { get; set; }
        public bool AllowSelectContentPayment { get; set; }
        public bool EnableAssociation { get; set; }
        public bool AskUserForReferences { get; set; }
        public bool AllowMultipleCards { get; set; }
        [Range(1, 99)]
        [DefaultValue(1)]
        public int MaxQuotaAllow { get; set; }

        public virtual ICollection<ServiceEnableEmail> HighwayEnableEmails { get; set; }
        public string ExtractEmail { get; set; }

        public string CertificateThumbprintExternal { get; set; }
        public string PostAssociationDesc { get; set; }
        public string TermsAndConditions { get; set; }

        public string UrlName { get; set; }

        public Guid? ServiceContainerId { get; set; }
        public virtual Service ServiceContainer { get; set; }

        public DiscountType DiscountType { get; set; }

        public string ExternalUrlAdd { get; set; }
        public string ExternalUrlRemove { get; set; }
        public string CertificateThumbprintVisa { get; set; }

        public bool InformCardBank { get; set; }
        public bool InformCardType { get; set; }
        public bool InformCardAffiliation { get; set; }
        public UrlIntegrationVersionEnum UrlIntegrationVersion { get; set; }
        public bool AllowVon { get; set; }
        public bool AllowWcfPayment { get; set; }

        [SkipTracking]
        [MaxLength(50)]
        public string CreationUser { get; set; }
        [SkipTracking]
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        [SkipTracking]
        public DateTime CreationDate { get; set; }
        [SkipTracking]
        public DateTime LastModificationDate { get; set; }

        public string ReferenceParamRegex { get; set; }
        public string ReferenceParamRegex2 { get; set; }
        public string ReferenceParamRegex3 { get; set; }
        public string ReferenceParamRegex4 { get; set; }
        public string ReferenceParamRegex5 { get; set; }
        public string ReferenceParamRegex6 { get; set; }

        public string IntroContent { get; set; }

        public Guid? InterpreterId { get; set; }
        public virtual Interpreter Interpreter { get; set; }
        public string InterpreterAuxParam { get; set; }

        public virtual ICollection<BinGroup> BinGroups { get; set; }

        [Range(0, 99)]
        public int Sort { get; set; }
    }
}
