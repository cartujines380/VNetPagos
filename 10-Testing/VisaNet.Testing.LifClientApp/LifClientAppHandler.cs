using System;
using System.Configuration;
using System.Globalization;
using Newtonsoft.Json;
using RestSharp;
using VisaNet.Common.Exceptions;
using VisaNet.Utilities.DigitalSignature;

namespace VisaNet.Testing.LifClientApp
{
    static class LifClientAppHandler
    {
        private static readonly CultureInfo Culture = CultureInfo.CreateSpecificCulture("es-UY");

        public static string NationalData()
        {
            var client = new RestClient(ConfigurationManager.AppSettings["nationalDataUri"]);

            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/json; charset=utf-8");

            var data = new
            {
                AppId = ConfigurationManager.AppSettings["AppId"],
            };

            var paramsArray = new[]
                {
                    data.AppId.ToLower()
                };

            var thumbprint = ConfigurationManager.AppSettings["LIFThumbprint"];
            var signature = DigitalSignature.GenerateSignature(paramsArray, thumbprint);

            request.AddHeader("Signature", signature);

            request.AddJsonBody(data);

            var response = client.Execute(request);

            var content = JsonConvert.DeserializeObject<dynamic>(response.Content);

            return content.Data.ToString();
        }

        public static string CardData(string bin)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["cardDataUri"]);

            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/json; charset=utf-8");

            var data = new
            {
                Bin = bin,
                AppId = ConfigurationManager.AppSettings["AppId"],
            };

            var paramsArray = new[]
                {
                    data.AppId.ToLower(),
                    data.Bin.ToLower()
                };

            var thumbprint = ConfigurationManager.AppSettings["LIFThumbprint"];
            var signature = DigitalSignature.GenerateSignature(paramsArray, thumbprint);

            request.AddHeader("Signature", signature);

            request.AddJsonBody(data);

            var response = client.Execute(request);

            var content = JsonConvert.DeserializeObject<dynamic>(response.Content);

            return content.Data.ToString();
        }

        public static string DiscountApp(string bin, string amount, string currency, bool isFinalCosumer, string lawId, string taxedAmount)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["discountAppUri"]);

            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/json; charset=utf-8");

            var data = new
            {
                AppId = ConfigurationManager.AppSettings["AppId"],
                Bill = new
                {
                    Amount = amount,
                    Currency = currency,
                    TaxedAmount = taxedAmount,
                    LawId = lawId,
                    IsFinalConsumer = isFinalCosumer
                },
                Bin = bin,
                OperationId = Guid.NewGuid()
            };

            var paramsArray = new[]
                {
                    data.AppId.ToLower(),
                    double.Parse(data.Bill.Amount).ToString(Culture),
                    data.Bill.Currency.ToLower(),
                    data.Bill.IsFinalConsumer.ToString().ToLower(),
                    data.Bill.LawId.ToLower(),
                    double.Parse(data.Bill.TaxedAmount).ToString(Culture),
                    data.Bin.ToLower(),
                    data.OperationId.ToString().ToLower()
                };

            var thumbprint = ConfigurationManager.AppSettings["LIFThumbprint"];

            string signature;
            try
            {
                signature = DigitalSignature.GenerateSignature(paramsArray, thumbprint);
            }
            catch (Exception)
            {
                throw new BusinessException("Error al generar la firma electrónica");
            }

            request.AddHeader("Signature", signature);

            request.AddJsonBody(data);

            var response = client.Execute(request);

            var content = JsonConvert.DeserializeObject<dynamic>(response.Content);

            return content.Data.ToString();
        }

    }
}