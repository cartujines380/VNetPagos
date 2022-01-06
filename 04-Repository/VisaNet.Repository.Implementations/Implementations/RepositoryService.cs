using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryService : BaseRepository<Service>, IRepositoryService
    {
        public RepositoryService(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {

        }

        public Service GetService(int codCommerce, int codBranch)
        {
            var cC = codCommerce.ToString();
            var cB = codBranch.ToString();
            var query = AllNoTracking(s => s.ServiceGateways.Any(x => x.ReferenceId.Equals(cC) && x.ServiceType.Equals(cB)),
                 s => s.HighwayEnableEmails, s => s.ServiceGateways, s => s.ServiceContainer, x => x.ServiceCategory);
            var service = query.FirstOrDefault();

            return service == null ? null : new Service()
            {
                Id = service.Id,
                Name = service.Name,
                MerchantId = service.MerchantId,
                CybersourceTransactionKey = service.CybersourceTransactionKey,
                HighwayEnableEmails = service.HighwayEnableEmails.Select(x => new ServiceEnableEmail()
                {
                    Id = x.Id,
                    Email = x.Email,
                }).ToList(),
                ServiceGateways = service.ServiceGateways.Select(x => new ServiceGateway()
                {
                    Active = x.Active,
                    ReferenceId = x.ReferenceId,
                    ServiceType = x.ServiceType,
                    GatewayId = x.GatewayId,
                    SendExtract = x.SendExtract,
                    AuxiliarData = x.AuxiliarData,
                    AuxiliarData2 = x.AuxiliarData2,
                }).ToList(),
                ServiceCategory = service.ServiceCategory != null ? new ServiceCategory()
                {
                    Name = service.ServiceCategory.Name
                } : new ServiceCategory()
                {
                    Name = ""
                }

            };
        }

        public Service GetService(string merchantId, string appId)
        {
            List<Service> result;
            if (!string.IsNullOrEmpty(merchantId))
            {
                result = AllNoTracking(x => x.MerchantId.Equals(merchantId, StringComparison.OrdinalIgnoreCase), x => x.ServiceGateways, x => x.ServiceGateways.Select(y => y.Gateway)).ToList();

                if (result.Count > 1)
                    throw new BusinessException(CodeExceptions.SERVICE_MERCHANTID_DUPLICATED);

                return result.FirstOrDefault();
            }

            result = AllNoTracking(x => x.UrlName.Equals(appId, StringComparison.OrdinalIgnoreCase), x => x.ServiceGateways, x => x.ServiceGateways.Select(y => y.Gateway)).ToList();

            if (result.Count > 1)
                throw new BusinessException(CodeExceptions.SERVICE_APPID_DUPLICATED);

            return result.FirstOrDefault();
        }

        public bool IsBinAssociatedToService(int binValue, Guid serviceId)
        {
            var paramServiceId = new SqlParameter
            {
                ParameterName = "serviceId",
                Value = serviceId.ToString()
            };
            var parambinValue = new SqlParameter
            {
                ParameterName = "bin",
                Value = binValue
            };
            var isAssociated = _db.SqlQuery<int>("exec StoreProcedure_IsBinAssociatedToService @serviceId, @bin", paramServiceId, parambinValue).First() == 1;
            return isAssociated;
        }

    }
}
