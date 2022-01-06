using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace VisaNet.Components.Banred.Implementations
{
    class BanredDigitalSignature
    {
        private static readonly string CryptoConfigMapName = CryptoConfig.MapNameToOID("SHA1");
        private static readonly string CertificateName = ConfigurationManager.AppSettings["BanredCertificate"];

        static X509Certificate2 GetCertificate()
        {
            var store = new X509Store(StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var certCollection = store.Certificates;
            var certificate = certCollection.Cast<X509Certificate2>().FirstOrDefault(c => c.FriendlyName == CertificateName);

            if (certificate == null)
                throw new Exception(string.Format("Certificate Not Found {0}", CertificateName));

            store.Close();

            return certificate;
        }

        #region private methods
        static byte[] GenerateHashBanred(string[] paramStrings)
        {
            var concatenatedParameters = string.Concat(paramStrings);
            return SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(concatenatedParameters));
        }

        static byte[] SignHashForBanred(byte[] hashToSign)
        {
            //var certificate = new X509Certificate2(CertPath, CertificatePassword);
            var csp = (RSACryptoServiceProvider)GetCertificate().PrivateKey;

            return csp.SignHash(hashToSign, CryptoConfigMapName);
        }

        static byte[] GenerateBanredSignatureForHash(byte[] hash)
        {
            return SignHashForBanred(hash);
        }

        static bool Verify(byte[] hashToValidate, byte[] signature/*, string certPath*/)
        {
            //var cert = new X509Certificate2(CertPublicKey);
            var csp = (RSACryptoServiceProvider)GetCertificate().PublicKey.Key;
            return csp.VerifyHash(hashToValidate, CryptoConfigMapName, signature);
        }
        #endregion

        public static string GenerateSignature(string[] paramsStrings)
        {
            var hash = GenerateHashBanred(paramsStrings);
            var signedHash = GenerateBanredSignatureForHash(hash);

            var signedHashStringBase64 = Convert.ToBase64String(signedHash);
            var signedHashBytes = Convert.FromBase64String(signedHashStringBase64);

            if (Verify(hash, signedHashBytes/*, CertPath*/))
                return signedHashStringBase64;
            else
                throw new Exception("Error generating signature");
        }
    }
}
