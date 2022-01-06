using System;

namespace VisaNet.Common.Exceptions
{
    public class BusinessException : Exception
    {
        private readonly string _code;
        private readonly object[] _overrides;


        public BusinessException(string code, params string[] overrides)
        {
            _code = code;
            _overrides = overrides;
            Code = _code;
        }


        public override string Message
        {
            get { return !string.IsNullOrEmpty(_code) ? BuildString() : string.Empty; }
        }

        public string Code { get; set; }


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
