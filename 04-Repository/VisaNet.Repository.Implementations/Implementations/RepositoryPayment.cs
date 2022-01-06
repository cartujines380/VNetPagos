using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryPayment : BaseRepository<Payment>, IRepositoryPayment
    {
        protected readonly DbContext _context;

        public RepositoryPayment(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
            _context = context;
        }


        public List<TransactionHistory> HistoryTrans(DateTime date, string idApp, string codBranch,
            string codCommerce, string refNumber, string refNumber2, string refNumber3, string refNumber4, string refNumber5, string refNumber6, string billNumber, string idMerchant)
        {
            return _context.Database.SqlQuery<TransactionHistory>(
                "StoreProcedure_VisaNet_TransactionHistoryWs @inputIdApp = {0}, @inputDate = {1}, @inputRefClient = {2}" +
                ", @inputRefClient2 = {3}, @inputRefClient3 = {4}, @inputRefClient4 = {5}, @inputRefClient5 = {6}" +
                ", @inputRefClient6 = {7} , @inputNroFactura = {8}, @inputCodCommerce = {9}, @inputCodBranch = {10}, @inputIdMerchant = {11} ",
                idApp, date, refNumber, refNumber2, refNumber3, refNumber4, refNumber5, refNumber6, billNumber, codCommerce, codBranch, idMerchant).ToList();
        }

    }
}
