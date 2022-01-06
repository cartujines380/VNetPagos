namespace VisaNet.Services.LifApi.Models
{
    public class GetNationalBinsModel
    {
        public string Bin { get; set; }
        public bool IncludeIssuingCompany { get; set; }
    }
}