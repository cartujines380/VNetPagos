using System.Collections.Generic;
using System.Runtime.Serialization;
using NLog;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Resultado del preprocesamiento de facturas a pagar.
    /// </summary>
    [DataContract]
    public class PreprocessPaymentResponse : VisaNetAccessBaseResponse
    {
        /// <summary>
        /// Listado de facturas listas para pagar.
        /// </summary>
        [DataMember]
        public List<VisaNetBillResponse> Response { get; set; }

        public PreprocessPaymentResponse(VisaNetAccessResponseCode responseCode) : base(responseCode) { }
    }
}