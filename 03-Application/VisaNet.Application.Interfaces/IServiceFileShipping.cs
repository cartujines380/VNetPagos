using System;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceFileShipping : IService<Bill, BillDto>
    {
        void NotifyPaymentsToService(DateTime date, int gateway);
        void SuciveBatchConsiliation(DateTime date);
        void NotifyPaymentsToGeocom(DateTime date);
        void SistarbancBatchConsiliation();

    }
}
