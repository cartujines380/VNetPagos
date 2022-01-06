using System;

namespace VisaNet.Presentation.Core
{
    public class WebApiClientProviderBusinessException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiClientProviderBusinessException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public WebApiClientProviderBusinessException(string message)
            : base(message)
        {
        }
    }
}