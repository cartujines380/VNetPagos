using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryBill : BaseRepository<Bill>, IRepositoryBill
    {
        public RepositoryBill(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }

        public bool IsBillExlternalIdRepitedByServiceId(string billExternalId, Guid serviceId)
        {
            var sql = string.Format("SELECT count(1) FROM Bills b " + 
                "INNER JOIN Payments p ON p.Id = b.PaymentId " +
                "WHERE b.BillExternalId = '{0}' " +
                "AND p.ServiceId ='{1}'", billExternalId, serviceId);
            
            var result = _db.SqlQuery<int>(sql).First();
            return result > 0;
        }

        public bool IsBillExlternalIdRepitedByMerchantId(string billExternalId, string merchantId)
        {
            var sql = string.Format("SELECT count(1) FROM Bills b " +
                      "INNER JOIN Payments p ON p.Id = b.PaymentId " +
                      "INNER JOIN Services s ON p.ServiceId = s.Id " +
                      "WHERE b.BillExternalId ='{0}' AND s.MerchantId ='{1}'", billExternalId, merchantId);

            var result = _db.SqlQuery<int>(sql).First();
            return result > 0;
        }
    }
}
