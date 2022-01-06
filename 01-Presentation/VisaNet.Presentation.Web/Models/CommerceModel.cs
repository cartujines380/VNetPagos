using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Web.CustomAttributes;

namespace VisaNet.Presentation.Web.Models
{
    public class CommerceModel
    {
        [CustomDisplay("CommerceModel_Id")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }

        public string ContactEmail { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string ContactAddress { get; set; }
        public string ServiceId { get; set; }

        public bool Disabled { get; set; }

        public CommerceCathegoryModel CommerceCathegoryModel { get; set; }
        public ICollection<ProductModel> ProductosListModel { get; set; }

        [CustomDisplay("CommerceModel_ProductSelected")]
        public Guid ProductSelected { get; set; }
    }

    public class ProductModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime? Expiration { get; set; }
        public int? DebitProductid { get; set; }
        public int? DebitMerchantId { get; set; }
        public ICollection<ProductPropertyModel> ProductPropertyModelList { get; set; }
    }

    public class ProductPropertyModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }

        public int MaxSize { get; set; }
        public int InputSequence { get; set; }
        public bool Requiered { get; set; }

        public int? DebitProductPropertyId { get; set; }
        public int? DebitProductId { get; set; }

        public string UserInput { get; set; }
    }

    public class CommerceCathegoryModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}