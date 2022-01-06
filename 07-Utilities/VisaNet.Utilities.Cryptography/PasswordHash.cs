using System;
using System.Security.Cryptography;
using System.Text;

namespace VisaNet.Utilities.Cryptography
{
    public static class PasswordHash
    {
        private const int SaltSize = 5;

        public static string CreateSalt()
        {
            // Generate a cryptographic random number using the cryptographic
            // service provider
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[SaltSize];
            rng.GetBytes(buff);
            // Return a Base64 string representation of the random number
            return Convert.ToBase64String(buff);
        }

        public static string CreatePasswordHash(string pwd, string salt)
        {
            var pwdAndSalt = String.Concat(pwd, salt);
            var sha1 = new SHA1CryptoServiceProvider();
            var hashedPwd = sha1.ComputeHash(Encoding.UTF8.GetBytes(pwdAndSalt));
            return Convert.ToBase64String(hashedPwd);
        }

        public static string CreatePasswordForApps(string pwd)
        {
            var passwordSalt = CreateSalt();
            var pass = CreatePasswordHash(pwd.Trim(), passwordSalt);
            return passwordSalt.Substring(0, 7) + pass;
        }

        public static void ReadPasswordFromApps(string pwd, out string password, out string salt)
        {
            salt = pwd.Substring(0, 7) + "="; // el salt es de 8 chars siempre
            password = pwd.Substring(7);
        }
    }
}
