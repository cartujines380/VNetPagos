using System;

namespace VisaNet.Common.Exceptions
{
    public class PageObjectException : Exception
    {
        public PageObjectException(string message) : base (message)
        {
        }
    }
}
