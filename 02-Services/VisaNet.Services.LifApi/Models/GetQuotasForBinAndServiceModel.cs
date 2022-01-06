using System;

namespace VisaNet.Services.LifApi.Models
{
    public class GetQuotasForBinAndServiceModel
    {
        public int CardBin { get; set; }
        public Guid ServiceId { get; set; }
    }
}