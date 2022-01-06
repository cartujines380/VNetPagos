using System;

namespace VisaNet.Common.Exceptions
{
    public class FatalException : Exception
    {
        private readonly string _code;
        private readonly object[] _overrides;
        private readonly string _internalCode;

        public FatalException(string code, params string[] overrides)
        {
            _code = code;
            _overrides = overrides;
        }

        public FatalException(string code, string internalCode, params string[] overrides)
        {
            _code = code;
            _overrides = overrides;
            _internalCode = internalCode;
        }


        public override string Message
        {
            get { return !string.IsNullOrEmpty(_code) ? BuildString(_code) : string.Empty; }
        }

        public string InternalMessage
        {
            get { return !string.IsNullOrEmpty(_internalCode) ? BuildString(_internalCode) : string.Empty; }
        }

        public static string GetMessageByCode(string code)
        {
            return !string.IsNullOrEmpty(code)
                    ? ExceptionMessages.ResourceManager.GetString(code)
                    : string.Empty;
        }
        
        private string BuildString(string code)
        {
            var str = ExceptionMessages.ResourceManager.GetString(code);

            if (!string.IsNullOrEmpty(str) && (_overrides.Length > 0))
                return string.Format(str, _overrides);

            return str;
        }
    }
}