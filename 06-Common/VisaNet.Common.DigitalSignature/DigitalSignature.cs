using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.Common.DigitalSignature
{
    public class DigitalSignature
    {
        private static readonly string CryptoConfigMapName = CryptoConfig.MapNameToOID("SHA1");
        private static string Thumbprint { get; set; }

        static X509Certificate2 GetCertificate()
        {
            try
            {
                var store = new X509Store(StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);

                var certCollection = store.Certificates;
                var certificate = certCollection.Cast<X509Certificate2>().FirstOrDefault(c => c.Thumbprint == Thumbprint);
                if (certificate == null)
                {
                    NLogLogger.LogEvent(NLogType.Error, "Certificado no encontrado. Thumbprint: " + Thumbprint);
                    throw new Exception(string.Format("Certificate Not Found {0}", Thumbprint));
                }
                store.Close();
                return certificate;
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Error, "Error con Certificado. Thumbprint: " + Thumbprint);
                NLogLogger.LogEvent(exception);
                throw exception;
            }

        }

        #region private methods
        static byte[] GenerateHash(string[] paramStrings)
        {
            var concatenatedParameters = string.Concat(paramStrings);
            return SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(concatenatedParameters));
        }

        static byte[] SignHash(byte[] hashToSign)
        {
            var csp = (RSACryptoServiceProvider)GetCertificate().PrivateKey;
            return csp.SignHash(hashToSign, CryptoConfigMapName);
        }

        static byte[] GenerateSignatureForHash(byte[] hash)
        {
            return SignHash(hash);
        }

        static bool Verify(byte[] hashToValidate, byte[] signature)
        {
            var csp = (RSACryptoServiceProvider)GetCertificate().PublicKey.Key;
            return csp.VerifyHash(hashToValidate, CryptoConfigMapName, signature);
        }
        #endregion

        public static string GenerateSignature(string[] paramsStrings, string thumbprint)
        {
            Thumbprint = thumbprint.ToUpper();
            var hash = GenerateHash(paramsStrings);
            var signedHash = GenerateSignatureForHash(hash);

            var signedHashStringBase64 = Convert.ToBase64String(signedHash);
            var signedHashBytes = Convert.FromBase64String(signedHashStringBase64);

            if (Verify(hash, signedHashBytes))
                return signedHashStringBase64;
            else
                throw new Exception("Error generating signature");
        }
        public static bool CheckSignature(string[] data, string signature, string certificateThumbprint)
        {
            Thumbprint = certificateThumbprint.ToUpper();
            var cert = GetCertificate();
            if (cert == null) return false;

            var csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
            var signature1 = Convert.FromBase64String(signature);
            var concatenatedParameters = string.Concat(data);
            var data1 = Encoding.UTF8.GetBytes(concatenatedParameters);
            return csp.VerifyData(data1, CryptoConfigMapName, signature1);
        }
    }
}
