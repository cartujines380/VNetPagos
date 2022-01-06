using System;
using System.Collections.Generic;
using VisaNet.Presentation.Administration.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class CommerceModel
    {
        [CustomDisplay("CommerceModel_Id")]
        public Guid Id { get; set; }
        [CustomDisplay("CommerceModel_Name")]
        public string Name { get; set; }
        public string Tags { get; set; }

        public string ContactEmail { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string ContactAddress { get; set; }
        [CustomDisplay("CommerceModel_ServiceId")]
        public string ServiceId { get; set; }

        public bool Disabled { get; set; }

        public CommerceCathegoryModel CommerceCathegoryModel { get; set; }
        public ICollection<ProductModel> ProductosListModel { get; set; }

        [CustomDisplay("CommerceModel_ProductSelected")]
        public Guid ProductSelected { get; set; }

        public string ImageUrl { get; set; }
        public string ImageName { get; set; }
        public bool DeleteImage { get; set; }
        public bool UploadImageDisabled { get; set; }

        public string ImageBlobName
        {
            get { return string.IsNullOrEmpty(ImageName) ? string.Empty : string.Format("{0}{1}", Id.ToString(), ImageName.Substring(ImageName.LastIndexOf("."))); }
        }
    }

    public class ProductModel
    {
        public Guid Id { get; set; }
        [CustomDisplay("ProductModel_Description")]
        public string Description { get; set; }
        public DateTime? Expiration { get; set; }
        public int? DebitProductid { get; set; }
        public int? DebitMerchantId { get; set; }
        public ICollection<ProductPropertyModel> ProductPropertyModelList { get; set; }
    }

    public class ProductPropertyModel
    {
        public Guid Id { get; set; }
        [CustomDisplay("ProductPropertyModel_Name")]
        public string Name { get; set; }
        [CustomDisplay("ProductPropertyModel_ContentType")]
        public string ContentType { get; set; }
        [CustomDisplay("ProductPropertyModel_MaxSize")]
        public int MaxSize { get; set; }
        [CustomDisplay("ProductPropertyModel_InputSequence")]
        public int InputSequence { get; set; }
        [CustomDisplay("ProductPropertyModel_Requiered")]
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
