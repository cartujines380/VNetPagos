
namespace VisaNet.Utilities.SFTPClient
{
    public class SftpConfigurationOptions : ISftpClientConfigurationOptions
    {
        public string SshPrivateKeyPath { get; set; }        
        public string SshPrivateKeyName { get; set; }        
        public string Password { get; set; }        
        public string HostName { get; set; }        
        public int PortNumber { get; set; }        
        public string UserName { get; set; }
        public string SshHostKeyFingerprint{ get; set; }
        public string SessionLogPath{ get; set; }
        public bool UsePassAndCertificate { get; set; }
    }
}
