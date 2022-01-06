using System;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Utilities.Cryptography;
using VisaNet.Utilities.DigitalSignature;

namespace VisaNet.Common.ApiClient
{
    public static class LIFApiClient<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper">Function to convert JSON to T</param>
        /// <param name="bill"></param>
        /// <param name="binValue"></param>
        /// <param name="serviceId"></param>
        /// <param name="lawId"></param>
        /// <param name="transactionContext">Used for getting user data</param>
        /// <returns></returns>
        public static T GetDiscount(Func<string, T> mapper, BillDto bill, string binValue, Guid serviceId, int lawId, ITransactionContext transactionContext)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["LIFApiDiscountUri"]);

            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/json; charset=utf-8");

            var data = new
            {
                Bin = binValue,
                OperationId = Guid.NewGuid(),
                ServiceId = serviceId,
                AppId = ConfigurationManager.AppSettings["AppId"],
                Bill = new
                {
                    bill.Amount,
                    bill.Currency,
                    bill.TaxedAmount,
                    LawId = lawId,
                    IsFinalConsumer = bill.FinalConsumer
                }
            };

            var authorizationTokenUnEncrypted = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}",
                                                                transactionContext.IP,
                                                                transactionContext.UserName,
                                                                transactionContext.TransactionIdentifier,
                                                                transactionContext.TransactionDateTime,
                                                                transactionContext.RequestUri,
                                                                transactionContext.SystemUserId,
                                                                transactionContext.ApplicationUserId,
                                                                transactionContext.AnonymousUserId,
                                                                transactionContext.SessionId,
                                                                transactionContext.TraceId);

            request.AddHeader("Authorization-Token", AESSecurity.Encrypt(authorizationTokenUnEncrypted));

            request.AddJsonBody(data);

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                ThrowException(response.Content);
            }

            return mapper(response.Content);
        }

        public static T GetQuotasForBinAndService(Func<string, T> mapper, int cardBin, Guid serviceId, ITransactionContext transactionContext)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["LIFApiQuotas"] + "GetQuotasForBinAndService");

            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/json; charset=utf-8");

            var data = new
            {
                CardBin = cardBin,
                ServiceId = serviceId,
            };

            var authorizationTokenUnEncrypted = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}",
                                                                transactionContext.IP,
                                                                transactionContext.UserName,
                                                                transactionContext.TransactionIdentifier,
                                                                transactionContext.TransactionDateTime,
                                                                transactionContext.RequestUri,
                                                                transactionContext.SystemUserId,
                                                                transactionContext.ApplicationUserId,
                                                                transactionContext.AnonymousUserId,
                                                                transactionContext.SessionId,
                                                                transactionContext.TraceId);

            request.AddHeader("Authorization-Token", AESSecurity.Encrypt(authorizationTokenUnEncrypted));

            request.AddJsonBody(data);

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                ThrowException(response.Content);
            }

            return mapper(response.Content);
        }

        public static T GetQuotasForBin(Func<string, T> mapper, int cardBin, ITransactionContext transactionContext)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["LIFApiQuotas"] + "GetQuotasForBin");

            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/json; charset=utf-8");

            var data = new
            {
                CardBin = cardBin,
            };

            var authorizationTokenUnEncrypted = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}",
                                                                transactionContext.IP,
                                                                transactionContext.UserName,
                                                                transactionContext.TransactionIdentifier,
                                                                transactionContext.TransactionDateTime,
                                                                transactionContext.RequestUri,
                                                                transactionContext.SystemUserId,
                                                                transactionContext.ApplicationUserId,
                                                                transactionContext.AnonymousUserId,
                                                                transactionContext.SessionId,
                                                                transactionContext.TraceId);

            request.AddHeader("Authorization-Token", AESSecurity.Encrypt(authorizationTokenUnEncrypted));

            request.AddJsonBody(data);

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                ThrowException(response.Content);
            }

            return mapper(response.Content);
        }

        #region Private methods

        private static void ThrowException(string content)
        {
            var response = JsonConvert.DeserializeObject<dynamic>(content);

            if (response.Code == null)
            {
                throw new BusinessException(CodeExceptions.GENERAL_ERROR);
            }

            var code = (int)response.Code;
            switch (code)
            {
                case 1:
                    throw new BusinessException(CodeExceptions.DISCOUNT_INVALID_MODEL);
                case 3:
                    throw new BusinessException(CodeExceptions.BIN_VALUE_NOT_RECOGNIZED);
                case 4:
                    throw new BusinessException(CodeExceptions.BIN_NOTACTIVE);
                case 5:
                    throw new BusinessException(CodeExceptions.BIN_NOTVALID_FOR_SERVICE);
                case 6:
                    throw new BusinessException(CodeExceptions.BIN_NOTACTIVE2);
                case 7:
                    throw new BusinessException(CodeExceptions.GENERAL_ERROR);
                default:
                    throw new BusinessException(CodeExceptions.GENERAL_ERROR);
            }
        }

        #endregion
    }
}
