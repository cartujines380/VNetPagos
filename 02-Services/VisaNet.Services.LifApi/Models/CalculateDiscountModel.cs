using System;
using VisaNet.Lif.Domain.EntitesDtos;

namespace VisaNet.Services.LifApi.Models
{
    public class CalculateDiscountModel
    {
        public BillDto Bill { get; set; }
        public string Bin { get; set; }
        public Guid ServiceId { get; set; }
    }
}