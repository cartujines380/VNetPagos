using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core
{
    public static class WebApiClient
    {
        public static void CallApiService(WebApiHttpRequest request)
        {
            request.Action();
        }

        public static async Task CallApiServiceAsync(WebApiHttpRequest request)
        {
            var response = await request.Action();
            await TraceErrorAsync(response);
        }

        public static async Task<TResponse> CallApiServiceAsync<TResponse>(WebApiHttpRequest request)
        {
            var response = await request.Action();

            await TraceErrorAsync(response);

            // Check that response was successful or read error information.
            return response.IsSuccessStatusCode
                       ? JsonConvert.DeserializeObject<TResponse>(
                           await response.Content.ReadAsStringAsync(),
                           new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects })
                       : default(TResponse);
        }

        /// <summary>
        /// Determines whether [is success status code] [the specified action].
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ApplicationException"></exception>
        public static async Task<bool> IsSuccessStatusCodeAsync(Func<HttpClient, Task<HttpResponseMessage>> action)
        {
            // Send a request asynchronously continue when complete.
            var response = await action(new HttpClient()).ConfigureAwait(false);

            // Trace errors if any.
            await TraceErrorAsync(response);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Calls the API service.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="action">The action to perfom.</param>
        /// <returns>The instance of response.</returns>
        public static async Task<TResponse> CallApiServiceAsync<TResponse>(Func<HttpClient, Task<HttpResponseMessage>> action)
        {
            // Create an HttpClient instance.
            var httpClient = new HttpClient();

            // Send a request asynchronously continue when complete.
            var response = await action(httpClient).ConfigureAwait(false);

            await TraceErrorAsync(response);

            // Check that response was successful or read error information.
            return response.IsSuccessStatusCode
                       ? JsonConvert.DeserializeObject<TResponse>(
                           await response.Content.ReadAsStringAsync(),
                           new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })
                       : default(TResponse);
        }

        #region Private Methods

        private static async Task TraceErrorAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;

            var errorInfo = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                if (!string.IsNullOrEmpty(errorInfo))
                    throw new WebApiClientBusinessException(errorInfo);
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                if (!string.IsNullOrEmpty(errorInfo))
                    throw new WebApiClientFatalException(errorInfo);
            }

            if (response.StatusCode == HttpStatusCode.BadGateway)
            {
                if (!string.IsNullOrEmpty(errorInfo))
                    throw new WebApiClientProviderBusinessException(errorInfo);
            }

            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                if (!string.IsNullOrEmpty(errorInfo))
                    throw new WebApiClientProviderFatalException(errorInfo);
            }

            if (response.StatusCode == HttpStatusCode.Unused)
            {
                if (!string.IsNullOrEmpty(errorInfo))
                    throw new WebApiClientBillBusinessException(errorInfo);
            }

            throw new Exception(errorInfo);
        }

        #endregion
    }
}
