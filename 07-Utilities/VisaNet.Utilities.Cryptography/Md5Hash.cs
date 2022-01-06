using System;
using System.Linq;
using System.Text;

namespace VisaNet.Utilities.Cryptography
{
    public static class Md5Hash
    {
        public static string GenerateHash(string source)
        {
            String hash;
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                hash = String.Concat(md5.ComputeHash(Encoding.ASCII.GetBytes(source)).Select(x => x.ToString("x2")));
            }
            return hash;
        }

        public static bool ValidateHash(string source, string hash)
        {
            String newHash = GenerateHash(source);
            return hash.Equals(newHash);
        }
    }
}
