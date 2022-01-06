using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.Utilities.DigitalSignature
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
            NLogLogger.LogEvent(NLogType.Info, "Info Concatenada: '" + concatenatedParameters + "'");
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
            if (string.IsNullOrEmpty(signature)) return false;
            Thumbprint = new string(certificateThumbprint.ToUpper().Where(c => c < 128).ToArray()); // certificateThumbprint.ToUpper();

            var cert = GetCertificate();
            if (cert == null) return false;

            var csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
            var signature1 = Convert.FromBase64String(signature);
            var concatenatedParameters = string.Concat(data);
            NLogLogger.LogEvent(NLogType.Info, "Info Concatenada: '" + concatenatedParameters + "'");
            //NLogLogger.LogEvent(NLogType.Info, "Thumbprint utilizado: '" + certificateThumbprint.ToUpper() + "'");
            var data1 = Encoding.UTF8.GetBytes(concatenatedParameters);
            return csp.VerifyData(data1, CryptoConfigMapName, signature1);
        }

        public static bool CheckSignature(string data, string signature, string certificateThumbprint)
        {
            if (string.IsNullOrEmpty(signature)) return false;
            Thumbprint = new string(certificateThumbprint.ToUpper().Where(c => c < 128).ToArray()); // certificateThumbprint.ToUpper();

            var cert = GetCertificate();
            if (cert == null) return false;

            var csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
            var signature1 = Convert.FromBase64String(signature);

            NLogLogger.LogEvent(NLogType.Info, "Info a chequear: " + data);

            var data1 = Encoding.UTF8.GetBytes(data);
            return csp.VerifyData(data1, CryptoConfigMapName, signature1);
        }
    }

}