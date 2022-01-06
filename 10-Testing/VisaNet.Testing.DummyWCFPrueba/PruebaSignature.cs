using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.Testing.DummyWCFPrueba
{
    class PruebaSignature
    {
        private static readonly string CertPath = ConfigurationManager.AppSettings["CertificatePath"];
        private static readonly string CertificatePassword = ConfigurationManager.AppSettings["CertificatePassword"];
        private static readonly string CryptoConfigMapName = CryptoConfig.MapNameToOID("SHA1");
        
        static X509Certificate2 GetCertificate()
        {
            var store = new X509Store(StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var certificate = new X509Certificate2(CertPath, CertificatePassword);

            if (certificate == null)
                throw new Exception(string.Format("Certificate Not Found {0}", CertPath));

            store.Close();

            return certificate;
        }

        #region private methods
        static byte[] GenerateHashBanred(string[] paramStrings)
        {
            var concatenatedParameters = string.Concat(paramStrings);
            //concatenatedParameters = concatenatedParameters.Replace('.', ',');
            NLogLogger.LogEvent(NLogType.Info, "Info Concatenada: '" + concatenatedParameters + "'");
            return SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(concatenatedParameters));
        }

        static byte[] SignHashForBanred(byte[] hashToSign)
        {
            var csp = (RSACryptoServiceProvider)GetCertificate().PrivateKey;
            return csp.SignHash(hashToSign, CryptoConfigMapName);
        }

        static byte[] GenerateBanredSignatureForHash(byte[] hash)
        {
            return SignHashForBanred(hash);
        }

        static bool Verify(byte[] hashToValidate, byte[] signature)
        {
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

            if (Verify(hash, signedHashBytes))
                return signedHashStringBase64;
            else
                throw new Exception("Error generating signature");
        }

        public static bool CheckSignature(string[] data, string signature, string certificateName)
        {
            var cert = GetCertificate();
            var csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
            var signature1 = Convert.FromBase64String(signature);
            var concatenatedParameters = string.Concat(data);
            NLogLogger.LogEvent(NLogType.Info, "Info Concatenada: '" + concatenatedParameters + "'");
            var data1 = Encoding.UTF8.GetBytes(concatenatedParameters);
            return csp.VerifyData(data1, CryptoConfigMapName, signature1);
        }

    }
}
