using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Resultado de la búsqueda de pagos realizados.
    /// </summary>
    [DataContract]
    public class SearchPaymentsResponse : VisaNetAccessBaseResponse
    {
        /// <summary>
        /// Listado de pagos realizados encontrados.
        /// </summary>
        [DataMember]
        public List<VisaNetPayment> Response { get; set; }

        public SearchPaymentsResponse(VisaNetAccessResponseCode responseCode) : base(responseCode) { }
    }
}