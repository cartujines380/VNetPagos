using System;

namespace VisaNet.LIF.Interfaces
{
    public interface IQuotaService
    {
        string GetQuotasForBin(int cardBin);
        string GetQuotasForBinAndService(int cardBin, Guid serviceId);
    }
}