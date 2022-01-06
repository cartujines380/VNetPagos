using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;

namespace VisaNet.Presentation.VisaNetOn.Models
{
    public abstract class PageModel
    {
        public string IdOperation { get; set; }
        public bool EnableEmailChange { get; set; }
        public Guid WebhookRegistrationId { get; set; }

        //User
        public UserData UserData { get; set; }
        public bool NewUser { get; set; }

        //Cards
        public Guid? SelectedCardId { get; set; }
        public int CardBin { get; set; }
        public bool NewCard { get; set; }

        public ServiceInfo ServiceInfo { get; set; }
    }

    public class UserData
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldEmail", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(50, ErrorMessageResourceName = "FieldEmailMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldNombre", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(50, ErrorMessageResourceName = "FieldNombreMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldApellido", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(50, ErrorMessageResourceName = "FieldApellidoMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string Surname { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldDireccion", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(50, ErrorMessageResourceName = "FieldDireccionMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string Address { get; set; }

        public Guid? ApplicationUserId { get; set; }
        public Guid? AnonymousUserId { get; set; }
        public long? CybersourceIdentifier { get; set; }
        public IList<CardData> CardList { get; set; }
    }

    public class CardData
    {
        public Guid Id { get; set; }
        public string MaskedNumber { get; set; }
        public DateTime DueDate { get; set; }
        public bool Active { get; set; }
        public bool Expired { get; set; }
        public string Image { get; set; }
        public string Quotas { get; set; }
    }

    public class ServiceInfo
    {
        public string PostAssociationDesc { get; set; }
        public string TermsAndConditionsService { get; set; }
        public Guid ServiceId { get; set; }
        public string ServiceContainerName { get; set; }
        public string ServiceName { get; set; }
        public string IdApp { get; set; }
        public string UrlCallback { get; set; }
        public string MerchantId { get; set; }
        public string IdUsuario { get; set; }
        public string ImageName { get; set; }
        public bool AllowsWebservicePayments { get; set; }
        public int MaxQuotasAllowed { get; set; }
        public string ReferenceNumber1 { get; set; }
        public string ReferenceNumber2 { get; set; }
        public string ReferenceNumber3 { get; set; }
        public string ReferenceNumber4 { get; set; }
        public string ReferenceNumber5 { get; set; }
        public string ReferenceNumber6 { get; set; }
    }

}