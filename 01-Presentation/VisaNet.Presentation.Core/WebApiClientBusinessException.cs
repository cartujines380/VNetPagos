using System;

namespace VisaNet.Presentation.Core
{
    public class WebApiClientBusinessException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiClientBusinessException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public WebApiClientBusinessException(string message)
            : base(message)
        {
        }
    }
}