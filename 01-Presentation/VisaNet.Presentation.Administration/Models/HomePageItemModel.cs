using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class HomePageItemModel
    {
        public Guid Id { get; set; }

        [CustomDisplay("HomePageItemDto_Headline1")]
        [MaxLength(100)]
        public string Headline1 { get; set; }

        [CustomDisplay("HomePageItemDto_Headline2")]
        [MaxLength(100)]
        public string Headline2 { get; set; }

        [CustomDisplay("HomePageItemDto_Description")]
        [MaxLength(500)]
        public string Description { get; set; }

        [CustomDisplay("HomePageItemDto_Image")]
        public string Image { get; set; }
        public string Image_originalname { get; set; }
        public string Image_internalname { get; set; }
        public Guid? ImageId { get; set; }
        public bool DeleteImage { get; set; }
        public string ImagenBD { get; set; }

        [CustomDisplay("HomePageItemDto_LinkUrl")]
        [MaxLength(500)]
        public string LinkUrl { get; set; }

        [CustomDisplay("HomePageItemDto_File")]
        public string File { get; set; }
        public string File_originalname { get; set; }
        public string File_internalname { get; set; }
        public Guid? FileId { get; set; }
        public bool DeleteFile { get; set; }
        public string FileBD { get; set; }

        [CustomDisplay("HomePageItemDto_LinkName")]
        [MaxLength(100)]
        public string LinkName { get; set; }
    }
}