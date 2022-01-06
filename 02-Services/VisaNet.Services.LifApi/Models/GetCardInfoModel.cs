namespace VisaNet.Services.LifApi.Models
{
    public class GetCardInfoModel
    {
        public bool IncludeIssuingCompany { get; set; }
        public string Bin { get; set; }
    }
}