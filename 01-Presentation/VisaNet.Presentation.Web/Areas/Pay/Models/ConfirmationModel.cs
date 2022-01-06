﻿using System;
using System.Collections.Generic;
using VisaNet.Presentation.Web.CustomAttributes;

namespace VisaNet.Presentation.Web.Areas.Pay.Models
{
    public class ConfirmationModel
    {
        public Guid Id { get; set; }

        public string ServiceName { get; set; }
        [CustomDisplay("Date")]
        public string Date { get; set; }
        [CustomDisplay("Hour")]
        public string Hrs { get; set; }

        public Guid AnonymousUserId { get; set; }

        public string Email { get; set; }
        [CustomDisplay("Transaction")]
        public string Transaction { get; set; }
        [CustomDisplay("Amount")]
        public string Amount { get; set; }

        public string Currency { get; set; }
        [CustomDisplay("Mask")]
        public string Mask { get; set; }

        public IDictionary<string, string> References { get; set; }

        //Aplica descuento
        public bool DiscountApplyed { get; set; }
        //Total facturas
        public double TotalAmount { get; set; }
        //Total gravado
        public double TotalTaxedAmount { get; set; }
        //Descuento
        public double Discount { get; set; }

        public Guid? ServiceAssociatedId { get; set; }
        public bool AlreadyHasAutomaticPayment { get; set; }
        public bool AllowsAutomaticPayment { get; set; }

        public string DiscountTypeText { get; set; }

        public int Quotas { get; set; }
    }
}