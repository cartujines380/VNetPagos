using System;

namespace VisaNet.Common.Exceptions
{
    public class BillException : Exception
    {
        private readonly string _code;
        private readonly object[] _overrides;

        public BillException(string code, params string[] overrides)
        {
            _code = code;
            _overrides = overrides;
        }

        public override string Message
        {
            get { return _code; }
        }


        public static string GetMessageByCode(string code)
        {
            return !string.IsNullOrEmpty(code)
                    ? ExceptionMessages.ResourceManager.GetString(code)
                    : string.Empty;
        }


        private string BuildString()
        {
            var str = ExceptionMessages.ResourceManager.GetString(_code);

            if (!string.IsNullOrEmpty(str) && (_overrides.Length > 0))
                return string.Format(str, _overrides);

            return str;
        }
    }
}
