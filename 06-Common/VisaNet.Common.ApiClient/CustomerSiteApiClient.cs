using System;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Security;
using VisaNet.CustomerSite.EntitiesDtos.TableFilters;
using VisaNet.Utilities.Cryptography;

namespace VisaNet.Common.ApiClient
{
    public class CustomerSiteApiClient<T> where T : class
    {
        private static readonly string BaseUrl = ConfigurationManager.AppSettings["BaseUrlCustomerSiteApi"];

        public static T GetdebitCommerces(Func<string, T> mapper, CustomerSiteCommerceFilterDto filterDto, ITransactionContext transactionContext)
        {
            var url = string.Format("{0}{1}", BaseUrl, ConfigurationManager.AppSettings["CustomerSiteApiGetCommerceDebit"]);
            var client = new RestClient(url);

            var request = new RestRequest(Method.GET);
            FillRequest(ref request, transactionContext);

            var data = new
            {
                filterDto = filterDto,
            };

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
        }

        private static void FillRequest(ref RestRequest request, ITransactionContext transactionContext)
        {
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
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
        }
        #endregion
    }

}
