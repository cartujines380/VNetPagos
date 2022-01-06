using System;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceHighwayFile : IService<HighwayBill, HighwayBillDto>
    {
        void NotifyPaymentsToService(DateTime date);
    }
}
