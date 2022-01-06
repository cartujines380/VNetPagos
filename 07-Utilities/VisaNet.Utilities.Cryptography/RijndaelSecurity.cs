using System.Security.Cryptography;

namespace VisaNet.Utilities.Cryptography
{
    public static class RijndaelSecurity
    {
        private static int _iterations = 2;
        private static int _keySize = 256;

//      private static string _hash = "SHA1";
        private static string _salt = "aselrias38490a32"; // Random
        private static string _vector = "8947az34awl34kjq"; // Random

        private static string _password = "VisaNet_Pagos_Password";
        

        public static string Encrypt(string value)
        {
            return (new Cryptography(_iterations, _keySize, _salt, _vector)).Encrypt<RijndaelManaged>(value, _password);
        }


        public static string Decrypt(string value)
        {
            return (new Cryptography(_iterations, _keySize, _salt, _vector)).Decrypt<RijndaelManaged>(value, _password);
        }

    }
}
