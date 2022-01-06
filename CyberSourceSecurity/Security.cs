using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;

namespace CyberSourceSecurity
{
    public static class Security
    {
        public static String Sign(IDictionary<string, string> paramsArray, String secretKey)
        {
            return Sign(BuildDataToSign(paramsArray), secretKey);
        }

        private static String Sign(String data, String secretKey)
        {
            var encoding = new UTF8Encoding();
            var keyByte = encoding.GetBytes(secretKey);

            var hmacsha256 = new HMACSHA256(keyByte);
            var messageBytes = encoding.GetBytes(data);
            return Convert.ToBase64String(hmacsha256.ComputeHash(messageBytes));
        }

        private static String BuildDataToSign(IDictionary<string, string> paramsArray)
        {
            var signedFieldNames = paramsArray["signed_field_names"].Split(',');
            IList<string> dataToSign = signedFieldNames.Select(signedFieldName => signedFieldName + "=" + paramsArray[signedFieldName]).ToList();
            return CommaSeparate(dataToSign);
        }

        private static String CommaSeparate(IEnumerable<string> dataToSign)
        {
            return String.Join(",", dataToSign);
        }
    }
}
