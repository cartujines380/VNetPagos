using System;
using System.Collections.Generic;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryPayment : IRepository<Payment>
    {
        //IEnumerable<PaymentBillDto> GetReportsTransactions(ReportsTransactionsFilterDto filters);
        List<TransactionHistory> HistoryTrans(DateTime date, string idApp, string codBranch, string codCommerce, string refNumber, string refNumber2, string refNumber3, string refNumber4, string refNumber5, string refNumber6, string billNumber, string idMerchant);
    }
}
