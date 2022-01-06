using System;

namespace VisaNet.Presentation.Core
{
    public class WebApiClientFatalException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiClientBusinessException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public WebApiClientFatalException(string message)
            : base(message)
        {
        }
    }
}