using System;

namespace VisaNet.Presentation.Core
{
    public class WebApiClientBillBusinessException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiClientProviderBusinessException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public WebApiClientBillBusinessException(string message)
            : base(message)
        {
        }
    }
}