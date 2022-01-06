using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.LIF;

namespace VisaNet.LIF.WebApi.Models
{
    public class NationalDataInModel
    {
        /// <summary>
        /// Identificador único de la apliacación que llama al servicio
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "AppId_Required")]
        public string AppId { get; set; }
    }
}