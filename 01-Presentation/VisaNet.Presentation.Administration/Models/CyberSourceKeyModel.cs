﻿using System.Collections.Generic;

namespace VisaNet.Presentation.Administration.Models
{
    public class CyberSourceKeyModel
    {
        public IDictionary<string, string> Keys { get; set; }

        public string Currency { get; set; }
        //Aplica descuento
        public bool DiscountApplyed { get; set; }
        //Total facturas
        public double TotalAmount { get; set; }
        //Total gravado
        public double TotalTaxedAmount { get; set; }
        //Descuento
        public double Discount { get; set; }

    }
}