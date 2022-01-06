using System.Collections.Generic;
using VisaNet.LIF.Domain;
using VisaNet.LIF.WebApi.Models;

namespace VisaNet.LIF.WebApi.Areas.HelpPage.Samples
{
    public class DiscountCalculationAppOutModel_Sample
    {
        public DiscountCalculationAppOutModel Data { get; set; }
        public int Code { get; set; }
    }

    public class DiscountCalculationServiceOutModel_Sample
    {
        public DiscountCalculationServiceOutModel Data { get; set; }
        public int Code { get; set; }
    }

    public class CardDataOutModel_Sample
    {
        public CardDataOutModel Data { get; set; }
        public int Code { get; set; }
    }

    public class NationalDataOutModel_Sample
    {
        public List<Bin> Data { get; set; }
        public int Code { get; set; }
    }
}