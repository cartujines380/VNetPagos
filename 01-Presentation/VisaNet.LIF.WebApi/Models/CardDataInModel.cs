using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using VisaNet.Common.Resource.LIF;

namespace VisaNet.LIF.WebApi.Models
{
    public class CardDataInModel
    {
        /// <summary>
        /// Valor del BIN de la tarjeta
        /// </summary>
        [MaxLength(6, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "Bin_MaxLenght")]
        [MinLength(6, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "Bin_MinLenght")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "Bin_Required")]
        public string Bin { get; set; }

        /// <summary>
        /// Identificador único de la aplicación que llama al servicio
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "AppId_Required")]
        public string AppId { get; set; }
    }
}