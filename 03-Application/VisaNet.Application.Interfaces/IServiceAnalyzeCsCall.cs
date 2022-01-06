using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceAnalyzeCsCall
    {

        CybersourceTransactionsDataDto ProcessCybersourceOperation(IDictionary<string, string> cybersourceData);

    }
}
