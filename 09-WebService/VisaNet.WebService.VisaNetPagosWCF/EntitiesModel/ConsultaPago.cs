using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    public class ConsultaPago
    {
        [Required(ErrorMessage = "CodComercio")]
        [Range(0, 999999, ErrorMessage = "CodComercioNegativo")]
        public int CodComercio { get; set; }
        [Required(ErrorMessage = "CodSucursal")]
        [Range(0, 999999, ErrorMessage = "CardBinNumbers")]
        public int CodSucursal { get; set; }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        public string NroFactura { get; set; }
        

        [Required(ErrorMessage = "FirmaDigital")]
        public string FirmaDigital { get; set; }
    }
}