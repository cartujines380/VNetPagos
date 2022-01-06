using System;
using System.Collections.Generic;
using System.Globalization;
using VisaNet.Testing.BankingWCF.Demouy448;

namespace VisaNet.Testing.BankingWCF
{
    public static class ExtensionMethods
    {
        public static string[] ToParamsArray(this BillsData data)
        {
            var paramsList = new List<String> { data.PaymentPlatform, data.ServiceId };

            if (data.ServiceReferenceNumber != null) paramsList.Add(data.ServiceReferenceNumber);
            if (data.ServiceReferenceNumber2 != null) paramsList.Add(data.ServiceReferenceNumber2);
            if (data.ServiceReferenceNumber3 != null) paramsList.Add(data.ServiceReferenceNumber3);
            if (data.ServiceReferenceNumber4 != null) paramsList.Add(data.ServiceReferenceNumber4);
            if (data.ServiceReferenceNumber5 != null) paramsList.Add(data.ServiceReferenceNumber5);
            if (data.ServiceReferenceNumber6 != null) paramsList.Add(data.ServiceReferenceNumber6);
            if (data.GatewayEnumDto != null) paramsList.Add(data.GatewayEnumDto);

            if (!string.IsNullOrEmpty(data.UserData.Ci)) paramsList.Add(data.UserData.Ci);
            if (!string.IsNullOrEmpty(data.UserData.Email)) paramsList.Add(data.UserData.Email);
            if (!string.IsNullOrEmpty(data.UserData.Name)) paramsList.Add(data.UserData.Name);
            if (!string.IsNullOrEmpty(data.UserData.Surname)) paramsList.Add(data.UserData.Surname);
            if (!string.IsNullOrEmpty(data.UserData.Address)) paramsList.Add(data.UserData.Address);

            return paramsList.ToArray();
        }

        public static string[] ToParamsArray(this PreprocessPaymentData data)
        {
            var paramsList = new List<String> { data.PaymentPlatform };

            foreach (var bill in data.Bills)
            {
                paramsList.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.AmountToCybersource));
                if (bill.BillId != null) paramsList.Add(bill.BillId);
                if (bill.BillNumber != null) paramsList.Add(bill.BillNumber);
                if (bill.Currency != null) paramsList.Add(bill.Currency);
                if (bill.Description != null) paramsList.Add(bill.Description);
                paramsList.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.Discount));
                paramsList.Add(bill.DiscountApplyed.ToString().ToLower());
                if (bill.ExpirationDate != null) paramsList.Add(DateTime.ParseExact(bill.ExpirationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd"));
                paramsList.Add(bill.FinalConsumer.ToString().ToLower());
                if (bill.Gateway != null) paramsList.Add(bill.Gateway);
                if (bill.GatewayTransactionId != null) paramsList.Add(bill.GatewayTransactionId);
                if (bill.CensusId != null) paramsList.Add(bill.CensusId);
                if (bill.Lines != null) paramsList.Add(bill.Lines);
                if (bill.ServiceId != null) paramsList.Add(bill.ServiceId);
                paramsList.Add(bill.Payable.ToString().ToLower());
                paramsList.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.TotalAmount));
                paramsList.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.TotalTaxedAmount));
                paramsList.Add(bill.CardBinNumbers.ToString());
                //if (bill.PreBillNumber != null) paramsList.Add(bill.PreBillNumber);
                if (bill.ServiceReferenceNumber != null) paramsList.Add(bill.ServiceReferenceNumber);
                if (bill.ServiceReferenceNumber2 != null) paramsList.Add(bill.ServiceReferenceNumber2);
                if (bill.ServiceReferenceNumber3 != null) paramsList.Add(bill.ServiceReferenceNumber3);
                if (bill.ServiceReferenceNumber4 != null) paramsList.Add(bill.ServiceReferenceNumber4);
                if (bill.ServiceReferenceNumber5 != null) paramsList.Add(bill.ServiceReferenceNumber5);
                if (bill.ServiceReferenceNumber6 != null) paramsList.Add(bill.ServiceReferenceNumber6);
                if (bill.MerchantReferenceCode != null) paramsList.Add(bill.MerchantReferenceCode);
                if (bill.MerchantId != null) paramsList.Add(bill.MerchantId);
                if (bill.ServiceType != null) paramsList.Add(bill.ServiceType);
                paramsList.Add(bill.MultipleBillsAllowed.ToString().ToLower());
                if (bill.CreationDate != null) paramsList.Add(bill.CreationDate);

            }

            return paramsList.ToArray();
        }
        
    }
}
