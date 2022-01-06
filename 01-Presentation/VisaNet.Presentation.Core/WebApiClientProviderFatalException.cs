using System;

namespace VisaNet.Presentation.Core
{
    public class WebApiClientProviderFatalException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiClientProviderFatalException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public WebApiClientProviderFatalException(string message)
            : base(message)
        {
        }
    }
}